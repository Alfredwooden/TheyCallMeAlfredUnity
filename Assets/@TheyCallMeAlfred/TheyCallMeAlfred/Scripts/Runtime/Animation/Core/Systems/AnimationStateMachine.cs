using System;
using System.Collections.Generic;
using UnityEngine;

namespace AlfredWooden.Animation {
    /// <summary>
    /// Internal config for enum-based state machine.
    /// </summary>
    [Serializable]
    internal class EnumStateConfig {
        public string StateName;
        public float TransitionDuration = 0.1f;
        public bool CanBeInterrupted = true;
        public bool IsLooping = true;
        public float SpeedMultiplier = 1f;
    }

    /// <summary>
    /// Generic animation state machine that works with any enum type.
    /// Maps enum values to animation states and handles transitions.
    ///
    /// For a simpler string-based approach, use CharacterAnimator instead.
    /// </summary>
    /// <typeparam name="TState">An enum type representing animation states.</typeparam>
    public abstract class AnimationStateMachine<TState> : MonoBehaviour where TState : Enum {
        [Header("Animation Controller")]
        [SerializeField] protected AnimationController _animationController;

        [Header("Debug")]
        [SerializeField] private bool _logTransitions = false;

        private TState _currentState;
        private TState _previousState;
        private bool _isLocked;
        private float _lockEndTime;

        private readonly Dictionary<TState, EnumStateConfig> _stateConfigs = new();
        private readonly Dictionary<TState, int> _stateHashes = new();

        /// <summary>
        /// The current animation state.
        /// </summary>
        public TState CurrentState => _currentState;

        /// <summary>
        /// The previous animation state.
        /// </summary>
        public TState PreviousState => _previousState;

        /// <summary>
        /// Whether the state machine is locked (cannot change states).
        /// </summary>
        public bool IsLocked => _isLocked && Time.time < _lockEndTime;

        /// <summary>
        /// Event fired when state changes.
        /// </summary>
        public event Action<TState, TState> OnStateChanged;

        protected virtual void Awake() {
            if (_animationController == null) {
                _animationController = GetComponent<AnimationController>();
            }

            if (_animationController == null) {
                Debug.LogError($"[AnimationStateMachine] No AnimationController found on {gameObject.name}");
            }

            // Initialize state configurations
            InitializeStates();

            // Pre-cache all state hashes
            foreach (var kvp in _stateConfigs) {
                _stateHashes[kvp.Key] = _animationController?.GetStateHash(kvp.Value.StateName) ?? 0;
            }
        }

        protected virtual void Start() {
            // Set initial state to first enum value
            var values = Enum.GetValues(typeof(TState));
            if (values.Length > 0) {
                _currentState = (TState)values.GetValue(0);
                PlayCurrentState();
            }
        }

        protected virtual void Update() {
            // Clear lock if time has passed
            if (_isLocked && Time.time >= _lockEndTime) {
                _isLocked = false;
            }
        }

        /// <summary>
        /// Override this to register all animation states.
        /// Call RegisterState for each enum value.
        /// </summary>
        protected abstract void InitializeStates();

        /// <summary>
        /// Register a state with its configuration.
        /// </summary>
        protected void RegisterState(TState state, string animatorStateName,
            float transitionDuration = 0.1f, bool canBeInterrupted = true,
            bool isLooping = true, float speedMultiplier = 1f) {

            _stateConfigs[state] = new EnumStateConfig {
                StateName = animatorStateName,
                TransitionDuration = transitionDuration,
                CanBeInterrupted = canBeInterrupted,
                IsLooping = isLooping,
                SpeedMultiplier = speedMultiplier
            };
        }

        /// <summary>
        /// Request a state change. Returns true if the change was successful.
        /// </summary>
        public bool RequestStateChange(TState newState) {
            // Don't change to same state
            if (EqualityComparer<TState>.Default.Equals(_currentState, newState)) {
                return false;
            }

            // Check if locked
            if (IsLocked) {
                if (_logTransitions) {
                    Debug.Log($"[AnimationStateMachine] {gameObject.name}: State change to {newState} blocked (locked)");
                }
                return false;
            }

            // Check if current state can be interrupted
            if (_stateConfigs.TryGetValue(_currentState, out var currentConfig)) {
                if (!currentConfig.CanBeInterrupted) {
                    if (_logTransitions) {
                        Debug.Log($"[AnimationStateMachine] {gameObject.name}: State change to {newState} blocked (cannot interrupt {_currentState})");
                    }
                    return false;
                }
            }

            // Perform state change
            _previousState = _currentState;
            _currentState = newState;

            PlayCurrentState();

            if (_logTransitions) {
                Debug.Log($"[AnimationStateMachine] {gameObject.name}: {_previousState} -> {_currentState}");
            }

            OnStateChanged?.Invoke(_previousState, _currentState);
            return true;
        }

        /// <summary>
        /// Force a state change, ignoring locks and interrupt flags.
        /// </summary>
        public void ForceStateChange(TState newState) {
            _isLocked = false;
            _previousState = _currentState;
            _currentState = newState;
            PlayCurrentState();
            OnStateChanged?.Invoke(_previousState, _currentState);
        }

        /// <summary>
        /// Lock the state machine for a duration, preventing state changes.
        /// </summary>
        public void LockForDuration(float duration) {
            _isLocked = true;
            _lockEndTime = Time.time + duration;
        }

        /// <summary>
        /// Unlock the state machine immediately.
        /// </summary>
        public void Unlock() {
            _isLocked = false;
        }

        private void PlayCurrentState() {
            if (_animationController == null) return;

            if (_stateConfigs.TryGetValue(_currentState, out var config)) {
                _animationController.SetSpeed(config.SpeedMultiplier);

                if (_stateHashes.TryGetValue(_currentState, out int hash)) {
                    _animationController.CrossFade(hash, config.TransitionDuration);
                } else {
                    _animationController.CrossFade(config.StateName, config.TransitionDuration);
                }
            }
        }

        /// <summary>
        /// Check if a state can be interrupted.
        /// </summary>
        public bool CanInterrupt(TState state) {
            return _stateConfigs.TryGetValue(state, out var config) && config.CanBeInterrupted;
        }

        /// <summary>
        /// Check if currently in a specific state.
        /// </summary>
        public bool IsInState(TState state) {
            return EqualityComparer<TState>.Default.Equals(_currentState, state);
        }

        /// <summary>
        /// Check if previously was in a specific state.
        /// </summary>
        public bool WasInState(TState state) {
            return EqualityComparer<TState>.Default.Equals(_previousState, state);
        }
    }
}
