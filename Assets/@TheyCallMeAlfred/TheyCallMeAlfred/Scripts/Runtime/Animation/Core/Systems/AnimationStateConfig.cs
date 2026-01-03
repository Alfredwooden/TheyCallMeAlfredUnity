using System;
using UnityEngine;

namespace AlfredWooden.Animation {
    /// <summary>
    /// Configuration for a single animation state.
    /// Add these to a CharacterAnimator to define available animations.
    /// </summary>
    [Serializable]
    public class AnimationStateConfig {
        [Tooltip("Unique key to reference this animation (e.g., 'Idle', 'Attack1', 'Death')")]
        public string Key;

        [Tooltip("The state name in the Animator Controller (defaults to Key if empty)")]
        public string AnimatorStateName;

        [Header("Transition")]
        [Tooltip("Layer index for this animation")]
        public int LayerIndex = 0;

        [Tooltip("Duration of crossfade transition into this state")]
        [Range(0f, 1f)]
        public float TransitionDuration = 0.15f;

        [Header("Behavior")]
        [Tooltip("Can other animations interrupt this one?")]
        public bool CanBeInterrupted = true;

        [Tooltip("Does this animation loop?")]
        public bool IsLooping = true;

        [Tooltip("Speed multiplier for this animation")]
        [Range(0.1f, 3f)]
        public float SpeedMultiplier = 1f;

        [Header("Optional")]
        [Tooltip("Lock state machine for this duration after playing (0 = no lock)")]
        public float LockDuration = 0f;

        /// <summary>
        /// Get the actual animator state name (uses Key if AnimatorStateName is empty).
        /// </summary>
        public string GetStateName() {
            return string.IsNullOrEmpty(AnimatorStateName) ? Key : AnimatorStateName;
        }

        /// <summary>
        /// Create a config with default values.
        /// </summary>
        public static AnimationStateConfig Create(string key, float transitionDuration = 0.15f) {
            return new AnimationStateConfig {
                Key = key,
                AnimatorStateName = key,
                TransitionDuration = transitionDuration,
                CanBeInterrupted = true,
                IsLooping = true,
                SpeedMultiplier = 1f
            };
        }
    }
}
