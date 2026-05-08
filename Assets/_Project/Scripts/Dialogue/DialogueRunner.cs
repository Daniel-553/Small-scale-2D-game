using System;
using System.Collections;
using UnityEngine;

namespace Game.Dialogue
{
    [DisallowMultipleComponent]
    public class DialogueRunner : MonoBehaviour
    {
        public static DialogueRunner Instance { get; private set; }

        [Header("Wiring")]
        [SerializeField] private DialogueUI ui;

        [Header("Behaviour")]
        [Tooltip("Minimum seconds a line must be visible before Continue accepts input. " +
                 "Prevents accidental skip when the player mashes the interact key.")]
        [SerializeField] private float minLineDuration = 0.15f;

        
        public bool IsRunning { get; private set; }

        
        public event Action<DialogueData> OnDialogueFinished;

        
        private DialogueData current;
        private int lineIndex;
        private float lineShownAt;
        private bool advanceRequested;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (ui == null)
                Debug.LogError($"{nameof(DialogueRunner)}: DialogueUI reference is missing.", this);
            else
                ui.Hide();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        
        public void Play(DialogueData data)
        {
            if (data == null) return;

            if (IsRunning)
            {
                Debug.LogWarning($"{nameof(DialogueRunner)}: ignoring Play({data.name}); already running.");
                return;
            }

            if (data.lines == null || data.lines.Length == 0)
            {
                Debug.LogWarning($"{nameof(DialogueRunner)}: '{data.name}' has no lines.", data);
                return;
            }

            StartCoroutine(RunRoutine(data));
        }

        
        public void RequestAdvance()
        {
            if (!IsRunning) return;
            if (Time.unscaledTime - lineShownAt < minLineDuration) return;
            advanceRequested = true;
        }

        private IEnumerator RunRoutine(DialogueData data)
        {
            IsRunning = true;
            current = data;
            lineIndex = 0;
            ui.Show();

            while (lineIndex < data.lines.Length)
            {
                ShowCurrentLine(data);

                advanceRequested = false;
                while (!advanceRequested)
                    yield return null;

                lineIndex++;
            }

            ui.Hide();
            IsRunning = false;
            current = null;

            
            OnDialogueFinished?.Invoke(data);
        }

        private void ShowCurrentLine(DialogueData data)
        {
            var line = data.lines[lineIndex];
            ui.SetSpeaker(data.speakerName);
            ui.SetLine(line);
            lineShownAt = Time.unscaledTime;
        }
    }
}
