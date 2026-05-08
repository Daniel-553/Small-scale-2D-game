using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Dialogue
{
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
