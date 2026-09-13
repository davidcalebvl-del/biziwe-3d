# Biziwe — Story & Campaign

A full story-driven campaign, GTA-style: named characters, faction politics,
betrayals, and a real ending shaped by player choices — not just open-world
systems with no throughline.

## Setting

**Obodo Bay** — a fictional Nigerian coastal city blending Lagos energy,
Port Harcourt grit, and Aba hustle-culture into one place that isn't
literally any single real city. Sprawling markets, danfo-choked roads, new
money high-rises next to old neighborhoods, a port everything flows
through — legal and not.

The world is Nigerian-rooted, but the player character is fully
customizable — any name, any appearance, any background. Obodo Bay pulls
in locals and outsiders alike; that's the hook, not a restriction.

## Tone

Gritty crime/hustle story. The player rises from nothing to controlling
something real — through street smarts, danger, and hard choices, not
handed power.

## Cast

- **Player Character** — custom name/appearance, player-defined. Starts
  with nothing, done being broke, done being disrespected.
- **Chidi "Chairman" Okoro** — aging boss who runs Obodo Bay's docks.
  Old-school, calculating. Gives the player their first real job.
- **Femi Ade** — Chairman's ambitious lieutenant. The one who actually
  notices the player's potential and mentors them.
- **Blessing "BB" Nwachukwu** — smuggler/fixer running her own independent
  crew. Sharp, funny, dangerous — a wildcard ally.
- **Inspector Adeyemi** — corrupt police commander. Takes bribes from
  everyone, switches sides whenever it benefits him.
- **Kelvin "Wire" Balogun** — leader of a rising cybercrime syndicate
  muscling into Chairman's territory. Young, flashy, ruthless.
- **Sarah Etim** — journalist investigating corruption in Obodo Bay. Her
  path crosses the player's — ally or liability, player's choice affects
  the ending.

## Act 1 — Getting In

1. **The Errand** — first job for Femi, prove reliability (delivery +
   avoid police). Teaches driving and the wanted system.
2. **The Setup** — first real job goes wrong, first taste of real danger.
   Teaches combat/chase mechanics.
3. **Made Man** — Chairman notices the player personally. Officially in.

## Act 2 — Rising, and the Cracks Show

4. **Wire's Play** — Wire's syndicate hits one of Chairman's operations;
   player sent to retaliate.
5. **BB's Offer** — Blessing quietly offers a better deal working for her
   instead — first real loyalty choice.
6. **The Journalist** — Sarah confronts the player about a job they did.
   Player can silence her, warn her off, or feed her information.
7. **Betrayal** — someone close to the player (Femi or BB, depending on
   earlier choices) turns out to be playing their own game.

## Act 3 — The Reckoning

8. **War in the Bay** — open conflict between Chairman's crew, Wire's
   syndicate, and whoever the player has aligned with.
9. **Inspector's Price** — Adeyemi demands an impossible payoff to call
   off police heat. Player can pay, fight, or expose him.
10. **The Choice** — final mission, multiple endings based on the
    player's path through the campaign:
    - **Take the throne** — player ends up running Obodo Bay's underworld.
    - **Walk away clean** — expose everyone (with Sarah's help), get out
      with the money.
    - **Burn it down** — destroy every faction, leave the city in chaos,
      become a legend or a ghost.

## Implementation notes

- Each numbered mission above maps to a `MissionData` asset (see
  `Assets/Scripts/Systems/MissionData.cs`) — create one per mission,
  reference its `id` from the corresponding `MissionTrigger` in the scene.
- Branching choices (BB's Offer, The Journalist, Inspector's Price) don't
  need new systems — they're just missions whose outcome sets a flag
  (e.g. a bool or int on a simple `StoryFlags` static/singleton class) that
  later missions and `The Choice`'s ending check before deciding which
  path plays out. Worth adding a small `StoryFlags.cs` once these branching
  missions are actually being built — not needed yet for the outline itself.
- Chapter 1-3 (Act 1) beat-by-beat breakdown — objectives, dialogue
  moments, which systems each mission exercises — is the natural next
  step once you're ready to start building actual mission content in
  Unity.
