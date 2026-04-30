using UnityEngine;

namespace Game.Interaction
{
    /// <summary>
    /// Data definition for a single interactable object (lamp, rug, chair, etc.).
    /// One asset per object in the world. The same asset can be referenced by
    /// the same object across all three rooms — the dialogue chosen depends on
    /// the current GameState, not on which scene/room the asset lives in.
    /// </summary>
    [CreateAssetMenu(
        fileName = "InteractableData_",
        menuName = "Game/Interactable Data",
        order = 10)]
    public class InteractableData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Stable id used by save data and event listeners.")]
        public InteractableId id;

        [Tooltip("Human-readable name, e.g. shown in a tooltip on hover.")]
        public string displayName;

        [Header("Per-Room Dialogues")]
        [Tooltip("Played the first time, and on repeat interactions, while in Room 1.")]
        public DialogueData room1Dialogue;

        [Tooltip("Default dialogue for Room 2 (before the NPC has given the hint).")]
        public DialogueData room2Dialogue;

        [Tooltip("Optional. If set, used in Room 2 AFTER the NPC has hinted about the key. " +
                 "Leave null if this object's Room 2 line shouldn't change.")]
        public DialogueData room2DialogueAfterHint;

        [Tooltip("Played in Room 3.")]
        public DialogueData room3Dialogue;

        /// <summary>
        /// Selects the appropriate dialogue for the current narrative state.
        /// Returning null is allowed — callers should treat it as "nothing to say"
        /// and skip running a dialogue (rather than crashing).
        /// </summary>
        public DialogueData GetDialogueFor(GameState state)
        {
            if (state == null) return null;

            switch (state.CurrentRoom)
            {
                case RoomId.Room1:
                    return room1Dialogue;

                case RoomId.Room2:
                    // After the NPC has dropped the hint, some objects (like "object x"
                    // and "object y") swap to a different line. If no override is set,
                    // fall back to the default Room 2 dialogue.
                    if (state.NpcHintGivenInRoom2 && room2DialogueAfterHint != null)
                        return room2DialogueAfterHint;
                    return room2Dialogue;

                case RoomId.Room3:
                    return room3Dialogue;

                default:
                    return null;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Light sanity check so missing data is obvious in the inspector.
            if (string.IsNullOrEmpty(displayName))
                displayName = name.Replace("InteractableData_", "");
        }
#endif
    }
}
