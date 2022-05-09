using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azimuth.ClientWrapper.Logic.Args
{
    public class StateChangedEventArgs : EventArgs
    {
        public StateMachine.State OldState;
        public string OldStateName => OldState.ToString();
        public StateMachine.State State;
        public string StateName => State.ToString();

        public StateChangedEventArgs(StateMachine.State state, StateMachine.State oldState)
        {
            State = state;
            OldState = oldState;
        }
    }
}
