using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
  private int _x, _y, _width, _height;
  private bool _sideRoom = false;

  public int X { get => _x; set => _x = value; }
  public int Y { get => _y; set => _y = value; }
  public int Width { get => _width; set => _width = value; }
  public int Height { get => _height; set => _height = value; }

  public bool SideRoom { get => _sideRoom; set => _sideRoom = value; }

  public Vector3Int GetCenter() => new(_x + _width / 2, _y + _height / 2);

  public Bounds GetBounds() => new Bounds(GetCenter(), new Vector3(_width, _height, 0));

  /// <summary>
  /// Return True if this room overlaps with another RoomManager.
  /// </summary>
  public bool Overlaps(List<RoomManager> otherRooms)
  {
    foreach (RoomManager _room in otherRooms)
    {
      if (GetBounds().Intersects(_room.GetBounds())) return true;
    }

    return false;
  }

  public void SetRoomProperties(int x, int y, int width, int height)
  {
    _x = x;
    _y = y;

    _width = width;
    _height = height;
  }
}
