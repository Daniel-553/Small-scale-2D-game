using Game.Dialogue;
using UnityEngine;

namespace Game.Interaction
{
    
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

    public enum InteractableHook
    {
        None = 0,
        GiveKeyIfHintGiven = 1,
    }
}
