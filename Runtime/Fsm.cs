using System;
using System.Collections.Generic;

namespace MM.FSM
{
    public sealed class Fsm<T> : IFsm<T>, IDisposable where T : class
    {
        private string _name;
        private T _owner;
        private readonly Dictionary<Type, IFsmState<T>> _states = new();
        private IFsmState<T> _currentState;
        private float _currentStateTime;
        private bool _isDestroyed = true;

        public string name
        {
            get => _name;
            private set => _name = value ?? string.Empty;
        }

        public string fullName => $"{ownerType.FullName}.{name}";
        public T owner => _owner;
        public Type ownerType => typeof(T);
        public int fsmStateCount => _states.Count;
        public bool isRunning => _currentState != null;
        public bool isDestroyed => _isDestroyed;
        public IFsmState<T> currentState => _currentState;
        public string currentStateName => _currentState != null ? _currentState.GetType().FullName : null;
        public float currentStateTime => _currentStateTime;

        public static Fsm<T> Create(string name, T owner, params IFsmState<T>[] states)
        {
            if (owner == null)
                return null;

            if (states == null || states.Length < 1)
                return null;

            var fsm = new Fsm<T>
            {
                name = name,
                _owner = owner,
                _isDestroyed = false
            };
            foreach (var state in states)
            {
                if (state == null)
                    continue;

                var stateType = state.GetType();
                if (fsm._states.ContainsKey(stateType))
                    continue;

                fsm._states.Add(stateType, state);
                state.OnInit(fsm);
            }
            return fsm;
        }

        public static Fsm<T> Create(string name, T owner, List<IFsmState<T>> states)
        {
            if (owner == null)
                return null;

            if (states == null || states.Count < 1)
                return null;

            var fsm = new Fsm<T>
            {
                name = name,
                _owner = owner,
                _isDestroyed = false
            };
            foreach (var state in states)
            {
                if (state == null)
                    continue;

                var stateType = state.GetType();
                if (fsm._states.ContainsKey(stateType))
                    continue;

                fsm._states.Add(stateType, state);
                state.OnInit(fsm);
            }

            return fsm;
        }

        public void Dispose()
        {
            _currentState?.OnExit(this, true);

            foreach (var state in _states)
            {
                state.Value.OnDestroy(this);
            }

            name = null;
            _owner = null;
            _states.Clear();

            _currentState = null;
            _currentStateTime = 0f;
            _isDestroyed = true;
        }

        public void Start<TState>() where TState : IFsmState<T>
        {
            if (isRunning)
                return;

            var state = GetState<TState>();
            if (state == null)
                return;

            _currentStateTime = 0f;
            _currentState = state;
            _currentState.OnEnter(this);
        }

        public void Start(Type stateType)
        {
            if (isRunning)
                return;

            if (stateType == null)
                return;

            if (!typeof(IFsmState<T>).IsAssignableFrom(stateType))
                return;

            var state = GetState(stateType);
            if (state == null)
                return;

            _currentStateTime = 0f;
            _currentState = state;
            _currentState.OnEnter(this);
        }

        public bool HasState<TState>() where TState : IFsmState<T>
        {
            return _states.ContainsKey(typeof(TState));
        }

        public bool HasState(Type stateType)
        {
            if (stateType == null)
                return false;

            if (!typeof(IFsmState<T>).IsAssignableFrom(stateType))
                return false;

            return _states.ContainsKey(stateType);
        }

        public TState GetState<TState>() where TState : IFsmState<T>
        {
            if (_states.TryGetValue(typeof(TState), out IFsmState<T> state))
            {
                return (TState)state;
            }

            return default(TState);
        }

        public IFsmState<T> GetState(Type stateType)
        {
            if (stateType == null)
                return null;

            if (!typeof(IFsmState<T>).IsAssignableFrom(stateType))
                return null;

            if (_states.TryGetValue(stateType, out IFsmState<T> state))
            {
                return state;
            }

            return null;
        }

        public IFsmState<T>[] GetAllStates()
        {
            var index = 0;
            var results = new IFsmState<T>[_states.Count];
            foreach (var state in _states)
            {
                results[index++] = state.Value;
            }

            return results;
        }

        public void GetAllStates(List<IFsmState<T>> results)
        {
            if (results == null)
                return;

            results.Clear();
            foreach (var state in _states)
            {
                results.Add(state.Value);
            }
        }

        public void Update(float elapseSeconds)
        {
            if (_currentState == null)
                return;

            _currentStateTime += elapseSeconds;
            _currentState.OnUpdate(this);
        }

        public void ChangeState<TState>() where TState : IFsmState<T>
        {
            ChangeState(typeof(TState));
        }

        public void ChangeState(Type stateType)
        {
            if (_currentState == null)
                return;

            var state = GetState(stateType);
            if (state == null)
                return;

            _currentState.OnExit(this, false);
            _currentStateTime = 0f;
            _currentState = state;
            _currentState.OnEnter(this);
        }
    }
}
