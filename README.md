#Sokoban

A Sokoban clone. The user must push all boxes onto the dots. This version that I wrote can take customized maps (puzzles) to play on. The user may design puzzles him/herself, after which they go into the "reserved" pool of puzzles. Only the "active" pool of puzzles are played. The reserved and active pools may be modified via the puzzle selector (from the main menu) shown in the fourth screenshot below.

A custom API was written to facilitate the creation of a GUI for the game. This included creating classes for buttons, forms, etc. This was done using the graphics functionality provided by the XNA framework (through Monogame).

Note that this is a work in progress, and is still a little rough around the edges. However, basic functionality is there (check TODO list, after the screenshots below, for information on bugs, missing features, etc.)

The project is very easy to compile: all you need is visual studio, with support for C#. Open the sln file in the root directory, and compile. However, you will most likely need Monogame installed for visual studio. Note that I have included a compiled version in the zip file in the root (SokobanCompiled.zip). But there was some difficulty when I tried to run this executable on another computer - most likely because .NET framework 4.0 was not installed on that PC.

<p align="center">
<img src="ScreenshotGame.png" width="550">
</p>

<p align="center">
<img src="ScreenshotGameMenu.png" width="550">
</p>

<p align="center">
<img src="ScreenshotDesigner.png" width="550">
</p>

<p align="center">
<img src="ScreenshotSelector.png" width="550">
</p>


##TODO

+ In puzzle editor,
 - add functionality for specifying the grid size
 - enable existing puzzles to be imported and modified
 
+ Enable scroller to be moved by dragging it (not only by pressing arrow keys, e.g. in lists in puzzle selector)

+ Find new color scheme/better artwork (not this ugly yellow)

+ Cleanup code (make naming more consistent, relocate certain functions/methods to more sensible locations, etc.)
