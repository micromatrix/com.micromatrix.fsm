using System;

namespace MM.FSM
{
    public interface IFsmState<T> where T : class
    {
        void OnInit(IFsm<T> fsm);
        void OnEnter(IFsm<T> fsm);
        void OnUpdate(IFsm<T> fsm);
        void OnExit(IFsm<T> fsm, bool isShutdown);
        void OnDestroy(IFsm<T> fsm);
        void ChangeState<TState>(IFsm<T> fsm) where TState : FsmState<T>;
        void ChangeState(IFsm<T> fsm, Type stateType);
        bool CanTransition(IFsm<T> fsm, IFsmState<T> state);
    }
}