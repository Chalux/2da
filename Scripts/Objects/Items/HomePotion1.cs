namespace da.Scripts.Objects.Items
{
    public partial class HomePotion1 : InteractableArea
    {
        public override void _Ready()
        {
            Utils.CheckSaveAndFree(this);
            base._Ready();
        }

        public override void Interact()
        {
            if (GameGlobal.Instance.player != null)
            {
                GameGlobal.Instance.player.AddItem(2, 5);
                Utils.SaveToMapData(this);
                GameGlobal.Instance.AddMsg("捡到了5瓶药水");
                QueueFree();
            }
        }
    }
}
