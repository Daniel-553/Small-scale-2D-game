using System;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// The single source of truth for narrative state. Anything that needs to
    /// know "which room am I in?" or "does the player have the key?" reads it
    /// from here. Anything that mutates state goes through the setter methods
    /// so OnStateChanged fires consistently.
    ///
    /// Implemented as a MonoBehaviour singleton so it survives scene loads
    /// (DontDestroyOnLoad) and so its values are inspectable in the editor
    /// at runtime — useful when debugging the narrative chain.
    /// </summary>
    [DisallowMultipleComponent]
    public class GameState : MonoBehaviour
    {
        public static GameState Instance { get; private set; }

        [Header("Narrative State (read-only at runtime)")]
        [SerializeField] private RoomId currentRoom = RoomId.Room1;
        [SerializeField] private bool hasKey;
        [SerializeField] private bool npcHintGivenInRoom2;
        [SerializeField] private bool ladderRevealedInRoom3;

        /// <summary>Fires whenever any tracked field changes.</summary>
        public event Action OnStateChanged;

        public RoomId CurrentRoom => currentRoom;
        public bool   HasKey      => hasKey;
        public bool   NpcHintGivenInRoom2 => npcHintGivenInRoom2;
        public bool   LadderRevealedInRoom3 => ladderRevealedInRoom3;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        // --- Mutators ---
        // Each setter only fires the event when the value actually changes,
        // so listeners aren't woken up by no-op writes.

        public void SetCurrentRoom(RoomId room)
        {
            if (currentRoom == room) return;
            currentRoom = room;
            OnStateChanged?.Invoke();
        }

        public void SetHasKey(bool value)
        {
            if (hasKey == value) return;
            hasKey = value;
            OnStateChanged?.Invoke();
        }

        public void SetNpcHintGivenInRoom2(bool value)
        {
            if (npcHintGivenInRoom2 == value) return;
            npcHintGivenInRoom2 = value;
            OnStateChanged?.Invoke();
        }

        public void SetLadderRevealedInRoom3(bool value)
        {
            if (ladderRevealedInRoom3 == value) return;
            ladderRevealedInRoom3 = value;
            OnStateChanged?.Invoke();
        }
    }
}
