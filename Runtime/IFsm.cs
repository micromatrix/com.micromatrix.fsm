using System;
using System.Collections.Generic;

namespace MM.FSM
{
    public interface IFsm<T> where T : class
    {
        string name { get; }
        string fullName { get; }
        T owner { get; }
        Type ownerType { get; }
        int fsmStateCount { get; }
        bool isRunning { get; }
        bool isDestroyed { get; }
        IFsmState<T> currentState { get; }
        string currentStateName { get; }
        float currentStateTime { get; }
        void Start<TState>() where TState : IFsmState<T>;
        void Start(Type stateType);
        bool HasState<TState>() where TState : IFsmState<T>;
        bool HasState(Type stateType);
        TState GetState<TState>() where TState : IFsmState<T>;
        IFsmState<T> GetState(Type stateType);
        IFsmState<T>[] GetAllStates();
        void GetAllStates(List<IFsmState<T>> results);
        void ChangeState<TState>() where TState : IFsmState<T>;
        void ChangeState(Type stateType);
    }
}
