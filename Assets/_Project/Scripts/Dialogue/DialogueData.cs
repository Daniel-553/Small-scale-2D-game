using UnityEngine;

namespace Game.Dialogue
{
   
    [CreateAssetMenu(
        fileName = "Dialogue_",
        menuName = "Game/Dialogue Data",
        order = 1)]
    public class DialogueData : ScriptableObject
    {
        [Tooltip("Shown above the line — e.g. 'Old Man', 'Lamp', or empty for narration.")]
        public string speakerName;

        [Tooltip("Lines are shown one at a time, advanced by the player.")]
        [TextArea(2, 6)]
        public string[] lines;
    }
}
