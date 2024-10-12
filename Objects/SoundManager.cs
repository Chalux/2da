using Godot;
using Godot.Collections;
using System.Linq;

public partial class SoundManager : Node
{
    public static SoundManager Ins { get; private set; }
    [Export] Node SFX;
    [Export] AudioStreamPlayer BGM;
    private Dictionary<string, AudioStreamPlayer> SFXs = new();
    public enum AudioBusEnum
    {
        Master = 0,
        SFX = 1,
        BGM = 2,
    }
    public override void _Ready()
    {
        Ins = this;
        foreach (AudioStreamPlayer sfx in SFX.GetChildren().Cast<AudioStreamPlayer>())
        {
            if (!SFXs.ContainsKey(sfx.Name)) SFXs.Add(sfx.Name, sfx);
        }
    }

    public void PlaySFX(string name)
    {
        if (SFXs.ContainsKey(name))
        {
            SFXs[name].Play();
        }
    }

    public static float GetVolume(int BusIndex)
    {
        return Mathf.DbToLinear(AudioServer.GetBusVolumeDb(BusIndex));
    }

    public static void SetVolume(int BusIndex, float Volume)
    {
        var volume = Mathf.LinearToDb(Volume);
        AudioServer.SetBusVolumeDb(BusIndex, volume);
    }

    public void PlayBGM(AudioStream stream)
    {
        if (BGM.Stream == stream && BGM.Playing) return;
        BGM.Stream = stream;
        BGM.Play();
    }

    public static void PlaySFXByStream(AudioStream stream)
    {
        var sfx = new AudioStreamPlayer
        {
            Stream = stream
        };
        Ins.AddChild(sfx);
        sfx.Play();
        sfx.Finished += sfx.QueueFree;
    }
}
