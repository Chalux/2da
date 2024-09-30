using da.Objects;
using da.Scripts;
using da.Scripts.Objects;
using Godot;

namespace da.Scenes
{
    public partial class RootScene : Node2D, IEvent
    {
        [Export] Camera2D camera;
        [Export] public Player player;
        [Export] public Control WorldControl;

        public override void _Ready()
        {
            //var UsedRect = map.GetUsedRect().Grow(-1);
            //var TileSize = map.TileSet.TileSize;

            //camera.LimitTop = UsedRect.Position.Y * TileSize.Y;
            //camera.LimitBottom = UsedRect.End.Y * TileSize.Y;
            //camera.LimitLeft = UsedRect.Position.X * TileSize.X;
            //camera.LimitRight = UsedRect.End.X * TileSize.X;
            camera.ResetSmoothing();
            EventMgr.RegisterEvent(this);

            if (player.IsNodeReady())
            {
                ReceiveEvent("PlayerReady", player);
                player.status.OnHealthChanged += GameGlobal.Instance.PlayerHealthChanged;
                player.status.OnMaxHealthChanged += GameGlobal.Instance.PlayerMaxHealthChanged;
            }

            EventMgr.DispatchEvent("RootReady", this);
        }

        public void ReceiveEvent(string eventName, params object[] datas)
        {
        }

        public void TeleportPlayer(Vector2 position, int Direction)
        {
            player.GlobalPosition = position;
            player.Direction = Direction;
            camera.ResetSmoothing();
            camera.ForceUpdateScroll();
        }
    }
}
