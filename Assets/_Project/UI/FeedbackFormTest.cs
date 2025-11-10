using UnityEngine;
using UnityEngine.UIElements;
using TheyCallMeAlfred.Feedback;

public class FeedbackFormTest : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private FeedbackForm feedbackForm;

    private TextField gameNameField;
    private TextField feedbackField;
    private Button submitButton;
    private Label statusLabel;

    private void OnEnable()
    {
        var root = uiDocument.rootVisualElement;

        gameNameField = root.Q<TextField>("GameNameField");
        feedbackField = root.Q<TextField>("FeedbackField");
        submitButton = root.Q<Button>("SubmitButton");
        statusLabel = root.Q<Label>("StatusLabel");

        submitButton.clicked += OnSubmitClicked;
        feedbackForm.OnSubmitComplete += OnSubmitComplete;

        gameNameField.value = feedbackForm.GetType()
            .GetField("gameName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(feedbackForm) as string;
    }

    private void OnDisable()
    {
        submitButton.clicked -= OnSubmitClicked;
        feedbackForm.OnSubmitComplete -= OnSubmitComplete;
    }

    private void OnSubmitClicked()
    {
        if (string.IsNullOrWhiteSpace(feedbackField.value))
        {
            statusLabel.text = "Please enter feedback before submitting";
            statusLabel.style.color = new Color(1f, 0.5f, 0f);
            return;
        }

        submitButton.SetEnabled(false);
        statusLabel.text = "Submitting...";
        statusLabel.style.color = Color.yellow;

        feedbackForm.SubmitFeedback(feedbackField.value);
    }

    private void OnSubmitComplete(bool success, string message)
    {
        submitButton.SetEnabled(true);
        statusLabel.text = message;
        statusLabel.style.color = success ? Color.green : Color.red;

        if (success)
        {
            feedbackField.value = "";
        }
    }
}
