using da.Scripts.Objects;
using Godot;
using Godot.Collections;

namespace da.Objects
{
    public partial class Door : Control
    {
        [Export] public Sprite2D sprite;
        [Export] public StaticBody2D Collision;
        [Export] public AudioStreamPlayer2D openDoorAudio;
        [Export] public AudioStreamPlayer2D closeDoorAudio;
        private bool _isOpened = false;
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
        }
    }
}
