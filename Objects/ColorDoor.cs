using da.Scripts;
using da.Scripts.Interfaces;
using da.Scripts.Objects;
using Godot;
using Godot.Collections;

public partial class ColorDoor : Control, IOpenable
{
    [Export] public Sprite2D sprite;
    [Export] public StaticBody2D Collision;
    [Export] public AudioStreamPlayer2D openDoorAudio;
    [Export] public AudioStreamPlayer2D closeDoorAudio;
    private bool _isOpened = false;
    [Export] public int UnlockKeyItemId = 0;
    public bool IsOpened
    {
        get { return _isOpened; }
        set
        {
            _isOpened = value;
            if (_isOpened)
            {
                sprite.RegionRect = new(192, 160, 16, 32);
                Collision.SetCollisionLayerValue(1, false);
                openDoorAudio.Play();
            }
            else
            {
                sprite.RegionRect = new Rect2(208, 160, 16, 32);
                Collision.SetCollisionLayerValue(1, true);
                closeDoorAudio.Play();
            }
        }
    }

    public void Open()
    {
        IsOpened = !IsOpened;
        var mapsavedata = GameGlobal.Instance.save.MapSaveData;
        var parent = FindParent("*Map*") ?? FindParent("*map*");
        if (parent == null)
            return;
        string mapName = parent.Name;
        if (!mapsavedata.ContainsKey(mapName))
        {
            mapsavedata.Add(mapName, new Dictionary<string, Variant>());
        }
        if (!mapsavedata[mapName].ContainsKey(Name))
        {
            mapsavedata[mapName].Add(Name, IsOpened);
        }
        else
        {
            mapsavedata[mapName][Name] = IsOpened;
        }
    }

    public override void _Ready()
    {
        SetMeta("CantSlide", true);
        Utils.LoadIOpenableNode(this);
    }

    public void SetToOpenedState()
    {
        sprite.RegionRect = new(192, 160, 16, 32);
        Collision.SetCollisionLayerValue(1, false);
        _isOpened = true;
    }

    public void SetToClosedState()
    {
        sprite.RegionRect = new Rect2(208, 160, 16, 32);
        Collision.SetCollisionLayerValue(1, true);
        _isOpened = false;
    }

    public void OnOpen()
    {
        Open();
    }

    public void OnClose()
    {
        Open();
    }
}
