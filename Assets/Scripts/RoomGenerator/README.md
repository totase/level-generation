# 🏠 Room Generator

Procedurally generates rooms in a grid-based layout with connecting corridors. It generates rooms in random directions based on the input sizes and supports main path rooms and optional side rooms.

## How it works

The generator starts by placing a single room in the center of the grid - `x: 0` and `y: 0`. From there, it begins carving out a path by placing rooms in random directions. Each new room connects to the previous one through doorways.

### Rooms

Each room is a self-contained prefab with its own floor tiles and walls. As rooms are placed, they undergo overlap checks to ensure they don't intersect with existing rooms.

### Directions

Left/Right movements are more common (40% chance each), down movement is less frequent (20% chance). After two rooms is placed in the same direction, it will force a direction change.

## Demo

In the demo below, the levels are generated with a main room count of 4, with 3x5 rooms.

https://github.com/user-attachments/assets/f8d49e42-224b-411b-9763-1bdbadf11335
