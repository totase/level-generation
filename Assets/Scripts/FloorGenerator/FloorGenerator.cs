using System.Collections.Generic;
using UnityEngine;

/// <summary>

/// </summary>
public class FloorGenerator : LevelManager
{
  // Start level generation in the center of the grid
  private int _x, _y = 0;

  public override void SetupScene()
  {
    GenerateLevel();
  }

  /// <summary>
  /// Generates a random level based on the room count and room size.
  /// </summary>
  void GenerateLevel()
  {

  }
}
