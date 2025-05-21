using da.Scripts.Interfaces;
using Godot;

namespace da.Scripts.Objects
{
    /// <summary>
    /// 一次性按钮
    /// </summary>
    public partial class OnetimeBtn : Area2D, IOpenable
    {
        [Export] public Node[] linkedObj;
        [Export] public Texture2D closedTexture;
        [Export] public Texture2D openedTexture;
        [Export] public Sprite2D sprite;
        [Export] public CollisionShape2D collision;
        public bool isActived = false;
        public override void _Ready()
        {
            base._Ready();
            if (Utils.CheckSaved(this))
            {
                SetToOpenedState();
            }
            else
            {
                SetToClosedState();
            }
        }

        public void OnClose()
        {
            if (sprite != null) sprite.Texture = closedTexture;
            collision.Disabled = true;
            isActived = false;
            Utils.SaveToMapData(this, false);
            foreach (Node node in linkedObj)
            {
                if (node is IOpenable openable)
                {
                    openable.OnClose();
                }
            }
        }

        public void OnOpen()
        {
            if (sprite != null) sprite.Texture = openedTexture;
            collision.Disabled = false;
            isActived = true;
            Utils.SaveToMapData(this, true);
            foreach (Node node in linkedObj)
            {
                if (node is IOpenable openable)
                {
                    openable.OnOpen();
                }
            }
        }

        public void SetToClosedState()
        {
            if (sprite != null) sprite.Texture = closedTexture;
            collision.Disabled = false;
            isActived = false;
            foreach (Node node in linkedObj)
            {
                if (node is IOpenable openable)
                {
                    openable.SetToClosedState();
                }
            }
        }

        public void SetToOpenedState()
        {
            if (sprite != null) sprite.Texture = openedTexture;
            collision.Disabled = true;
            isActived = true;
            foreach (Node node in linkedObj)
            {
                if (node is IOpenable openable)
                {
                    openable.SetToOpenedState();
                }
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);
            if (!isActived && HasOverlappingBodies())
            {
                OnOpen();
            }
        }
    }
}
