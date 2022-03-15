using Microsoft.FlightSimulator.SimConnect;
using SimConnectWrapper.Core.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimConnectWrapper.Core
{
    public class Connection
    {
        internal enum Definitions : uint
        {
            PositionStruct
        }

        public enum Requests
        {
            Position
        }

        public SimConnect? instance;

        public event EventHandler? Initialised;
        public event EventHandler? Quit;
        public event EventHandler? FrameEvent;
        public event EventHandler? LocationChanged;

        const int WM_USER_SIMCONNECT = 0x0402;

        public Connection()
        {
        }

        public void Initialise(IntPtr handle)
        {
            instance = new SimConnect("Flight Rewinder", handle, WM_USER_SIMCONNECT, null, 0);

            instance.OnRecvOpen += new SimConnect.RecvOpenEventHandler(Connection_OnOpen);
            instance.OnRecvQuit += new SimConnect.RecvQuitEventHandler(Connection_OnQuit);

            instance.OnRecvException += Instance_OnRecvException;
            instance.OnRecvEvent += Instance_OnRecvEvent;
            instance.OnRecvSimobjectData += Instance_OnRecvSimobjectData;
            instance.OnRecvEventFrame += Instance_OnRecvEventFrame;
            PrepareData();

            Initialised?.Invoke(this, new EventArgs());
        }

        private void Instance_OnRecvEventFrame(SimConnect sender, SIMCONNECT_RECV_EVENT_FRAME data)
        {
        }

        private void Instance_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
        }

        private void Instance_OnRecvEvent(SimConnect sender, SIMCONNECT_RECV_EVENT data)
        {
        }

        private void Instance_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
        }

        private void Connection_OnQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            Console.WriteLine("Simconnect has exited.");

            if (instance != null)
            {
                instance.Dispose();
                instance = null;
            }
            else
            {
                Console.WriteLine("An error has occurred.");
            }
            Quit?.Invoke(this, new());
        }

        private void Connection_OnOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            Console.WriteLine("Connected To " + data.szApplicationName);
        }

        public void PrepareData()
        {
            if (instance == null)
                return;

            instance.AddToDataDefinition(Definitions.PositionStruct, "PLANE ALTITUDE", "Feet", SIMCONNECT_DATATYPE.FLOAT64, 0, SimConnect.SIMCONNECT_UNUSED);
            
            //Going to add more soon.
            
            instance.RegisterDataDefineStruct<Structs.PositionStruct>(Definitions.PositionStruct);
        }
    }
}
