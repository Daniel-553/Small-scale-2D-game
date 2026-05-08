using System.Collections.Generic;
using Game.Dialogue;
using Game.Interaction;
using UnityEngine;

namespace Game.Player
{
    [DisallowMultipleComponent]
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private KeyCode interactKey = KeyCode.E;

        private readonly List<IInteractable> inRange = new();

        private void Update()
        {
            if (!Input.GetKeyDown(interactKey)) return;

            
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
