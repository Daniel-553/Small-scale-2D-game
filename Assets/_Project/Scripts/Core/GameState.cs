using System;
using UnityEngine;

namespace Game
{
    [DisallowMultipleComponent]
    public class GameState : MonoBehaviour
    {
        public static GameState Instance { get; private set; }

        [Header("Narrative State (read-only at runtime)")]
        [SerializeField] private RoomId currentRoom = RoomId.Room1;
        [SerializeField] private bool hasKey;
        [SerializeField] private bool npcHintGivenInRoom2;
        [SerializeField] private bool ladderRevealedInRoom3;

        
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
