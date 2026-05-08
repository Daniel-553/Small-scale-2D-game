using Game.Dialogue;
using UnityEngine;

namespace Game.Interaction
{
    
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class Door : MonoBehaviour, IInteractable
    {
        [SerializeField] private DialogueData lockedDialogue;
        [SerializeField] private DialogueData unlockedDialogue;

        [Tooltip("Hook for ending the demo / triggering credits / loading a scene. " +
                 "Fired once the unlocked dialogue finishes.")]
        [SerializeField] private UnityEngine.Events.UnityEvent onExit;

        public bool CanInteract => !DialogueRunner.Instance.IsRunning;
        public Vector3 Position => transform.position;

        private void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        public void Interact()
        {
            var state = GameState.Instance;

            if (state.HasKey)
            {
                if (unlockedDialogue == null)
                {
                    onExit?.Invoke();
                    return;
                }

                void OnFinished(DialogueData finished)
                {
                    if (finished != unlockedDialogue) return;
                    DialogueRunner.Instance.OnDialogueFinished -= OnFinished;
                    onExit?.Invoke();
                }
                DialogueRunner.Instance.OnDialogueFinished += OnFinished;
                DialogueRunner.Instance.Play(unlockedDialogue);
            }
            else
            {
                DialogueRunner.Instance.Play(lockedDialogue);
            }
        }
    }
}
