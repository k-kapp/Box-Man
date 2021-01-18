# Box-Man

## Contents

1. [Introduction](#introduction)
2. [TODO list](#todo)
3. [Screenshots](#screenshots)

## Introduction

A game similar to Sokoban, written in C#. The object of the game is to push all boxes onto the dots. This version that I wrote can take customized maps (puzzles) to play on. The user may design puzzles him/herself, after which they go into the "reserved" pool of puzzles. Only the "active" pool of puzzles are played. The reserved and active pools may be modified via the puzzle selector (from the main menu) shown in the fourth screenshot below. 

Note that the puzzle designer checks basic validity of the puzzle, such as whether it has a starting position, whether the walls enclose the puzzle and whether all areas are reachable. The last aforementioned requirement was checked by implementing a relatively simple depth-first search algorithm: all connected paths in the puzzle are visited, after which it is easy to determine whether all paths are reachable. I have done some basic testing of the algorithm, and of the validation of the designed puzzles, but I hope to test it a bit more thoroughly soon.

A custom API was written to facilitate the creation of a GUI for the game. This included creating my own code for buttons, forms, scrollbars, etc. This was done using the raw graphics functionality provided by the XNA framework (through Monogame). 

Note that this is still a little rough around the edges. However, basic functionality is there (check TODO list, after the screenshots below, for information on bugs, missing features, etc.). When I have some more time, I might come back to it at a later stage to improve it a bit.

Currently, I cannot compile it with Visual Studio 2019. This was actually written in 2017 with Visual Studio 2015 or 2017, with an older version of Monogame. Plus, some of the function calls do not seem to match any signatures of known functions, probably due to the outdated Monogame framework I used to write this originally in. I will make modifications to compile it on newer version of Visual Studio and Monogame at a later stage. In the meantime, feel free to take a look at the code!

## TODO

+ In puzzle editor, enable existing puzzles to be imported and modified
 
+ Enable scroller to be moved by dragging it (not only by pressing arrow keys, e.g. in lists in puzzle selector)

+ Find new color scheme/better artwork (not this ugly yellow)

+ Do more thorough testing of puzzle validation

+ Cleanup code (make naming more consistent, relocate certain functions/methods to more sensible locations, etc.)

## Screenshots

<p align="center">
<img src="ScreenshotGame.png" width="550">
</p>

<p align="center">
<img src="ScreenshotDesigner.png" width="550">
</p>

