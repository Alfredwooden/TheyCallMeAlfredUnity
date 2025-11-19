using UnityEditor;
using UnityEngine;

namespace TheyCallMeAlfredUnity.Editor.Audio {
    public static class PlayAudioSource {
        const string k_menu = "CONTEXT/AudioSource/Play";

        [MenuItem(k_menu)]
        static void PlayAudioSourceMenuItem(MenuCommand command) {
            AudioSource audioSource = (AudioSource)command.context;
            audioSource.Play();
        }

        [MenuItem(k_menu, validate = true)]
        static bool PlayAudioSourceMenuItemValidate() {
            return Application.isPlaying;
        }

        [InitializeOnLoadMethod]
        static void AudioSourceHeaderButton() {
            UnityEditor.Editor.finishedDefaultHeaderGUI += OnFinishedHeaderGUI;
        }

        static void OnFinishedHeaderGUI(UnityEditor.Editor editor) {
            if (!(editor.target is GameObject gameObject)) return;
            if (gameObject.TryGetComponent<AudioSource>(out AudioSource audioSource)) {
                GUILayout.BeginHorizontal();
                GUILayout.Space(65f);
                GUILayout.Label("Audio Source");
                if (audioSource.isPlaying) {
                    if (GUILayout.Button(EditorGUIUtility.IconContent("d_StopButton"))) {
                        audioSource.Stop();
                    }
                }
                else {
                    if (GUILayout.Button(EditorGUIUtility.IconContent("d_PlayButton"))) {
                        audioSource.Play();
                    }
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
    }
}
