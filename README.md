# CS-480 Assignment 2


---

## Custom Lerp Door
**By Joshua Henrikson**

There are three modes this door can function in this is defined by an Enum `DoorMode` and selected in the inpector for each door.
I adapted the origonal door code which simply deleted the door on colision. I changed this interaction to a trigger and added a 
seperate trigger box for more range. I also added `doorState` enum to track the doors state to check to see if the door is already open

---

## Slide Mode

In slide mode, I `Vector3.Lerp` between `startPos` and `targetPos` in acordance to the set offset.
This lerp is controlled by a clamped and curved `deltaTime`, to acieve a smooth transition between the two points. The exponential easing 
function used is, `tEased = 1 - Mathf.Exp(-5 * t)`. Speed is controlled by the variable `speed` and set independantly on each door.

---

## Rotate Mode

The Rotate mode is similar to the slide mode we just `Quaternion.Lerp` between `startRot` and `targetRot` defined at start using `rotationAmount` as the rotation offset.

---

## Axis Rotation Mode

The axis rotation mode uses an definable gameObject that acts as a pivot for the door. We calculate the current angle and then the `targetAngle` using a `Mathf.Lerp` controlled by `tEased`. We find the difference between these two angles and pass it into `transform.RotateAround(pivot.position, rotationAxis, delta);` And use that angle to rotate around the pivot's `rotationAxis`.

---

## Key pickup particle and sound (trigger)

**By David Haddad**

This satisfies the assignment items for a **new particle effect with trigger(s)** and a **new sound effect with trigger(s)**. Pickup is still driven by the key’s **trigger collider** (`OnTriggerEnter` on the player).

Implementation lives in [`Assets/_3DStealthGame/Tutorial_Demo/Demo_Scripts/Bonus Features/Key.cs`](Assets/_3DStealthGame/Tutorial_Demo/Demo_Scripts/Bonus%20Features/Key.cs). The `Key` component exposes optional **`pickupParticles`** (`ParticleSystem`) and **`pickupClip`** (`AudioClip`). When the player enters the trigger, the script adds the key to inventory, plays **`AudioSource.PlayClipAtPoint`** at the key’s position if a clip is assigned, then—if a particle system is assigned—**unparents** it (so it is not destroyed with the key), **`Play()`**s it, and **`Destroy`**s the particle GameObject after `duration + start lifetime` (plus a short buffer) so the burst can finish without relying on the Particle System **Stop Action**.

**Authoring:** In **DemoScene**, **`Key_Red`** uses a child **Particle System** (e.g. a short red-tinted burst with **Looping** off and **Play On Awake** off). Assign that system to **`Pickup Particles`** and assign a pickup sound from [`Assets/Assignment2/Audios/`](Assets/Assignment2/Audios/) (short **`.ogg`** jingles) to **`Pickup Clip`**. The same clip can be assigned on **`Key_Blue`** if every key pickup should make sound. **Save the scene** so those overrides stay in version control.
## Key Compass
**By David Haddad**

The key compass is a world-space hint that points toward the next key the player still needs. It is implemented by `StealthGame.NextKeyCompass` on the player (`Assets/Assignment2/Scripts/NextKeyCompass.cs`). In the demo player prefab, a `CompassPivot` child rotates on Y only toward the target key; the `CompassNeedle` is assigned in the Inspector (optional `compassParticles` can fire when the target key changes). The script picks the next unowned key in order `keyRed` then `keyBlue` using `PlayerMovement.OwnKey` and `Key` objects in the scene, computes horizontal direction and signed yaw with `Atan2` from the cross and dot of forward vs. to-key, and hides the pivot when there is no target or the player is within `minHorizontalDistance`. With `useDotProductForVisibility`, the needle scales down when the key is more behind the player.

**HQP Studios** (HQP) is the publisher of the third-party Unity asset pack **Low Poly 3D Icons - Pack Lite** under `Assets/HQP Studios/`. The compass needle visual is not custom-modeled geometry: it is a prefab instance from that pack (e.g. `Arrow_3D_Icon_01`) with materials from the same pack. Redistribution follows the pack's license documents in `Assets/HQP Studios/Low Poly 3D Icons - Pack Lite/Document/`.

---

