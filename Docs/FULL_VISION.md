# Biziwe 3D — Full Vision (Phases 2+)

This captures the complete feature list Dave wants for Biziwe long-term.
Phase 1 (see `PHASE1_PLAN.md`) stays deliberately small — walk, drive, one
simple mission loop, working and stable. Everything below gets layered in
*after* that foundation is solid, in the order listed. Building these before
Phase 1 works reliably is how ambitious solo projects collapse — so resist
the urge to jump ahead, even though it's tempting.

## Performance goal
Feel **faster and smoother than Payback²** — this is a standing priority
alongside every phase, not a late-stage polish pass. Keep controls
responsive and frame rate stable as systems get added; if a new feature
makes the game feel heavier or laggier, that's a signal to optimize before
adding more, not to push forward regardless.

## World & Story
- Fictional Nigerian coastal city — **Obodo Bay** (Lagos/Port Harcourt/Aba
  energy blended, not a literal recreation of any one real city)
- Gritty crime/hustle tone — player rises from nothing to controlling
  something real
- Central conflict between rival factions (old-money cartels, corrupt
  police, cybercrime syndicate, street gangs)
- **Fully custom player character** — player picks name (free text input),
  appearance, and background at creation. Not locked to a Nigerian
  character or nationality — the world is Nigerian-rooted, the player can
  be from anywhere, same way real-world players of any background play
  GTA's Los Santos

## Vehicles (Phase 2)
- Carjacking: approach any vehicle, pull the driver out, take control
- Full drivable physics (already scaffolded in `VehicleController.cs`)
- Multiple vehicle types over time (cars, motorcycles, trucks)

## Combat (Phase 2–3)
- Player can fight or shoot police and NPCs
- Weapon categories: pistols, SMGs, rifles, shotguns, melee
- **Explosives**: bombs that cause area damage — destroying nearby cars,
  damaging shopfronts and houses in the blast radius, not just a single
  target
- **Bazooka / rocket launcher** as a heavier weapon option
- Wanted/police system already scaffolded (`WantedSystem.cs`) — expands
  here to react to combat, not just proximity

## Economy (Phase 3)
- Money system — earn from missions/jobs, spend on weapons, vehicles,
  upgrades, property
- Player has a persistent cash balance tracked across play sessions

## World Simulation (Phase 3)
- Day/night cycle — time actually passes, lighting changes, NPC routines
  shift (people out during the day, fewer at night, etc.)
- Weather variation layered in once day/night is stable

## Build order summary
1. **Phase 1** — walking/driving/one mission works, stable, on-device
2. **Phase 2** — carjacking, basic combat, expanded wanted system
3. **Phase 3** — economy, day/night cycle, weather, more missions
4. **Phase 4+** — everything from the original full-scale vision doc
   (larger world, more districts, deeper NPC simulation, etc.)

Each phase only starts once the previous one is genuinely playable and
stable — not just "written," but tested and working.
