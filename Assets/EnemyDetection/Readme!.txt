1. Place "Detection" and "EnemyScript" scripts on Enemy object

2. Create an empty named pathfinder and create multiple child objects below it named "point"
(these "points" will serve as the nodes the enemies are going to be following.)
 
3. Then, define the "Path Holder" under the "Enemy Script" which will give u gizmos that give a visible point to the nodes.

4. After, drag the player into the Detection script. 
(this will allow the enemy to detect and follow the player character.) 
(the following line turns green when player is within the max angle)


Limitations:
- enemy does not rotate on node so detection radisu stays in the same direction
- enemy cant change vertical levels when moving 



Yanxi - "Detection" code
Peter - Scene management and "EnemyScript" code
First-person movement - http://www.torahhorse.com/index.php/portfolio/first-person-drifter-for-unity/







