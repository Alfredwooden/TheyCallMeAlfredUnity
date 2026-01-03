using System;
using UnityEngine;

namespace AlfredWooden.Animation {
    /// <summary>
    /// Interface for controlling animations on a character.
    /// Implement this to create custom animation controllers.
    /// </summary>
    public interface IAnimationController {
        /// <summary>
        /// The Unity Animator component being controlled.
        /// </summary>
        Animator Animator { get; }

        /// <summary>
        /// Whether an animation is currently playing (not in idle/default state).
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// The name of the currently playing animation state.
        /// </summary>
        string CurrentStateName { get; }

        /// <summary>
        /// Play an animation state immediately.
        /// </summary>
        /// <param name="stateName">The name of the animation state in the Animator Controller.</param>
        /// <param name="layer">The animator layer (default 0).</param>
        void Play(string stateName, int layer = 0);

        /// <summary>
        /// Play an animation state using its hash (more performant).
        /// </summary>
        /// <param name="stateHash">The hash of the animation state.</param>
        /// <param name="layer">The animator layer (default 0).</param>
        void Play(int stateHash, int layer = 0);

        /// <summary>
        /// Crossfade to an animation state over a duration.
        /// </summary>
        /// <param name="stateName">The name of the animation state.</param>
        /// <param name="transitionDuration">Duration of the crossfade in seconds.</param>
        /// <param name="layer">The animator layer (default 0).</param>
        void CrossFade(string stateName, float transitionDuration, int layer = 0);

        /// <summary>
        /// Crossfade to an animation state using its hash.
        /// </summary>
        /// <param name="stateHash">The hash of the animation state.</param>
        /// <param name="transitionDuration">Duration of the crossfade in seconds.</param>
        /// <param name="layer">The animator layer (default 0).</param>
        void CrossFade(int stateHash, float transitionDuration, int layer = 0);

        /// <summary>
        /// Set a float parameter on the animator.
        /// </summary>
        void SetFloat(string paramName, float value);

        /// <summary>
        /// Set a bool parameter on the animator.
        /// </summary>
        void SetBool(string paramName, bool value);

        /// <summary>
        /// Set a trigger parameter on the animator.
        /// </summary>
        void SetTrigger(string paramName);

        /// <summary>
        /// Reset a trigger parameter on the animator.
        /// </summary>
        void ResetTrigger(string paramName);

        /// <summary>
        /// Event fired when an animation state starts.
        /// </summary>
        event Action<string> OnAnimationStarted;

        /// <summary>
        /// Event fired when an animation state completes (reaches end or is interrupted).
        /// </summary>
        event Action<string> OnAnimationCompleted;
    }
}
