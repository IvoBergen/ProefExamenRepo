# ReadME van team 04 Stumble guys

De mechanics van team 04 Stumble guys.

## moving platform by Ivo

The moving platforms are added to challenge the player’s movement, timing, and overall control. Instead of simply walking or jumping across static surfaces, the player must carefully observe the platform’s motion and choose the right moment to move.

## Moving Cubes

The moving cubes are made as an obstacle for the player. They Move to random locations decided by the input values on the X an Z axis. The cube also rotates between 90, 0 and -90 degrees to add for an extra layer of difficulty.

![MovingCubes gif](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/MovingCubes.gif)

This is the visual sheet for the moving cube scripts

![Moving Cubes visualsheet](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/VisualSheetMovingCube.png)

* [Moving Cubes](ProefExamenGame/Assets/Scripts/MovingCubes)

## Ink Spot by Ivo

The Ink spot is added onto the course to add extra challenge for the player to avoid them so their speed doesn't decrease. They work with a collision check and if the player walks through them their movement speed
will be decreased.

This is the visual sheet for the ink spot

![Ink Spot visualsheet](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/InkSpot.png)

* [Ink Spot](ProefExamenGame/Assets/Scripts/InkSpot/InkSpot.cs)

## Speed Boost by Ivo

The power up, in this case a speed boost is added for extra variety in the gameplay loop. It adds extra challenge and a fun factor to the game. It does a collision check to check if the player touched it and then it
triggers an Action that calls the other script responsible for increasing and decreasing the player's speed.

This is the visual sheet for the speed boost power up.

![Speed Boost visualsheet](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/SpeedBoost.png)

* [Speed Boost](ProefExamenGame/Assets/Scripts/PowerUps/SpeedBoost/PowerUpSpeedBoost.cs)

## Menu's by Ivo

These are the main menu and pause menu in the game, to help the player exit the game and to make sure the player has a start screen and isn't instantly dumped into the main game. The main menu also has a character customization menu where the players outfit can get changed.

This is the visual sheet for the menu's.

![Menu's visualsheet](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/Menus.png)

* [Main Menu](ProefExamenGame/Assets/Scripts/MainMenu/MainMenu.cs)
* [Pause Menu](ProefExamenGame/Assets/Scripts/PauseMenu/PauseMenu.cs)

## Character Customization by Ivo

This is made for the player to change their characters outfit and make it so you can have a unique experience every single time. The player can cycle through the outfits with the buttons on the screen.

![Customization Screenshot](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/CustomizationScreenshot.png)

![Character Customization visualsheet](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/CharacterCustomization.png)

* [Character Customization](ProefExamenGame/Assets/Scripts/CharacterCustomization)

## Respawn mechanic by Owen

This respawn system ensures that the player is automatically placed back at the last reached checkpoint when they fall off the platform or hit a respawn trigger.

When the player enters the trigger:
The system checks whether there is an active checkpoint.
The player’s current physics (velocity and angular velocity) are reset so they do not keep moving or sliding forward.
The player is moved to the position and rotation of the checkpoint.
After that, physics is reactivated so the player can continue playing normally.

In short: falling = reset to checkpoint without any strange physics bugs

![Foto van respawn systeem](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/BWP-VWO-Visualsheet.png)

* [Respawnsysteem](https://github.com/IvoBergen/ProefExamenRepo/wiki/RespawnSystem)

## AI pathfinding by Owen

This mechanic is used to make the level more interesting to play. the bot is the main win/lose condition of the game. you're supposed to race against it and be faster than 50% of the ai to win the game.

![AIVisualsheet](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/Bot-MovementBWP-VWO-Visualsheet.png)
![AIJumpGif](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/AiJumping.gif)

* [AI PathFinding](https://github.com/IvoBergen/ProefExamenRepo/wiki/BotProgamming)

## Moving obstackle by Christiaan Oosterwouder

This moving obstacle is designed to increase the level of challenge and encourage the player to interact with the environment in a more dynamic way. Instead of allowing the player to move through the level in a static or predictable pattern, the obstacle forces them to constantly adjust their timing, positioning, and movement strategy.

As the obstacle moves, it creates changing gaps, shifting hazards, or temporary blockades that require the player to observe its behavior and respond accordingly. This adds a layer of timing-based gameplay, where success depends on reading movement patterns and choosing the right moment to act.

Because the obstacle is not stationary, it keeps the gameplay engaging and prevents the level from feeling repetitive. The player must stay alert, react quickly, and adapt to the environment as it changes in real time.

In short: a moving obstacle = dynamic gameplay, increased challenge, and more active player decision-making.

![moving obstackle](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/Battery_movement_spread_sheet.png)

* [Moving obstacle](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/Battery_movement_spread_sheet.png)

##  knikker obstackle by Christiaan Oosterwouder

The marble script creates a moving obstacle that spawns marbles from a fixed point above the level. Each marble follows a predefined waypoint path using physics forces, allowing it to move along a curved or irregular route through the obstacle section.

While moving along the path, the marbles interact with the player through the existing knockback system. When a marble collides with a player, it triggers the KnockbackReceiver, pushing the player away from the obstacle.

This system adds dynamic movement and environmental hazards to the level. Because the marbles follow a curved path and use physics-based movement, the obstacle feels less predictable and requires players to react quickly and adjust their movement to avoid being knocked off the course.

![Knikker obstacle](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/Knikker-Gif.gif)

* [Knikker obstacle](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/KnikkerVisualSheet.png)

## Player movement by Akari

movement for the player

![example](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/Player-Movement-Visualsheet.png)

* [PlayerMovement](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/Player-Movement-Visualsheet.png)

## Playtest
 ![PLaytest Delisha](https://youtu.be/gwe25sFgblg)
  * [Playtest Vragenlijst](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/Usertest-Delisha.png)

 ![PLaytest Tim](https://www.youtube.com/shorts/Ecp9Lpt6ecY)
  * [Playtest vragenlijst Tim](https://github.com/IvoBergen/ProefExamenRepo/blob/develop/ReadMEFiles/PlaytestTim.png)


