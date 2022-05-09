using Azimuth.ClientWrapper.Logic.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azimuth.ClientWrapper.Logic
{
    public abstract class CoreStateMachine
    {
        public event EventHandler<StateChangedEventArgs>? StateChangedEvent;
        public Dictionary<StateMachine.Event, Dictionary<StateMachine.State, Transition>> RegisteredTransitions = new Dictionary<StateMachine.Event, Dictionary<StateMachine.State, Transition>>();
        public StateMachine.State CurrentState { get; private set; } = StateMachine.State.Start;

        public CoreStateMachine()
        {
        }


        public async Task<bool> TransitionAsync(StateMachine.Event eventToTransition)
        {
            if (!RegisteredTransitions.ContainsKey(eventToTransition))
                throw new InvalidOperationException("Cannot tranfer to unregistered event.");
            if (RegisteredTransitions[eventToTransition].ContainsKey(CurrentState))
            {
                var trans = RegisteredTransitions[eventToTransition][CurrentState];
                if (trans.ViaEvents != null)
                {
                    return await MultiEventTransition(eventToTransition, trans.ViaEvents, trans.ToState);
                }
                else
                {
                    return await SingleEventTransition(trans);
                }

            }
            else
            {
                throw new InvalidOperationException($"Cannot transition from {CurrentState} with {eventToTransition}");
            }
        }

        private async Task<bool> SingleEventTransition(Transition transition)
        {
            var originalState = CurrentState;
            var successful = true;

            var newState = await transition.Execute();

            if (newState.HasValue)
            {
                CurrentState = newState.Value;
                StateChangedEvent?.Invoke(this, new(CurrentState, originalState));
            }
            else
            {
                successful = false;
            }

            return successful;
        }

        private async Task<bool> MultiEventTransition(StateMachine.Event originatingEvent, StateMachine.Event[] viaEvents, StateMachine.State newState)
        {
            var originalState = CurrentState;
            var successful = true;

            foreach(var via in viaEvents)
            {
                var oldState = CurrentState;
                successful = await TransitionAsync(via);
                if (!successful)
                {
                    await RevertPreviousState(originalState);
                    return false;
                }
            }
            CurrentState = newState;
            StateChangedEvent?.Invoke(this, new(CurrentState, originalState));
            return successful;
        }

        private async Task RevertPreviousState(StateMachine.State originalState)
        {
            foreach(var even in RegisteredTransitions)
            {
                foreach(var transition in even.Value)
                {
                    if(transition.Value.ToState == originalState && transition.Value.FromState == CurrentState && transition.Value.Via == null)
                    {
                        await TransitionAsync(transition.Value.Cause);
                        return;
                    }
                }
            }

            //This isn't safe, but whatever.
            CurrentState = originalState;
            StateChangedEvent?.Invoke(this, new(CurrentState, originalState));
        }

        public void Register(Transition transition)
        {
            if (transition == null)
                throw new ArgumentNullException("Cannot register null transition");
            if (!RegisteredTransitions.ContainsKey(transition.Cause))
            {
                RegisteredTransitions.Add(transition.Cause, new Dictionary<StateMachine.State, Transition>());
            }
            if (RegisteredTransitions[transition.Cause].ContainsKey(transition.FromState))
            {
                throw new InvalidOperationException("Dictionary already contains transition definition.");
            }
            RegisteredTransitions[transition.Cause].Add(transition.FromState, transition);
        }
    }
}
