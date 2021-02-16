# GrapplingHook
#By: Alessia Ianni- Palarchio, Beau Mcgee, Jude Pinto
A grappling hook mechanic for Level Design


Note that this uses the first person drifter, but should work so long as you use the 'character controller' component. 

## What Can The Grappling Hook Do?

Click to 'shoot' the hook at possible objects. 


Press R to retract the hook. 

If the hook is attached to an object, the object will come to you! 

If the hook is attached to a wall, the hook will detach and return.

If you are fishing, reel in your fish!


When you have hooked a wall, press F to pull yourself to the location!

To release, press space.

*enemy how do here*

## How to use this asset

The aiming reticle canvas is just to know the point you are aiming at, and is recommended, but not necessary. 

'Grappling Hook Gun', 'Grappling Hook Hook', and 'FishingHook' should all be separate children of your player character.

Objects you want the player to be able to pull towards themself should be tagged with the tag 'HookableObjects'. 

Objects you want the player to grapple towards should be tagged with the tag 'StableObjects'.

All objects (grabbable and grapple walls) should be kinematic to avoid physics collision causing problems. :)

Enemy game objects should be tagged with the tag "Enemy" and should have the EnemyHealth.cs script component attached. This script auto generates a healthbar for testing purposes at the upper left hand corner of the screen, and can be damaged with the grappling hook. The damage value can be set via the "damage" int variable in HookableObjects.

To fish, tag a fishing spot with the "FishingSpot" tag. Attach the "FishingSpot.cs" script component to this object. When aiming the grappling hook at water, press R to start fishing and R to reel in. You can customize the kinds of fish you can reel in by creating your own fish gameObjects and adding them to the Fish var (a list of game objects) in the FishingSpot.cs component. You can also change the behavior of fishing (randomization, rarity, etc.) in OnTriggerStay() in  FishingSpot.cs.

