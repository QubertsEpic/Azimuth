using Microsoft.FlightSimulator.SimConnect;
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
            Struct1 = 0
        }

        public SimConnect? instance;

        public event EventHandler? Initialised;
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

            Initialised?.Invoke(this, new EventArgs());
        }

        private void Connection_OnQuit(SimConnect sender, SIMCONNECT_RECV data)
        {

        }

        private void Connection_OnOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {

        }

        public void PrepareData()
        {
            if (instance == null)
                return;

            instance.AddToDataDefinition(Definitions.Struct1, "Plane Altitude", "feet", SIMCONNECT_DATATYPE.FLOAT64, 0, SimConnect.SIMCONNECT_UNUSED);

            instance.RegisterDataDefineStruct<Structs.PositionStruct>(Definitions.Struct1);
        }
    }
}
