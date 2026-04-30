using UnityEngine;

namespace Game.World
{
    /// <summary>
    /// Sits on the parent GameObject of each room. When the player walks into
    /// the room's trigger, it tells GameState which room is now current.
    ///
    /// Setup:
    ///   - Add a Collider2D to this GameObject covering the room's footprint.
    ///   - Mark it as a trigger.
    ///   - Set roomId in the inspector.
    ///
    /// Nothing in this component cares about narrative — it's a pure
    /// "I am Room N, the player is here" announcer.
    /// </summary>
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
