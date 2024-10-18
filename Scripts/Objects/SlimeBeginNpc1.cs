using DialogicRuntime;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts.Objects
{
    public partial class SlimeBeginNpc1 : InteractableArea
    {
        [Export] public Marker2D SlimeBubbleMarker;
        public override void Interact()
        {
            int slimeSkillCount = GameGlobal.Instance.save.GlobalSaveData.GetValueOrDefault("enemy_kill_count", 0).AsInt32();
            Dialogic.Instance.Get("VAR").AsGodotObject().Set("BeginSlimeKilled", slimeSkillCount);
            Node layout = Dialogic.Start("BeginSlime1");
            //Resource character1 = ResourceLoader.Load("res://Dialogic/Characters/Owens.dch");
            //Resource character2 = ResourceLoader.Load("res://Dialogic/Characters/SlimeNpc.dch");
            //layout.Call("register_character", character1, GameGlobal.Instance.player.BubbleMarker);
            //layout.Call("register_character", character2, SlimeBubbleMarker);
        }
    }
}
