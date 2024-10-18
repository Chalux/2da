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
        public AudioStream onHitSound;
        /// <summary>
        /// 是否顿帧
        /// </summary>
        public bool isStunFrame;
        /// <summary>
        /// 顿帧持续时间
        /// </summary>
        public float stunDuration;
        /// <summary>
        /// 顿帧强度，timescale的数值，最好是0.01-1，越低越强
        /// </summary>
        public float stunPower;
    }
}
