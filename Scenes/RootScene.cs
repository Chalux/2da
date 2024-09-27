using da.Objects;
using Godot;

namespace da.Scenes
{
    public partial class RootScene : Node2D
    {
        [Export] Camera2D camera;
        [Export] TileMap map;
        [Export] public Player player;

        public override void _Ready()
        {
            //var UsedRect = map.GetUsedRect().Grow(-1);
            //var TileSize = map.TileSet.TileSize;

            //camera.LimitTop = UsedRect.Position.Y * TileSize.Y;
            //camera.LimitBottom = UsedRect.End.Y * TileSize.Y;
            //camera.LimitLeft = UsedRect.Position.X * TileSize.X;
            //camera.LimitRight = UsedRect.End.X * TileSize.X;
            camera.ResetSmoothing();
        }
    }
}
