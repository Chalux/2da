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
        /// <summary>
        /// 特殊操作，详细看代码
        /// </summary>
        [Export] uint specOp = 0;

        public override void Interact()
        {
            CrateOpenAudioPlayer.Play();
            if (IsOpen)
            {
                if (ShowMsg && OpenedMsg != "" && OpenedMsg != null) GameGlobal.Instance.AddMsg(OpenedMsg);
            }
            else
            {
                if (ShowMsg && NotOpenMsg != "" && NotOpenMsg != null) GameGlobal.Instance.AddMsg(NotOpenMsg);
                if (specOp == 0)
                {
                    if (GameGlobal.Instance.player?.AddItem(ItemID, ItemAmount) ?? false)
                    {
                        IsOpen = true;
                        GameGlobal.Instance.save.CrateOpened.Add(Name);
                    }
                }
                else
                {
                    switch (specOp)
                    {
                        case 1:
                            if (GameGlobal.Instance.player == null) return;
                            GameGlobal.Instance.player.status.MaxHealth += 1;
                            GameGlobal.Instance.save.CrateOpened.Add(Name);
                            IsOpen = true;
                            break;
                    }
                }
            }
        }
    }
}
