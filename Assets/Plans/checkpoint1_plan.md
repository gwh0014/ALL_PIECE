# Project Overview
- **Game Title**: Pirate's Cove: Tiny Sails
- **High-Level Concept**: A simple, vibrant top-down 3D cartoon pirate ship game. The player sails a tiny pirate ship through ocean levels from island to island, collecting golden treasure chests (point system) while steering clear of menacing enemy pirate ships and hungry, roaming giant sharks.
- **Players**: Single-player
- **Inspiration / Reference Games**: Wind Waker (Sailing style), Sid Meier's Pirates!, Tiny Sails
- **Tone / Art Direction**: Stylized, colorful, and cartoonish 3D (using stylized primitive meshes, vibrant water shader/material, and simple visual cues).
- **Target Platform**: PC (StandaloneWindows64)
- **Screen Orientation / Resolution**: Landscape 1920x1080
- **Render Pipeline**: Universal Render Pipeline (URP) with `PC_RPAsset`

---

# Core Requirements Matrix (CRITICAL IMPORTANCE)
Every requirement listed in the Checkpoint 1 rubric is classified as **CRITICAL** and must be fully realized with high-quality, bug-free, and extensible implementations tailored to this top-down pirate adventure:

| Requirement ID | Rubric Requirement | Our Implementation Design Choice | Importance |
| :--- | :--- | :--- | :--- |
| **REQ-01** | Create/continue original 3D game | Build the Pirate Sail framework in `Assets/Scripts/` and implement the gameplay loop in `SampleScene.unity`. | **CRITICAL** |
| **REQ-02** | Playable 3D world/level | Construct a beautiful top-down ocean scene populated with multiple distinct, custom islands of varying sizes and structures (sandy shores, rocky formations, palm tree placeholders). | **CRITICAL** |
| **REQ-03** | Controllable player | Implement a smooth, physics-based top-down **Pirate Ship Controller** with natural boat-like steering and momentum, using the **New Input System** (`InputSystem_Actions`). | **CRITICAL** |
| **REQ-04** | Working camera system | Implement a top-down camera system that smoothly follows the player's pirate ship, providing a perfect birds-eye overview of the ocean, nearby islands, and approaching hazards. | **CRITICAL** |
| **REQ-05** | At least three interactive/collectible/usable objects | Implement three distinct types of sea-faring items:<br>1. **Treasure Chests** (collectible points on islands)<br>2. **Wood Planks / Repair Kits** (collectible to restore ship health)<br>3. **Wind Boost Zones** or **Level Portals** (interactive objects to speed up the ship or finish the level). | **CRITICAL** |
| **REQ-06** | At least one interaction system | Build an event-driven `InteractionSystem` where the ship can trigger events (e.g., dock near an island, salvage floating wreckage/planks, activate a whirlpool/wind boost, or enter the Level Exit Portal). | **CRITICAL** |
| **REQ-07** | Basic UI feedback | Implement an immersive Pirate HUD canvas showing:<br>1. Current Level & Objective (e.g., "Level 1: Collect 3 Chests & Escape!")<br>2. Treasure Score ("Treasure: 0 / 3")<br>3. Ship Hull Integrity/Health ("Hull: 100%")<br>4. Contextual interaction prompt when near items/portals. | **CRITICAL** |
| **REQ-08** | Basic or placeholder audio | Set up `SimpleAudioManager` with:<br>1. Epic cartoon pirate music / sea shanty loop (BGM)<br>2. Spatial SFX for collecting treasure, taking damage (colliding with sharks/ships), and ship creaking/wind boosts. | **CRITICAL** |
| **REQ-09** | Add your own design choices | Add unique, playful hazards: **Roaming Enemy Ships** and **Big Sharks** that patrol the waters between islands. Colliding with them damages the ship's hull. | **CRITICAL** |
| **REQ-10** | Run without major errors | Ensure absolute stability, zero-console errors, and rigorous null-checks to allow seamless player testing. | **CRITICAL** |
| **REQ-11** | Update project README | Create a comprehensive `README.md` at the project root explaining the controls, Pirate concept, objectives, and asset credits. | **CRITICAL** |

---

# Game Mechanics

## Core Gameplay Loop
1. **Sailing (Player Ship)**: The player controls a tiny cartoon pirate ship, sailing over a stylized ocean. Movement features realistic but fun water drag and rotational turning.
2. **Foraging & Salvaging**: The player navigates close to islands or wreckage to collect Treasure Chests (for points) and Wood Planks (to repair hull damage).
3. **Hazard Avoidance**: The player must carefully steer around active hazard patrol loops containing:
   - **Enemy Pirate Ships**: Patrol between major islands, chasing the player if they sail too close.
   - **Big Sharks**: Fast, roaming underwater hazards that move in erratic circles or linear paths.
4. **Level Completion**: Once the player has gathered enough treasure, they must navigate to the Level Exit Portal (e.g., a golden light column or safe port) to unlock and transition to the next level.

## Controls and Input Methods
Fully integrated with the New Input System reading from `InputSystem_Actions`:
- **Movement (Steering & Sail Power)**: WASD or Left Stick (`Player/Move` action).
  - `W` / `Up` raises sails (adds forward momentum).
  - `S` / `Down` drops sails (brakes / slows down).
  - `A`/`D` / `Left`/`Right` steers the ship (rotates).
- **Interact / Dock**: Keyboard `E` / Gamepad West Button (`Player/Interact` action) to trigger specific island dockings or portal exits.
- **Jump/Action (Optional Boost)**: Spacebar / Gamepad South Button (`Player/Jump` action) to trigger a temporary wind speed boost.

---

# UI Design & HUD Wireframe
The Screen-space UI Canvas provides instant ocean feedback:
1. **Captain's Log (Top Left)**: Shows level goal (e.g., "Level: 1 - Tortuga Shallows | Objective: Collect 3 Chests to Open Safe Port").
2. **Ship Hull Integrity (Bottom Left)**: A stylized red health bar (or numeric display) reading "Hull: [========] 100%".
3. **Treasure Cargo (Top Right)**: Coin icon displaying total treasureCollected (e.g., "Treasure: 0 / 3").
4. **Crow's Nest Alert (Center Bottom)**: Prompts like "[E] Salvage Wood Planks" or "BEWARE! Sharks Nearby!" or "[E] Sail to Next Level".

---

# Key Asset & Context

### 1. `SimpleAudioManager.cs`
Plays background sea shanties and plays sounds for gold pickup, taking damage, and level completion.
```csharp
public class SimpleAudioManager : MonoBehaviour {
    public static SimpleAudioManager Instance { get; private set; }
    public AudioSource BGM;
    public AudioSource SFX;
    public void PlaySFX(AudioClip clip);
}
```

### 2. `IInteractable.cs` & `InteractionSystem`
A completely decoupled interaction model.
```csharp
public interface IInteractable {
    string InteractionPrompt { get; }
    void OnInteract(GameObject instigator);
}
```

### 3. `PirateShipController.cs`
A robust movement script using a standard `Rigidbody` or `CharacterController` representing the boat. Includes forward thrust (sail power) and rotational torque (steering) to feel boat-like on the Y-plane (with gravity keeping the boat on the water surface).

### 4. `Hazards (Sharks & Enemy Ships)`
- **EnemyShipPatrol.cs**: Simple waypoint-based navigation. If the player gets within a certain radius, it sails towards them.
- **SharkPatrol.cs**: Swims in a circular or sinusoidal pattern, acting as a hazard.
- Both deal damage to the player's hull upon collision, prompting the player to use Wood Planks.

### 5. `Interactables`
- **TreasureChest.cs**: Collectible chest on islands. Automatically picked up on collision or via interaction key, incrementing score.
- **WoodPlank.cs**: Floats on the water. Automatically repairs a portion of hull health on collision.
- **LevelExitPortal.cs**: Safe port or whirlpool that transitions to the next level (or restarts the level loop with higher difficulty) if the chest requirements are met.

---

# Implementation Steps

### Step 1: Create the Stylized Ocean & Islands Environment (REQ-01, REQ-02)
- **Description**: Model/set up a beautiful ocean level inside `SampleScene.unity`. Place multiple stylized islands. Decorate them with 3D primitives (cylinders/capsules for palm trees, sand banks, and rocky formations).
- **Assigned Role**: Developer
- **Dependencies**: None
- **Parallelizable**: Yes

### Step 2: Implement the Pirate Ship Controller (REQ-03)
- **Description**: Add the player's pirate ship prefab (using simple 3D primitives - a brown box boat with a white sail plane). Attach a `Rigidbody` and write `PirateShipController.cs` that maps WASD controls to steering rotation and forward sail force.
- **Assigned Role**: Developer
- **Dependencies**: Step 1
- **Parallelizable**: No

### Step 3: Implement top-down Follow Camera (REQ-04)
- **Description**: Position and configure the Main Camera looking downward at an angle (e.g., 45-60 degrees) following the player ship smoothly without clipping.
- **Assigned Role**: Developer
- **Dependencies**: Step 2
- **Parallelizable**: No

### Step 4: Implement Interaction & Interactive Objects (REQ-05, REQ-06)
- **Description**: Code `IInteractable.cs`, `TreasureChest.cs` (for scoring), `WoodPlank.cs` (for healing), and `LevelExitPortal.cs` (for transitioning). Set up physical colliders on islands and floating items.
- **Assigned Role**: Developer
- **Dependencies**: Step 2
- **Parallelizable**: Yes

### Step 5: Implement Enemy Ships & Roaming Sharks (REQ-09)
- **Description**: Create hazard prefabs (gray enemy ship, blue/dark gray shark primitive). Write `EnemyShipPatrol.cs` and `SharkPatrol.cs` to patrol water lanes between islands and deal hull damage on collision with the player.
- **Assigned Role**: Developer
- **Dependencies**: Step 2, Step 4
- **Parallelizable**: Yes

### Step 6: Create the Pirate HUD & UI (REQ-07)
- **Description**: Implement `UIManager.cs` and a Screen-space Canvas. Display live Treasure count, Hull Health percentage, Level Goals, and interaction prompts.
- **Assigned Role**: Developer
- **Dependencies**: Step 4, Step 5
- **Parallelizable**: No

### Step 7: Audio Integration (REQ-08)
- **Description**: Setup `SimpleAudioManager.cs`. Assign sea-shanty BGM and connect sound clips for picking up chests, hull damage, and sailing boosts.
- **Assigned Role**: Developer
- **Dependencies**: Step 4, Step 6
- **Parallelizable**: Yes

### Step 8: Multi-Level Architecture, Validation & README (REQ-10, REQ-11)
- **Description**: Wrap everything in a game manager that handles multiple levels (repositioning islands, resetting chests/enemies, tracking progression). Complete a fully detailed, high-quality `README.md` at the project root.
- **Assigned Role**: Developer/Explorer
- **Dependencies**: All prior steps
- **Parallelizable**: No

---

# Verification & Testing
1. **Sailing Controls Check**: Verify that steering left/right rotates the ship smoothly, and moving forward adds satisfying momentum, feeling like sailing on water.
2. **Hazard Damage Check**: Intentionally collide with an Enemy Ship or a Shark. Confirm that the Hull Integrity percentage decreases and a red flashing indicator or damage sound triggers.
3. **Salvage Check**: Drive over a floating Wood Plank. Verify that it restores Hull Health up to 100% and plays a repair/splash sound.
4. **Treasure & Progression Check**: Dock at islands and collect Treasure Chests. Confirm the HUD score increments.
5. **Level Win Check**: Collect all 3 chests, sail to the Level Exit Portal, and confirm the level transitions successfully, resetting chest states and advancing the level counter.
6. **No Errors Policy**: Verify that the console is entirely clean of errors or warnings during play.
