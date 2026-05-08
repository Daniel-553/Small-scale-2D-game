using UnityEngine;

namespace Game.Interaction
{
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

        public void Reveal()
        {
            gameObject.SetActive(true);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            GameState.Instance.SetHasKey(true);
            Destroy(gameObject);
        }
    }
}
