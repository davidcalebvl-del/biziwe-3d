# Biziwe — Living World: Street Chatter & Future Voice Goals

## What's built now — ambient street conversations

Civilians near the player will pause, face each other, and show a short
line of dialogue above their heads — simple, natural street chatter that
makes the city feel populated rather than empty. Only checked near the
player (not simulated city-wide) so it stays cheap regardless of how big
the world eventually gets.

**Scripts:** `AmbientLines.cs` (data), `AmbientChatter.cs` (pairs up nearby
civilians), `DialogueBubble.cs` (the floating text), and a `StartTalking()`
method added to `CivilianNpc.cs`.

**To expand later:** create more `AmbientLines` assets per
district/neighborhood (market talk vs. dockside talk vs. upscale-area
talk) for variety, and assign different banks to civilians spawned in
different parts of the city.

## Future goal — voice-powered NPC conversations

The bigger idea: speak out loud into your phone, have the game hear you
through the mic, and have an NPC actually respond to what you said — a
real spoken conversation, not a dialogue menu.

This is genuinely possible, but it's a different technology stack from
everything else in this repo, and comes with real requirements worth
being upfront about before building it:

- **Needs internet while playing** — this can't run fully offline like
  the rest of the game.
- **Needs a speech-to-text step** — converts what the player says into
  text the game can use.
- **Needs an AI service to generate the NPC's reply** — this has an
  ongoing real cost per use (not a one-time thing), and needs an API key/
  account set up specifically for this feature.
- **Needs text-to-speech** — to have the NPC's reply actually spoken back.
- **Needs moderation/guardrails** — since it's open conversation with an
  AI character, not fixed scripted lines, worth thinking through what
  happens if a player says something unexpected.

This is realistic as a genuine stretch goal for later — after the core
game (movement, missions, story campaign) is built, playable, and stable.
Not a Phase 1-3 item. Noted here so it isn't lost, revisit once the
foundation is solid.
