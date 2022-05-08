using Azimuth.RecordingLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azimuth.ClientWrapper.Logic
{
    public class StateMachine : CoreStateMachine
    {
        private readonly Recorder _recordingLogic;
        private readonly Rewind _rewindLogic;

        public enum State
        {
            Recording,
            Idle,
            Replaying,
            Start
        }

        public enum Event
        {
            Null,
            Record,
            StopRecording,
            Rewind,
            StopRewinding,
            RestartRecording,
            ResetRecordings
        }

        public StateMachine(Recorder recordingLogic, Rewind rewindingLogic)
        {
            _recordingLogic = recordingLogic;
            _rewindLogic = rewindingLogic;
            RegisterTransitions();
        }

        public void RegisterTransitions()
        {
            Register(new Transition().From(State.Start).To(State.Recording).Event(Event.Record).Do(StartRecording));
            Register(new Transition().From(State.Idle).To(State.Recording).Event(Event.Record).Do(StartRecording));
            Register(new Transition().From(State.Replaying).To(State.Recording).Event(Event.Record).Via((Event.StopRewinding), Event.Record));
            Register(new Transition().From(State.Recording).To(State.Recording).Event(Event.RestartRecording).Via(Event.StopRecording, Event.ResetRecordings, Event.Record));

            Register(new Transition().From(State.Idle).To(State.Replaying).Event(Event.Rewind).Do(StartRewinding));
            Register(new Transition().From(State.Recording).To(State.Replaying).Event(Event.Rewind).Via(Event.StopRecording, Event.Rewind));

            Register(new Transition().From(State.Idle).To(State.Idle).Event(Event.ResetRecordings).Do(ResetRecordings));
            Register(new Transition().From(State.Replaying).To(State.Idle).Event(Event.StopRewinding).Do(StopReplaying));
            Register(new Transition().From(State.Recording).To(State.Idle).Event(Event.StopRecording).Do(StopRecording));
        }

        private bool ResetRecordings()
        {
            _recordingLogic.ResetRecording();
            return true;
        }

        private bool StopRecording()
        {
            _recordingLogic.StopRecording();
            return true;
        }

        private bool StartRewinding()
        {
            var data = _recordingLogic.DumpData();
            if (data?.Frames != null)
            {
                _rewindLogic.LoadFrames(data.Frames);
                _rewindLogic.SeekRewind(data.Frames.Count - 1);
                _rewindLogic.StartRewind();
                return true;
            }
            return false;
        }

        private bool StopReplaying()
        {
            _rewindLogic.StopReplay();
            return true;
        }

        private bool StartRecording()
        {
            _recordingLogic.StartRecording();
            return true;
        }
    }
}
