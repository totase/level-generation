# 🚶 Drunkard's Walk

A level generator using a random walk algorithm, inspired by [this algorithm](https://poppants.itch.io/the-drunkard-walk) found on itch.io. It generates rooms/corridors in random directions based on the input size.

## How it works

Starting from a given point, the algorithm takes a walk in a random direction and marks each position in `_walkLength` as part of the path. This process is repeated `_numWalks` times.

## Demo

In the demo below, `_numWalks` is set to 4 and `_walkLength` is set to 12.

| 1x1 rooms | 2x2 rooms |
| --------- | --------- |
|           |           |
