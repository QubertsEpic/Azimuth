using Azimuth.ClientWrapper;
using Azimuth.ClientWrapper.Logic;
using Azimuth.Data.DataEventArgs;
using Azimuth.RecordingLogic;
using Microsoft.FlightSimulator.SimConnect;
using SimConnectWrapper.Core;
using SimConnectWrapper.Core.SimEventArgs;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace FlighRewindClientWrapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Connection? _simConnection;
        Recorder? _recorder;
        Rewind? _rewinder;
        Hotkeys? _hotkeyHandler;
        IntPtr Handle;
        StateMachine _stateMachine;
        string State
        {
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    StateTextBox.Text = "State: " + value;
            }
        }

        const string Idle = "Idle", Recording = "Recording", Rewinding = "Rewinding";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void _simConnection_Initialised(object? sender, EventArgs e) => DefaultTextBlock.Text = "Ready";

        private IntPtr HandleHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr iParam, ref bool handled)
        {
            try
            {
                switch (msg)
                {
                    case Connection.WM_USER_SIMCONNECT:
                        _simConnection?.HandleEvents(msg, ref handled);
                        break;
                    case Hotkeys.HOTKEY_MESSAGE_ID:
                        _hotkeyHandler?.HandleMessages(wParam, iParam);
                        break;

                }
                return IntPtr.Zero;
            }
            catch (Exception)
            {
                return IntPtr.Zero;
            }
        }

        public void InitSetup()
        {
            Handle = new WindowInteropHelper(this).Handle;
            _hotkeyHandler = new Hotkeys(Handle);

            var handleSource = HwndSource.FromHwnd(Handle);
            handleSource.AddHook(HandleHook);

            RegisterHotkeys();
            Connect();
        }

        public void Connect()
        {
            _simConnection = new Connection();
            _rewinder = new Rewind(_simConnection);
            _recorder = new Recorder(_simConnection);
            _stateMachine = new StateMachine(_recorder, _rewinder);
            try
            {
                _simConnection.Initialised += _simConnection_Initialised;
                _simConnection.Initialise(Handle);
                AddEvents();
            }
            catch (COMException)
            {
                Console.WriteLine("Failed to connect to SimConnect!");
                DefaultTextBlock.Text = "Please open Microsoft Flight Simulator and re-open this software.";
                SetButtonStatus(false);

                _simConnection = null;
                _rewinder = null;
                _recorder = null;

                return;
            }
            catch (Exception ex)
            {
                DefaultTextBlock.Text = ex.Message;
#if DEBUG
                throw;
#else
                return;
#endif
            }
        }

        private void _stateMachine_StateChangedEvent(object? sender, Azimuth.ClientWrapper.Logic.Args.StateChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => 
            {
                if (e != null)
                {
                    State = e.StateName;
                }
            });
           
        }

        private void SetButtonStatus(bool status)
        {
            StartButton.IsEnabled = status;
            ReplayButton.IsEnabled = status;
            ReplayStopButton.IsEnabled = status;
            ReplayRateButton.IsEnabled = status;
        }

        private async void _hotkeyHandler_InputReceived(object? sender, HotkeyPressedEventArgs e)
        {
            if (e == null)
                return;
            switch (e.HotkeyID)
            {
                case 1:
                    await _stateMachine.TransitionAsync(StateMachine.Event.Rewind);
                    break;
                case 2:
                    await _stateMachine.TransitionAsync(StateMachine.Event.Record);
                    break;
            }
        }

        //Make a method for rewinding, stop recording, dump the data and start playing it in reverse.

        public void RegisterHotkeys()
        {
            if (_hotkeyHandler == null || _hotkeyHandler?.WindowHandle == null)
                throw new NullReferenceException("Refuse to assign hotkeys to thread, make sure that you have attatched the window handle.");

            //Key for replay start.   
            _hotkeyHandler.RegisterHotKeys(1, (Hotkeys.MOD_CTRL | Hotkeys.MOD_SHIFT, ((uint)ConsoleKey.M)));

            //Key for replay stop.
            _hotkeyHandler.RegisterHotKeys(2, (Hotkeys.MOD_CTRL | Hotkeys.MOD_SHIFT, ((uint)ConsoleKey.N)));
        }


        public void AddEvents()
        {
            if (_hotkeyHandler != null)
            {
                _hotkeyHandler.HotKeyInputReceived += _hotkeyHandler_InputReceived;
            }
            if (_rewinder != null)
            {
                _rewinder.FrameFinished += FrameChanged;
                _rewinder.ReplayStopped += _rewinder_ReplayStopped;
            }
            if (_stateMachine != null)
            {
                _stateMachine.StateChangedEvent += _stateMachine_StateChangedEvent;
            }
        }

        private async void _rewinder_ReplayStopped(object? sender, EventArgs e)
        {
            await _stateMachine.TransitionAsync(StateMachine.Event.Record);
        }

        private async void RestartButtonClick(object sender, RoutedEventArgs e) => await _stateMachine.TransitionAsync(StateMachine.Event.RestartRecording);

        private async void ReplayButtonClick(object sender, RoutedEventArgs e) => await _stateMachine.TransitionAsync(StateMachine.Event.Rewind);
        private async void ReplayStopButtonClick(object sender, RoutedEventArgs e) => await _stateMachine.TransitionAsync(StateMachine.Event.StopRewinding);
        private void Window_Loaded(object sender, RoutedEventArgs e) => InitSetup();

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if((sender as MenuItem)?.Header is string header && double.TryParse(header.Substring(1), out var rate))
            {
                ReplayRateButton.Content = header;
                if(_rewinder != null)
                    _rewinder.RewindRate = rate;
            }
        }



        public void FrameChanged(object? sender, ReplayFrameChangedEventArgs args)
        {
            if (args?.FrameIndex == null)
#if DEBUG
                throw new ArgumentNullException(nameof(args), "Cannot use null args!");
#else
                return;
#endif
            if (_recorder != null)
            {
                _recorder.RemoveFrame(args.FrameIndex.Value);
            }
        }
    }
}
