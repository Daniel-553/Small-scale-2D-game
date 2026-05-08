using UnityEngine;

namespace Game.Interaction
{
    /// <summary>
    /// Anything the player can target with the interact key implements this.
    /// PlayerInteractor finds these via GetComponent on colliders inside its
    /// trigger range, picks the closest one whose CanInteract is true, and
    /// calls Interact() on the interact key press.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Whether the player should be allowed to interact right now. Used to
        /// gate things like a Door that's locked, or an object that's already
        /// been used and shouldn't trigger again. Returning false also lets
        /// PlayerInteractor skip past this object to the next nearest one.
        /// </summary>
        bool CanInteract { get; }

        /// <summary>
        /// World position used by PlayerInteractor to pick the nearest target
        /// when multiple interactables are in range.
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// Triggered by the player. Implementations typically open a dialogue,
        /// pick up an item, or fire a game event.
        /// </summary>
        void Interact();
    }
}
