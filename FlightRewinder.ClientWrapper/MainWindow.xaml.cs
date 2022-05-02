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
        bool updatePosition = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void _simConnection_Initialised(object? sender, EventArgs e)
        {
            DefaultTextBlock.Text = "Connected.";
        }
        private void Button_Click_1(object sender, RoutedEventArgs e) => Setup();
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

        public void Setup()
        {
            Handle = new WindowInteropHelper(this).Handle;
            _simConnection = new Connection();
            _hotkeyHandler = new Hotkeys(Handle);
            _hotkeyHandler.KeyPressed += _hotkeyHandler_KeyPressed;
            _simConnection.Initialised += _simConnection_Initialised;
            var handleSource = HwndSource.FromHwnd(Handle);
            handleSource.AddHook(HandleHook);
            _rewinder = new Rewind(_simConnection);
            _recorder = new Recorder(_simConnection);
            RegisterHotkeys();

            try
            {
                _simConnection.Initialise(Handle);
                AddEvents();
            }
            catch (COMException)
            {
                Console.WriteLine("Failed to connect to SimConnect!");
                DefaultTextBlock.Text = "Could not connect to Flight Simulator.";
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

        private void _hotkeyHandler_KeyPressed(object? sender, HotkeyPressedEventArgs e)
        {
            if (e == null)
                return;
            if (e.HotkeyID < 1 || e.HotkeyID > 2)
                return;
            SetReplay(e.HotkeyID == 1);
        }

        //Make a method for rewinding, stop recording, dump the data and start playing it in reverse.

        public void RegisterHotkeys()
        {
            if (_hotkeyHandler == null || _hotkeyHandler?.WindowHandle == null)
                throw new NullReferenceException("Refuse to assign hotkeys to thread, make sure that you have attatched the window handle.");

            //Key for replay start.   
            _hotkeyHandler.RegisterHotKeys(1, (Hotkeys.MOD_SHIFT | Hotkeys.MOD_CTRL, 0x4D));

            //Key for replay stop.
            _hotkeyHandler.RegisterHotKeys(2, (Hotkeys.MOD_SHIFT | Hotkeys.MOD_CTRL, 0x4E));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) => Setup();


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _recorder?.RestartRecording();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            SetReplay(true);
        }

        public void SetReplay(bool state)
        {
            if (_recorder != null && _rewinder != null)
            {
                if (state)
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
                else
                {
                    if (_rewinder.Playing)
                    {
                        _rewinder.StopReplay();
                        _recorder.StartRecording();
                    }
                }
            }
        }

        public void AddEvents()
        {
            if (_rewinder != null)
            {
                _rewinder.FrameFinished += FrameChanged;
                _rewinder.ReplayStopped += _rewinder_ReplayStopped;
            }
            if (_simConnection != null)
            {
                _simConnection.LocationChanged += _simConnection_LocationChanged;
            }
        }

        private void _rewinder_ReplayStopped(object? sender, EventArgs e)
        {
            _recorder?.StartRecording();
        }

        private void _simConnection_LocationChanged(object? sender, LocationChangedEventArgs e)
        {
            if (updatePosition)
                UpdateDisplay(e.Position);
        }

        public void UpdateDisplay(PositionStruct set)
        {
            AltitudeBox.Text = set.Altitude.ToString();
            LongitudeBox.Text = set.Longitude.ToString();
            LatitudeBox.Text = set.Latitude.ToString();

            BankBox.Text = set.Bank.ToString();
            PitchBox.Text = set.Pitch.ToString();
            HeadingBox.Text = set.Heading.ToString();
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

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
#if DEBUG
            var saveData = _recorder?.DumpData();
            FileStream steram = File.Open("epic.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(steram);
            if (saveData?.Frames != null)
            {
                foreach (var data in saveData.Frames)
                {
                    writer.WriteLine(PositionStructOperators.GetString(data.Position));
                }
            }
            steram.Flush();
            steram.Close();
#endif
        }
    }
}
