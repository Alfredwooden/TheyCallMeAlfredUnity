using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using System.IO;
using AlfredWooden.Animation;

namespace AlfredWooden.Editor.Animation {
    [CustomEditor(typeof(CharacterAnimator))]
    public class CharacterAnimatorEditor : UnityEditor.Editor {
        private SerializedProperty _animationsProp;
        private SerializedProperty _defaultTransitionDurationProp;
        private SerializedProperty _logStateChangesProp;
        private bool _showDebugControls = true;

        private void OnEnable() {
            _animationsProp = serializedObject.FindProperty("_animations");
            _defaultTransitionDurationProp = serializedObject.FindProperty("_defaultTransitionDuration");
            _logStateChangesProp = serializedObject.FindProperty("_logStateChanges");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            CharacterAnimator script = (CharacterAnimator)target;

            // Draw default settings
            EditorGUILayout.PropertyField(_defaultTransitionDurationProp);
            EditorGUILayout.PropertyField(_logStateChangesProp);

            EditorGUILayout.Space();
            
            // Auto-fill button
            if (GUILayout.Button("Auto-fill from Animator Controller", GUILayout.Height(30))) {
                AutoFillAnimations(script);
            }
            
            if (GUILayout.Button("Generate AnimationKeys.cs", GUILayout.Height(30))) {
                GenerateKeysScript(script);
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_animationsProp, true);

            // Debug Controls (Runtime only)
            if (Application.isPlaying) {
                EditorGUILayout.Space();
                _showDebugControls = EditorGUILayout.Foldout(_showDebugControls, "Runtime Debug Controls", true);
                
                if (_showDebugControls) {
                    EditorGUILayout.HelpBox("Click to play animations immediately.", MessageType.Info);
                    
                    // Iterate through the serialized property to get keys (more reliable than script.GetAllKeys() if changed at runtime)
                    for (int i = 0; i < _animationsProp.arraySize; i++) {
                        SerializedProperty element = _animationsProp.GetArrayElementAtIndex(i);
                        string key = element.FindPropertyRelative("Key").stringValue;
                        
                        if (string.IsNullOrEmpty(key)) continue;

                        bool isPlaying = script.IsPlaying(key);
                        
                        if (isPlaying) {
                            GUI.backgroundColor = new Color(0.6f, 1f, 0.6f);
                            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                            GUI.backgroundColor = Color.white;
                        } else {
                            EditorGUILayout.BeginHorizontal();
                        }

                        EditorGUILayout.LabelField(key, GUILayout.Width(150));
                        
                        if (GUILayout.Button("Play", GUILayout.Width(60))) {
                            script.Play(key, true); // forcePlay = true
                        }
                        
                        if (GUILayout.Button("Play (No Force)", GUILayout.Width(100))) {
                            script.Play(key, false);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Unlock Animator")) {
                        script.Unlock();
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private struct StateInfo {
            public string Name;
            public int LayerIndex;
        }

        private void AutoFillAnimations(CharacterAnimator script) {
            Animator animator = script.GetComponent<Animator>();
            if (animator == null) {
                EditorUtility.DisplayDialog("Error", "No Animator component found on this GameObject.", "OK");
                return;
            }

            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
            if (controller == null) {
                // Handle OverrideController
                var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
                if (overrideController != null) {
                    controller = overrideController.runtimeAnimatorController as AnimatorController;
                }
            }

            if (controller == null) {
                EditorUtility.DisplayDialog("Error", "No valid Animator Controller found.", "OK");
                return;
            }

            if (!EditorUtility.DisplayDialog("Confirm Auto-fill", 
                "This will replace your current animation list with states found in the Animator Controller. Are you sure?", 
                "Yes", "Cancel")) {
                return;
            }

            _animationsProp.ClearArray();
            
            var states = new List<StateInfo>();
            for (int i = 0; i < controller.layers.Length; i++) {
                DiscoverStates(controller.layers[i].stateMachine, i, states);
            }

            for (int i = 0; i < states.Count; i++) {
                _animationsProp.InsertArrayElementAtIndex(i);
                SerializedProperty element = _animationsProp.GetArrayElementAtIndex(i);
                
                string stateName = states[i].Name;
                string sanitizedKey = stateName.Replace(".", "_");
                
                element.FindPropertyRelative("Key").stringValue = sanitizedKey;
                element.FindPropertyRelative("AnimatorStateName").stringValue = stateName;
                element.FindPropertyRelative("LayerIndex").intValue = states[i].LayerIndex;
                
                // Defaults
                element.FindPropertyRelative("TransitionDuration").floatValue = _defaultTransitionDurationProp.floatValue;
                element.FindPropertyRelative("CanBeInterrupted").boolValue = true;
                element.FindPropertyRelative("IsLooping").boolValue = true;
                element.FindPropertyRelative("SpeedMultiplier").floatValue = 1f;
                element.FindPropertyRelative("LockDuration").floatValue = 0f;
            }
            
            Debug.Log($"[CharacterAnimatorEditor] Auto-filled {states.Count} animations.");
        }

        private void DiscoverStates(AnimatorStateMachine stateMachine, int layerIndex, List<StateInfo> states) {
             foreach (var childState in stateMachine.states) {
                // Check if already added to avoid duplicates if accidentally traversed twice (unlikely here but safe)
                if (states.FindIndex(s => s.Name == childState.state.name && s.LayerIndex == layerIndex) == -1) {
                    states.Add(new StateInfo { Name = childState.state.name, LayerIndex = layerIndex });
                }
            }
            foreach (var childStateMachine in stateMachine.stateMachines) {
                DiscoverStates(childStateMachine.stateMachine, layerIndex, states);
            }
        }

        private void GenerateKeysScript(CharacterAnimator script) {
            SerializedProperty animationsProp = serializedObject.FindProperty("_animations");
            if (animationsProp.arraySize == 0) {
                EditorUtility.DisplayDialog("Error", "No animations to generate keys for.", "OK");
                return;
            }

            // Default path
            string defaultName = $"{script.gameObject.name}AnimationKeys";
            string folderPath = "Assets/@TheyCallMeAlfred/TheyCallMeAlfred/Scripts/Runtime/Animation/Generated";
            
            // Ensure default folder exists for convenience
            if (!Directory.Exists(folderPath)) {
                // Try to find a valid default path if the hardcoded one doesn't exist or just use Assets
                if (Directory.Exists(Application.dataPath + "/@TheyCallMeAlfred"))
                    folderPath = Application.dataPath + "/@TheyCallMeAlfred/TheyCallMeAlfred/Scripts/Runtime/Animation/Generated";
                else 
                    folderPath = Application.dataPath;
            }

            string path = EditorUtility.SaveFilePanel("Save Animation Keys", folderPath, defaultName, "cs");

            if (string.IsNullOrEmpty(path)) return;

            string className = Path.GetFileNameWithoutExtension(path);
            // Ensure class name is valid C# identifier
            className = System.Text.RegularExpressions.Regex.Replace(className, @"[^a-zA-Z0-9_]", "");
            if (char.IsDigit(className[0])) className = "_" + className;

            using (StreamWriter writer = new StreamWriter(path)) {
                writer.WriteLine("namespace AlfredWooden.Animation {");
                writer.WriteLine($"    public static class {className} {{");
                
                var existingKeys = new HashSet<string>();

                for (int i = 0; i < animationsProp.arraySize; i++) {
                    SerializedProperty element = animationsProp.GetArrayElementAtIndex(i);
                    string key = element.FindPropertyRelative("Key").stringValue;
                    
                    if (string.IsNullOrEmpty(key)) continue;

                    // Sanitize key for variable name
                    string varName = System.Text.RegularExpressions.Regex.Replace(key, @"[^a-zA-Z0-9_]", "");
                    if (string.IsNullOrEmpty(varName)) continue;
                    if (char.IsDigit(varName[0])) varName = "_" + varName;

                    if (existingKeys.Contains(varName)) continue;
                    existingKeys.Add(varName);

                    writer.WriteLine($"        public const string {varName} = \"{key}\";");
                }

                writer.WriteLine("    }");
                writer.WriteLine("}");
            }

            AssetDatabase.Refresh();
            Debug.Log($"[CharacterAnimatorEditor] Generated keys at {path}");
        }
    }
}
