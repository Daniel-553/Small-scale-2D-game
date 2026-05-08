# How to edit content

Practical recipes for the things you'll actually do day-to-day. Each recipe is self-contained — read the one you need, ignore the rest.

If you haven't already, skim **[`WORKFLOW.md`](WORKFLOW.md)** first so the words "DialogueData" and "InteractableData" mean something to you.

---

## Quick reference: where things live

| What | Where | How to find in Unity |
|---|---|---|
| Dialogue lines | `Assets/_Project/ScriptableObjects/Dialogues/` | Project window, navigate to the folder |
| Object definitions (lamp, rug, etc.) | `Assets/_Project/ScriptableObjects/Interactables/` | Same |
| NPC definitions | `Assets/_Project/ScriptableObjects/NPCs/` | Same |
| The actual rooms | The Main scene | Hierarchy window, open Main.unity |
| The popup look | `Assets/_Project/Prefabs/UI/DialogueBox.prefab` | Project window |

---

## Recipe 1 — Change what an NPC or object says in a specific room

This is the most common task. Let's say you want to change what the NPC says in Room 1.

1. In Unity's Project window, navigate to `Assets/_Project/ScriptableObjects/Dialogues/`.
2. Find the dialogue file for that line. By naming convention it's something like `Dialogue_NPC_Room1.asset`. Click it.
3. In the Inspector (right side), you'll see two fields:
   - **Speaker Name** — what shows above the line (e.g. "Old Man")
   - **Lines** — a list of strings, each one a separate popup
4. Edit the lines. Click the `+` to add a line, the `-` to remove one. Drag the handles to reorder.
5. Save the project (Ctrl+S / Cmd+S). Done.

> ⚠️ **No need to enter Play mode to test small edits** — you can check the wording by reading the asset directly. To see the new lines in-game, just press Play.

### What if the dialogue file doesn't exist yet?

If you want a *new* line that has no asset yet:

1. In the Dialogues folder, right-click → **Create → Game → Dialogue Data**.
2. Name it descriptively, e.g. `Dialogue_Lamp_Room2_AfterHint`.
3. Fill in the Speaker Name and Lines.
4. Now go assign it to the object that should use it (see Recipe 2).

---

## Recipe 2 — Make an object use a different dialogue

Let's say you've created a new dialogue called `Dialogue_Lamp_Room2_AfterHint` and you want the lamp to use it after the NPC drops the hint.

1. Navigate to `Assets/_Project/ScriptableObjects/Interactables/`.
2. Click `InteractableData_Lamp.asset` (or whatever object you're editing).
3. In the inspector you'll see a list of dialogue slots:
   - Room 1 Dialogue
   - Room 2 Dialogue *(default — used before the hint)*
   - Room 2 Dialogue After Hint *(optional — used once the NPC has dropped the hint)*
   - Room 3 Dialogue
4. Drag your new `Dialogue_Lamp_Room2_AfterHint` asset from the Project window into the **Room 2 Dialogue After Hint** slot.
5. Save.

That's it. Next time the player examines the lamp in Room 2 *after* the NPC has spoken, it will use the new dialogue. If they examine it before the NPC speaks, it falls back to the regular Room 2 dialogue.

> 💡 **The "After Hint" field is optional.** If you leave it empty, the object just uses the regular Room 2 dialogue regardless of whether the hint has been given. Most objects don't need to change — only the ones meaningful to the narrative (like object x and object y).

---

## Recipe 3 — Add a brand-new object type (e.g. a Clock)

You want to put a clock on the wall in all three rooms. Here's the full process.

### Part A — Add the type to the project

1. Open `Assets/_Project/Scripts/Core/Enums.cs` in any text editor (Visual Studio, VS Code, or even Notepad — you're not really programming, just adding a name to a list).
2. Find this section:
   ```csharp
   public enum InteractableId
   {
       None     = 0,
       Lamp     = 1,
       Rug      = 2,
       // ...
   }
   ```
3. Add a new line at the bottom **before the closing brace**:
   ```csharp
       Clock    = 99,   // pick a number not already used
   ```
4. Save the file. Unity will recompile automatically when you switch back to it.

> ⚠️ **Never reorder or delete entries** in this enum. Old assets reference these by number, so changing them mid-project breaks existing data.

### Part B — Create the dialogues

For each room you want the clock to talk in, create a DialogueData asset (Recipe 1, second part). You'll typically end up with:
- `Dialogue_Clock_Room1.asset`
- `Dialogue_Clock_Room2.asset`
- `Dialogue_Clock_Room3.asset`

### Part C — Create the InteractableData

1. In `Assets/_Project/ScriptableObjects/Interactables/`, right-click → **Create → Game → Interactable Data**.
2. Name it `InteractableData_Clock`.
3. In the inspector:
   - **Id**: Clock (the new entry you added)
   - **Display Name**: "Clock"
   - **Room 1 Dialogue**: drag in `Dialogue_Clock_Room1`
   - **Room 2 Dialogue**: drag in `Dialogue_Clock_Room2`
   - **Room 3 Dialogue**: drag in `Dialogue_Clock_Room3`
   - Leave "Room 2 Dialogue After Hint" empty unless the clock should change mid-Room-2.

### Part D — Place the clock in the scene

1. Open `Main.unity`.
2. The easiest path: duplicate an existing interactable GameObject (right-click in the Hierarchy → Duplicate). Rename it "Clock."
3. Replace its sprite with the clock sprite.
4. In its `Interactable` component, drag in `InteractableData_Clock` to replace the old data reference.
5. Move it to where you want it in Room 1.
6. Repeat for Room 2 and Room 3 — duplicate the Clock GameObject, drop it in the right room.

> 💡 **You can also just duplicate the same Clock GameObject** into all three rooms. They all reference the same `InteractableData_Clock` asset, which already knows what to say in each room. Whichever room the player is currently in determines the line.

---

## Recipe 4 — Change which object hides the key in Room 2

Right now, "object x" is whichever Interactable has its **Hook** field set to `GiveKeyIfHintGiven`. You can move this to any object.

1. Open the Main scene.
2. Find the current "object x" GameObject in the hierarchy. Look at its Interactable component — the Hook field will be set to `GiveKeyIfHintGiven`.
3. Set its Hook field back to `None`.
4. Find the new object you want to be "object x." Set *its* Hook field to `GiveKeyIfHintGiven`.
5. (Optional) Update the dialogue lines so the NPC's hint mentions the new object, and the new object's "Room 2 After Hint" dialogue makes sense for finding a key under it.

The narrative is now wired to the new object — no code changes required.

---

## Recipe 5 — Adjust the dialogue popup's appearance

The popup is a Unity prefab. Edit it like any other UI.

1. In the Project window, open `Assets/_Project/Prefabs/UI/DialogueBox.prefab`.
2. Unity will open it in prefab edit mode.
3. Change fonts, colors, sizes, the box graphic, the Continue button — anything visual.
4. Save the prefab (Ctrl+S).

The DialogueUI script doesn't care about styling — it just sets the text content. As long as the speaker label, line label, and continue button references stay wired up in the DialogueUI component fields, you can redesign freely.

---

## Recipe 6 — Tweak gameplay feel

These are small numbers exposed in the inspector. No code editing needed.

| What | Where | What it does |
|---|---|---|
| Player movement speed | Player GameObject → PlayerController → Move Speed | Higher = faster |
| Interact key | Player GameObject → PlayerInteractor → Interact Key | Default is E. Set to Space, F, etc. |
| Minimum line duration | DialogueRunner → Min Line Duration | How long a line stays visible before the player can advance. Prevents accidental skipping. Default 0.15s. |
| Interaction range | Player GameObject → second Collider2D (the trigger one) | Make the collider bigger to allow interacting from further away |

---

## Recipe 7 — Test the narrative chain quickly

You don't always want to play through all three rooms to test something in Room 3. Two shortcuts:

### Edit GameState directly during Play mode

1. Press Play.
2. Select the GameState GameObject in the hierarchy.
3. In the inspector, manually set `Has Key = true`, `Current Room = Room3`, etc.
4. Move the player to where you want and test.

> ⚠️ **Changes during Play mode don't persist.** When you exit Play mode, everything resets. This is what you want — but it means if you tweak a value and like it, you have to make the change again outside Play mode.

### Use a debug starting state

If you find yourself constantly setting up the same test conditions, ask the developer to add a "Debug Start" option to GameState (e.g. a checkbox that auto-sets the key on game start). Takes them five minutes.

---

## Recipe 8 — Add a fourth room (the bigger surgery)

This requires a small code change because the room count is baked into a few enums and data asset fields. It's still mostly mechanical:

1. **Add the enum entry** in `Enums.cs`: `Room4 = 4` (Recipe 3, Part A pattern).
2. **Add fields to InteractableData.cs and NPCData.cs:** a `room4Dialogue` field, and update the `GetDialogueFor` method to handle Room4. This is real code editing — ask the developer if you're not comfortable.
3. **Re-fill all your existing InteractableData and NPCData assets** with their new Room 4 dialogues. (Empty dialogue = silence in that room, which may be fine.)
4. **Build the room in the scene** — duplicate Room 3, change its `Room` component's `RoomId` field to `Room4`, move it into position.

---

## A few do's and don'ts

✅ **Do:**
- Name your dialogue assets descriptively. `Dialogue_NPC_Room2_HintGiven` beats `Dialogue47`.
- Test edits in Play mode, but make changes in Edit mode.
- Use the same DialogueData asset in multiple places if the same line is repeated. Edit once, applies everywhere.
- Watch the GameState component during Play mode to see flags flipping in real time. It's the best way to confirm the narrative chain is working.

❌ **Don't:**
- Reorder or renumber entries in `InteractableId` or `RoomId`. Existing assets reference them by number.
- Delete a DialogueData asset that's still referenced somewhere. Unity will complain ("Missing reference") and the object will fall back to silence. If you must delete, search the project first to find what uses it.
- Edit the C# files in `Scripts/Dialogue/` or `Scripts/Player/` unless you know what you're doing — those are the engine, not the content.

---

## Cheat sheet — what controls what

```
"Change what someone says"                     → DialogueData asset
"Change what an object says room-to-room"      → InteractableData / NPCData asset
"Change the rules of when things happen"       → C# code (talk to the dev)
"Change how the popup looks"                   → DialogueBox prefab
"Change where things are placed in the world"  → Main scene
"Change who has what behavior"                 → GameObject components in the scene
```
