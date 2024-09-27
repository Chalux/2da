using da.Scripts.Objects;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
