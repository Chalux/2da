using Godot;

namespace da.Scripts.Objects
{
    public partial class Crate : InteractableArea
    {
        [Export] AudioStreamPlayer2D CrateOpenAudioPlayer;
        public bool _isOpen = false;
        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                _isOpen = value;
                if (value)
                {
                    CloseSprite.Visible = false;
                    OpenedSprite.Visible = true;
                }
                else
                {
                    CloseSprite.Visible = true;
                    OpenedSprite.Visible = false;
                }
            }
        }
        [Export] string OpenedMsg;
        [Export] string NotOpenMsg;
        [Export] bool ShowMsg;
        [Export] int ItemID;
        [Export] int ItemAmount;
        [Export] Sprite2D CloseSprite;
        [Export] Sprite2D OpenedSprite;

        public override void Interact()
        {
            CrateOpenAudioPlayer.Play();
            if (IsOpen)
            {
                if (ShowMsg) GameGlobal.Instance.AddMsg(OpenedMsg);
            }
            else
            {
                if (ShowMsg) GameGlobal.Instance.AddMsg(NotOpenMsg);
                if (GameGlobal.Instance.player?.AddItem(ItemID, ItemAmount) ?? false)
                {
                    IsOpen = true;
                    GameGlobal.Instance.save.CrateOpened.Add(Name);
                }
            }
        }
    }
}
