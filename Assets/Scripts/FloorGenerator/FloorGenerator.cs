using System.Collections.Generic;
using UnityEngine;

/// <summary>

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

  // Start level generation in the center of the grid
  private int _x, _y = 0;

  [Header("Floor properties")]
  [SerializeField] private int _floorWidth;
  [SerializeField] private int _floorHeight;

  [Header("Room properties")]
  // Rooms to fill in with each floor.
  // Each floor will at least have 4 rooms - one in each corner.
  [SerializeField] private int _roomCount;
  [SerializeField] private GameObject _room;

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

        _floorPositions.Add(_position);

        if (x == -1 || x == _floorWidth || y == -1 || y == _floorHeight)
        {
          InstantiateTile(_position, _wallTile, gameObject);
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
    AdjustVerticalGapBetweenRooms(_topLeft, _bottomLeft);
    AdjustHorizontalGapBetweenRooms(_bottomLeft, _bottomRight);
    AdjustVerticalGapBetweenRooms(_topRight, _bottomRight);

    // Place tiles for each room
    foreach (RoomManager _room in _rooms)
    {
      PlaceRoomTiles(_room);
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
      if (Random.Range(0, 2) == 0) room.Width = room.Width - 2;
      // Expand room to fill the gap
      else room.Width = _floorWidth - nextRoom.Width - 1;

    }
  }

  /// <summary>
  /// Adjusts the vertical gap between rooms to ensure there's enough space between them.
  /// The function will either expand the room to fill the gap or reduce the room size.
  /// 
  /// The minimum gap between rooms is 3 (two wall tiles and a floor tile).
  /// </summary>
  void AdjustVerticalGapBetweenRooms(RoomManager room, RoomManager nextRoom, int gap = 3)
  {
    if (_floorHeight - (room.Height + nextRoom.Height) <= gap)
    {
      // Reduce room size
      if (Random.Range(0, 2) == 0) room.Height = room.Height - 2;
      // Expand room to fill the gap
      else room.Height = _floorHeight - nextRoom.Height - 1;
    }
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
          InstantiateTile(_position, _wallTile, room.gameObject);
        }
        else InstantiateTile(_position, _floorTile, room.gameObject);
      }
    }
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
