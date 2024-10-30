using da.Scripts;
using da.Scripts.Objects;
using Godot;
using System;

public partial class ItemObj : Button
{
    Item data = null;
    [Export] TextureRect ItemIcon;
    [Export] PopupMenu PopupMenu;
    [Export] Label StackCountLabel;
    public void SetData(Item data)
    {
        this.data = data;
        if (data != null)
        {
            ItemIcon.Texture = ResourceLoader.Load<CompressedTexture2D>(data.IconPath);
            if (data.StackCount > 1)
            {
                StackCountLabel.Text = data.StackCount.ToString();
            }
            else
            {
                StackCountLabel.Text = "";
            }
        }
        else
        {
            ItemIcon.Texture = null;
            StackCountLabel.Text = "";
        }
    }
    public override void _Ready()
    {
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
        ButtonDown += OnPressed;
        PopupMenu.IndexPressed += OnPopupMenuIndexPressed;
    }

    private void OnPressed()
    {
        PopupMenu.Position = (Vector2I)GetWindow().GetMousePosition() + new Vector2I(10, 10);
        PopupMenu.Visible = !PopupMenu.Visible;
    }

    private void OnPopupMenuIndexPressed(long index)
    {
        switch (index)
        {
            case 0:
                data?.Use();
                break;
        }
    }

    private void OnMouseExited()
    {
        GameGlobal.Instance.HideRichHint();
    }

    private void OnMouseEntered()
    {
        if (data != null)
        {
            string desc = $"{data.name}\n{data.description}";
            GameGlobal.Instance.ShowRichHint(desc);
        }
    }
}
