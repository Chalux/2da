using da.Scripts.Objects;
using Godot;
using System.Collections.Generic;

public partial class BreakableWall : StaticBody2D
{
    [Export] private Area2D Area2D;
    [Export] private GpuParticles2D gpuParticles;
    private int _HitCount = 0;
    public int HitCount
    {
        get => _HitCount;
        set
        {
            _HitCount = value;
            if (_HitCount >= 3)
            {
                Node map = FindParent("*Map*");
                if (map != null)
                {
                    var savedata = GameGlobal.Instance.save.MapSaveData;
                    if (savedata.ContainsKey(map.Name))
                    {
                        savedata[map.Name].TryAdd(Name, true);
                    }
                }
                QueueFree();
            }
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Node map = FindParent("*Map*");
        if (map != null)
        {
            var savedata = GameGlobal.Instance.save.MapSaveData;
            if (savedata.TryGetValue(map.Name, out var value))
            {
                if (value.ContainsKey(Name))
                {
                    QueueFree();
                    return;
                }
            }
        }
        Area2D.AreaEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        gpuParticles.Emitting = true;
        SoundManager.Ins.PlaySFX("HitWall");
        HitCount += 1;
    }
}
