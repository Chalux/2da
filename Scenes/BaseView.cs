using da.Scripts;
using da.Scripts.Objects;
using Godot;

namespace da.Scenes;

public partial class BaseView : Control
{
    public override void _Ready()
    {
        base._Ready();
        if (this is IEvent eventView)
        {
            EventMgr.RegisterEvent(eventView);
        }

        OnAdd();
        DoShow();
    }

    public override void _ExitTree()
    {
        if (this is IEvent eventView)
        {
            EventMgr.UnRegisterEvent(eventView);
        }

        DoClose();
        base._ExitTree();
    }

    protected virtual void DoShow()
    {
        Visible = true;
        ProcessMode = ProcessModeEnum.Inherit;
    }

    protected virtual void OnAdd()
    {
    }

    protected virtual void DoClose()
    {
    }

    protected virtual void DoHide()
    {
        Visible = false;
        ProcessMode = ProcessModeEnum.Disabled;
    }
}