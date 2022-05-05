using FlightRewinder.ClientWrapper;
using FlightRewinder.Data.DataEventArgs;
using FlightRewinder.Data.Enums;
using FlightRewinder.Structs;
using FlightRewinderRecordingLogic;
using Microsoft.FlightSimulator.SimConnect;
using SimConnectWrapper.Core;
using SimConnectWrapper.Core.SimEventArgs;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
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

        private void SetButtonStatus(bool status)
        {
            StartButton.IsEnabled = status;
            ReplayButton.IsEnabled = status;
            ReplayStopButton.IsEnabled = status;
        }

        private void _hotkeyHandler_InputReceived(object? sender, HotkeyPressedEventArgs e)
        {
            if (e == null)
                return;
            switch (e.HotkeyID)
            {
                case 1:
                    StartReplaying();
                    break;
                case 2:
                    StopReplay();
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


        public void StartReplaying()
        {
            if (_recorder != null && _rewinder != null)
            {
                if (_rewinder.Playing)
                    return;
                if (_recorder.Recording)
                    _recorder.StopRecording();
                var data = _recorder.DumpData();
                try
                {
                    if (data?.Frames != null)
                    {
                        _rewinder.LoadFrames(data.Frames);
                        _rewinder.SeekRewind(data.Frames.Count - 1);
                        _rewinder.StartRewind();
                    }
                }
                catch (NullReferenceException)
                {
                    DefaultTextBlock.Text = "Cannot start replay.";
#if DEBUG
                    throw;
#endif
                }
            }
        }

        public void CheckState()
        {
            if (_rewinder?.Playing == true)
            {
                State = Rewinding;
                return;
            }

            if (_recorder?.Recording == true)
            {
                State = Recording;
                return;
            }
            State = Idle;
        }

        public void StartRecording()
        {
            _recorder?.StartRecording();
        }

        public void StopRecording()
        {
            _recorder?.StopRecording();
        }

        public void StopReplay()
        {
            if (_recorder != null && _rewinder != null)
            {
                if (_rewinder.Playing)
                {
                    _rewinder.StopReplay();
                }
            }
        }
        private void StopReplayStartRecording()
        {
            StopReplay();
            StartRecording();
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
        }

        private void RestartButtonClick(object sender, RoutedEventArgs e) 
        { 
            _recorder?.RestartRecording();
            State = "Running";
        }
        
        private void ReplayButtonClick(object sender, RoutedEventArgs e) => StartReplaying();
        private void ReplayStopButtonClick(object sender, RoutedEventArgs e) => StopReplayStartRecording();
        private void Window_Loaded(object sender, RoutedEventArgs e) => InitSetup();

        private void _rewinder_ReplayStopped(object? sender, EventArgs e)
        {
            StartRecording();
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
