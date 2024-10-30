using da.Scripts.Objects;
using Godot;

public partial class VolumeSlider : HSlider
{
    [Export] public StringName Bus = "Master";
    private int BusIndex;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BusIndex = AudioServer.GetBusIndex(Bus);
        Value = SoundManager.GetVolume(BusIndex);
        ValueChanged += OnValueChanged;

        AudioStream res = ResourceLoader.Load<AudioStream>("res://Resources/Music/Goblins_Den_(Regular).wav");
        if (res != null)
            SoundManager.Ins.PlayBGM(res);
    }

    private void OnValueChanged(double value)
    {
        SoundManager.SetVolume(BusIndex, (float)value);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        GameGlobal.SaveConfig();
    }
}
