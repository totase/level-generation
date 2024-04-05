# ⛏️ Spelunky generator

A simple level generator made for Unity, inspired by the level generation of [Spelunky](https://spelunkyworld.com/original.html). It generates a number of rooms based on a grid size, made up of x rows and y columns.

## How it works

The script generates a grid based on the rows and columns variables. It will then cycle through each row and column, and place rooms until the bottom is reached and a path is created.

For each room, a new direction will be given (left, right or down). If the current room is on the edge of the level board, the new room will automatically be placed below.

### Rooms

The position for each room will be calculated by the `roomHeight` and `roomWidth` variables. Ideally, these would be set in the room prefabs, for example via a `RoomManager` script.

Note: Since the rooms in this repository are static sprites, editing these variables will produce gaps between the rooms.

## Demo

In the demo below, levels are generated on a 3x3 grid, with 6x8 rooms.

https://github.com/totase/level-generation/assets/18186823/a843a9eb-bea8-460f-820e-19677a74f84f
