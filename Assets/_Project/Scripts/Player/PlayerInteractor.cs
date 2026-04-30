using System.Collections.Generic;
using Game.Dialogue;
using Game.Interaction;
using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// Tracks IInteractable objects that are inside this trigger collider,
    /// and on the interact key picks the closest one and calls Interact().
    ///
    /// While a dialogue is running, the same key advances the dialogue instead
    /// of starting a new interaction — this gives the player one consistent
    /// "do the thing" button across world and dialogue contexts.
    ///
    /// Setup:
    ///   - Sit this on the Player GameObject alongside PlayerController.
    ///   - Add a SECOND Collider2D (in addition to the player's body collider),
    ///     marked isTrigger, sized to your interaction radius (~1.0–1.5 units).
    /// </summary>
    [DisallowMultipleComponent]
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private KeyCode interactKey = KeyCode.E;

        private readonly List<IInteractable> inRange = new();

        private void Update()
        {
            if (!Input.GetKeyDown(interactKey)) return;

            // If a dialogue is up, the interact key advances it.
            if (DialogueRunner.Instance != null && DialogueRunner.Instance.IsRunning)
            {
                DialogueRunner.Instance.RequestAdvance();
                return;
            }

            var target = FindClosest();
            target?.Interact();
        }

        private IInteractable FindClosest()
        {
            IInteractable best = null;
            float bestSqr = float.MaxValue;

            // Iterate backwards so we can drop stale entries (destroyed objects)
            // in the same pass without screwing up the indices.
            for (int i = inRange.Count - 1; i >= 0; i--)
            {
                var candidate = inRange[i];
                if (candidate == null || (candidate is Object u && u == null))
                {
                    inRange.RemoveAt(i);
                    continue;
                }
                if (!candidate.CanInteract) continue;

                float d = (candidate.Position - transform.position).sqrMagnitude;
                if (d < bestSqr)
                {
                    bestSqr = d;
                    best = candidate;
                }
            }
            return best;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // GetComponentInParent so a collider on a child of an Interactable still works.
            var interactable = other.GetComponentInParent<IInteractable>();
            if (interactable != null && !inRange.Contains(interactable))
                inRange.Add(interactable);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var interactable = other.GetComponentInParent<IInteractable>();
            if (interactable != null)
                inRange.Remove(interactable);
        }
    }
}
