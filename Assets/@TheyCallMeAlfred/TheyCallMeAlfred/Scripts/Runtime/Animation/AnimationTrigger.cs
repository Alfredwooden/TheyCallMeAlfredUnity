using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace TheyCallMeAlfredUnity.Animation {
    [AddComponentMenu("@TheyCallMeAlfred/Animation/Animation Trigger")]
    public class AnimationTrigger : MonoBehaviour {
        [Header("Animation Settings")] public Animator animator;
        public float animationDuration = 3f;
        public float blendDuration = 0.3f;
        public bool autoStart = true;

        [Header("Auto-discovered States")] [SerializeField]
        private string[] discoveredStates;

        private Coroutine animationCoroutine;
        private readonly List<string> animationStates = new();

        private int currentAnimationIndex;

        private void Start() {
            if (animator == null) {
                animator = GetComponent<Animator>();
            }

            if (animator == null) {
                Debug.LogError("No Animator component found on " + gameObject.name);
                return;
            }

            DiscoverAnimationStates();

            if (autoStart && animationStates.Count > 0) {
                StartAnimationLoop();
            }
        }

        private void DiscoverAnimationStates() {
            animationStates.Clear();

            if (animator.runtimeAnimatorController == null) {
                Debug.LogWarning("No Animator Controller assigned to " + gameObject.name);
                return;
            }

#if UNITY_EDITOR
            for (int i = 0; i < animator.layerCount; i++) {
                AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
                if (controller != null) {
                    var stateMachine = controller.layers[i].stateMachine;
                    DiscoverStatesRecursive(stateMachine);
                }
            }
#else
        for (int i = 0; i < animator.layerCount; i++) {
            AnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(i);
            foreach (var clipInfo in clipInfos) {
                if (clipInfo.clip != null && !animationStates.Contains(clipInfo.clip.name)) {
                    animationStates.Add(clipInfo.clip.name);
                }
            }
        }
#endif

            discoveredStates = animationStates.ToArray();
            Debug.Log($"Discovered {animationStates.Count} animation states: {string.Join(", ", animationStates)}");
        }

#if UNITY_EDITOR
        private void DiscoverStatesRecursive(AnimatorStateMachine stateMachine) {
            foreach (var state in stateMachine.states) {
                if (!animationStates.Contains(state.state.name)) {
                    animationStates.Add(state.state.name);
                }
            }

            foreach (var subStateMachine in stateMachine.stateMachines) {
                DiscoverStatesRecursive(subStateMachine.stateMachine);
            }
        }
#endif

        public void StartAnimationLoop() {
            if (animationCoroutine != null) {
                StopCoroutine(animationCoroutine);
            }

            animationCoroutine = StartCoroutine(AnimationLoop());
        }


        private IEnumerator AnimationLoop() {
            while (true) {
                if (animationStates.Count == 0) {
                    yield break;
                }

                string currentState = animationStates[currentAnimationIndex];
                animator.CrossFade(currentState, blendDuration);

                yield return new WaitForSeconds(animationDuration);

                currentAnimationIndex = (currentAnimationIndex + 1) % animationStates.Count;
            }
        }
    }
}
