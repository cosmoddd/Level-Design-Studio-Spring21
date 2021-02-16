HAZARD ZONE DOCUMENTATION:

BASIC FUNCTIONALITY:
If the player touches the Hazard zone, they lose character control, a audioclip is played, some text appears on screen, and then either they telport to a respawn point, or the scene is reset

HAZARD ZONE SCRIPT VAIRABLES:
Respawn Point : The game object the player teleports to if the scene is NOT reset. This object does not need to be childed to the Hazard Zone object.
Reset Scene : Set to true if you want the scene to reset at the end of the waitTimer. Set to false if you want to player to teleport to the Respawn Point Game Object
Wait Timer: Number of FixedUpdate frames to wait before resetting the player / scene. It will take half this number of frames for the Text element to finish its animation.
Tm : The Canvas Text object

Important Notes:
Set up how you want the text to look in editor by the end of its animation. The script will reset it to be invisible when the game starts. 
If you don't want any text element, leave it null. The script should still run. 

CREDITS:
Finn Carney - Audio and Text funcionality, Scene Reset
Will Rhodes - Basic Funcionality

