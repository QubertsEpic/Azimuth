using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azimuth.ClientWrapper.Logic
{
    public class Transition
    {
        public StateMachine.State FromState;
        public StateMachine.Event Cause;
        public List<object> Actions = new List<object>();
        public StateMachine.Event[]? ViaEvents;
        public StateMachine.State ToState;

        public Transition Event(StateMachine.Event e)
        {
            Cause = e;
            return this;
        }

        public Transition From(StateMachine.State state)
        {
            FromState = state;
            return this;
        }

        public Transition Do(Func<bool> function)
        {
            Actions.Add(function);
            return this;
        }

        public Transition Do(Func<Task<bool>> function)
        {
            Actions.Add(function);
            return this;
        }

        public Transition Via(params StateMachine.Event[] events)
        {
            ViaEvents = events;
            return this;
        }

        public Transition To(StateMachine.State endState)
        {
            ToState = endState;
            return this;
        }

        public async Task<StateMachine.State?> Execute()
        {
            if (ViaEvents != null)
                throw new InvalidOperationException("This process cannot be completed within this transition.");
            foreach (var action in Actions)
            {
                switch (action)
                {
                    case Func<bool> function:
                        if (!function())
                            return null;
                        break;
                    case Func<Task<bool>> task:
                        if (!await task())
                            return null;
                        break;
                }
            }
            return ToState;
        }
    }
}
