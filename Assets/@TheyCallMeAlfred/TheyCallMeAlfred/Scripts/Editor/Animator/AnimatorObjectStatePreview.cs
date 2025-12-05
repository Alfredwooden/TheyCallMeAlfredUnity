using UnityEditor;
using UnityEngine;
using System.Reflection;
using UnityEditor.Animations;

namespace TheyCallMeAlfredUnity.Editor.Animator {
    [CustomPreview(typeof(AnimatorState))]
    public class AnimatorObjectStatePreview : ObjectPreview {
        private static FieldInfo _cachedAvatarPreviewField;
        private static FieldInfo _cachedTimeControlField;
        private static FieldInfo _cachedStopTimeField;

        private UnityEditor.Editor _preview;
        private int _animationClipId;

        public override void Initialize(Object[] targets) {
            base.Initialize(targets);

            if (targets.Length > 1 || Application.isPlaying) {
                return;
            }

            SourceAnimationClipEditorFields();

            AnimationClip clip = GetAnimationClip(target as AnimatorState);
            if (clip != null) {
                _preview = UnityEditor.Editor.CreateEditor(clip);
                _animationClipId = clip.GetInstanceID();
            }
        }

        public override void Cleanup() {
            base.Cleanup();
            CleanupPreviewEditor();
        }

        public override bool HasPreviewGUI() {
            return _preview?.HasPreviewGUI() ?? false;
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background) {
            base.OnInteractivePreviewGUI(r, background);

            AnimationClip currentClip = GetAnimationClip(target as AnimatorState);
            if (currentClip != null && currentClip.GetInstanceID() != _animationClipId) {
                CleanupPreviewEditor();
                _preview = UnityEditor.Editor.CreateEditor(currentClip);
                _animationClipId = currentClip.GetInstanceID();
                return;
            }

            if (_preview != null) {
                UpdateAnimationClipEditor(_preview, currentClip);
                _preview.OnInteractivePreviewGUI(r, background);
            }
        }

        private AnimationClip GetAnimationClip(AnimatorState state) {
            return state?.motion as AnimationClip;
        }

        private void CleanupPreviewEditor() {
            if (_preview != null) {
                Object.DestroyImmediate(_preview);
                _preview = null;
                _animationClipId = 0;
            }
        }

        private static void SourceAnimationClipEditorFields() {
            if (_cachedAvatarPreviewField != null) {
                return;
            }

            _cachedAvatarPreviewField = System.Type.GetType("UnityEditor.AnimationClipEditor, UnityEditor")
                .GetField("s_AvatarPreview", BindingFlags.NonPublic | BindingFlags.Instance);
            _cachedTimeControlField = System.Type.GetType("UnityEditor.AvatarPreview, UnityEditor")
                .GetField("timeControl", BindingFlags.Public | BindingFlags.Instance);
            _cachedStopTimeField = System.Type.GetType("UnityEditor.TimeControl, UnityEditor")
                .GetField("stopTime", BindingFlags.Public | BindingFlags.Instance);
        }

        private void UpdateAnimationClipEditor(UnityEditor.Editor editor, AnimationClip clip) {
            if (_cachedAvatarPreviewField == null || _cachedTimeControlField == null || _cachedStopTimeField == null) {
                return;
            }

            var avatarPreview = _cachedAvatarPreviewField.GetValue(editor);
            var timeControl = _cachedTimeControlField.GetValue(avatarPreview);

            _cachedStopTimeField.SetValue(timeControl, clip.length);
        }
    }
}
