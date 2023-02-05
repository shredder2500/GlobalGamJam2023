using System.Drawing;
using Silk.NET.Maths;

namespace GameJam;

public static class Config
{
    public static Vector2D<int> GridSize = new Vector2D<int>(25, 16);
    public static Size StumpyTileSheetSize = new(320, 192);
    public static int PPU = 16;
    public static int SpawnCount = 30;
    public static int StartingEnergy = 6;
}