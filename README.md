# Three-Room Demo — Project Documentation

A small top-down 2D Unity game where the player walks through three connected rooms, talks to an NPC and various objects, finds a key, and escapes through a door.

This documentation is written for **designers and non-programmers**. You don't need to read or write C# to do most things in this project — the dialogue, the room logic, and the narrative branching are all controlled by data files (called ScriptableObjects) that you edit through Unity's inspector, like filling in a form.

---

## Where to start

If you just want to **add or change dialogue**, read **[`docs/HOW-TO-EDIT-CONTENT.md`](docs/HOW-TO-EDIT-CONTENT.md)** — that's the one you'll use day-to-day.

If you want to **understand the big picture** of how a player's interaction in the game becomes a popup on screen, read **[`docs/WORKFLOW.md`](docs/WORKFLOW.md)**.

If you want a **map of the project files** and what each script does, the rest of this README covers that.

---

## What's in the box

The project is laid out under `Assets/_Project/`. The leading underscore is intentional — it keeps your folder at the top of Unity's Project window, above any imported packages.

```
_Project/
├── Art/                    ← sprites and tile images
├── Audio/                  ← music and sound effects
├── Prefabs/                ← reusable game objects (Player, NPC, Interactable, UI)
├── ScriptableObjects/      ← the DATA files you'll edit most often
│   ├── Dialogues/          ← one file per dialogue (lines of text)
│   ├── Interactables/      ← one file per object type (lamp, rug, etc.)
│   └── NPCs/               ← one file per NPC
├── Scenes/
│   └── Main.unity          ← the actual game scene with all 3 rooms
└── Scripts/                ← C# code (you don't need to touch these)
    ├── Core/               ← shared definitions, game state
    ├── Player/             ← movement and interaction
    ├── Interaction/        ← objects, NPCs, doors, the ladder
    ├── Dialogue/           ← popup system
    └── World/              ← rooms
```

---

## The three rooms (the narrative)

The player walks left to right through three rooms. Each room contains:

- **The same set of light-gray interactive objects** (lamp, rug, chair, etc.)
- **An NPC** (the figure standing on the right side of each room)

The objects look identical room to room — but **what they say is different** in each room. That's the whole point of the design.

### Room 1 — Introduction
The player meets the NPC and pokes around the objects. Both NPC and objects say their "Room 1 lines." Nothing else happens; this room is for setting tone.

### Room 2 — The hint
The NPC tells the player *"the key is hidden under object x."* After hearing that, the same objects start saying different things — and examining object x reveals (or grants) the key.

### Room 3 — The escape
The NPC says one more line. Once that line ends, a ladder appears in the middle of the room. The player climbs up to a door at the top.

- **If they have the key:** the door opens and the demo ends.
- **If they don't:** the door's locked dialogue plays. (Shouldn't happen if Room 2 was completed, but it's a safety net.)

---

## How the code is organized (a 30-second tour)

Every script file lives under `_Project/Scripts/` in one of five folders. Here's what each folder is for and which files matter:

### `Scripts/Core/` — the shared rulebook

| File | What it does |
|---|---|
| `Enums.cs` | Defines the names `Room1`, `Room2`, `Room3` and the list of interactable types (Lamp, Rug, etc.). When you want to add a new object type, this is the only code file you'll need to peek into. |
| `GameState.cs` | The "memory" of the game. Keeps track of which room the player is in, whether they have the key, and whether the NPC has given the hint. Everything else asks this file "what's the situation right now?" |

### `Scripts/Dialogue/` — the popup system

| File | What it does |
|---|---|
| `DialogueData.cs` | Defines what a "dialogue" is — a speaker name and a list of lines. You don't edit this code; you create *assets* based on it (see the how-to). |
| `DialogueRunner.cs` | The thing that actually shows lines on screen one at a time and waits for the player to click Continue. |
| `DialogueUI.cs` | The visual popup itself — the box, the text, the Continue button. |

### `Scripts/Interaction/` — things you can talk to

| File | What it does |
|---|---|
| `IInteractable.cs` | A "contract" that says: any object the player can interact with must have these three things. |
| `InteractableData.cs` | The data file for an object type. Stores its three (or four) dialogues — one per room state. |
| `Interactable.cs` | Sits on each light-gray object in the world. Looks up the right dialogue based on what room the player is in, and plays it. |
| `NPCData.cs` | Same idea as InteractableData, but for NPCs. |
| `NPC.cs` | Sits on the NPC figure. Plays the right dialogue and triggers narrative beats (like setting the "hint given" flag in Room 2). |
| `Door.cs` | The exit at the top of the ladder in Room 3. Checks if the player has the key. |
| `Ladder.cs` | The ladder that appears in Room 3. Starts hidden, reveals itself when the right flag flips. |
| `KeyPickup.cs` | Optional. A physical key the player can walk over to pick up, if you want that flow. |

### `Scripts/Player/` — the player character

| File | What it does |
|---|---|
| `PlayerController.cs` | Top-down WASD movement. Stops when a dialogue is open. |
| `PlayerInteractor.cs` | Detects which interactable is closest to the player, and when you press E, calls "Interact" on it. |

### `Scripts/World/` — rooms

| File | What it does |
|---|---|
| `Room.cs` | Sits on each room. When the player walks into the room, it tells GameState "the player is now in Room N." |

---

## The mental model

If you only remember one thing from this whole document, make it this:

> **Code is the machinery. Data files are the script.**

The C# scripts are written and (mostly) finished. They define *how* dialogues play, *how* the ladder appears, *how* the door checks the key. **You will rarely touch them.**

The actual content of the game — what the NPC says, what each object says in each room, which objects exist — lives in **ScriptableObject assets** under `_Project/ScriptableObjects/`. Editing those is just filling in fields in Unity's inspector. **That's where you'll spend your time.**

---

## Next steps

- **[`docs/WORKFLOW.md`](docs/WORKFLOW.md)** — how a button press becomes a popup on screen, step by step.
- **[`docs/HOW-TO-EDIT-CONTENT.md`](docs/HOW-TO-EDIT-CONTENT.md)** — practical recipes for the most common tasks: adding a dialogue, changing what an object says in Room 2, etc.

---

## Glossary (terms you'll see)

| Term | What it means |
|---|---|
| **GameObject** | A "thing" in the Unity scene — the player, an object, a wall. Everything visible is a GameObject. |
| **Component** | A behavior or property attached to a GameObject. A GameObject is a Lego brick; components are the bumps on top. The same GameObject can have a sprite component, a collider component, a script component, etc. |
| **Prefab** | A pre-built GameObject template you can stamp out copies of. The Player prefab, for example, is built once and reused. |
| **ScriptableObject** | A data file that lives in the project (not in a scene). Holds settings or content. We use these for dialogues, interactable definitions, and NPC definitions. |
| **Asset** | Any file in the project — a sprite, a sound, a prefab, a ScriptableObject. |
| **Inspector** | The panel on the right side of Unity that shows the properties of whatever you've selected. This is where you fill in fields. |
| **Scene** | The actual game world layout — where things are placed. Our project has one scene: `Main.unity`. |
| **Trigger** | A type of collider that doesn't physically block movement but detects when something enters it. We use these for "the player walked into Room 2" and "the player is close enough to interact." |
