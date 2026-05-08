using System;
using System.Collections;
using UnityEngine;

namespace Game.Dialogue
{
    /// <summary>
    /// Single point of control for showing dialogues. Lives on a persistent
    /// GameObject in the scene (e.g. attached to the DialogueBox UI prefab's root,
    /// or a "_Systems" object). Player movement and other interactables should
    /// check IsRunning and refuse input while a dialogue is active.
    /// </summary>
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

        /// <summary>True while a dialogue is on screen.</summary>
        public bool IsRunning { get; private set; }

        /// <summary>Fires when a dialogue completes naturally (last line dismissed).</summary>
        public event Action<DialogueData> OnDialogueFinished;

        // --- Internal playback state ---
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

        /// <summary>
        /// Starts playing the given dialogue. If another dialogue is already running,
        /// the new request is ignored — callers should gate on IsRunning.
        /// Passing null is a no-op (safe for InteractableData.GetDialogueFor returning null).
        /// </summary>
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

        /// <summary>
        /// Called by the UI's Continue button (and/or by PlayerInteractor when the
        /// interact key is pressed while a dialogue is active).
        /// </summary>
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

            // Fire after state is fully cleared, so listeners that immediately
            // start a new dialogue (e.g. chained NPC lines) don't get rejected.
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
