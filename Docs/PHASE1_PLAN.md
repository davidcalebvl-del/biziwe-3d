# Biziwe 3D — Phase 1 Plan & Setup Guide

Real 3D open-world game, built with Unity. This is Phase 1: the smallest
real, playable foundation — everything after this builds on top of it.

## What's in this repo right now

Scripts are ready and written, but **this is not yet a full Unity project**
you can double-click and open. Unity projects need files (ProjectSettings,
.meta files, Packages) that only get created correctly by Unity itself when
you make a new project. Trying to fake those by hand risks a broken/corrupt
project — so instead, you'll create a fresh Unity project yourself (takes 2
minutes) and then drop these scripts in. Full steps below.

```
Assets/Scripts/Player/PlayerController.cs        — walk, run, jump, crouch, dies via Health
Assets/Scripts/Player/CameraFollow.cs            — third-person camera
Assets/Scripts/Player/CharacterCustomization.cs  — free-text name + appearance selection
Assets/Scripts/Vehicle/VehicleController.cs      — drivable car physics
Assets/Scripts/Vehicle/VehicleStats.cs           — per-vehicle-type handling data (sports car vs. truck, etc.)
Assets/Scripts/Vehicle/CarjackHandler.cs         — pull driver out, take the car
Assets/Scripts/Vehicle/VehicleHealth.cs          — cars take damage, smoke, explode when destroyed
Assets/Scripts/AI/NpcController.cs               — NPC patrol/chase; police now fight back in range
Assets/Scripts/AI/CivilianNpc.cs                 — wandering civilians that flee danger
Assets/Scripts/AI/CarjackVictim.cs               — victim shouts, flees, seeks a car, chases, fights
Assets/Scripts/AI/PoliceVehicleAI.cs             — police cars drive-chase the player
Assets/Scripts/AI/TrafficCarAI.cs                — civilian traffic driving fixed routes
Assets/Scripts/AI/NpcSpawner.cs                  — population streaming/LOD pooling for civilians
Assets/Scripts/AI/NpcSchedule.cs                 — NPC daily routine (home/work/free-roam by time of day)
Assets/Scripts/World/BuildingEntrance.cs         — enter a building (shop, house, etc)
Assets/Scripts/World/InteriorExit.cs             — leave the interior, back outside
Assets/Scripts/World/ShopInterior.cs             — actual buy ammo/weapons/healing, spends real money
Assets/Scripts/World/VehicleGarage.cs            — buy/spawn owned vehicles, real money spent
Assets/Scripts/World/RaceCheckpoint.cs           — checkpoint + RaceManager for the Racing mission type
Assets/Scripts/Systems/MissionCondition.cs       — gates missions on wanted level, time of day, or money
Assets/Scripts/World/AmbientLines.cs             — street chatter line bank (data)
Assets/Scripts/World/AmbientChatter.cs           — pairs nearby civilians to talk near the player
Assets/Scripts/World/DialogueBubble.cs           — floating text display for street chatter
Assets/Scripts/Systems/GameManager.cs            — central hub for the systems below
Assets/Scripts/Systems/Health.cs                 — shared damage/death component (player + NPCs)
Assets/Scripts/Systems/WantedSystem.cs           — 3-star wanted level/police response
Assets/Scripts/Systems/PoliceDispatcher.cs       — escalates police units as stars rise
Assets/Scripts/Systems/RoadblockSystem.cs        — spawns police roadblocks at higher wanted levels
Assets/Scripts/Systems/IDamageable.cs            — shared damage interface
Assets/Scripts/Systems/Destructible.cs           — shops/houses/cars that can be destroyed
Assets/Scripts/Systems/EconomySystem.cs          — money balance, capped at 1,000,000
Assets/Scripts/Systems/DayNightCycle.cs          — day/night sun + ambient lighting
Assets/Scripts/Systems/MissionData.cs            — mission definitions as data assets (scales to hundreds)
Assets/Scripts/Systems/MissionDatabase.cs        — loads/looks up MissionData assets by id
Assets/Scripts/Systems/MissionManager.cs         — mission state tracking (runtime)
Assets/Scripts/Systems/MissionTrigger.cs         — start/complete a mission by location
Assets/Scripts/Systems/SaveSystem.cs             — persists money, character, and mission progress
Assets/Scripts/Weapons/Weapon.cs                 — pistols through the rocket launcher
Assets/Scripts/Weapons/MeleeWeapon.cs            — knife combat with stealth finisher
Assets/Scripts/Weapons/Explosive.cs              — bombs with area blast damage
Assets/Scripts/Weapons/WeaponHolder.cs           — weapon inventory/switching
Assets/Scripts/UI/TouchInputManager.cs           — wires on-screen controls to the above
Assets/Scripts/UI/GameHUD.cs                     — stars, money, ammo, mission text — auto-synced
```

## Step-by-step setup (do this once you're on a PC/laptop)

1. **Install Unity Hub** — download from unity.com/download (free).
2. In Unity Hub, install the latest **Unity Editor LTS version**, and during
   install, check the box for **Android Build Support** (so you can build to
   your phone later).
3. Create a **New Project** → choose the **3D (URP)** template → name it
   `Biziwe3D`.
4. Once it opens, close Unity, and copy this repo's `Assets/Scripts` folder
   into your new project's `Assets` folder (replacing/merging).
5. Reopen Unity — it will auto-import the scripts. Any errors will show in
   the Console tab; that's normal at this stage since the scripts expect
   objects (a player capsule, wheel colliders, etc.) that don't exist in an
   empty project yet.
6. Follow the `SETUP` comments at the top of each script — they explain
   exactly what GameObjects and components to create in the Editor to wire
   everything up (e.g. adding a CharacterController to your player, creating
   WheelColliders for the car).

## Phase 1 scope (what we're building first — nothing more yet)

- [ ] One small city district (simple blockout geometry — grey boxes for
      buildings is fine at this stage, detail comes later)
- [ ] One playable character with walk/run/jump/crouch
- [ ] One drivable car
- [ ] Basic NPCs that patrol
- [ ] Basic police that chase when wanted level > 0
- [ ] Touch controls (on-screen joystick + buttons) for mobile build
- [ ] 3 simple missions (e.g. drive to a point, deliver something, escape
      police)

## Why this scope, not more yet

Every extra system (weapons, 40 countries, millions of NPCs, vehicle damage)
gets dramatically easier to add correctly once this core loop — move, drive,
get chased, complete a simple objective — actually works and feels good.
Building those bigger systems before this foundation is solid is how
projects like this collapse under their own weight. Small and real beats
big and broken.

## Next steps once Phase 1 works

See the original full vision doc for Phase 2 onward (more vehicles, more
NPC behaviours, traffic, better police AI) — we'll tackle that once Phase 1
is playable and stable on your device.
