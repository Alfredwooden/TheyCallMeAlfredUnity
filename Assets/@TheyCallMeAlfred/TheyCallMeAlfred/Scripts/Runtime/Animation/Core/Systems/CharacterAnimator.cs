using System;
using System.Collections.Generic;
using UnityEngine;

namespace AlfredWooden.Animation {
    /// <summary>
    /// Flexible character animator that plays animations by key.
    /// Add animation configs in the Inspector to define available animations.
    ///
    /// Usage:
    ///   characterAnimator.Play("Idle");
    ///   characterAnimator.Play("Attack1");
    ///   characterAnimator.SetLocomotion(speed, "Idle", "Walk", "Run");
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimator : MonoBehaviour {
        [Header("Animations")]
        [Tooltip("List of available animations. Add configs here via Inspector.")]
        [SerializeField] private List<AnimationStateConfig> _animations = new();

        [Header("Settings")]
        [SerializeField] private float _defaultTransitionDuration = 0.15f;

        [Header("Debug")]
        [SerializeField] private bool _logStateChanges = false;

        private Animator _animator;
        private readonly Dictionary<string, AnimationStateConfig> _configLookup = new();
        private readonly Dictionary<string, int> _hashCache = new();

        private string _currentAnimationKey = "";
        private AnimationStateConfig _currentConfig;
        private bool _isLocked;
        private float _lockEndTime;

        /// <summary>
        /// The Unity Animator component.
        /// </summary>
        public Animator Animator => _animator;

        /// <summary>
        /// Currently playing animation key.
        /// </summary>
        public string CurrentAnimation => _currentAnimationKey;

        /// <summary>
        /// Whether the animator is locked from state changes.
        /// </summary>
        public bool IsLocked => _isLocked && Time.time < _lockEndTime;

        /// <summary>
        /// Event fired when animation changes. (previousKey, newKey)
        /// </summary>
        public event Action<string, string> OnAnimationChanged;

        private void Awake() {
            _animator = GetComponent<Animator>();
            BuildLookup();
        }

        private void Update() {
            if (_isLocked && Time.time >= _lockEndTime) {
                _isLocked = false;
            }
        }

        private void BuildLookup() {
            _configLookup.Clear();
            _hashCache.Clear();

            foreach (var config in _animations) {
                if (string.IsNullOrEmpty(config.Key)) continue;

                if (_configLookup.ContainsKey(config.Key)) {
                    Debug.LogWarning($"[CharacterAnimator] Duplicate key '{config.Key}' on {gameObject.name}");
                    continue;
                }

                _configLookup[config.Key] = config;
                _hashCache[config.Key] = Animator.StringToHash(config.GetStateName());
            }
        }

        #region Play Methods

        /// <summary>
        /// Play an animation by key.
        /// </summary>
        /// <param name="key">The animation key (must match a config in the list).</param>
        /// <param name="forcePlay">If true, ignores locks and interrupt settings.</param>
        /// <returns>True if the animation was played.</returns>
        public bool Play(string key, bool forcePlay = false) {
            if (string.IsNullOrEmpty(key)) return false;

            // Same animation check
            if (key == _currentAnimationKey && !forcePlay) return false;

            // Get config
            if (!_configLookup.TryGetValue(key, out var config)) {
                Debug.LogWarning($"[CharacterAnimator] Animation '{key}' not found on {gameObject.name}");
                return false;
            }

            // Lock check
            if (!forcePlay && IsLocked) {
                if (_logStateChanges) {
                    Debug.Log($"[CharacterAnimator] {gameObject.name}: '{key}' blocked (locked)");
                }
                return false;
            }

            // Interrupt check
            if (!forcePlay && _currentConfig != null && !_currentConfig.CanBeInterrupted) {
                if (_logStateChanges) {
                    Debug.Log($"[CharacterAnimator] {gameObject.name}: '{key}' blocked (cannot interrupt '{_currentAnimationKey}')");
                }
                return false;
            }

            // Play the animation
            string previousKey = _currentAnimationKey;
            _currentAnimationKey = key;
            _currentConfig = config;

            _animator.speed = config.SpeedMultiplier;

            if (_hashCache.TryGetValue(key, out int hash)) {
                _animator.CrossFade(hash, config.TransitionDuration, config.LayerIndex);
            } else {
                _animator.CrossFade(config.GetStateName(), config.TransitionDuration, config.LayerIndex);
            }

            // Apply lock if configured
            if (config.LockDuration > 0) {
                LockForDuration(config.LockDuration);
            }

            if (_logStateChanges) {
                Debug.Log($"[CharacterAnimator] {gameObject.name}: '{previousKey}' -> '{key}'");
            }

            OnAnimationChanged?.Invoke(previousKey, key);
            return true;
        }

        /// <summary>
        /// Play an animation immediately without crossfade.
        /// </summary>
        public bool PlayImmediate(string key, bool forcePlay = false) {
            if (string.IsNullOrEmpty(key)) return false;
            if (key == _currentAnimationKey && !forcePlay) return false;

            if (!_configLookup.TryGetValue(key, out var config)) {
                Debug.LogWarning($"[CharacterAnimator] Animation '{key}' not found on {gameObject.name}");
                return false;
            }

            if (!forcePlay && IsLocked) return false;
            if (!forcePlay && _currentConfig != null && !_currentConfig.CanBeInterrupted) return false;

            string previousKey = _currentAnimationKey;
            _currentAnimationKey = key;
            _currentConfig = config;

            _animator.speed = config.SpeedMultiplier;

            if (_hashCache.TryGetValue(key, out int hash)) {
                _animator.Play(hash, config.LayerIndex);
            } else {
                _animator.Play(config.GetStateName(), config.LayerIndex);
            }

            if (config.LockDuration > 0) {
                LockForDuration(config.LockDuration);
            }

            OnAnimationChanged?.Invoke(previousKey, key);
            return true;
        }

        #endregion

        #region Locomotion Helper

        /// <summary>
        /// Set locomotion animation based on speed.
        /// </summary>
        /// <param name="speed">Current movement speed.</param>
        /// <param name="idleKey">Animation key for idle (speed below walkThreshold).</param>
        /// <param name="walkKey">Animation key for walk.</param>
        /// <param name="runKey">Animation key for run (speed above runThreshold).</param>
        /// <param name="walkThreshold">Speed threshold for walk.</param>
        /// <param name="runThreshold">Speed threshold for run.</param>
        public void SetLocomotion(float speed, string idleKey, string walkKey, string runKey,
            float walkThreshold = 0.1f, float runThreshold = 4f) {
            if (speed < walkThreshold) {
                Play(idleKey);
            } else if (speed >= runThreshold) {
                Play(runKey);
            } else {
                Play(walkKey);
            }
        }

        /// <summary>
        /// Set locomotion with just idle and walk.
        /// </summary>
        public void SetLocomotion(float speed, string idleKey, string walkKey, float walkThreshold = 0.1f) {
            Play(speed < walkThreshold ? idleKey : walkKey);
        }

        #endregion

        #region Lock Methods

        /// <summary>
        /// Lock the animator for a duration, preventing animation changes.
        /// </summary>
        public void LockForDuration(float duration) {
            _isLocked = true;
            _lockEndTime = Time.time + duration;
        }

        /// <summary>
        /// Unlock the animator immediately.
        /// </summary>
        public void Unlock() {
            _isLocked = false;
        }

        #endregion

        #region Query Methods

        /// <summary>
        /// Check if a specific animation is currently playing.
        /// </summary>
        public bool IsPlaying(string key) => _currentAnimationKey == key;

        /// <summary>
        /// Check if an animation key exists.
        /// </summary>
        public bool HasAnimation(string key) => _configLookup.ContainsKey(key);

        /// <summary>
        /// Get the config for an animation key.
        /// </summary>
        public AnimationStateConfig GetConfig(string key) {
            return _configLookup.TryGetValue(key, out var config) ? config : null;
        }

        /// <summary>
        /// Get all registered animation keys.
        /// </summary>
        public IEnumerable<string> GetAllKeys() => _configLookup.Keys;

        #endregion

        #region Runtime Config

        /// <summary>
        /// Add an animation config at runtime.
        /// </summary>
        public void AddAnimation(AnimationStateConfig config) {
            if (config == null || string.IsNullOrEmpty(config.Key)) return;

            _animations.Add(config);
            _configLookup[config.Key] = config;
            _hashCache[config.Key] = Animator.StringToHash(config.GetStateName());
        }

        /// <summary>
        /// Add an animation with default settings at runtime.
        /// </summary>
        public void AddAnimation(string key, string animatorStateName = null, float transitionDuration = 0.15f) {
            var config = new AnimationStateConfig {
                Key = key,
                AnimatorStateName = animatorStateName ?? key,
                TransitionDuration = transitionDuration,
                CanBeInterrupted = true,
                IsLooping = true,
                SpeedMultiplier = 1f
            };
            AddAnimation(config);
        }

        /// <summary>
        /// Remove an animation config at runtime.
        /// </summary>
        public void RemoveAnimation(string key) {
            if (_configLookup.TryGetValue(key, out var config)) {
                _animations.Remove(config);
                _configLookup.Remove(key);
                _hashCache.Remove(key);
            }
        }

        #endregion

        #region Animator Parameters

        /// <summary>
        /// Set a float parameter on the animator.
        /// </summary>
        public void SetFloat(string param, float value) => _animator.SetFloat(param, value);

        /// <summary>
        /// Set a bool parameter on the animator.
        /// </summary>
        public void SetBool(string param, bool value) => _animator.SetBool(param, value);

        /// <summary>
        /// Set a trigger parameter on the animator.
        /// </summary>
        public void SetTrigger(string param) => _animator.SetTrigger(param);

        /// <summary>
        /// Reset a trigger parameter on the animator.
        /// </summary>
        public void ResetTrigger(string param) => _animator.ResetTrigger(param);

        /// <summary>
        /// Set the animator speed.
        /// </summary>
        public void SetSpeed(float speed) => _animator.speed = speed;

        #endregion

        #region Utility

        /// <summary>
        /// Get normalized time (0-1) of current animation.
        /// </summary>
        public float GetNormalizedTime(int layer = 0) {
            return _animator.GetCurrentAnimatorStateInfo(layer).normalizedTime;
        }

        /// <summary>
        /// Check if animator is transitioning.
        /// </summary>
        public bool IsTransitioning(int layer = 0) {
            return _animator.IsInTransition(layer);
        }

        /// <summary>
        /// Rebuild the lookup dictionary. Call after modifying _animations directly.
        /// </summary>
        public void RefreshLookup() => BuildLookup();

        #endregion
    }
}
