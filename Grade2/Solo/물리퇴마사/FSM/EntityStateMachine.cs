using System;
using System.Collections.Generic;
using System.Diagnostics;
using _01Scripts.Entities;

namespace _01Scripts.FSM
{
    public class EntityStateMachine
    {
        public EntityState CurrentState { get; private set; }

        private Dictionary<string, EntityState> _states;

        public EntityStateMachine(Entity entity, StateDataSO[] stateList)
        {
            _states = new Dictionary<string, EntityState>();
            foreach (StateDataSO state in stateList)
            {
                Type type = Type.GetType(state.className);
                Debug.Assert(type != null, $"Finding type is null : {state.className}");
                EntityState entityState = Activator.CreateInstance(type, entity, state.aniamtionHash)
                                                                                    as EntityState;

                _states.Add(state.stateName, entityState);
            }
        }

        public void ChangeState(string newStateName, bool forced = false)
        {
            CurrentState?.Exit();
            EntityState newState = _states.GetValueOrDefault(newStateName);
            Debug.Assert(newState != null, $"State is null {newStateName}");

            if(!forced && CurrentState ==newState)
                return;
            
            CurrentState = newState;
            CurrentState.Enter();
        }

        public void ResetStates()
        {
            foreach (var state in _states)
            {
                state.Value.Reset();
            }
        }
        
        public void UpdateStateMachine()
        {
            CurrentState?.Update();
        }
    }
}

