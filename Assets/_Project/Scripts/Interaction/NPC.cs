using Game.Dialogue;
using UnityEngine;

namespace Game.Interaction
{
    /// <summary>
    /// The talkative figure in each room. Structurally near-identical to
    /// Interactable; kept separate because NPCs are likely to grow their
    /// own concerns (facing the player, idle anim, post-dialogue state
    /// flags that drive the narrative chain).
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class NPC : MonoBehaviour, IInteractable
    {
        [SerializeField] private NPCData data;

        public bool CanInteract => data != null && !DialogueRunner.Instance.IsRunning;
        public Vector3 Position => transform.position;

        private void Awake()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        public void Interact()
        {
            if (data == null) return;

            var state = GameState.Instance;
            var line = data.GetDialogueFor(state);
            if (line == null) return;

            void OnFinished(DialogueData finished)
            {
                if (finished != line) return;
                DialogueRunner.Instance.OnDialogueFinished -= OnFinished;
                ApplyPostDialogueState(state);
            }
            DialogueRunner.Instance.OnDialogueFinished += OnFinished;

            DialogueRunner.Instance.Play(line);
        }

        /// <summary>
        /// The narrative beats that fire once an NPC line ends. Keeping these
        /// hardcoded by room is fine for a three-room demo; if this grows, lift
        /// it into a flag-based system on NPCData (mirroring InteractableHook).
        /// </summary>
        private void ApplyPostDialogueState(GameState state)
        {
            switch (state.CurrentRoom)
            {
                case RoomId.Room2:
                    // After the NPC's Room 2 line, object x's dialogue should change
                    // and the "key under object x" path opens up.
                    state.SetNpcHintGivenInRoom2(true);
                    break;

                case RoomId.Room3:
                    // The NPC's Room 3 line is what makes the ladder appear.
                    state.SetLadderRevealedInRoom3(true);
                    break;
            }
        }
    }
}
