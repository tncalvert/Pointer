Pointless
===
***
Developed by Chris Hanna, Tim Calvert and Ryan Santos for the final project in IMGD 4100 Artificial Intelligence for Games at WPI.

Description
---
A sort of reverse horror game, you play as a monster roaming a city while attempting to eliminate all the residents.

Controls
---
- Move by left-clicking on a space in front of you
- Attack victims by pressing space when nearby
- Hold shift to pause the game
	- While paused, hover the mouse over victims or colored buildings to see additional information
- Escape allows you to exit to the main menu

Playing
---
Move throughout the city, hunting down the city's residents. But, be careful! They are capable of arming themselves and fighting back. Destroy them all to progress to the next level.

Technical Information
---
- The city is randomly generated
- Steering/Flocking and Pathfinding
	- We use an A* implementation to find paths through the city on a macro level
	- On a micro level, we using steering behaviors to move both the player and the victims
	- Victims also implement flocking behaviors in regards to each other
- Fuzzy Logic
	- Victims' main behaviors are controlled via a fuzzy logic system. They have attributes for bravery, toughness, independence, sleepiness, etc. All of these have an effect on what actions the victim chooses to do.
- Genetic Algorithm
	- Each victim monitors a small number of factors in regards to how well it performed during the level. Factors such as how long it survived and how much damage it is deals the player are all tracked. After each level, the fitness of the victims are calculated, the best half is taken and bread to create a new generation. New victims have a small chance of receiving mutations to their data.