using da.Objects;
using da.Scripts.Objects;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts
{
    public partial class Map : Control, IEvent
    {
        [Export] public Player player;
        [Export] public AudioStream BGM;

        public override void _Ready()
        {
            MouseFilter = MouseFilterEnum.Ignore;
            GameGlobal.Instance.UIControl.Visible = true;
            GameGlobal.Instance.player = player;
            GameGlobal.Instance.camera = player.PlayerCamera;
            //var UsedRect = map.GetUsedRect().Grow(-1);
            //var TileSize = map.TileSet.TileSize;

            //camera.LimitTop = UsedRect.Position.Y * TileSize.Y;
            //camera.LimitBottom = UsedRect.End.Y * TileSize.Y;
            //camera.LimitLeft = UsedRect.Position.X * TileSize.X;
            //camera.LimitRight = UsedRect.End.X * TileSize.X;
            player.PlayerCamera.ResetSmoothing();

            ReceiveEvent("PlayerReady", player);
            player.status.OnHealthChanged += GameGlobal.Instance.PlayerHealthChanged;
            player.status.OnMaxHealthChanged += GameGlobal.Instance.PlayerMaxHealthChanged;

            player.PlayerCamera.ResetSmoothing();
            player.PlayerCamera.ForceUpdateScroll();

            if (BGM != null)
            {
                SoundManager.Ins.PlayBGM(BGM);
            }
        }

        public override void _ExitTree()
        {
            //GameGlobal.Instance.player = null;
        }

        public void ReceiveEvent(string eventName, params object[] datas)
        {
        }
    }
}
