using System;
using System.Collections.Generic;
using UnityEngine;

namespace AlfredWooden.Animation {
    /// <summary>
    /// Advanced animation state machine that uses state classes with lifecycle methods.
    /// Each state is a class instance with EnterState/UpdateState/ExitState/GetNextState.
    /// Use this when you need complex state logic (combos, timing, hitboxes, etc.).
    /// </summary>
    /// <typeparam name="TState">An enum type representing animation states.</typeparam>
    public abstract class AnimationStateMachineAdvanced<TState> : MonoBehaviour where TState : Enum {
        [Header("Animation Controller")]
        [SerializeField] protected AnimationController _animationController;

        [Header("Settings")]
        [SerializeField] private bool _autoTransition = true;
        [SerializeField] private bool _logTransitions = false;

        protected readonly Dictionary<TState, AnimationState<TState>> States = new();

        private AnimationState<TState> _currentState;
        private AnimationState<TState> _previousState;
        private bool _isLocked;
        private float _lockEndTime;

        /// <summary>
        /// The animation controller being used.
        /// </summary>
        public AnimationController AnimController => _animationController;

        /// <summary>
        /// The current state object.
        /// </summary>
        public AnimationState<TState> CurrentState => _currentState;

        /// <summary>
        /// The current state key.
        /// </summary>
        public TState CurrentStateKey => _currentState != null ? _currentState.StateKey : default;

        /// <summary>
        /// Whether the state machine is locked.
        /// </summary>
        public bool IsLocked => _isLocked && Time.time < _lockEndTime;

        /// <summary>
        /// Event fired when state changes. (previousState, newState)
        /// </summary>
        public event Action<TState, TState> OnStateChanged;

        protected virtual void Awake() {
            if (_animationController == null) {
                _animationController = GetComponent<AnimationController>();
            }

            if (_animationController == null) {
                Debug.LogError($"[AnimationStateMachineAdvanced] No AnimationController on {gameObject.name}");
            }
        }

        protected virtual void Start() {
            InitializeStates();

            // Set state machine reference on all states
            foreach (var state in States.Values) {
                state.SetStateMachine(this);
            }

            // Start with first state if not set
            if (_currentState == null && States.Count > 0) {
                var enumerator = States.Values.GetEnumerator();
                if (enumerator.MoveNext()) {
                    _currentState = enumerator.Current;
                    _currentState.EnterState();
                }
            }
        }

        protected virtual void Update() {
            // Clear lock if time passed
            if (_isLocked && Time.time >= _lockEndTime) {
                _isLocked = false;
            }

            if (_currentState == null) return;

            _currentState.UpdateState();

            // Auto-transition based on GetNextState
            if (_autoTransition) {
                TState nextState = _currentState.GetNextState();
                if (!EqualityComparer<TState>.Default.Equals(nextState, _currentState.StateKey)) {
                    RequestStateChange(nextState);
                }
            }
        }

        protected virtual void FixedUpdate() {
            _currentState?.FixedUpdateState();
        }

        /// <summary>
        /// Override this to register all animation states.
        /// Call AddState for each state.
        /// </summary>
        protected abstract void InitializeStates();

        /// <summary>
        /// Add a state to the state machine.
        /// </summary>
        protected void AddState(TState key, AnimationState<TState> state) {
            States[key] = state;
            state.SetStateMachine(this);
        }

        /// <summary>
        /// Request a state change. Returns true if successful.
        /// </summary>
        public bool RequestStateChange(TState newState) {
            if (_currentState != null &&
                EqualityComparer<TState>.Default.Equals(_currentState.StateKey, newState)) {
                return false;
            }

            if (IsLocked) {
                if (_logTransitions) {
                    Debug.Log($"[AnimStateMachine] {gameObject.name}: {newState} blocked (locked)");
                }
                return false;
            }

            if (_currentState != null && !_currentState.CanBeInterrupted) {
                if (_logTransitions) {
                    Debug.Log($"[AnimStateMachine] {gameObject.name}: {newState} blocked (cannot interrupt)");
                }
                return false;
            }

            if (!States.TryGetValue(newState, out var nextState)) {
                Debug.LogWarning($"[AnimStateMachine] State {newState} not found!");
                return false;
            }

            PerformTransition(nextState);
            return true;
        }

        /// <summary>
        /// Force a state change, ignoring locks and interrupt flags.
        /// </summary>
        public void ForceStateChange(TState newState) {
            if (!States.TryGetValue(newState, out var nextState)) {
                Debug.LogWarning($"[AnimStateMachine] State {newState} not found!");
                return;
            }

            _isLocked = false;
            PerformTransition(nextState);
        }

        private void PerformTransition(AnimationState<TState> nextState) {
            TState prevKey = _currentState != null ? _currentState.StateKey : default;

            _previousState = _currentState;
            _currentState?.ExitState();

            _currentState = nextState;
            _currentState.EnterState();

            if (_logTransitions) {
                Debug.Log($"[AnimStateMachine] {gameObject.name}: {prevKey} -> {_currentState.StateKey}");
            }

            OnStateChanged?.Invoke(prevKey, _currentState.StateKey);
        }

        /// <summary>
        /// Lock the state machine for a duration.
        /// </summary>
        public void LockForDuration(float duration) {
            _isLocked = true;
            _lockEndTime = Time.time + duration;
        }

        /// <summary>
        /// Unlock the state machine.
        /// </summary>
        public void Unlock() {
            _isLocked = false;
        }

        /// <summary>
        /// Check if currently in a specific state.
        /// </summary>
        public bool IsInState(TState state) {
            return _currentState != null &&
                   EqualityComparer<TState>.Default.Equals(_currentState.StateKey, state);
        }

        /// <summary>
        /// Get a state object by key.
        /// </summary>
        public AnimationState<TState> GetState(TState key) {
            return States.TryGetValue(key, out var state) ? state : null;
        }

        /// <summary>
        /// Get a state object cast to a specific type.
        /// </summary>
        public T GetState<T>(TState key) where T : AnimationState<TState> {
            return GetState(key) as T;
        }
    }
}
