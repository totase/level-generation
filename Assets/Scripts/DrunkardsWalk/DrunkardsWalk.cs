using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a level using a random walk algorithm.
/// </summary>
public class DrunkardsWalk : LevelManager
{
    /// <summary>
    /// Enum representing the directions.
    /// </summary>
    enum Direction
    {
        North,
        East,
        South,
        West
    }

    [Header("Drunkards walk")]
    // Number of walks to perform.
    [SerializeField] private int _numWalks;
    // Length of each walk.
    [SerializeField] private int _walkLength;

    // Starting position of the walk.
    private int _startX, _startY = 0;
    private List<Vector3> _gridPositions = new List<Vector3>();

    [Header("Room properties")]
    [SerializeField] private int _roomWidth;
    [SerializeField] private int _roomHeight;

    [Space]
    [SerializeField] private GameObject _tile;

    private Transform _levelHolder;

    /// <summary>
    /// Generates the level by performing a number of "random walks".
    /// </summary>
    void GenerateLevel()
    {
        // make sure initial position is added as start position
        AddToGridPositions(_startX, _startX);

        for (int i = 0; i < _numWalks; i++)
        {
            int _tempX = _startX;
            int _tempY = _startY;

            for (int y = 0; y < _walkLength; y++)
            {
                Direction _tempDirection = GetRandomDirection();

                switch (_tempDirection)
                {
                    case Direction.North:
                        _tempY += _roomHeight;
                        break;
                    case Direction.East:
                        _tempX += _roomWidth;
                        break;
                    case Direction.South:
                        _tempY -= _roomHeight;
                        break;
                    case Direction.West:
                        _tempX -= _roomWidth;
                        break;
                }

                AddToGridPositions(_tempX, _tempY);
            }
        }

        GenerateTiles();

        FinishSetup();
    }

    /// <summary>
    /// Adds a new position to the grid, ensuring no duplicates are added.
    /// </summary>
    void AddToGridPositions(int _posX, int _posY)
    {
        for (int x = 0; x < _roomWidth; x++)
        {
            for (int y = 0; y < _roomHeight; y++)
            {
                Vector3 _targetPos = new Vector3(x + _posX, y + _posY, 0f);

                if (!_gridPositions.Contains(_targetPos))
                {
                    _gridPositions.Add(_targetPos);
                }
            }
        }
    }

    /// <summary>
    /// Instantiates tiles at all positions in the grid.
    /// </summary>
    void GenerateTiles()
    {
        for (int i = 0; i < _gridPositions.Count; i++)
        {
            GameObject _currentTile = Instantiate(_tile, _gridPositions[i], Quaternion.identity);
            _currentTile.transform.SetParent(_levelHolder);
        }
    }

    Direction GetRandomDirection()
    {
        int _tempDirection = Random.Range(0, 4);

        return (Direction)_tempDirection;
    }

    public override void SetupScene()
    {
        GenerateLevel();
    }

    /// <summary>
    /// Finishes the setup process and notifies the GameManager.
    /// </summary>
    void FinishSetup()
    {
        GameManager.instance.FinishSetup();
    }
}
