using UnityEngine;

namespace Game.Interaction
{
    /// <summary>
    /// Optional middle-step between "examine object x" and "have the key."
    /// You have two ways to wire the Room 2 key flow:
    ///
    ///   A) Skip this script entirely. The Interactable on object x has its
    ///      hook set to GiveKeyIfHintGiven, which sets HasKey directly when
    ///      the dialogue finishes. Simplest.
    ///
    ///   B) Use this script for a more "physical" flow: examining object x
    ///      reveals a KeyPickup in the world, the player walks to it, and
    ///      it sets HasKey on contact. This file supports option B.
    ///
    /// Pick whichever matches the feel you want; both work with the same state.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class KeyPickup : MonoBehaviour
    {
        [Tooltip("Start hidden? Typically true — the key only appears after object x is examined.")]
        [SerializeField] private bool startHidden = true;

        private void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
            if (startHidden) gameObject.SetActive(false);
        }

        /// <summary>
        /// Called by whatever narrative beat should reveal the key
        /// (e.g. UnityEvent on the Interactable's hook, or a custom listener).
        /// </summary>
        public void Reveal()
        {
            gameObject.SetActive(true);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Only the Player picks it up. Tagging the player as "Player" in
            // the inspector is the standard Unity convention for this.
            if (!other.CompareTag("Player")) return;

            GameState.Instance.SetHasKey(true);
            Destroy(gameObject);
        }
    }
}
