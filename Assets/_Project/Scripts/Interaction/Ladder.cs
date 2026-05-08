using UnityEngine;

namespace Game.Interaction
{
    /// <summary>
    /// The ladder in Room 3 starts hidden. Once GameState.LadderRevealedInRoom3
    /// flips to true (set by the NPC's Room 3 dialogue completing), the ladder
    /// becomes visible/active and the player can use it to reach the door.
    ///
    /// Implementation detail: rather than wiring a specific NPC reference here,
    /// we just listen for state changes. Anyone who sets the flag — the NPC,
    /// a debug command, a save load — will trigger the same reveal.
    /// </summary>
    [DisallowMultipleComponent]
    public class Ladder : MonoBehaviour
    {
        [Tooltip("The visual + collider root that should toggle on/off. " +
                 "If left empty, falls back to this GameObject.")]
        [SerializeField] private GameObject visualRoot;

        private void OnEnable()
        {
            if (GameState.Instance != null)
                GameState.Instance.OnStateChanged += Refresh;
            Refresh();
        }

        private void OnDisable()
        {
            if (GameState.Instance != null)
                GameState.Instance.OnStateChanged -= Refresh;
        }

        private void Refresh()
        {
            var target = visualRoot != null ? visualRoot : gameObject;
            var state = GameState.Instance;
            if (state == null) return;

            target.SetActive(state.LadderRevealedInRoom3);
        }
    }
}
