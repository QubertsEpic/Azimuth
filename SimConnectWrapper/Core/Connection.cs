using FlightRewinderData.Enums;
using FlightRewinderData.Structs;
using Microsoft.FlightSimulator.SimConnect;
using SimConnectWrapper.Core.SimEventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimConnectWrapper.Core
{
    public class Connection
    {
        public SimConnect? instance;
        public bool Alive;

        public event EventHandler? Initialised;
        public event EventHandler? Quit;
        public event EventHandler? FrameEvent;
        public event EventHandler<LocationChangedEventArgs>? LocationChanged;

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
            instance.OnRecvSimobjectData += Instance_OnRecvSimobjectData;
            instance.OnRecvEventFrame += Instance_OnRecvEventFrame;
            PrepareData();

            StartDataTransfer();

            Initialised?.Invoke(this, new EventArgs());
        }

        private void Instance_OnRecvEventFrame(SimConnect sender, SIMCONNECT_RECV_EVENT_FRAME data)
        {
            FrameEvent?.Invoke(this, new());
        }

        private void Instance_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            if (data.dwRequestID == ((uint)Requests.PlaneLocation))
            {
                var position = data.dwData[0] as PositionStruct?;
                if (position.HasValue)
                {
                    AircraftPositionUpdate(position.Value);
                }
            }
        }

        public IntPtr HandleEvents(int msg, ref bool handled)
        {
            if (msg == WM_USER_SIMCONNECT)
            {
                if (instance != null)
                    instance.ReceiveMessage();
            }
            else
            {
                Console.WriteLine("Unknown message");
            }
            return IntPtr.Zero;
        }

        public void AircraftPositionUpdate(PositionStruct posStruct)
        {
            Console.WriteLine("Position Change");
            LocationChanged?.Invoke(this, new LocationChangedEventArgs(posStruct));
        }
        private void Instance_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            switch (data.dwException)
            {
                case ((uint)SIMCONNECT_EXCEPTION.ILLEGAL_OPERATION):
                    
                    break;
            }
        }

        private void Connection_OnQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            Close();
            Quit?.Invoke(this, new());
        }

        public void Close()
        {
            if (instance != null)
            {
                Console.WriteLine("Simconnect has exited.");
                instance.Dispose();
                instance = null;
            }
            else
            {
                Console.WriteLine("An error has occurred.");
            }
            Alive = false;        
        }

        private void Connection_OnOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            Console.WriteLine("Connected To " + data.szApplicationName);
        }

        public void SetData(Enum definition, uint objectID, object data)
        {
            instance?.SetDataOnSimObject(definition, objectID, SIMCONNECT_DATA_SET_FLAG.DEFAULT, data);
        }
        public void AddToDataDefinition<T>(Enum definition, params (string name, string unit, SIMCONNECT_DATATYPE dataType)[] objects)
        {
            if (objects == null)
                return;
            foreach (var obj in objects)
            {
                instance?.AddToDataDefinition(definition, obj.name, obj.unit, obj.dataType, 0, SimConnect.SIMCONNECT_UNUSED);
            }
            instance?.RegisterDataDefineStruct<T>(definition);
        }

        public void StartDataTransfer()
        {
            if (instance == null)
                throw new NullReferenceException("Cannott use null sim connect instance.");
            instance.RequestDataOnSimObject(Requests.PlaneLocation, Definitions.LocationStruct, 0, SIMCONNECT_PERIOD.SIM_FRAME, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0,0,0);
        }

        public void PrepareData()
        {
            if (instance == null)
                return;

            instance.AddToDataDefinition(Definitions.LocationStruct, "PLANE ALTITUDE", "Feet", SIMCONNECT_DATATYPE.FLOAT64, 0, SimConnect.SIMCONNECT_UNUSED);
            instance.AddToDataDefinition(Definitions.LocationStruct, "PLANE LONGITUDE", "Feet", SIMCONNECT_DATATYPE.FLOAT64, 0, SimConnect.SIMCONNECT_UNUSED);
            instance.AddToDataDefinition(Definitions.LocationStruct, "PLANE LATITUDE", "Feet", SIMCONNECT_DATATYPE.FLOAT64, 0, SimConnect.SIMCONNECT_UNUSED);

            //Going to add more soon.

            instance.RegisterDataDefineStruct<PositionStruct>(Definitions.LocationStruct);
        }
    }
}
