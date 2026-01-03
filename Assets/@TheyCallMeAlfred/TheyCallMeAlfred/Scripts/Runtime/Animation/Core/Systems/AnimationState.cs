using System;
using UnityEngine;

namespace AlfredWooden.Animation {
    /// <summary>
    /// Base class for animation states with lifecycle methods.
    /// Extend this to create custom state logic (e.g., combo timing, attack hitboxes).
    /// </summary>
    /// <typeparam name="TState">The enum type for animation states.</typeparam>
    public abstract class AnimationState<TState> where TState : Enum {
        /// <summary>
        /// The enum key for this state.
        /// </summary>
        public TState StateKey { get; }

        /// <summary>
        /// Reference to the state machine managing this state.
        /// </summary>
        protected AnimationStateMachineAdvanced<TState> StateMachine { get; private set; }

        /// <summary>
        /// Reference to the animation controller.
        /// </summary>
        protected AnimationController AnimController => StateMachine?.AnimController;

        /// <summary>
        /// The name of the animation in the Animator Controller.
        /// </summary>
        public virtual string AnimationName => StateKey.ToString();

        /// <summary>
        /// Default transition time into this state.
        /// </summary>
        public virtual float TransitionDuration => 0.15f;

        /// <summary>
        /// Whether this state can be interrupted by other states.
        /// </summary>
        public virtual bool CanBeInterrupted => true;

        /// <summary>
        /// Whether this state loops.
        /// </summary>
        public virtual bool IsLooping => true;

        /// <summary>
        /// Time spent in this state.
        /// </summary>
        protected float StateTime { get; private set; }

        public AnimationState(TState stateKey) {
            StateKey = stateKey;
        }

        /// <summary>
        /// Called by the state machine to set the reference.
        /// </summary>
        internal void SetStateMachine(AnimationStateMachineAdvanced<TState> stateMachine) {
            StateMachine = stateMachine;
        }

        /// <summary>
        /// Called when entering this state.
        /// </summary>
        public virtual void EnterState() {
            StateTime = 0f;
            PlayAnimation();
        }

        /// <summary>
        /// Called when exiting this state.
        /// </summary>
        public virtual void ExitState() { }

        /// <summary>
        /// Called every frame while in this state.
        /// </summary>
        public virtual void UpdateState() {
            StateTime += Time.deltaTime;
        }

        /// <summary>
        /// Called every fixed update while in this state.
        /// </summary>
        public virtual void FixedUpdateState() { }

        /// <summary>
        /// Determine the next state. Return current StateKey to stay.
        /// Override this for automatic transitions (e.g., attack -> idle after duration).
        /// </summary>
        public virtual TState GetNextState() {
            return StateKey;
        }

        /// <summary>
        /// Play the animation for this state.
        /// </summary>
        protected virtual void PlayAnimation() {
            AnimController?.CrossFade(AnimationName, TransitionDuration);
        }

        /// <summary>
        /// Play animation with custom blend time.
        /// </summary>
        protected void PlayAnimationWithBlend(string animationName, float transitionTime) {
            AnimController?.CrossFade(animationName, transitionTime);
        }

        /// <summary>
        /// Get the length of an animation clip by name.
        /// </summary>
        protected float GetAnimationLength(string animationName) {
            if (AnimController?.Animator?.runtimeAnimatorController == null) return 1f;

            var clips = AnimController.Animator.runtimeAnimatorController.animationClips;
            foreach (var clip in clips) {
                if (clip.name == animationName) return clip.length;
            }

            Debug.LogWarning($"[AnimationState] Animation clip '{animationName}' not found. Using default 1s.");
            return 1f;
        }

        /// <summary>
        /// Get the length of this state's animation.
        /// </summary>
        protected float GetAnimationLength() => GetAnimationLength(AnimationName);

        /// <summary>
        /// Request a transition to another state.
        /// </summary>
        protected void TransitionTo(TState newState) {
            StateMachine?.RequestStateChange(newState);
        }
    }
}
