using Game.Dialogue;
using UnityEngine;

namespace Game.Interaction
{
    /// <summary>
    /// Sits on each light-gray object in the world (lamp, rug, chair, etc.).
    /// All it does is look up the right dialogue for the current state and
    /// hand it to DialogueRunner. The "what to say in which room" smarts
    /// live in InteractableData, not here.
    ///
    /// The same physical object can stay in the scene across all three rooms;
    /// only its dialogue selection changes based on GameState.CurrentRoom.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField] private InteractableData data;

        [Tooltip("Optional. If set, this event fires after the dialogue ends. " +
                 "Useful for things like 'after the player examines object x in Room 2, " +
                 "give them the key.'")]
        [SerializeField] private InteractableHook hook = InteractableHook.None;

        public bool CanInteract => data != null && !DialogueRunner.Instance.IsRunning;
        public Vector3 Position => transform.position;

        private void Awake()
        {
            // RequireComponent guarantees a collider exists; we just nudge it
            // into trigger mode so it doesn't accidentally block movement.
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        public void Interact()
        {
            if (data == null) return;

            var state = GameState.Instance;
            var line = data.GetDialogueFor(state);
            if (line == null) return;

            // Subscribe just for this one interaction, then unsubscribe.
            // Avoids leaking handlers if the object is interacted with many times.
            void OnFinished(DialogueData finished)
            {
                if (finished != line) return;
                DialogueRunner.Instance.OnDialogueFinished -= OnFinished;
                ApplyHook(state);
            }
            DialogueRunner.Instance.OnDialogueFinished += OnFinished;

            DialogueRunner.Instance.Play(line);
        }

        private void ApplyHook(GameState state)
        {
            switch (hook)
            {
                case InteractableHook.None:
                    break;

                case InteractableHook.GiveKeyIfHintGiven:
                    // Room 2: examining "object x" after the NPC has hinted gives the key.
                    if (state.CurrentRoom == RoomId.Room2 && state.NpcHintGivenInRoom2)
                        state.SetHasKey(true);
                    break;
            }
        }
    }

    /// <summary>
    /// Tiny enum of post-dialogue side effects an interactable can trigger.
    /// Kept in this file because it's only meaningful alongside Interactable.
    /// Add cases here when new narrative beats need them.
    /// </summary>
    public enum InteractableHook
    {
        None = 0,
        GiveKeyIfHintGiven = 1,
    }
}
