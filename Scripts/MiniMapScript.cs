using da.Scripts;
using da.Scripts.Objects;
using Godot;

public partial class MiniMapScript : TextureRect
{
    private MiniMapClass miniMap = new();
    [Export] public int ZoomScale = 8;
    [Export] bool CanZoom = false;
    [Export] Vector2 BaseSize = new(165, 70);
    [Export] Vector2 BaseScale = new(1, 1);
    private int count = 0;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        UpdateMiniMapSize();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        count = (count + 1) % ZoomScale;
        if (count != 0 && CanZoom) return;
        if (GameGlobal.Instance.player == null)
        {
            Visible = false;
            return;
        }
        var maps = GetTree().GetNodesInGroup("maps");
        if (maps.Count > 0 && maps[0] is Map map)
        {
            Visible = true;
            if (map.mainTileMap != null)
            {
                miniMap.DrawMiniMap((Vector2I)GameGlobal.Instance.player.Position, map.mainTileMap, ZoomScale);
            }
        }
        else
        {
            Visible = false;
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (!CanZoom) return;
        if (@event.IsActionPressed("wheel_up"))
        {
            ZoomScale = Mathf.Clamp(ZoomScale / 2, 1, 32);
        }
        else if (@event.IsActionPressed("wheel_down"))
        {
            ZoomScale = Mathf.Clamp(ZoomScale * 2, 1, 32);
        }
        UpdateMiniMapSize();
        count = -1;
        _Process(0);
        GetViewport().SetInputAsHandled();
    }

    private void UpdateMiniMapSize()
    {
        miniMap.UpdateSize((Vector2I)Size);
        miniMap.LinkTexture(this);
        //Size = BaseSize * ZoomScale;
        Size = BaseSize * ZoomScale;
        Scale = BaseScale / ZoomScale;
    }
}
