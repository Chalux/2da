using da.Scripts.Interfaces;
using Godot;

namespace da.Scripts.Objects.Others
{
    public partial class Oprated : Node, IOpenable
    {
        [Export] AnimationPlayer animationPlayer;
        public override void _Ready()
        {
            base._Ready();
            Utils.LoadIOpenableNode(this);
        }
        public void OnClose()
        {
            animationPlayer.Play("close");
            Utils.SaveToMapData(this, false);
        }

        public void OnOpen()
        {
            animationPlayer.Play("open");
            Utils.SaveToMapData(this, true);
        }

        public void SetToClosedState()
        {
            animationPlayer.Play("set_to_close");
        }

        public void SetToOpenedState()
        {
            animationPlayer.Play("set_to_open");
        }
    }
}
