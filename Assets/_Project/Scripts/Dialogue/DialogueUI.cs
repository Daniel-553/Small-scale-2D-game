using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Dialogue
{
    /// <summary>
    /// View layer for the dialogue popup. Sits on the DialogueBox prefab root.
    /// Knows nothing about narrative — DialogueRunner pushes data into it.
    ///
    /// Hook this up in the inspector:
    ///   - rootCanvasGroup: a CanvasGroup on the popup root (for show/hide + fade)
    ///   - speakerLabel:    TMP_Text for the speaker name
    ///   - lineLabel:       TMP_Text for the current line
    ///   - continueButton:  Button that calls DialogueRunner.Instance.RequestAdvance()
    /// </summary>
    [DisallowMultipleComponent]
    public class DialogueUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup rootCanvasGroup;
        [SerializeField] private TMP_Text speakerLabel;
        [SerializeField] private TMP_Text lineLabel;
        [SerializeField] private Button continueButton;

        private void Awake()
        {
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(() =>
                {
                    if (DialogueRunner.Instance != null)
                        DialogueRunner.Instance.RequestAdvance();
                });
            }
        }

        public void Show()
        {
            SetVisible(true);
        }

        public void Hide()
        {
            SetVisible(false);
            if (speakerLabel != null) speakerLabel.text = string.Empty;
            if (lineLabel != null)    lineLabel.text    = string.Empty;
        }

        public void SetSpeaker(string speaker)
        {
            if (speakerLabel == null) return;
            speakerLabel.text = speaker ?? string.Empty;
            // Hide the label entirely when there's no speaker, so narration lines
            // don't leave an empty box floating above the text.
            speakerLabel.gameObject.SetActive(!string.IsNullOrEmpty(speaker));
        }

        public void SetLine(string line)
        {
            if (lineLabel == null) return;
            lineLabel.text = line ?? string.Empty;
        }

        private void SetVisible(bool visible)
        {
            if (rootCanvasGroup == null)
            {
                gameObject.SetActive(visible);
                return;
            }
            rootCanvasGroup.alpha = visible ? 1f : 0f;
            rootCanvasGroup.interactable = visible;
            rootCanvasGroup.blocksRaycasts = visible;
        }
    }
}
