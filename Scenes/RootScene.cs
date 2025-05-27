using da.Scripts;
using da.Scripts.Objects;
using da.Scripts.Objects.PlayerScript;
using Godot;

namespace da.Scenes
{
    public partial class RootScene : Node2D, IEvent
    {
        [Export] public Control WorldControl;

        public override void _Ready()
        {
            EventMgr.RegisterEvent(this);

            EventMgr.DispatchEvent("RootReady", this);
        }

        public void ReceiveEvent(string eventName, params object[] datas)
        {
        }

        public static void TeleportPlayer(Vector2 position, int Direction)
        {
            var camera = GameGlobal.Instance.camera;
            var player = GameGlobal.Instance.player;
            player.GlobalPosition = position;
            player.Direction = Direction;
            player.StateMachine.ChangeState(PlayerState.Idle);
            camera.ResetSmoothing();
            camera.ForceUpdateScroll();
        }
    }
}
