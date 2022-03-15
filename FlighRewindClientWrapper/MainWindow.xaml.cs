using SimConnectWrapper.Core;
using SimConnectWrapper.Core.SimEventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlighRewindClientWrapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Connection _simConnection;
        public MainWindow()
        {
            InitializeComponent();   
            _simConnection = new Connection();
            _simConnection.Initialised += _simConnection_Initialised;
        }

        private void _simConnection_Initialised(object? sender, EventArgs e)
        {
            DefaultTextBlock.Text = "Connected.";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _simConnection.ManualDataRequest();
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
                _simConnection.HandleEvents(msg, ref handled);
                return IntPtr.Zero;
            }
            catch (Exception)
            {
                return IntPtr.Zero;
            }
        }
    }
}
