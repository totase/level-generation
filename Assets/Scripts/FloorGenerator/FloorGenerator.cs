using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a floor with a room in each corner and additional rooms between them,
/// connected with doors and a hallway.
/// </summary>
public class FloorGenerator : LevelManager
{
  /// <summary>
  /// Enum representing floor edges.
  /// </summary>
  enum Direction
  {
    Top,
    Left,
    Down,
    Right
  }

  [Header("Floor properties")]
  [SerializeField] private int _floorWidth;
  [SerializeField] private int _floorHeight;

  [Header("Room properties")]
  // Rooms to fill in with each floor.
  // Each floor will at least have 4 rooms - one in each corner.
  [SerializeField] private int _roomCount;
  [SerializeField] private GameObject _room;

  // Should only expand a room to fill horizontal gap once. Track that here.
  private bool _didExpandRoom = false;

  Direction _previoudDirection;
  int _maxAttempts;

  [Space]
  [SerializeField] private int _minWidth;
  [SerializeField] private int _maxWidth;
  [SerializeField] private int _minHeight;
  [SerializeField] private int _maxHeight;

  [Header("Tiles")]
  [SerializeField] private GameObject _floorTile;
  [SerializeField] private GameObject _wallTile;

  private List<RoomManager> _rooms;

  private List<Vector3Int> _floorPositions;

  public override void SetupScene()
  {
    _maxAttempts = _roomCount * 2;

    _rooms = new List<RoomManager>();
    _floorPositions = new List<Vector3Int>();

    GenerateLevel();
  }

  /// <summary>
  /// Generates a random level based on the room count and room size.
  /// </summary>
  void GenerateLevel()
  {
    // Initialize _floorPositions with width and height of the current floor list
    for (int x = -1; x < _floorWidth + 1; x++)
    {
      for (int y = -1; y < _floorHeight + 1; y++)
      {
        Vector3Int _position = new Vector3Int(x, y, 0);

        if (x == -1 || x == _floorWidth || y == -1 || y == _floorHeight)
        {
          InstantiateTile(_position, _wallTile, gameObject);
        }
        else
        {
          _floorPositions.Add(_position);
        }
      }
    }

    // Generate a room in each corner of the floor
    List<Vector3Int> _corners = GetFloorCorners();

    foreach (Vector3Int _corner in _corners)
    {
      int _roomWidth = Random.Range(_minWidth, _maxWidth);
      int _roomHeight = Random.Range(_minHeight, _maxHeight);

      RoomManager _newRoom = AddRoom(_corner.x, _corner.y, _roomWidth, _roomHeight);

      // If the room is outside the floor bounds, move it inside
      if (_newRoom.X + _newRoom.Width > _floorWidth) _newRoom.X = _floorWidth - _newRoom.Width;
      if (_newRoom.Y + _newRoom.Height > _floorHeight) _newRoom.Y = _floorHeight - _newRoom.Height;

      _rooms.Add(_newRoom);
    }

    RoomManager _topLeft = _rooms[0];
    RoomManager _topRight = _rooms[1];
    RoomManager _bottomRight = _rooms[2];
    RoomManager _bottomLeft = _rooms[3];

    // If there's only three or less (one floor position + two walls) spaces between rooms, 
    // expand the room to fill the space or reduce the room size.
    AdjustHorizontalGapBetweenRooms(_topLeft, _topRight);
    AdjustHorizontalGapBetweenRooms(_bottomLeft, _bottomRight);

    for (int i = 0; i < _roomCount && i < _maxAttempts;)
    {
      Direction _direction = GetRandomDirection();

      if (_previoudDirection == _direction) continue;
      else _previoudDirection = _direction;

      bool _roomCreated = false;

      switch (_direction)
      {
        case Direction.Top:
          _roomCreated = CreateVerticalRoomBetweenRooms(_topLeft, _topRight);
          break;
        case Direction.Left:
          _roomCreated = CreateHorizontalRoomBetweenRooms(_bottomLeft, _topLeft);
          break;
        case Direction.Down:
          _roomCreated = CreateVerticalRoomBetweenRooms(_bottomLeft, _bottomRight);
          break;
        case Direction.Right:
          _roomCreated = CreateHorizontalRoomBetweenRooms(_bottomRight, _topRight);
          break;
      }

      if (_roomCreated) i++;
      else _maxAttempts++;
    }

    // Place tiles for each room
    foreach (RoomManager _room in _rooms)
    {
      PlaceRoomTiles(_room);
    }

    // Fill the remaining floor positions with floor tiles
    foreach (Vector3Int _position in _floorPositions)
    {
      InstantiateTile(_position, _floorTile, gameObject);
    }
  }

  /// <summary>
  /// Adjusts the horizontal gap between rooms to ensure there's enough space between them.
  /// The function will either expand the room to fill the gap or reduce the room size.
  ///
  /// The minimum gap between rooms is 3 (two wall tiles and a floor tile).
  /// </summary>
  void AdjustHorizontalGapBetweenRooms(RoomManager room, RoomManager nextRoom, int gap = 3)
  {
    if (_floorWidth - (room.Width + nextRoom.Width) <= gap)
    {
      // Reduce room size
      if (Random.Range(0, 2) == 0 || _didExpandRoom) room.Width = room.Width - 2;
      else
      {
        // Expand room to fill the gap
        room.Width = _floorWidth - nextRoom.Width - 1;

        _didExpandRoom = true;
      }
    }
  }

  /// <summary>
  /// Creates a room between two vertically aligned rooms.
  /// 
  /// Returns True if a room was created, False otherwise.
  /// </summary>
  bool CreateVerticalRoomBetweenRooms(RoomManager room, RoomManager nextRoom)
  {
    // Subtract an additional 2 to account for the adjacent wall tiles
    int _spaceBetweenRooms = _floorWidth - (room.Width + nextRoom.Width) - 2;

    if (_spaceBetweenRooms >= 2)
    {
      int _roomX = room.X + room.Width + 1;
      int _roomY = room.Y;

      RoomManager _newRoom = AddRoom(_roomX, _roomY, _spaceBetweenRooms, room.Height);

      _rooms.Add(_newRoom);

      return true;
    }

    return false;
  }

  /// <summary>
  /// Creates a room between two horizontally aligned rooms.
  /// 
  /// Returns True if a room was created, False otherwise.
  /// </summary>
  bool CreateHorizontalRoomBetweenRooms(RoomManager room, RoomManager nextRoom)
  {
    // Subtract an additional 2 to account for the adjacent wall tiles
    int _spaceBetweenRooms = _floorHeight - (room.Height + nextRoom.Height) - 2;

    if (_spaceBetweenRooms >= 2)
    {
      int _roomX = room.X;
      int _roomY = room.Y + room.Height + 1;

      int _roomWidth = room.Width < nextRoom.Width ? room.Width : nextRoom.Width;

      RoomManager _newRoom = AddRoom(_roomX, _roomY, _roomWidth, _spaceBetweenRooms);

      _rooms.Add(_newRoom);

      return true;
    }

    return false;
  }

  List<Vector3Int> GetFloorCorners()
  {
    List<Vector3Int> _corners = new List<Vector3Int>
    {
      // Floor corners in the following order:
      // Top left, top right, bottom left, bottom right
      new Vector3Int(0, _floorHeight - 1, 0),
      new Vector3Int(_floorWidth - 1, _floorHeight - 1, 0),
      new Vector3Int(_floorWidth - 1, 0, 0),
      new Vector3Int(0, 0, 0),
    };

    return _corners;
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

  /// <summary>
  /// Places floor and wall tiles for a room and adds doors between connected rooms.
  /// </summary>
  void PlaceRoomTiles(RoomManager room)
  {
    for (int x = room.X - 1; x < room.X + room.Width + 1; x++)
    {
      for (int y = room.Y - 1; y < room.Y + room.Height + 1; y++)
      {
        Vector3Int _position = new Vector3Int(x, y, 0);
        if (x == room.X - 1 || x == room.X + room.Width || y == room.Y - 1 || y == room.Y + room.Height)
        {
          // InstantiateTile(_position, _wallTile, room.gameObject);
        }
        else InstantiateTile(_position, _floorTile, room.gameObject);

        _floorPositions.Remove(_position);
      }
    }

    Vector3Int _doorPosition = new Vector3Int(room.X + 1, room.GetCenter().y, 0);
    bool _topHit = false, _leftHit = false, _bottomHit = false, _rightHit = false;

    // Add door to the room, but make sure the door is not placed on a floor border tile
    if (room.X - 1 == -1) _leftHit = true;
    if (room.X + room.Width == _floorWidth) _rightHit = true;
    if (room.Y - 1 == -1) _bottomHit = true;
    if (room.Y + room.Height == _floorHeight) _topHit = true;

    int _random = Random.Range(0, 3);

    if (_topHit && _rightHit || _topHit && _leftHit) _doorPosition.y = room.Y - 1;
    else if (_bottomHit && _leftHit || _bottomHit && _rightHit) _doorPosition.y = room.Y + room.Height;
    else if (_leftHit) _doorPosition.x = room.X + room.Width;
    else if (_rightHit) _doorPosition.x = room.X - 1;
    else if (_topHit)
    {
      if (_random == 0) _doorPosition.x = room.X + room.Width;
      else if (_random == 1) _doorPosition.x = room.X - 1;
      else _doorPosition.y = _doorPosition.y = room.Y - 1;
    }
    else if (_bottomHit)
    {
      if (_random == 0) _doorPosition.x = room.X + room.Width;
      else if (_random == 1) _doorPosition.x = room.X - 1;
      else _doorPosition.y = _doorPosition.y = room.Y + room.Height;
    }

    InstantiateTile(_doorPosition, _floorTile, room.gameObject);
  }

  void InstantiateTile(Vector3Int position, GameObject tile, GameObject room)
  {
    GameObject _tileToInstantiate = Instantiate(tile, position, Quaternion.identity);

    _tileToInstantiate.transform.SetParent(room.transform);
  }

  /// <summary>
  /// Returns a random floor edge.
  /// </summary>
  Direction GetRandomDirection()
  {
    int _random = Random.Range(1, 4);

    switch (_random)
    {
      case 1:
        return Direction.Top;
      case 2:
        return Direction.Left;
      case 3:
        return Direction.Down;
      case 4:
        return Direction.Right;
      default:
        Debug.LogError("Direction not found: " + _random);
        return 0;
    }
  }
}
