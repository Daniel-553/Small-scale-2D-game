using Game.Dialogue;
using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// Top-down 2D movement with Rigidbody2D. Reads WASD/arrow keys via the
    /// legacy Input axes ("Horizontal" / "Vertical") so this works without
    /// any Input System setup. If the project uses the new Input System,
    /// replace the two Input.GetAxisRaw calls with action references.
    ///
    /// Movement is suppressed while a dialogue is on screen.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4.5f;

        private Rigidbody2D rb;
        private Vector2 inputDir;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;       // top-down — no gravity
            rb.freezeRotation = true;   // walk into walls, don't tip over
        }

        private void Update()
        {
            if (DialogueRunner.Instance != null && DialogueRunner.Instance.IsRunning)
            {
                inputDir = Vector2.zero;
                return;
            }

            // GetAxisRaw gives unsmoothed -1/0/1 — snappier than GetAxis for top-down.
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            inputDir = new Vector2(x, y).normalized;
        }

        private void FixedUpdate()
        {
            // Use velocity rather than transform.position so collisions resolve cleanly.
            rb.linearVelocity = inputDir * moveSpeed;
        }
    }
}
