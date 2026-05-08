using Game.Dialogue;
using UnityEngine;

namespace Game.Interaction
{
    [CreateAssetMenu(
        fileName = "NPCData_",
        menuName = "Game/NPC Data",
        order = 11)]
    public class NPCData : ScriptableObject
    {
        [Header("Identity")]
        public string displayName;

        [Header("Per-Room Dialogues")]
        public DialogueData room1Dialogue;
        public DialogueData room2Dialogue;
        public DialogueData room3Dialogue;

        public DialogueData GetDialogueFor(GameState state)
        {
            if (state == null) return null;
            switch (state.CurrentRoom)
            {
                case RoomId.Room1: return room1Dialogue;
                case RoomId.Room2: return room2Dialogue;
                case RoomId.Room3: return room3Dialogue;
                default: return null;
            }
        }
    }
}
