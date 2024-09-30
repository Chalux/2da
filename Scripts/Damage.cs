using da.Scripts.Objects;
using Godot;

namespace da.Scripts
{
    public partial class Damage : RefCounted
    {
        public int value;
        public HitBox source;
        public HurtBox target;
        public bool repel;
    }
}
