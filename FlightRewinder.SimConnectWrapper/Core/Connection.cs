using FlightRewinder.Enums;
using FlightRewinder.Structs;
using FlightRewinder.StructAttributes;
using FlightRewinder.Data.Enums;
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
        public bool Alive => instance != null;
        public uint RequestCount = 0;
        public static readonly uint UserPlane = SimConnect.SIMCONNECT_OBJECT_ID_USER;

        public event EventHandler? Initialised;
        public event EventHandler? Quit;
        public event EventHandler? FrameEvent;
        public event EventHandler<SIMCONNECT_RECV_EVENT>? Event;
        public event EventHandler<LocationChangedEventArgs>? LocationChanged;

        public object lockObject = new();
        public const int WM_USER_SIMCONNECT = 0x0402;


        public Connection()
        {

        }

        public void Initialise(IntPtr handle)
        {
            lock (lockObject)
            {
                instance = new SimConnect("Flight Rewinder", handle, WM_USER_SIMCONNECT, null, 0);

                instance.OnRecvOpen += new SimConnect.RecvOpenEventHandler(Connection_OnOpen);
                instance.OnRecvQuit += new SimConnect.RecvQuitEventHandler(Connection_OnQuit);

                instance.OnRecvException += Instance_OnRecvException;
                instance.OnRecvSimobjectData += Instance_OnRecvSimobjectData;
                instance.OnRecvEventFrame += Instance_OnRecvEventFrame;
                instance.OnRecvEvent += Instance_OnRecvEvent;
                instance.AddToDataDefinition(Definitions.InitialPosition, "Initial Position", null, SIMCONNECT_DATATYPE.INITPOSITION, 0f, SimConnect.SIMCONNECT_UNUSED);

                instance.SubscribeToSystemEvent(Events.FRAME, "Frame");
                PrepareData();

                StartDataTransfer();

                Initialised?.Invoke(this, new EventArgs());
            }
        }

        private void Instance_OnRecvEvent(SimConnect sender, SIMCONNECT_RECV_EVENT data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data), "Cannot use null arguments.");
                Event?.Invoke(this, data);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message + "\n\nDisconnecting...");
                Dispose();
            }
        }

        private void Dispose()
        {
            if (instance != null)
            {
                instance.Dispose();
                instance = null;
            }
        }

        private void Instance_OnRecvEventFrame(SimConnect sender, SIMCONNECT_RECV_EVENT_FRAME data)
        {
            lock (lockObject)
            {
                FrameEvent?.Invoke(this, new());
            }
        }

        private void Instance_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            lock (lockObject)
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

        public void Init(uint planeID, PositionStruct position)
        {
            lock (lockObject)
            {
                instance?.SetDataOnSimObject(Definitions.InitialPosition, planeID, SIMCONNECT_DATA_SET_FLAG.DEFAULT, PositionStructOperators.PosStructToInitPos(position));
            }
        }

        private void Connection_OnQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            Close();
            Quit?.Invoke(this, new());
        }

        public void Close()
        {
            lock (lockObject)
            {
                if (instance != null)
                {
                    Console.WriteLine("Simconnect has exited.");
                    instance?.Dispose();
                    instance = null;
                }
                else
                {
                    Console.WriteLine("An error has occurred.");
                }
            }
        }

        private void Connection_OnOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            Console.WriteLine("Connected To " + data.szApplicationName);
        }
        /// <summary>
        /// Creates an AI Aircraft that can be modified. Returns the aircraft ID.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="initPosition"></param>
        /// <returns></returns>
        public uint CreatePlane(string title, PositionStruct initPosition)
        {
            lock (lockObject)
            {
                var request = Requests.CreatePlane + RequestCount;
                RequestCount = (RequestCount++) % 10000;
                try
                {
                    instance?.AICreateNonATCAircraft(title, "Rewind", PositionStructOperators.PosStructToInitPos(initPosition), request);
                    return RequestCount;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                    throw;
                }
            }
        }

        public void SetPos(uint aircraftID, PositionSetStruct dataToSet)
        {
            lock (lockObject)
            {
                try
                {
                    instance?.SetDataOnSimObject(Definitions.SetLocation, aircraftID, SIMCONNECT_DATA_SET_FLAG.DEFAULT, dataToSet);
                }
                catch (Exception)
                {
                    Console.WriteLine("Error while setting data.");
                    return;
                }
            }
        }

        public void RemovePlane(uint aircraftID)
        {
            lock (lockObject)
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
        }

        public void SetInputActivationStatus(Enum group, SIMCONNECT_STATE state)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group), "Cannot use null group");
            lock (lockObject)
            {
                instance?.SetInputGroupState(group, ((uint)state));
            }
        }

        public void MapClientEventToSimEvent(params (Enum eventId, string eventName)[] args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args), "Cannot use null argument.");
            foreach ((Enum id, string name) in args)
            {
                lock (lockObject)
                {
                    instance?.MapClientEventToSimEvent(id, name);
                }
            }
        }

        
        public void AddToDataDefinition<T>(Enum definition, params (string name, string unit, SIMCONNECT_DATATYPE dataType)[] objects)
        {
            lock (lockObject)
            {
                if (objects == null)
                    return;
                foreach (var obj in objects)
                {
                    instance?.AddToDataDefinition(definition, obj.name, obj.unit, obj.dataType, 0, SimConnect.SIMCONNECT_UNUSED);
                }
                instance?.RegisterDataDefineStruct<T>(definition);
            }
        }

        public void AddToDataDefinition<T>(Enum definition, List<DefinitionAttribute> arrays)
        {
            lock (lockObject)
            {
                if (arrays == null)
                    throw new ArgumentNullException(nameof(arrays), "Cannot use null arrays.");
                foreach (var obj in arrays)
                {
                    instance?.AddToDataDefinition(definition, obj.VariableName, obj.Unit, obj.DataType, 0, SimConnect.SIMCONNECT_UNUSED);
                }
                instance?.RegisterDataDefineStruct<T>(definition);
            }
        }

        public void StartDataTransfer()
        {
            lock (lockObject)
            {
                if (instance == null)
                    throw new NullReferenceException("Cannott use null sim connect instance.");
                instance.RequestDataOnSimObject(Requests.PlaneLocation, Definitions.LocationStruct, 0, SIMCONNECT_PERIOD.SIM_FRAME, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
            }
        }

        public void PrepareData()
        {
            if (instance == null)
                return;
            List<DefinitionAttribute> attributes = PositionStructOperators.GetAllAttributes();
            AddToDataDefinition<PositionStruct>(Definitions.LocationStruct, attributes);
            AddToDataDefinition<PositionSetStruct>(Definitions.SetLocation, attributes);
        }
    }
}
