using System;

namespace MM.FSM
{
    public class FsmState<T> : IFsmState<T> where T : class
    {
        public IFsm<T> owner { get; private set; }

        public virtual void OnInit(IFsm<T> fsm)
        {
            this.owner = fsm;
        }
        public virtual void OnEnter(IFsm<T> fsm) {}
        public virtual void OnUpdate(IFsm<T> fsm) {}
        public virtual void OnExit(IFsm<T> fsm, bool isShutdown) {}
        public virtual void OnDestroy(IFsm<T> fsm) {}

        public void ChangeState<TState>(IFsm<T> fsm) where TState : FsmState<T>
        {
            fsm?.ChangeState<TState>();
        }

        public void ChangeState(IFsm<T> fsm, Type stateType)
        {
            if (fsm == null)
                return;

            if (stateType == null)
                return;

            if (!typeof(FsmState<T>).IsAssignableFrom(stateType))
                return;

            fsm.ChangeState(stateType);
        }
        
        public virtual bool CanTransition(IFsm<T> fsm, IFsmState<T> state)
        {
            return true;
        }
    }
}