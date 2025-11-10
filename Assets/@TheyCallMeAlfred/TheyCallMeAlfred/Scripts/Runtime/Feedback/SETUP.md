# Feedback Form Setup Guide

## Google Forms Setup

### 1. Create a Google Form
1. Go to [Google Forms](https://forms.google.com)
2. Click **+ Blank** to create a new form
3. Name it "Game Feedback" (or any name you prefer)
4. Add two questions:
   - **Question 1**: Short answer, title: "Game Name"
   - **Question 2**: Paragraph, title: "Feedback"

### 2. Get the Form ID
1. Click **Send** button (top right)
2. Click the **Link** icon
3. Copy the URL - it will look like:
   ```
   https://docs.google.com/forms/d/e/1FAIpQLSd_XXXXXXXXXXXXXXXXXXXXX/viewform
   ```
4. Extract the Form ID (the part between `/d/e/` and `/viewform`)
   - Example: `1FAIpQLSd_XXXXXXXXXXXXXXXXXXXXX`

### 3. Get Entry IDs for Fields
1. Open your form in **edit mode**
2. Right-click on the page and select **View Page Source** (or press Ctrl+U / Cmd+U)
3. Search for `entry.` - you'll find entries like `entry.123456789`
4. Find the two entry IDs:
   - **Game Name field**: Look for entry near your "Game Name" question (e.g., `entry.123456789`)
   - **Feedback field**: Look for entry near your "Feedback" question (e.g., `entry.987654321`)

**Quick Method**: Use browser DevTools
1. Open form in edit mode
2. Press F12 to open DevTools
3. Go to Console tab
4. Paste and run:
```javascript
[...document.querySelectorAll('[name^="entry."]')].map(e => e.name)
```
5. This will show all entry IDs in order

### 4. Unity Setup

1. Create a GameObject in your scene
2. Add the `FeedbackForm` component
3. Fill in the Inspector fields:
   - **Google Form Id**: The form ID from step 2
   - **Game Name Entry Id**: The entry ID for Game Name field (numbers only, e.g., `123456789`)
   - **Feedback Entry Id**: The entry ID for Feedback field (numbers only, e.g., `987654321`)
   - **Game Name**: Pre-filled game name (e.g., "My Awesome Game")

### Example Usage (Script)

```csharp
using TheyCallMeAlfred.Feedback;

public class MyGameManager : MonoBehaviour
{
    [SerializeField] private FeedbackForm feedbackForm;

    private void Start()
    {
        feedbackForm.SetGameName("My Game Name");
        feedbackForm.OnSubmitComplete += HandleSubmit;
    }

    public void SendFeedback(string playerFeedback)
    {
        feedbackForm.SubmitFeedback(playerFeedback);
    }

    private void HandleSubmit(bool success, string message)
    {
        if (success)
        {
            Debug.Log("Feedback sent!");
        }
        else
        {
            Debug.LogError($"Failed to send feedback: {message}");
        }
    }
}
```

### 5. View Responses
1. Go to your Google Form
2. Click the **Responses** tab
3. All feedback submissions will appear here
4. You can also link to a Google Sheet for easier analysis

## Troubleshooting

**Submission Failed**: Double-check that the Form ID and Entry IDs are correct

**Wrong Entry IDs**: Make sure you're using the numeric part only (e.g., `123456789` not `entry.123456789`)

**No Responses Showing**: Check that your form is set to accept responses (not closed)
