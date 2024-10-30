
using da.Scripts.Objects;
using Godot;

public partial class CrateInteract : InteractableArea
{
    [Export] AudioStreamPlayer2D CrateOpenAudioPlayer;
    //[Export] AudioStreamPlayer2D CrateCloseAudioPlayer;
    public bool isOpen = false;

    public override void Interact()
    {
        CrateOpenAudioPlayer.Play();
        if (isOpen)
        {
            GameGlobal.Instance.AddMsg($"家里的老木箱，貌似什么东西都没有了");
        }
        else
        {
            GameGlobal.Instance.AddMsg($"家里的老木箱，里面似乎有一本书");
            if (GameGlobal.Instance.player?.AddItem(1, 1) ?? false)
            {
                isOpen = true;
                GameGlobal.Instance.save.CrateOpened.Add(Name);
            }
        }
    }
}
