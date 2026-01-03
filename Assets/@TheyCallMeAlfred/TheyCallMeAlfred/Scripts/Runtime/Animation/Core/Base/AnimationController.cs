using System;
using System.Collections.Generic;
using UnityEngine;

namespace AlfredWooden.Animation {
    /// <summary>
    /// Base animation controller that wraps Unity's Animator.
    /// Provides a clean API for playing animations and tracking state.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimationController : MonoBehaviour, IAnimationController {
        [Header("Settings")]
        [SerializeField] private float _defaultTransitionDuration = 0.1f;
        [SerializeField] private bool _logStateChanges = false;

        private Animator _animator;
        private readonly Dictionary<string, int> _stateHashCache = new();
        private readonly Dictionary<string, int> _paramHashCache = new();

        private string _currentStateName = "";
        private string _previousStateName = "";
        private int _currentStateHash;
        private bool _isTransitioning;

        public Animator Animator => _animator;
        public bool IsPlaying => !string.IsNullOrEmpty(_currentStateName);
        public string CurrentStateName => _currentStateName;
        public float DefaultTransitionDuration => _defaultTransitionDuration;

        public event Action<string> OnAnimationStarted;
        public event Action<string> OnAnimationCompleted;

        protected virtual void Awake() {
            _animator = GetComponent<Animator>();
            if (_animator == null) {
                Debug.LogError($"[AnimationController] No Animator found on {gameObject.name}");
            }
        }

        protected virtual void Update() {
            if (_animator == null) return;
            TrackAnimationState();
        }

        private void TrackAnimationState() {
            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

            // Check if we're in a new state
            if (stateInfo.fullPathHash != _currentStateHash) {
                _previousStateName = _currentStateName;
                _currentStateHash = stateInfo.fullPathHash;

                // Try to find the state name from our cache
                string newStateName = FindStateNameByHash(_currentStateHash);
                if (!string.IsNullOrEmpty(newStateName)) {
                    _currentStateName = newStateName;

                    if (!string.IsNullOrEmpty(_previousStateName)) {
                        OnAnimationCompleted?.Invoke(_previousStateName);
                    }
                    OnAnimationStarted?.Invoke(_currentStateName);

                    if (_logStateChanges) {
                        Debug.Log($"[AnimationController] {gameObject.name}: {_previousStateName} -> {_currentStateName}");
                    }
                }
            }

            _isTransitioning = _animator.IsInTransition(0);
        }

        private string FindStateNameByHash(int hash) {
            foreach (var kvp in _stateHashCache) {
                if (kvp.Value == hash) return kvp.Key;
            }
            return null;
        }

        #region Play Methods

        public void Play(string stateName, int layer = 0) {
            if (_animator == null || string.IsNullOrEmpty(stateName)) return;

            int hash = GetStateHash(stateName);
            _animator.Play(hash, layer);
            _currentStateName = stateName;
            _currentStateHash = hash;
        }

        public void Play(int stateHash, int layer = 0) {
            if (_animator == null) return;
            _animator.Play(stateHash, layer);
            _currentStateHash = stateHash;
            _currentStateName = FindStateNameByHash(stateHash) ?? "";
        }

        public void CrossFade(string stateName, float transitionDuration, int layer = 0) {
            if (_animator == null || string.IsNullOrEmpty(stateName)) return;

            int hash = GetStateHash(stateName);
            _animator.CrossFade(hash, transitionDuration, layer);
            _currentStateName = stateName;
            _currentStateHash = hash;
        }

        public void CrossFade(int stateHash, float transitionDuration, int layer = 0) {
            if (_animator == null) return;
            _animator.CrossFade(stateHash, transitionDuration, layer);
            _currentStateHash = stateHash;
            _currentStateName = FindStateNameByHash(stateHash) ?? "";
        }

        /// <summary>
        /// CrossFade using the default transition duration.
        /// </summary>
        public void CrossFade(string stateName, int layer = 0) {
            CrossFade(stateName, _defaultTransitionDuration, layer);
        }

        #endregion

        #region Parameter Methods

        public void SetFloat(string paramName, float value) {
            if (_animator == null) return;
            _animator.SetFloat(GetParamHash(paramName), value);
        }

        public void SetFloat(int paramHash, float value) {
            if (_animator == null) return;
            _animator.SetFloat(paramHash, value);
        }

        public void SetBool(string paramName, bool value) {
            if (_animator == null) return;
            _animator.SetBool(GetParamHash(paramName), value);
        }

        public void SetBool(int paramHash, bool value) {
            if (_animator == null) return;
            _animator.SetBool(paramHash, value);
        }

        public void SetTrigger(string paramName) {
            if (_animator == null) return;
            _animator.SetTrigger(GetParamHash(paramName));
        }

        public void SetTrigger(int paramHash) {
            if (_animator == null) return;
            _animator.SetTrigger(paramHash);
        }

        public void ResetTrigger(string paramName) {
            if (_animator == null) return;
            _animator.ResetTrigger(GetParamHash(paramName));
        }

        public void ResetTrigger(int paramHash) {
            if (_animator == null) return;
            _animator.ResetTrigger(paramHash);
        }

        public float GetFloat(string paramName) {
            if (_animator == null) return 0f;
            return _animator.GetFloat(GetParamHash(paramName));
        }

        public bool GetBool(string paramName) {
            if (_animator == null) return false;
            return _animator.GetBool(GetParamHash(paramName));
        }

        #endregion

        #region Hash Caching

        /// <summary>
        /// Get or create a cached hash for a state name.
        /// </summary>
        public int GetStateHash(string stateName) {
            if (!_stateHashCache.TryGetValue(stateName, out int hash)) {
                hash = Animator.StringToHash(stateName);
                _stateHashCache[stateName] = hash;
            }
            return hash;
        }

        /// <summary>
        /// Get or create a cached hash for a parameter name.
        /// </summary>
        public int GetParamHash(string paramName) {
            if (!_paramHashCache.TryGetValue(paramName, out int hash)) {
                hash = Animator.StringToHash(paramName);
                _paramHashCache[paramName] = hash;
            }
            return hash;
        }

        /// <summary>
        /// Pre-cache state hashes for better performance.
        /// Call this in Start() with all your state names.
        /// </summary>
        public void CacheStateHashes(params string[] stateNames) {
            foreach (var name in stateNames) {
                GetStateHash(name);
            }
        }

        /// <summary>
        /// Pre-cache parameter hashes for better performance.
        /// </summary>
        public void CacheParamHashes(params string[] paramNames) {
            foreach (var name in paramNames) {
                GetParamHash(name);
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Check if the animator is currently in a specific state.
        /// </summary>
        public bool IsInState(string stateName, int layer = 0) {
            if (_animator == null) return false;
            return _animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName);
        }

        /// <summary>
        /// Check if the animator is currently in a specific state by hash.
        /// </summary>
        public bool IsInState(int stateHash, int layer = 0) {
            if (_animator == null) return false;
            return _animator.GetCurrentAnimatorStateInfo(layer).fullPathHash == stateHash;
        }

        /// <summary>
        /// Get the normalized time (0-1) of the current animation.
        /// </summary>
        public float GetNormalizedTime(int layer = 0) {
            if (_animator == null) return 0f;
            return _animator.GetCurrentAnimatorStateInfo(layer).normalizedTime;
        }

        /// <summary>
        /// Check if the animator is currently transitioning between states.
        /// </summary>
        public bool IsTransitioning(int layer = 0) {
            if (_animator == null) return false;
            return _animator.IsInTransition(layer);
        }

        /// <summary>
        /// Set the animator speed multiplier.
        /// </summary>
        public void SetSpeed(float speed) {
            if (_animator == null) return;
            _animator.speed = speed;
        }

        /// <summary>
        /// Get the current animator speed multiplier.
        /// </summary>
        public float GetSpeed() {
            if (_animator == null) return 1f;
            return _animator.speed;
        }

        #endregion
    }
}
