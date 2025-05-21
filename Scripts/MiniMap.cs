using da.Scripts.Objects;
using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace da.Scripts
{
    internal class MiniMapClass
    {
        private Image mapImage;
        private ImageTexture tex;
        public Vector2I size;
        private static MiniMapClass _ins;
        //public static MiniMapClass Ins { get => _ins ??= new MiniMapClass(); }
        public void UpdateSize(Vector2I newSize)
        {
            size = newSize;
            mapImage = Image.CreateEmpty(size.X, size.Y, false, Image.Format.Rgba8);
        }
        public void LinkTexture(TextureRect texture)
        {
            tex = ImageTexture.CreateFromImage(mapImage);
            texture.Texture = tex;
        }
        public void DrawMiniMap(Vector2I playerPosition, TileMapLayer map, int ZoomScale)
        {
            var mapdata = GameGlobal.Instance.save.MapSaveData.GetValueOrDefault(map.Name, new());
            if (!mapdata.ContainsKey("VisitedPoints"))
            {
                mapdata.Add("VisitedPoints", new());
            }
            var visited = mapdata["VisitedPoints"].AsGodotDictionary<int, bool>();
            if (tex == null || mapImage == null || GameGlobal.Instance.player == null) return;
            for (int i = 0; i < size.X; i += ZoomScale)
            {
                for (int j = 0; j < size.Y; j += ZoomScale)
                {
                    if (i == 0 || j == 0)
                    {
                        mapImage.FillRect(new(i, j, new(ZoomScale, ZoomScale)), new(255, 255, 255));
                        continue;
                    }
                    else if (i >= size.X - ZoomScale)
                    {
                        mapImage.FillRect(new(size.X - ZoomScale, j, new(ZoomScale, ZoomScale)), new(255, 255, 255));
                        continue;
                    }
                    else if (j >= size.Y - ZoomScale)
                    {
                        mapImage.FillRect(new(i, size.Y - ZoomScale, new(ZoomScale, ZoomScale)), new(255, 255, 255));
                        continue;
                    }

                    int x = playerPosition.X - size.X / 2 + i;
                    int y = playerPosition.Y - size.Y / 2 + j;
                    //int x = playerPosition.X + i;
                    //int y = playerPosition.Y + j;
                    int xcoord = x / 16;
                    int ycoord = y / 16;
                    int coord = xcoord * 10000 + ycoord;

                    if (new Vector2(x, y).DistanceTo(GameGlobal.Instance.player.Position) / ZoomScale < 1000)
                    {
                        if (!visited.ContainsKey(coord))
                        {
                            visited.Add(coord, false);
                        }
                        visited[coord] = true;
                    }

                    //if (x > 0 && y > 0 && x < mapSize.X - tileSize && y < mapSize.Y - tileSize)
                    //{
                    if (visited.ContainsKey(coord) && visited[coord])
                    {
                        var cell = map.GetCellTileData(new(xcoord, ycoord));
                        int data = cell?.GetCustomData("MiniMapData").AsInt32() ?? 0;
                        switch (data)
                        {
                            case 0:
                                mapImage.FillRect(new(i, j, new(ZoomScale, ZoomScale)), Colors.Blue);
                                break;
                            case 1:
                                mapImage.FillRect(new(i, j, new(ZoomScale, ZoomScale)), Colors.Gray);
                                break;
                            case 2:
                                mapImage.FillRect(new(i, j, new(ZoomScale, ZoomScale)), Colors.Green);
                                break;
                        }
                    }
                    else
                    {
                        mapImage.FillRect(new(i, j, new(ZoomScale, ZoomScale)), new(.2f, .2f, .2f, .5f));
                    }
                    //}
                }
            }

            mapImage.FillRect(new(size.X / 2, size.Y / 2, new(ZoomScale, ZoomScale)), Colors.Red);

            tex.Update(mapImage);
        }
    }
}
