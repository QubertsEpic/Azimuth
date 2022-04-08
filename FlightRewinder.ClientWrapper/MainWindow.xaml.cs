using FlightRewinder.Classes;
using FlightRewinder.Data.DataEventArgs;
using FlightRewinder.Data.Enums;
using FlightRewinder.StructAttributes;
using FlightRewinder.Structs;
using FlightRewinderRecordingLogic;
using Microsoft.FlightSimulator.SimConnect;
using SimConnectWrapper.Core;
using SimConnectWrapper.Core.SimEventArgs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
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
        IntPtr Handle;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void _simConnection_Initialised(object? sender, EventArgs e)
        {
            DefaultTextBlock.Text = "Connected.";
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (_simConnection == null)
                _simConnection = new Connection();
            IntPtr handle = new WindowInteropHelper(this).Handle;
            _simConnection.Initialised += _simConnection_Initialised;
            var handleSource = HwndSource.FromHwnd(handle);
            handleSource.AddHook(HandleHook);
            Connect();
        }

        private IntPtr HandleHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr iParam, ref bool handled)
        {
            try
            {
                _simConnection?.HandleEvents(msg, ref handled);
                return IntPtr.Zero;
            }
            catch (Exception)
            {
                return IntPtr.Zero;
            }
        }

        public void Connect()
        {
            if (_simConnection == null)
                throw new NullReferenceException("Cannot connect to simconnect.");
            try
            {
                _simConnection.Initialise(Handle);
                BindKeyEvents();
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
                return;
            }
        }
        //Make a method for rewinding, stop recording, dump the data and start playing it in reverse.

        public void BindKeyEvents()
        {
            if(_simConnection != null)
            {
                _simConnection.MapInputToGroup(InputGroups.Group1, new List<(string inputDefinition, Enum downEventID, uint downValue, Enum upEventID, uint upValue)>() { { ("B", Events.InputEventRewindDown, 0, Events.InputEventRewindUp, 0) } });
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _simConnection = new Connection();
            _recorder = new Recorder(_simConnection);
            _rewinder = new Rewind(_simConnection);
            Handle = new WindowInteropHelper(this).Handle;
            var handleHook = HwndSource.FromHwnd(Handle);
            handleHook.AddHook(HandleHook);
            Connect();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _recorder?.RestartRecording();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (_recorder != null)
            {
                if(_recorder.Recording)
                    _recorder.StopRecording();
                var data = _recorder.DumpData();
                if (data?.Frames != null)
                {
                    _rewinder?.LoadFrames(data.Frames);
                    _rewinder?.StartRewind();
                }
            }
        }

        public void AddEvents()
        {
            if (_rewinder != null)
            {
                _rewinder.FrameChanged += FrameChanged;
                _rewinder.ReplayStopped += _rewinder_ReplayStopped;
            }
            if (_simConnection != null)
            {
                _simConnection.Event += _simConnection_Event;
                _simConnection.LocationChanged += _simConnection_LocationChanged;
            }
        }

        private void _simConnection_Event(object? sender, SIMCONNECT_RECV_EVENT e)
        {
            if(e.uGroupID == (uint)InputGroups.Group1)
            {
                switch (e.uEventID)
                {
                    case (uint)Events.InputEventRewindDown:
                        break;
                }
            }
        }

        private void _rewinder_ReplayStopped(object? sender, EventArgs e)
        {
            _recorder?.StartRecording();
        }

        private void _simConnection_LocationChanged(object? sender, LocationChangedEventArgs e)
        {
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
                throw new ArgumentNullException(nameof(args), "Cannot use null args!");
            if (_recorder != null)
            {
                _recorder.RemoveFrame(args.FrameIndex.Value);
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
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
        }
    }
}
