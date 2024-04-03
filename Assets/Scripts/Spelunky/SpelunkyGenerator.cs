using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

/// <summary>
/// Generates a level by creating a grid of rooms and corridors.
/// ref: https://tinysubversions.com/spelunkyGen/
/// </summary>
public class SpelunkyGenerator : LevelManager
{
    [Header("Room objects")]
    [SerializeField] private GameObject _top;
    [SerializeField] private GameObject _bottom;
    [SerializeField] private GameObject _corridor;
    [SerializeField] private GameObject _uniform;

    [Header("Room properties")]
    [SerializeField] private int _roomHeight = 6;
    [SerializeField] private int _roomWidth = 8;

    [Header("Board properties")]
    [SerializeField] private int _rows = 3;
    [SerializeField] private int _columns = 3;

    private int _startColumn = 0;
    private bool _hasPath = false;

    private Transform _levelHolder;
    private List<Vector3> _gridPositions = new List<Vector3>();

    void InitialiseGrid()
    {
        _gridPositions.Clear();

        for (int x = 0; x < _columns; x += _roomWidth)
        {
            for (int y = 0; y < _rows; y += _roomHeight)
            {
                _gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void RemoveFromPositions(Vector3 position)
    {
        _gridPositions.Remove(position);
    }

    /// <summary>
    /// Gets the position of the next room based on the current direction.
    /// </summary>
    /// 
    /// <returns>The position of the next room.</returns>
    Vector3 GetRoomPosition(int direction, Vector3 currentPos)
    {
        Vector3 _targetPos = Vector3.zero;

        switch (direction)
        {
            case 1: // Left
                if (currentPos.x == 0)
                {
                    // If current position is 0 (on level edge), move down instead
                    _targetPos.Set(currentPos.x, currentPos.y - _roomHeight, 0f);
                }
                else
                {
                    _targetPos.Set(currentPos.x - _roomWidth, currentPos.y, 0f);
                }

                break;
            case 2: // Right
                if (currentPos.x == _columns - _roomWidth)
                {
                    // If current position is the same as column value (on level edge), move down instead
                    _targetPos.Set(currentPos.x, currentPos.y - _roomHeight, 0f);
                }
                else
                {
                    _targetPos.Set(currentPos.x + _roomWidth, currentPos.y, 0f);
                }

                break;
            case 3: // Down
                _targetPos.Set(currentPos.x, currentPos.y - _roomHeight, 0f);
                break;
        }

        return _targetPos;
    }

    /// <summary>
    /// Gets a random direction for the next room.
    /// </summary>
    /// 
    /// <returns>A random direction, either left, right or down.</returns>
    int GetRandomDirection()
    {
        int _targetDirection = Random.Range(1, 6);

        switch (_targetDirection)
        {
            case 1:
            case 2:
                return 1; // Left
            case 3:
            case 4:
                return 2; // Right
            case 5:
                return 3; // Down
            default:
                Debug.LogError("Direction not found: " + _targetDirection);
                return 0;
        }
    }

    /// <summary>
    /// Determines the type of room to instantiate based on the positions of the last, current, and target rooms.
    /// </summary>
    ///
    /// <returns>The type of room to instantiate.</returns>
    GameObject GetRoomType(Vector3 lastPos, Vector3 currentPos, Vector3 targetPos)
    {
        if (targetPos.y == currentPos.y)
        {
            if (lastPos.y > currentPos.y)
            {
                return _bottom;
            }

            return _corridor;
        }

        if (targetPos.y < currentPos.y)
        {
            if (currentPos.y == lastPos.y)
            {
                return _top;
            }

            if (targetPos.x < currentPos.x)
            {
                return _corridor;
            }
        }

        return _uniform;
    }

    /// <summary>
    /// Sets up the level by instantiating rooms and corridors.
    /// </summary>
    void LevelSetup()
    {
        bool _hasEntrance = false;

        int _currentDir = 0;
        // Position to start room instantiation 
        float _startRow = _rows - _roomHeight;

        Vector3 _targetPos;
        Vector3 _lastPos = new Vector3();
        Vector3 _currentPos = new Vector3();

        while (!_hasPath)
        {
            if (!_hasEntrance)
            {
                _currentPos.Set(_startColumn, _startRow, 0f);

                _currentDir = 1;
                // If on an edge
                if (_currentPos.x == 0 && _currentDir == 1)
                {
                    // Move the opposite direction
                    _currentDir = 2;
                }
            }
            else
            {
                if (GetRandomDirection() == 3)
                {
                    _currentDir = 3;
                }
            }

            _targetPos = GetRoomPosition(_currentDir, _currentPos);

            // If on a new level	
            if (_targetPos.y < _currentPos.y)
            {
                // Get new direction
                _currentDir = GetRandomDirection();

                // If on an edge
                if (_currentPos.x == 0 && _currentDir == 1)
                {
                    // Move the opposite direction
                    _currentDir = 2;
                }
                else if (_currentPos.x == _columns - _roomWidth && _currentDir == 2)
                {
                    _currentDir = 1;
                }
            }

            if (_currentPos.y < 0)
            {
                _hasPath = true;

                break;
            }

            GameObject _toInstantiate;
            if (!_hasEntrance)
            {
                _hasEntrance = true;
                // Could be its own entrance room
                _toInstantiate = _bottom;
            }
            else
            {
                _toInstantiate = GetRoomType(_lastPos, _currentPos, _targetPos);
            }

            GenerateRoom(_toInstantiate, _currentPos);

            _lastPos = _currentPos;
            _currentPos = _targetPos;
        }

        FinishSetup();
    }

    /// <summary>
    /// Instantiates a room at a given position, and sets the level holder as its parent.
    /// </summary>
    void GenerateRoom(GameObject room, Vector3 pos)
    {
        GameObject _currentRoom;
        RemoveFromPositions(pos);

        _currentRoom = Instantiate(room, pos, Quaternion.identity);
        _currentRoom.transform.SetParent(_levelHolder);
    }

    public override void SetupScene()
    {
        // Get a random column to start on
        _startColumn = Random.Range(0, _columns) * _roomWidth;

        // Create a new level holder to keep hierarchy organized
        _levelHolder = new GameObject("Level").transform;

        // Multiply room width by columns to get correct values
        _columns = _roomWidth * _columns;
        // Do the same with the room height
        _rows = _roomHeight * _rows;

        // Set up grid with new column and row values --
        InitialiseGrid();
        // -- and set up the level
        LevelSetup();
    }

    /// <summary>
    /// Finishes the setup process and notifies the GameManager.
    /// </summary>
    void FinishSetup()
    {
        // Add side rooms

        GameManager.instance.FinishSetup();
    }
}
