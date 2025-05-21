using da.Scripts.Objects;
using DialogicRuntime;
using Godot;
using System.Collections.Generic;

public partial class SpeakPoint : Area2D
{
    [Export] CollisionShape2D collision;
    [Export] public string SpeakResource = "";
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (GameGlobal.Instance.save.GlobalSaveData.ContainsKey(Name))
        {
            QueueFree();
        }
        else
        {
            BodyEntered += OnBodyEntered;
        }
    }

    /// <summary>
    /// 如果没有别的逻辑的话 不需要重写这个方法，否则必须要调用父类的此方法
    /// </summary>
    /// <param name="body"></param>
    public virtual void OnBodyEntered(Node2D body)
    {
        Dialogic.Start(SpeakResource);
        if (!GameGlobal.Instance.save.GlobalSaveData.TryAdd(Name, true))
        {
            GameGlobal.Instance.save.GlobalSaveData[Name] = true;
        }
        QueueFree();
    }
}
