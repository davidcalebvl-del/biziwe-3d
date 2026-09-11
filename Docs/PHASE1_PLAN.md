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
Assets/Scripts/Player/PlayerController.cs   — walk, run, jump, crouch
Assets/Scripts/Player/CameraFollow.cs       — third-person camera
Assets/Scripts/Vehicle/VehicleController.cs — drivable car physics
Assets/Scripts/AI/NpcController.cs          — NPC patrol + chase behaviour
Assets/Scripts/Systems/WantedSystem.cs      — basic wanted-level/police system
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
