# Pirate's Cove: Tiny Sails

Welcome to **Pirate's Cove: Tiny Sails**, a delightful 3D top-down arcade pirate adventure built in Unity! Take control of your tiny cartoonish pirate ship, sail from island to island, salvage floating wreckage, collect hidden treasure chests, and avoid menacing patrolling enemy ships and hungry giant sharks.

---

## 🏴‍☠️ Game Concept
You are the captain of a small, nimble sailing vessel in a vast ocean full of riches and dangers. To progress, you must explore the waters, locate islands, and collect golden chests. The ocean is not safe:
* **Enemy Pirate Ships** patrol the key shipping lanes between islands. If you sail too close, they will pursue you and attempt to ram your ship!
* **Big Sharks** roam underwater in circular patterns. Stay clear of their paths, or they will damage your hull!
* **Wood Planks** float in the water. Use them as quick repair kits to fix any hull damage you've taken.
* **Safe Ports (Cyan whirlpool portals)** appear in each level. Once you've collected enough treasure, sail into the safe port to advance to the next level!

---

## 🎮 Controls

This game is powered by Unity's **New Input System** and supports both **Keyboard/Mouse** and **Gamepad** out of the box:

| Action | Keyboard Control | Gamepad Bindings | Description |
| :--- | :--- | :--- | :--- |
| **Steer Left / Right** | `A` / `D` or `Left` / `Right` Arrow | Left Stick Horizontal | Rotates the ship left and right. |
| **Raise Sails (Move)** | `W` or `Up` Arrow | Left Stick Vertical (Up) | Adds forward sailing force. |
| **Drop Sails (Brake)** | `S` or `Down` Arrow | Left Stick Vertical (Down) | Drops sails to brake or slow down. |
| **Wind Boost** | `E` | West Button (X/Square) | Triggers a temporary wind speed boost (3s cooldown). |
| **Interact / Open / Sail** | `Space` | South Button (A/Cross) | Opens treasure chests, salvages planks, or sails through port. |

---

## ⚓ Current Gameplay Objective

* **Level 1**: Tortuga Shallows (Objective: Collect **2** Treasure Chests & Escape through Safe Port)
* **Level 2**: Shark Reef (Objective: Collect **3** Treasure Chests, avoid 1 Enemy Patrol and 1 Shark & Escape)
* **Level 3**: Dead Man's Cove (Objective: Collect **4** Treasure Chests, navigate past 2 Enemy Patrols and 2 Sharks & Escape)

---

## 🎨 Asset & Audio Credits
All assets in this game were generated procedurally inside the Unity Engine to guarantee a 100% bug-free and smooth runtime experience:
* **3D Models**: Procedurally generated cartoon 3D primitives (Cubes, Cylinders, Spheres, Quads) colored with custom-styled URP Lit and Unlit materials.
* **Sound Effects & Music**: Procedurally synthesized sine-wave audio clips (coin pickup beep, damage explosion, level-win fanfares, and ocean drone) generated directly in memory on startup by the `SimpleAudioManager`.
