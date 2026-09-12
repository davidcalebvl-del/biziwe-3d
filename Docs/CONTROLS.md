# Biziwe 3D — Controls & UI Button Map

Every on-screen button needed, organized by context. Wire each of these to
the matching method already written in the scripts — nothing here needs new
code, just UI buttons in Unity's Canvas connected via OnClick / EventTrigger.

## Always visible
| Button | Calls |
|---|---|
| Movement joystick | Feeds `Horizontal`/`Vertical` input (or wire directly into `PlayerController`) |
| Pause menu | Your own pause UI |

## On foot
| Button | Calls |
|---|---|
| Jump | `PlayerController` jump (via `TouchInputManager.OnJumpPressed()`) |
| Crouch (toggle) | `TouchInputManager.OnCrouchToggle()` |
| Run (hold or toggle) | Already automatic via Shift key on desktop; on mobile, wire a Run button to hold `IsRunning` true while pressed |
| Enter Vehicle | `CarjackHandler.TryEnterOrCarjack()` (shown when near a car) |
| Fire (ranged weapon) | `Weapon.Fire(aimDirection)` |
| Reload | `Weapon.Reload(magazineSize)` |
| Melee Attack | `MeleeWeapon.Attack()` |
| Weapon Switch (wheel or cycle button) | Enable/disable the active `Weapon`/`MeleeWeapon` GameObject |
| Throw Explosive | Instantiate an `Explosive` prefab in front of the player, call `Arm()` |

## In a vehicle
| Button | Calls |
|---|---|
| Steer Left (hold) | `TouchInputManager.OnSteerLeftHeld(true/false)` |
| Steer Right (hold) | `TouchInputManager.OnSteerRightHeld(true/false)` |
| Gas (hold) | `TouchInputManager.OnGasHeld(true/false)` |
| Brake/Reverse (hold) | `TouchInputManager.OnBrakeHeld(true/false)` |
| Handbrake (hold) | `TouchInputManager.OnHandbrakeHeld(true/false)` |
| **Exit Vehicle** | `TouchInputManager.ExitVehicle()` |
| Horn | Play an AudioSource clip — cosmetic, no gameplay hook needed |
| Headlights (toggle) | Enable/disable Light components on the car prefab |

## HUD (read-only display, not buttons)
- **Wanted stars** — bind to `WantedSystem.wantedLevel` (0–3, hard cap)
- **Money balance** — bind to `EconomySystem.CurrentBalance` (subscribe to `OnBalanceChanged`)
- **Ammo count** — bind to `Weapon.CurrentAmmo`
- **Mission prompt/objective text** — bind to `MissionManager` events

## Design note
Keep the button layout close to genre convention (movement joystick
bottom-left, action buttons bottom-right, contextual buttons like
Enter/Exit appearing only when relevant) — players already know this
pattern from other open-world mobile games, so there's no learning curve.

## Platform note
Unity builds this exact project to **Android, iOS, and WebGL (browser)**
from the same codebase — this isn't a limitation, it's one of Unity's core
strengths. Switching target platform is done in Unity's Build Settings
(File > Build Settings > pick platform > Switch Platform). Some
platform-specific tuning is normal (touch controls for mobile, WebGL has
tighter performance/size limits than a native build), but the core game
logic in this repo doesn't need to be rewritten per platform.
