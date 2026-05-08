namespace Game
{
    /// <summary>
    /// Identifies one of the three rooms. Used by GameState and Room components.
    /// Values are explicit so they're stable across save data and inspector changes.
    /// </summary>
    public enum RoomId
    {
        None  = 0,
        Room1 = 1,
        Room2 = 2,
        Room3 = 3,
    }

    /// <summary>
    /// Stable identifier for each interactable object type that appears in the rooms.
    /// Add new entries at the bottom — never reorder, since these values may end up
    /// referenced by save files or analytics events.
    /// </summary>
    public enum InteractableId
    {
        None     = 0,
        Lamp     = 1,
        Rug      = 2,
        Chair    = 3,
        Table    = 4,
        Bookcase = 5,
        // Add more as needed.
    }
}
