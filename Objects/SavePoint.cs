using da.Scripts.Objects;
using Godot;

public partial class SavePoint : InteractableArea
{
    [Export] AnimationPlayer animationPlayer;
    private bool _active = false;
    public bool Active
    {
        get => _active;
        set
        {
            _active = value;
            if (value)
            {
                animationPlayer?.Play("actived");
            }
            else
            {
                animationPlayer?.Play("unactived");
            }
        }
    }
    public override void Interact()
    {
        base.Interact();

        if (Active)
        {
            GameGlobal.Instance.SaveGame();
        }
        else
        {
            Active = true;
            if (!GameGlobal.Instance.save.ActivedSavePoints.ContainsKey(Name)) GameGlobal.Instance.save.ActivedSavePoints.Add(Name, GameGlobal.Instance.root.WorldControl.GetChild(0).Name);
            GameGlobal.Instance.SaveGame();
        }
    }
}
