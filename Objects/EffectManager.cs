using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
