# 🏗️ Floor Generator

Generates a floor with a room in each corner and additional rooms between them, connected with doors and a hallway.

## How it works

Every floor will get a room in each corner, with a random size. After those have been placed `_roomCount` amount of rooms will be placed in between them, and lastly connect them with a door.

If the gap between the corner rooms is less than three tiles (one floor and to walls), the room wil randomly expand to fill the space, or shrink by one tile.

### Rooms

Rooms will get a random size between the min and max values. The doors for the corner rooms will always be pointed down or up, determined by their horizontal position, but middle rooms will get a door on a random edge.

## Demo

In the demo below, the floors are generated with a size of 12x10, and rooms with min/max width of 3 - 6 and min/max height of 2x4.

https://github.com/user-attachments/assets/9763cef8-52a7-4ea3-a574-2148596da057
