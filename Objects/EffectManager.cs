using Godot;

namespace da.Objects
{
    public partial class EffectManager : Node
    {
        public static EffectManager Ins { get; private set; }
        public override void _Ready()
        {
            Ins = this;
        }
    }
}
