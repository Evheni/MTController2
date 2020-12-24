using System;
using System.Collections.Generic;
using System.Text;

namespace MTController2.ExtraClasses
{
    // ReSharper disable InconsistentNaming
    public enum ProcessState { INITIALIZATION, WORKING, STOPPING, PAUSED, PAUSING, DEINITIALIZATION  }
    // ReSharper restore InconsistentNaming

    public class ControllerStateManager
    {
        private ProcessState _state;
        //private readonly Dictionary<string, string> _stateAliases;

        public ControllerStateManager()
        {
            _state = ProcessState.INITIALIZATION;
            //_stateAliases = new Dictionary<string, string>();

            //_stateAliases.Add(ProcessState.INITIALIZATION.ToString(), Common.ProcessInitialization);
            //_stateAliases.Add(ProcessState.WORKING.ToString(), Common.WorkingS);
            //_stateAliases.Add(ProcessState.STOPPED.ToString(), Common.StopS);
            //_stateAliases.Add(ProcessState.STOPPING.ToString(), Common.ProcessStopping);
            //_stateAliases.Add(ProcessState.PAUSED.ToString(), Common.Paused);
            //_stateAliases.Add(ProcessState.PAUSING.ToString(), Common.ProcessPausing);
            ////StateAliases.Add(ProcessState.RESUMING.ToString(), "Возобновление работы");
            //_stateAliases.Add(ProcessState.DEINITIALIZATION.ToString(), Common.Deinitialization);
        }

        public ProcessState GetState()
        {
            return _state;
        }

        public void SetState(ProcessState state)
        {
            _state = state;
        }

        //public string GetStateAsString()
        //{
        //    if (!_stateAliases.ContainsKey(_state.ToString()))
        //    {
        //        return _state.ToString();
        //    }

        //    return _stateAliases[_state.ToString()];
        //}
    }
}
