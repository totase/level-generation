using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Procedurally generates rooms in a grid-based layout with connecting corridors.
/// Supports main path rooms and optional side rooms.
/// </summary>
public class RoomGenerator : LevelManager
{
  /// <summary>
  /// Enum representing directions.
  /// </summary>
  enum Direction
  {
    Left,
    Right,
    Down
  }

  // Start level generation in the center of the grid
  private int _x = 0;
  private int _y = 0;

  private int _duplicateDirection = 0;
  private bool _didCreateSideRoom = false;
  private Direction _direction, _previousDirection;

  private const int MAX_DUPLICATES_DIRECTION = 2;

  [Header("Tiles")]
  [SerializeField] private GameObject _floorTile;

  [Header("Room properties")]
  [SerializeField] private int _roomCount;

  [Space]
  [SerializeField] private int _roomWidth;
  [SerializeField] private int _roomHeight;

  [Space]
  [SerializeField] private RoomManager _room;

  private List<RoomManager> _rooms;

  /// <summary>
  /// Generates a random level based on the room count and room size.
  /// </summary>
  void GenerateLevel()
  {
    // Initialize the rooms list
    _rooms = new List<RoomManager>();

    // Generate start room
    RoomManager _startRoom = AddRoom(_x, _y, _roomWidth, _roomHeight);

    PlaceRoomTiles(null, _startRoom);

    _rooms.Add(_startRoom);

    // i starts on 1 to account for start room
    for (int i = 1; i < _roomCount; i++)
    {
      GenerateRoom();
    }
  }

  /// <summary>
  /// Creates a new room in a random direction from the current position.
  /// May also generate an optional side room.
  /// </summary>
  public void GenerateRoom()
  {
    _direction = GetRandomDirection();

    RoomManager _newRoom = CreateRoomAtPosition(_x, _y, _roomWidth, _roomHeight);

    if (_newRoom.Overlaps(_rooms))
    {
      RemoveRoom(_newRoom);

      GenerateRoom();

      return;
    }

    _x = _newRoom.X;
    _y = _newRoom.Y;

    RoomManager _previousRoom = _didCreateSideRoom ? _rooms[_rooms.Count - 2] : _rooms[_rooms.Count - 1];

    if (_didCreateSideRoom) _didCreateSideRoom = false;

    PlaceRoomTiles(_previousRoom, _newRoom);

    _rooms.Add(_newRoom);

    bool _createSideRoom = Random.Range(0, 2) == 0;

    if (_createSideRoom)
    {
      _didCreateSideRoom = true;
      RoomManager _sideRoom = CreateRoomAtPosition(_x, _y, _roomWidth, _roomHeight, true);

      if (_sideRoom.Overlaps(_rooms))
      {
        RemoveRoom(_sideRoom);
      }
      else
      {
        PlaceRoomTiles(_newRoom, _sideRoom);

        _sideRoom.SideRoom = true;

        _rooms.Add(_sideRoom);
      }
    }
  }

  /// <summary>
  /// Places floor and wall tiles for a room and adds doors between connected rooms.
  /// </summary>
  void PlaceRoomTiles(RoomManager previousRoom, RoomManager newRoom)
  {
    for (int x = newRoom.X - 1; x < newRoom.X + newRoom.Width + 1; x++)
    {
      for (int y = newRoom.Y - 1; y < newRoom.Y + newRoom.Height + 1; y++)
      {
        Vector3Int _position = new Vector3Int(x, y, 0);
        if (x == newRoom.X - 1 || x == newRoom.X + newRoom.Width || y == newRoom.Y - 1 || y == newRoom.Y + newRoom.Height)
        {
          // Set wall tile
        }
        else InstantiateFloorTile(_position);
      }
    }

    if (previousRoom) AddDoorBetweenRooms(previousRoom, newRoom);
    else
    {
      // Chance of creating special room
    }
  }

  void InstantiateFloorTile(Vector3Int position)
  {
    Instantiate(_floorTile, position, Quaternion.identity);
  }

  /// <summary>
  /// Creates a room at the specified position, handling direction logic and room spacing.
  /// </summary>
  RoomManager CreateRoomAtPosition(int x, int y, int roomWidth, int roomHeight, bool isSideRoom = false)
  {
    switch (_direction)
    {
      case Direction.Left:
        if (isSideRoom)
        {
          x -= roomWidth + 1;
          _previousDirection = Direction.Left;

          break;
        }

        if (_previousDirection == Direction.Left) _duplicateDirection++;

        if (_previousDirection == Direction.Right || _duplicateDirection >= MAX_DUPLICATES_DIRECTION)
        {
          y -= roomHeight + 1;
          _previousDirection = Direction.Down;
          _duplicateDirection = 0;
        }
        else
        {
          x -= roomWidth + 1;
          _previousDirection = Direction.Left;
        }

        break;

      case Direction.Right:
        if (isSideRoom)
        {
          x += roomWidth + 1;
          _previousDirection = Direction.Right;

          break;
        }

        if (_previousDirection == Direction.Right) _duplicateDirection++;

        if (_previousDirection == Direction.Left || _duplicateDirection >= MAX_DUPLICATES_DIRECTION)
        {
          y -= roomHeight + 1;
          _previousDirection = Direction.Down;
          _duplicateDirection = 0;
        }
        else
        {
          x += roomWidth + 1;
          _previousDirection = Direction.Right;
        }

        break;

      case Direction.Down:
        if (isSideRoom)
        {
          y -= roomHeight + 1;
          _previousDirection = Direction.Down;

          break;
        }

        if (_previousDirection == Direction.Down) _duplicateDirection++;

        if (_duplicateDirection >= MAX_DUPLICATES_DIRECTION)
        {
          _duplicateDirection = 0;
          int randomDirection = Random.Range(0, 2);

          if (randomDirection == 0)
          {
            x -= roomWidth + 1;
            _previousDirection = Direction.Left;
          }
          else
          {
            x += roomWidth + 1;
            _previousDirection = Direction.Right;
          }
        }
        else
        {
          y -= roomHeight + 1;
          _previousDirection = Direction.Down;
        }

        break;
    }

    return AddRoom(x, y, roomWidth, roomHeight);
  }

  /// <summary>
  /// Creates a door connection between two adjacent rooms.
  /// </summary>
  void AddDoorBetweenRooms(RoomManager previousRoom, RoomManager newRoom)
  {
    Vector3Int _previousRoomCenter = previousRoom.GetCenter();
    Vector3Int _randomPoint = previousRoom.GetCenter();

    int _posX = 0;
    int _posY = 0;

    if (_previousDirection == Direction.Left)
    {
      _posX = _previousRoomCenter.x - previousRoom.Width / 2 - 1;
      _posY = _randomPoint.y;
    }
    else if (_previousDirection == Direction.Right)
    {
      _posX = _previousRoomCenter.x + previousRoom.Width / 2 + 1;
      _posY = _randomPoint.y;
    }
    else if (_previousDirection == Direction.Down)
    {
      _posX = _randomPoint.x;
      _posY = _previousRoomCenter.y - previousRoom.Height / 2 - 1;
    }

    Vector3Int _doorPosition = new Vector3Int(_posX, _posY, 0);

    InstantiateFloorTile(_doorPosition);

    // Place door object
  }

  /// <summary>
  /// Instantiates and initializes a new room with the specified dimensions.
  /// </summary>
  public RoomManager AddRoom(int x, int y, int roomWidth, int roomHeight)
  {
    Vector3 _instantiatePosition = new Vector3(x + roomWidth / 2, y + roomHeight / 2, 0);
    RoomManager _toInstantiate = Instantiate(_room, _instantiatePosition, Quaternion.identity).GetComponent<RoomManager>();

    _toInstantiate.SetRoomProperties(x, y, roomWidth, roomHeight);

    return _toInstantiate;
  }

  public void RemoveRoom(RoomManager room)
  {
    _rooms.Remove(room);

    Destroy(room.gameObject);
  }

  public override void SetupScene()
  {
    _rooms = new List<RoomManager>();

    GenerateLevel();

    RoomManager _startRoom = _rooms[0];
    RoomManager _endRoom = _rooms[_rooms.Count - 1];
    // If this _endRoom is a side room, use the second to last room in the list
    if (_endRoom.SideRoom) _endRoom = _rooms[_rooms.Count - 2];

    // Instantiate player in the first room

    // Instantiate exit in the last room
  }

  /// <summary>
  /// Returns a random cardinal direction with weighted probabilities.
  /// Left/Right are more common than Down.
  /// </summary>
  Direction GetRandomDirection()
  {
    int _random = Random.Range(1, 6);

    switch (_random)
    {
      case 1:
      case 2:
        return Direction.Left;
      case 3:
      case 4:
        return Direction.Right;
      case 5:
        return Direction.Down;
      default:
        Debug.LogError("Direction not found: " + _random);
        return 0;
    }
  }
}
