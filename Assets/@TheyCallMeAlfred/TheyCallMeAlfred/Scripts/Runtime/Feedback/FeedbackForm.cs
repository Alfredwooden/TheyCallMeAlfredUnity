using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace TheyCallMeAlfred.Feedback
{
    public class FeedbackForm : MonoBehaviour
    {
        [SerializeField] private string googleFormId;
        [SerializeField] private string gameNameEntryId;
        [SerializeField] private string feedbackEntryId;
        [SerializeField] private string gameName;

        public event Action<bool, string> OnSubmitComplete;

        private Coroutine submitCoroutine;

        public void SubmitFeedback(string feedbackText)
        {
            if (string.IsNullOrEmpty(googleFormId))
            {
                Debug.LogError("Google Form ID not set!");
                OnSubmitComplete?.Invoke(false, "Configuration error");
                return;
            }

            if (string.IsNullOrEmpty(feedbackText))
            {
                OnSubmitComplete?.Invoke(false, "Feedback cannot be empty");
                return;
            }

            if (submitCoroutine != null)
            {
                StopCoroutine(submitCoroutine);
            }

            submitCoroutine = StartCoroutine(SendFeedbackCoroutine(feedbackText));
        }

        private IEnumerator SendFeedbackCoroutine(string feedbackText)
        {
            string formUrl = $"https://docs.google.com/forms/d/e/{googleFormId}/formResponse";

            var form = new WWWForm();
            form.AddField($"entry.{gameNameEntryId}", gameName);
            form.AddField($"entry.{feedbackEntryId}", feedbackText);

            using (UnityWebRequest request = UnityWebRequest.Post(formUrl, form))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success ||
                    request.responseCode == 200 ||
                    request.responseCode == 302)
                {
                    OnSubmitComplete?.Invoke(true, "Feedback submitted successfully");
                }
                else
                {
                    OnSubmitComplete?.Invoke(false, $"Error: {request.error}");
                }
            }

            submitCoroutine = null;
        }

        public void SetGameName(string name)
        {
            gameName = name;
        }
    }
}
