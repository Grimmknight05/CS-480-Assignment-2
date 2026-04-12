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

## Key Compass
**By David Haddad**

The key compass is a world-space hint that points toward the next key the player still needs. It is implemented by `StealthGame.NextKeyCompass` on the player (`Assets/Assignment2/Scripts/NextKeyCompass.cs`). In the demo player prefab, a `CompassPivot` child rotates on Y only toward the target key; the `CompassNeedle` is assigned in the Inspector (optional `compassParticles` can fire when the target key changes). The script picks the next unowned key in order `keyRed` then `keyBlue` using `PlayerMovement.OwnKey` and `Key` objects in the scene, computes horizontal direction and signed yaw with `Atan2` from the cross and dot of forward vs. to-key, and hides the pivot when there is no target or the player is within `minHorizontalDistance`. With `useDotProductForVisibility`, the needle scales down when the key is more behind the player.

**HQP Studios** (HQP) is the publisher of the third-party Unity asset pack **Low Poly 3D Icons - Pack Lite** under `Assets/HQP Studios/`. The compass needle visual is not custom-modeled geometry: it is a prefab instance from that pack (e.g. `Arrow_3D_Icon_01`) with materials from the same pack. Redistribution follows the pack's license documents in `Assets/HQP Studios/Low Poly 3D Icons - Pack Lite/Document/`.

---

