# Biziwe 3D — UI Assets (Icons & Buttons)

White, flat, minimal icon set matching the reference style you shared —
functional placeholder art, ready to wire into the HUD/controls right now.

## What's here

```
Assets/Sprites/UI/Buttons/
  chevron_up_white.png     — the layered chevron/arrow button (Gas)
  chevron_down_white.png   — same, flipped (Brake/Reverse)
  chevron_left_white.png   — rotated (Steer Left)
  chevron_right_white.png  — rotated (Steer Right)

Assets/Sprites/UI/Icons/
  icon_pistol.png
  icon_shotgun.png
  icon_knife.png
  icon_grenade.png
  icon_rocket.png
  icon_medkit.png
  icon_ammo.png
```

All transparent-background PNGs, white foreground — they'll take a colour
tint fine if you want to recolor per-weapon-type later (Unity Image
components have a Color field that multiplies over a white sprite, so this
set is deliberately built to support that).

## Wiring them in

1. Select the PNG in Unity's Project window.
2. In the Inspector, set **Texture Type: Sprite (2D and UI)**, Apply.
3. Drag it onto a UI **Button** or **Image** component's Source Image field.
4. For the chevron buttons specifically: use `chevron_up_white` /
   `chevron_down_white` for the Gas/Brake buttons, and
   `chevron_left_white` / `chevron_right_white` for Steer Left/Right —
   these map directly to the buttons already described in
   `Docs/CONTROLS.md`.

## Honest note on quality

These are clean, functional placeholders — good enough to wire up the UI
and playtest with right now. They're hand-generated flat icons, not
professional game art. When you're ready for a truly polished final look,
swapping these for real designed icons is a drop-in replacement (same
file names, same import settings) — no code changes needed anywhere,
since every script references these by Inspector-assigned sprite, not by
hardcoded path.
