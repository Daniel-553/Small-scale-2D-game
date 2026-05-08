using Game.Dialogue;
using UnityEngine;

namespace Game.Interaction
{
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
        private void ApplyPostDialogueState(GameState state)
        {
            switch (state.CurrentRoom)
            {
                case RoomId.Room2:
                    state.SetNpcHintGivenInRoom2(true);
                    break;

                case RoomId.Room3:
                    state.SetLadderRevealedInRoom3(true);
                    break;
            }
        }
    }
}
