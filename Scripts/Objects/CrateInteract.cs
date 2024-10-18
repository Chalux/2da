
using da.Scripts.Objects;
using Godot;

public partial class CrateInteract : InteractableArea
{
    [Export] AudioStreamPlayer2D CrateOpenAudioPlayer;
    //[Export] AudioStreamPlayer2D CrateCloseAudioPlayer;

    public override void Interact()
    {
        CrateOpenAudioPlayer.Play();
        GameGlobal.Instance.AddMsg($"家里的老木箱，貌似什么东西都没有");
    }
}
