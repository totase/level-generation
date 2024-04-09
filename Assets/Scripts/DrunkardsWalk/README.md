# 🚶 Drunkard's Walk

A level generator using a random walk algorithm, inspired by [this algorithm](https://poppants.itch.io/the-drunkard-walk) found on itch.io. It generates rooms/corridors in random directions based on the input size.

## How it works

Starting from a given point, the algorithm takes a walk in a random direction and marks each position in `_walkLength` as part of the path. This process is repeated `_numWalks` times.

The starting point is set to 0 on both `x` and `y`, but a random starting point within a given board can easily be implemented.

## Demo

In the demo below, `_numWalks` is set to 4 and `_walkLength` is set to 12.

| 1x1 rooms | 2x2 rooms |
| --------- | --------- |
|![Screen Recording 2024-04-08 at 22 13 04](https://github.com/totase/level-generation/assets/18186823/11ee8bee-c534-41a2-8038-043ddf90313e) | ![Screen Recording 2024-04-08 at 22 16 41](https://github.com/totase/level-generation/assets/18186823/896732d4-e432-4406-ac92-000539729f0c) |
