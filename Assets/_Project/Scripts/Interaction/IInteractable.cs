using UnityEngine;

namespace Game.Interaction
{
   
    public interface IInteractable
    {
       
        bool CanInteract { get; }

       
        Vector3 Position { get; }

    
        void Interact();
    }
}
