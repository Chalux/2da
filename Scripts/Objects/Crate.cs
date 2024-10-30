using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace da.Scripts.Objects
{
    public partial class Crate : InteractableArea
    {
        [Export] AudioStreamPlayer2D CrateOpenAudioPlayer;
        public bool isOpen = false;
        [Export] string OpenedMsg;
        [Export] string NotOpenMsg;
        [Export] bool ShowMsg;
        [Export] int ItemID;
        [Export] int ItemAmount;

        public override void Interact()
        {
            CrateOpenAudioPlayer.Play();
            if (isOpen)
            {
                if (ShowMsg) GameGlobal.Instance.AddMsg(OpenedMsg);
            }
            else
            {
                if (ShowMsg) GameGlobal.Instance.AddMsg(NotOpenMsg);
                if (GameGlobal.Instance.player?.AddItem(ItemID, ItemAmount) ?? false)
                {
                    isOpen = true;
                    GameGlobal.Instance.save.CrateOpened.Add(Name);
                }
            }
        }
    }
}
