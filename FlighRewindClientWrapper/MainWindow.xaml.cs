using FlightRewinderRecordingLogic;
using SimConnectWrapper.Core;
using SimConnectWrapper.Core.SimEventArgs;
using System;
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
            _simConnection.LocationChanged += _simConnection_LocationChanged;
            var handleSource = HwndSource.FromHwnd(handle);
            handleSource.AddHook(HandleHook);
            _simConnection.Initialise(handle);
        }

        private void _simConnection_LocationChanged(object? sender, LocationChangedEventArgs e)
        {
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
                _simConnection.LocationChanged += _simConnection_LocationChanged;
            }
            catch (COMException)
            {
                Console.WriteLine("Failed to connect to SimConnect!");
                return;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _simConnection = new Connection();
            _recorder = new Recorder();
            Handle = new WindowInteropHelper(this).Handle;
            var handleHook = HwndSource.FromHwnd(Handle);
            handleHook.AddHook(HandleHook);
            Connect();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _recorder?.StartRecording();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

        }
    }
}
