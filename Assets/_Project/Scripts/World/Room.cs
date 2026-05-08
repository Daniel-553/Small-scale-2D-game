using UnityEngine;

namespace Game.World
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class Room : MonoBehaviour
    {
        [SerializeField] private RoomId roomId = RoomId.None;

        private void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (GameState.Instance == null) return;

            GameState.Instance.SetCurrentRoom(roomId);
        }
    }
}
