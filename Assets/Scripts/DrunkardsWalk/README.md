# 🚶 Drunkards walk

A level generator using a random walk algorithm, inspired by [this algorithm](https://poppants.itch.io/the-drunkard-walk) found on itch.io. It generates rooms/corridors in random direction based on input size.

## How it works

Starting from a given point, the algorithm takes a walk in a random direction and marks each position in `_walkLength` as a part of the path. This process is repeated `_numWalks` times.

## Demo
