# ReadME van team 04 Stumble guys

Ivo Bergen

* [Moving Cubes](ProefExamenGame/Assets/Scripts/MovingCubes)
* [Ink Spot](ProefExamenGame/Assets/Scripts/InkSpot/InkSpot.cs)
* [Speed Boost](ProefExamenGame/Assets/Scripts/PowerUps/SpeedBoost/PowerUpSpeedBoost.cs)

Owen Stas:

* [Respawnsysteem](https://github.com/IvoBergen/ProefExamenRepo/wiki/RespawnSystem)
* [AI PathFinding](https://github.com/IvoBergen/ProefExamenRepo/wiki/BotProgamming)

Christiaan Oosterwouder

* [Moving obstacle](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/Battery_movement_spread_sheet.png)
* Some other Game object

Akari Le

* [PlayerMovement](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/Player-Movement-Visualsheet.png)
* Some other Game object

## moving platform by Ivo

The moving platforms are added to challenge the player’s movement, timing, and overall control. Instead of simply walking or jumping across static surfaces, the player must carefully observe the platform’s motion and choose the right moment to move.

## Moving Cubes

The moving cubes are made as an obstacle for the player. They Move to random locations decided by the input values on the X an Z axis. The cube also rotates between 90, 0 and -90 degrees to add for an extra layer of difficulty.

![MovingCubes gif](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/MovingCubes.gif)

This is the visual sheet for the moving cube scripts

![Moving Cubes visualsheet](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/VisualSheetMovingCube.png)

## Ink Spot by Ivo

The Ink spot is added onto the course to add extra challenge for the player to avoid them so their speed doesn't decrease. They work with a collision check and if the player walks through them their movement speed
will be decreased.

This is the visual sheet for the ink spot

![Ink Spot visualsheet](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/InkSpot.png)

## Speed Boost by Ivo

The power up, in this case a speed boost is added for extra variety in the gameplay loop. It adds extra challenge and a fun factor to the game. It does a collision check to check if the player touched it and then it
triggers an Action that calls the other script responsible for increasing and decreasing the player's speed.

This is the visual sheet for the speed boost power up.

![Speed Boost visualsheet](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/SpeedBoost.png)

## Respawn mechanic by Owen

This respawn system ensures that the player is automatically placed back at the last reached checkpoint when they fall off the platform or hit a respawn trigger.

When the player enters the trigger:
The system checks whether there is an active checkpoint.
The player’s current physics (velocity and angular velocity) are reset so they do not keep moving or sliding forward.
The player is moved to the position and rotation of the checkpoint.
After that, physics is reactivated so the player can continue playing normally.

In short: falling = reset to checkpoint without any strange physics bugs

![Foto van respawn systeem](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/BWP-VWO-Visualsheet.png)
## AI pathfinding by Owen

This mechanic is used to make the level more interesting to play. the bot is the main win/lose condition of the game. you're supposed to race against it and be faster than 50% of the ai to win the game. 

![AIVisualsheet](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/Bot-MovementBWP-VWO-Visualsheet.png)
![AIJumpGif](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/AiJumping.gif)

## Moving obstackle by Christiaan Oosterwouder

This moving obstacle is designed to increase the level of challenge and encourage the player to interact with the environment in a more dynamic way. Instead of allowing the player to move through the level in a static or predictable pattern, the obstacle forces them to constantly adjust their timing, positioning, and movement strategy.

As the obstacle moves, it creates changing gaps, shifting hazards, or temporary blockades that require the player to observe its behavior and respond accordingly. This adds a layer of timing-based gameplay, where success depends on reading movement patterns and choosing the right moment to act.

Because the obstacle is not stationary, it keeps the gameplay engaging and prevents the level from feeling repetitive. The player must stay alert, react quickly, and adapt to the environment as it changes in real time.

In short: a moving obstacle = dynamic gameplay, increased challenge, and more active player decision-making.

![moving obstackle](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/Battery_movement_spread_sheet.png)

## Player movement by Akari

movement for the player

![example](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/Player-Movement-Visualsheet.png)

## Some beautifull script by Student Z

Contrary to popular belief, Lorem Ipsum is not simply random text. It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, discovered the undoubtable source. Lorem Ipsum comes from sections 1.10.32 and 1.10.33 of "de Finibus Bonorum et Malorum" (The Extremes of Good and Evil) by Cicero, written in 45 BC. This book is a treatise on the theory of ethics, very popular during the Renaissance. The first line of Lorem Ipsum, "Lorem ipsum dolor sit amet..", comes from a line in section 1.10.32.

![example](https://user-images.githubusercontent.com/1262745/189135129-34d15823-0311-46b5-a041-f0bbfede9e78.png)

## Some other Game object by Student Z

Contrary to popular belief, Lorem Ipsum is not simply random text. It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, discovered the undoubtable source. Lorem Ipsum comes from sections 1.10.32 and 1.10.33 of "de Finibus Bonorum et Malorum" (The Extremes of Good and Evil) by Cicero, written in 45 BC. This book is a treatise on the theory of ethics, very popular during the Renaissance. The first line of Lorem Ipsum, "Lorem ipsum dolor sit amet..", comes from a line in section 1.10.32.

![example](https://user-images.githubusercontent.com/1262745/189135129-34d15823-0311-46b5-a041-f0bbfede9e78.png)
