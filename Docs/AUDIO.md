# Biziwe — Audio

## What's built (the playback system)

- **`MusicManager.cs`** — background music that reacts to game state:
  calm exploration track, tense chase track (triggers automatically off
  `WantedSystem`), mission track, menu track. Crossfades smoothly between
  them, no hard cuts.
- **`CityAmbience.cs`** — looping city atmosphere (traffic, crowd noise)
  that crossfades between a day and night version automatically, tied to
  `DayNightCycle`.
- **`PoliceSirenAudio.cs`** — siren on/off tied to whether a police
  unit (on-foot or vehicle) is actively chasing.
- **Weapon fire sounds** — `Weapon.cs` plays `fireSound` at the muzzle
  point directly (no manager needed for one-shot SFX like this).
- **Engine sound** — `VehicleController.cs` has an optional
  `engineAudioSource` whose pitch rises with speed/throttle for a real
  revving feel.

## What's NOT built — actual audio files

None of this comes with real music or sound files — that's licensed/
recorded content, not something generated through code. Here's where to
get genuinely usable, free/legal audio:

### Music
- **Incompetech** (incompetech.com) — Kevin MacLeod's library, huge
  selection, free with attribution (or paid to remove attribution
  requirement). Good variety of tense/calm/action tracks.
- **Pixabay Music** (pixabay.com/music) — free, no attribution required
  even for commercial use.
- **YouTube Audio Library** — free tracks built for exactly this kind of use.

### Sound Effects (gunfire, explosions, engine, sirens, footsteps)
- **Freesound.org** — huge community library, filter by license (search
  "CC0" for zero-restriction sounds).
- **Pixabay Sound Effects** — same free/no-attribution terms as their music.
- **Kenney.nl** — free game asset packs, includes solid UI and
  impact/weapon sound sets.

## Wiring real audio in

1. Download clips from the sources above.
2. Drag them into Unity's `Assets/Audio` folder.
3. Assign them to the matching fields — `MusicManager.exploreTrack`,
   `CityAmbience.dayAmbience`, `Weapon.fireSound`, etc. — all already
   exposed in the Inspector on each script.

No code changes needed to swap in real audio — every clip is an
Inspector-assigned field, same pattern as the UI icons and vehicle stats.
