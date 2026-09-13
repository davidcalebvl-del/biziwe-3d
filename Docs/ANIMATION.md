# Biziwe — Getting Realistic Character Animation (Not Robotic)

## The honest breakdown

"Robotic walking" comes from one of two things:
1. **No animation data at all** — characters just glide/slide with legs not
   moving. This was the actual state of the project before this doc —
   `PlayerController` and `NpcController` moved positions but never touched
   an Animator.
2. **Animation clips that exist but blend poorly** — snapping instantly
   between Idle and Walk instead of smoothly transitioning looks stiff even
   with good source animations.

**#2 is fixed by code** — `PlayerAnimatorController.cs` and
`NpcAnimatorController.cs` (added in this repo) smooth those transitions.

**#1 needs real animation data**, which is an art asset, not code. Here's
the practical, free way to get genuinely realistic human walk/run/idle
animations:

## Getting real animations — Mixamo (free)

1. Go to **mixamo.com**, sign in with a free Adobe account.
2. Browse **Characters** — pick one (or use your own model later once you
   have one, uploadable here for auto-rigging).
3. Browse **Animations** — search "Walking", "Running", "Idle", "Jump".
   These are real motion-captured animations — this is what actually
   solves the robotic-look problem.
4. Download each with these settings: Format **FBX for Unity**, Skin
   **Without Skin** (if you already have your character in Unity) or
   **With Skin** (if downloading the Mixamo character itself).
5. Drag the downloaded FBX files into Unity's `Assets/Animations` folder.

## Wiring them up in Unity

1. Select your character model → Inspector → **Rig** tab → Animation Type:
   **Humanoid** → Apply. (Mixamo animations are built for Unity's Humanoid
   rig system — this step is what makes them work with any humanoid model,
   not just the exact Mixamo character.)
2. Create an **Animator Controller** asset (right-click in Project →
   Create → Animator Controller).
3. Open it (double-click) → this opens the Animator window.
4. Right-click in the graph → Create State → From New Blend Tree.
5. Open the Blend Tree, add your Idle/Walk/Run clips as motion fields, set
   the Blend Tree's parameter to **Speed** (this must match
   `PlayerAnimatorController.speedParam`, which defaults to `"Speed"`).
6. Set each clip's threshold: Idle = 0, Walk = ~0.5, Run = 1.
7. Add an **Animator** component to your character, assign this Animator
   Controller to it.
8. Add `PlayerAnimatorController.cs` (player) or `NpcAnimatorController.cs`
   (NPCs) alongside it — they'll drive the Speed parameter automatically
   based on real movement.

## Why this combination matters

Good animation clips + poor blending = still looks stiff.
Poor animation clips + good blending = still looks bad, just smoothly bad.

You need both — real motion-captured clips (Mixamo) for the actual human
movement quality, and smooth parameter blending (already built into this
repo's animator scripts) so transitions between those clips don't snap.
Together, that's what gets you "looks like a person walking on film"
instead of a robot.
