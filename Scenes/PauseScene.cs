using da.Objects;
using da.Scripts;
using da.Scripts.Objects;
using Godot;
using System;
using System.Linq;

public partial class PauseScene : Control, IEvent
{
    [Export] Button ResumeBtn;
    [Export] Button TitleBtn;
    [Export] TabBar SkillTabbar;
    [Export] ItemList SkillList;
    [Export] GridContainer BagGrid;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Hide();
        ResumeBtn.Pressed += OnResumeBtnPressed;
        TitleBtn.Pressed += OnTitleBtnPressed;
        VisibilityChanged += OnVisibilityChanged;
        SkillList.ItemSelected += OnItemSelected;
        Hidden += OnHide;
        EventMgr.RegisterEvent(this);
    }

    public override void _ExitTree()
    {
        EventMgr.UnRegisterEvent(this);
    }

    private void OnHide()
    {
        SkillList.Clear();
        GameGlobal.Instance.UIControl.Show();
    }

    private void OnItemSelected(long index)
    {
        var skillname = SkillList.GetItemText((int)index);
        if (GameGlobal.Instance.SkillSlot.IndexOf(skillname) == SkillTabbar.CurrentTab)
        {
            GameGlobal.Instance.SetSkill(SkillTabbar.CurrentTab, null);
            return;
        }
        if (GameGlobal.Instance.SkillSlot.IndexOf(skillname) > -1)
        {
            GameGlobal.Instance.SkillSlot[GameGlobal.Instance.SkillSlot.IndexOf(skillname)] = null;
        }
        var slotid = SkillTabbar.CurrentTab;
        GameGlobal.Instance.SetSkill(slotid, skillname);
    }

    private void OnVisibilityChanged()
    {
        GetTree().Paused = Visible;
    }

    private void OnTitleBtnPressed()
    {
        _ = GameGlobal.Instance.ChangeSceneAsync("res://Scenes/TitleScene.tscn");
    }

    private void OnResumeBtnPressed()
    {
        Hide();
    }

    public void ShowPause()
    {
        Show();
        if (GameGlobal.Instance.player != null)
        {
            UpdateSkill();
            UpdateBag();
            ResumeBtn.GrabFocus();
            GameGlobal.Instance.UIControl.Hide();
        }
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("pause"))
        {
            Hide();
            GetWindow().SetInputAsHandled();
        }
    }

    private void UpdateBag()
    {
        var c = GameGlobal.Instance.player.Bag.Count;
        if (BagGrid.GetChildCount() < c)
        {
            var res = ResourceLoader.Load<PackedScene>("res://Objects/ItemObj.tscn");
            if (res != null)
            {
                for (int i = BagGrid.GetChildCount(); i < c; i++)
                {
                    BagGrid.AddChild(res.Instantiate());
                }
            }
        }
        foreach (ItemObj itemObj in BagGrid.GetChildren().Cast<ItemObj>())
        {
            if (itemObj.GetIndex() < c)
            {
                itemObj.SetData(GameGlobal.Instance.player.Bag[itemObj.GetIndex()]);
            }
            else
            {
                itemObj.SetData(null);
            }
        }
    }

    public void ReceiveEvent(string eventName, params object[] datas)
    {
        if (eventName == "BagUpdate")
        {
            UpdateBag();
        }
        else if (eventName == "SkillUpdate")
        {
            UpdateSkill();
        }
    }

    private void UpdateSkill()
    {
        SkillList.Clear();
        if (GameGlobal.Instance.player != null)
        {
            foreach (var kvpair in SkillManager.Instance.SkillDict)
            {
                bool isLearned = GameGlobal.Instance.player.LearnedSkill.Contains(kvpair.Key);
                var str = kvpair.Key + (isLearned ? "" : "(未习得)");
                var idx = SkillList.AddItem(str, ResourceLoader.Load<CompressedTexture2D>(kvpair.Value.IconPath)
                    , isLearned);
                if (GameGlobal.Instance.SkillSlot.IndexOf(kvpair.Key) == SkillTabbar.CurrentTab)
                {
                    SkillList.Select(idx);
                }
            }
        }
    }
}
