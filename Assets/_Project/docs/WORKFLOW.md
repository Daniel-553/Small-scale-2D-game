# How the game works, end to end

This document walks through what happens when the player does something, from button press to popup on screen. The goal is for you to be able to **read the game like a flowchart**, so when something goes wrong (or you want to change behavior), you know where to look.

Read this once front-to-back. After that, treat it as a reference.

---

## The cast of characters

Think of the project as a small team of specialists who only know how to do one job each. They don't talk to each other directly — they pass messages through a shared notebook.

| Role | Who | What they know |
|---|---|---|
| **The Notebook** | `GameState` | Which room the player is in. Whether they have the key. Whether the NPC has given the hint. Whether the ladder should be visible. **Everyone reads from this notebook.** |
| **The Player's Hands** | `PlayerInteractor` | What objects are nearby. When the player presses E, picks the closest one and tells it "do your thing." |
| **An Object** | `Interactable` | "I am a lamp. When someone interacts with me, I look up what I should say right now and ask the popup system to show it." |
| **The NPC** | `NPC` | Same as an object, but also flips flags in the notebook when its dialogue ends ("the hint has been given"). |
| **The Stage Manager** | `DialogueRunner` | Shows lines on screen one at a time. While it's working, it tells the player to stop moving. |
| **The Popup** | `DialogueUI` | The actual visible box with text and the Continue button. |
| **The Room** | `Room` | Each room has one. When the player walks in, it updates the notebook: "player is now in Room 2." |
| **The Ladder** | `Ladder` | Watches the notebook. When the "ladder revealed" flag flips on, it appears. |
| **The Door** | `Door` | When interacted with, checks the notebook for the key, then plays the appropriate dialogue. |

The key idea: **no one has direct phone numbers for anyone else**. The Ladder doesn't know about the NPC. The Door doesn't know about the Key. They all just read the notebook (`GameState`) and react to what's there. This is what makes the system flexible — you can rewire the narrative without rewriting code.

---

## The data files (where the content lives)

There are three kinds of data files you'll work with. They all live under `_Project/ScriptableObjects/`. Here's how they nest inside each other:

```
NPC asset (NPCData)
  ├── Room 1 dialogue → Dialogue asset (DialogueData) → list of lines
  ├── Room 2 dialogue → Dialogue asset (DialogueData) → list of lines
  └── Room 3 dialogue → Dialogue asset (DialogueData) → list of lines

Object asset (InteractableData), e.g. "Lamp"
  ├── Room 1 dialogue → Dialogue asset
  ├── Room 2 dialogue → Dialogue asset (default)
  ├── Room 2 dialogue (after hint) → Dialogue asset (optional)
  └── Room 3 dialogue → Dialogue asset
```

A **DialogueData asset** is the smallest unit. It's just:
- A speaker name (e.g. "Old Man" or "Lamp")
- A list of lines that play one after another

**An NPCData or InteractableData asset is a wrapper** that holds references to dialogue assets — one for each room. It's the thing that decides "which dialogue should I play right now?" based on what's in the notebook.

---

## What happens when the player interacts with something

Let's trace a real example. The player is in Room 1 and presses E next to the lamp.

### Step 1 — The player presses E
`PlayerInteractor` is watching for E presses. When it hears one, it looks at all the interactables currently in range and picks the closest one — the lamp.

It calls `Interact()` on the lamp.

### Step 2 — The lamp asks: "what should I say?"
The lamp is a GameObject with the `Interactable` component on it. That component has one field: a reference to an `InteractableData` asset called `InteractableData_Lamp`.

The lamp asks `InteractableData_Lamp`: *"the player just talked to me — what's my line?"*

### Step 3 — The data asset checks the notebook
`InteractableData_Lamp` looks at `GameState`:
- Current room? Room 1.
- *(For Room 2 only: has the hint been given? Doesn't matter here.)*

Based on that, it returns the **Room 1 dialogue asset** for the lamp.

### Step 4 — The dialogue plays
The lamp hands that dialogue to `DialogueRunner`, which:

1. Tells `DialogueUI` to show the popup.
2. Puts the speaker name and first line on screen.
3. Tells the player controller to stop accepting movement input.
4. Waits for the player to click Continue (or press E again).
5. Shows the next line. Repeats until done.
6. Hides the popup. Tells the player they can move again.

### Step 5 — Done
Nothing else happens for a normal object. The player walks away and might interact with the next thing.

---

## What happens when the player talks to the NPC in Room 2

This is the most interesting case because it changes the world.

### Steps 1–4 — same as above
The NPC's data asset checks the notebook (Room 2), finds the Room 2 dialogue, and plays it. The dialogue says *"the key is hidden under object x."*

### Step 5 — The NPC flips a flag
When the dialogue finishes, the NPC component does one extra thing: it writes to the notebook.

```
GameState.NpcHintGivenInRoom2 = true
```

That's it. No other system is told directly. The NPC just sets the flag and walks away (so to speak).

### Step 6 — The next time the player examines an object
Now when the player interacts with object x:

- The `Interactable` on object x asks `InteractableData_ObjectX` for its line.
- The data asset checks the notebook: current room is Room 2, **and** the hint flag is set.
- It returns the **"Room 2 after hint"** dialogue instead of the default Room 2 dialogue.

Same code path, different content. That's the whole trick.

### Step 7 — The key is granted
Object x has an extra setting in its `Interactable` component called the **hook**. It's set to `GiveKeyIfHintGiven`. After the dialogue finishes, the hook fires and sets:

```
GameState.HasKey = true
```

The player now has the key — even though there's nothing visibly in their inventory. The notebook says they have it, and that's what matters.

---

## What happens in Room 3

### The player walks into Room 3
`Room.cs` (on the Room 3 GameObject) detects the player entering and updates the notebook:

```
GameState.CurrentRoom = Room3
```

### The player talks to the NPC
Same flow as before. The NPC plays its Room 3 dialogue. When that dialogue ends, the NPC flips another flag:

```
GameState.LadderRevealedInRoom3 = true
```

### The ladder is watching
The `Ladder` component is subscribed to the notebook — it's listening for any change. When `LadderRevealedInRoom3` flips to true, the ladder turns its visuals on. It's been sitting in the scene the whole time, just invisible.

### The player climbs and reaches the door
The player presses E next to the door. The door checks the notebook:

- `HasKey == true`? → Plays the unlocked dialogue, then fires the "exit" event (loads the credits scene, ends the demo, whatever you wired up).
- `HasKey == false`? → Plays the locked dialogue. The door doesn't open.

---

## A picture of the flow

Here's the whole Room 2 narrative chain in one diagram. Read top to bottom.

```
Player presses E near NPC
        │
        ▼
NPC.Interact()
        │  asks NPCData "which line?"
        ▼
NPCData reads GameState.CurrentRoom = Room2
        │  returns the Room 2 dialogue
        ▼
DialogueRunner plays the dialogue
        │  (player can't move while it plays)
        ▼
Dialogue ends
        │
        ▼
NPC sets GameState.NpcHintGivenInRoom2 = true
        │
        ▼
[Player walks to object x]
        │
        ▼
Player presses E near object x
        │
        ▼
Interactable.Interact()
        │  asks InteractableData "which line?"
        ▼
InteractableData reads GameState
        │  CurrentRoom = Room2, HintGiven = true
        │  returns the "Room 2 after hint" dialogue
        ▼
DialogueRunner plays it
        │
        ▼
Dialogue ends
        │
        ▼
Hook fires: GameState.HasKey = true
        │
        ▼
[Player walks to Room 3, narrative continues]
```

---

## What changes — and what doesn't — when you edit content

Most edits you'll make **don't touch any of the flow above**. You're just changing what fills in the boxes.

| You want to... | What you change | Code changes? |
|---|---|---|
| Change what the NPC says in Room 2 | Edit a DialogueData asset | No |
| Add a fourth line to a dialogue | Edit a DialogueData asset | No |
| Make the lamp say something different in Room 3 | Edit `InteractableData_Lamp`'s Room 3 field | No |
| Add a new object type (e.g. a clock) | Create a new InteractableData asset, drop it on a GameObject | Maybe — if it's a brand new *type* you might add it to the InteractableId enum |
| Change which object x is in Room 2 | Move the "GiveKeyIfHintGiven" hook to a different object | No |
| Add a fourth room | Add Room4 to the enum, add fields to InteractableData/NPCData | Yes, small edits |
| Change how the dialogue popup looks | Edit the DialogueBox prefab in Unity (font, colors, layout) | No |
| Change the dialogue advance key from E to Space | Edit the field on PlayerInteractor in the inspector | No |
| Make the ladder appear from a different trigger | Set `LadderRevealedInRoom3 = true` from somewhere else | Possibly minor edit |

The pattern: **content changes are almost always asset edits**, not code edits. If you find yourself thinking "I need to ask the developer to change the code for this," double-check — there's a good chance it's a field somewhere.

---

## When something goes wrong

The most common issues, and where to look:

**Nothing happens when I press E next to an object.**
- Does the object have an `Interactable` component? (Select it, look in the inspector.)
- Does that component have a Data asset assigned? (The field shouldn't be empty.)
- Does the Data asset have a dialogue assigned for the current room?
- Is the object's collider set to "Is Trigger"? (It should be — the script forces this on Awake, but worth checking.)

**The dialogue plays but nothing on screen changes after.**
- This is normal for most objects. Only the NPC and "object x" are supposed to change state.
- If you expected something to change, check whether the relevant flag in `GameState` is being set. (You can watch this live during play mode — select the GameState GameObject and watch its inspector.)

**The ladder isn't appearing in Room 3.**
- Did the Room 3 NPC's dialogue actually finish? (It only flips the flag at the end, not on the first line.)
- Is the Ladder GameObject's `visualRoot` field assigned correctly?

**The player can't move after a dialogue.**
- The `DialogueRunner.IsRunning` flag is probably stuck. This usually means a dialogue was started but never ended properly. Check if there's an empty DialogueData asset somewhere (no lines).

---

## Where to go next

- **[`HOW-TO-EDIT-CONTENT.md`](HOW-TO-EDIT-CONTENT.md)** — step-by-step recipes for the most common edits.
