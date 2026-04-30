using Game.Dialogue;
using UnityEngine;

namespace Game.Interaction
{
    /// <summary>
    /// The exit at the top of the ladder in Room 3. Has key → ends the demo.
    /// No key → plays the "locked" line.
    ///
    /// "Ends the demo" is intentionally vague here — wire the OnExit UnityEvent
    /// in the inspector to whatever you want (load a credits scene, fire an
    /// event, show a UI). That keeps Door.cs free of any project-specific flow.
    /// </summary>
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
