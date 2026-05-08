using Game.Dialogue;
using UnityEngine;

namespace Game.Player
{
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
            rb.gravityScale = 0f;       
            rb.freezeRotation = true;   
        }

        private void Update()
        {
            if (DialogueRunner.Instance != null && DialogueRunner.Instance.IsRunning)
            {
                inputDir = Vector2.zero;
                return;
            }

            
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            inputDir = new Vector2(x, y).normalized;
        }

        private void FixedUpdate()
        {
            
            rb.linearVelocity = inputDir * moveSpeed;
        }
    }
}
