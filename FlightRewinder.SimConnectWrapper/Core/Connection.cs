using FlightRewinder.Data.Enums;
using FlightRewinderData.Enums;
using FlightRewinderData.StructAttributes;
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
    public partial class Connection
    {
        public SimConnect? instance;
        public bool Alive;
        public uint RequestCount = 0;
        public static readonly uint UserPlane = SimConnect.SIMCONNECT_OBJECT_ID_USER;

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

            instance.SubscribeToSystemEvent(Events.FRAME, "Frame");

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
        /// <summary>
        /// Creates an AI Aircraft that can be modified. Returns the aircraft ID.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public uint CreatePlane(string title, PositionStruct position)
        {
            var request = Requests.CreatePlane + RequestCount;
            RequestCount = (RequestCount++) % 10000;
            try
            {
                SIMCONNECT_DATA_INITPOSITION initPosition = new SIMCONNECT_DATA_INITPOSITION()
                {
                    Altitude = position.Altitude,
                    Longitude = position.Longitude,
                    Latitude = position.Latitude,
                    Bank = position.Bank,
                    Heading = position.Heading,
                    Pitch = position.Pitch,
                };
                instance?.AICreateNonATCAircraft(title, "Rewind", initPosition, request);
                return RequestCount;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                throw;
            }
        }

        public void SetData(uint aircraftID, Enum definition, object dataToSet)
        {
            try
            {
                instance?.SetDataOnSimObject(definition, aircraftID, SIMCONNECT_DATA_SET_FLAG.DEFAULT, dataToSet);
            } 
            catch (Exception)
            {
                Console.WriteLine("Error while setting data.");
                return;
            }
        }

        public void RemovePlane(uint aircraftID)
        {
            try
            {
                instance?.AIRemoveObject(aircraftID, Requests.RemovePlane);
            }
            catch (Exception) 
            {
                Console.WriteLine("Error while removing plane, possibly missing aircraftID");
                return;
            }
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

        public void AddToDataDefinition<T>(Enum definition, List<DefinitionAttribute> arrays)
        {
            if (arrays == null)
                throw new ArgumentNullException(nameof(arrays), "Cannot use null arrays.");
            foreach (var obj in arrays)
            {
                instance?.AddToDataDefinition(definition, obj.VariableName, obj.Unit, obj.DataType, 0, SimConnect.SIMCONNECT_UNUSED);
            }
            instance?.RegisterDataDefineStruct<T>(definition);
        }

        public void StartDataTransfer()
        {
            if (instance == null)
                throw new NullReferenceException("Cannott use null sim connect instance.");
            instance.RequestDataOnSimObject(Requests.PlaneLocation, Definitions.LocationStruct, 0, SIMCONNECT_PERIOD.SIM_FRAME, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
        }

        public void PrepareData()
        {
            if (instance == null)
                return;
            List<DefinitionAttribute> attributes = PositionStruct.GetAllAttributes();
            AddToDataDefinition<PositionStruct>(Definitions.LocationStruct, attributes);
            AddToDataDefinition<PositionStruct>(Definitions.SetLocation, attributes);
        }
    }
}
