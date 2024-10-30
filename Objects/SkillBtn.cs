using da.Scripts;
using da.Scripts.Objects;
using Godot;

public partial class SkillBtn : Control
{
    [Export] ColorRect Background;
    //[Export] TextureRect Icon;
    [Export] Label TimeoutLabel;
    [Export] TextureProgressBar Prog;
    [Export] public TextureButton Button;
    public Skill currSkill;

    public override void _Ready()
    {
        TimeoutLabel.Text = "";
        Button.Pressed += OnPressed;
    }

    public void OnPressed()
    {
        if (currSkill != null && GameGlobal.Instance.player != null)
        {
            currSkill.CheckIsReady();
            if (currSkill.IsReady)
            {
                GameGlobal.Instance.player.currSkill = currSkill;
                GameGlobal.Instance.player.StateMachine.ChangeState(da.Scripts.Objects.PlayerScript.PlayerState.Skill);
            }
        }
    }

    public override void _Process(double delta)
    {
        if (currSkill != null)
        {
            if (currSkill.CurrentCooldown > 0)
            {
                TimeoutLabel.Text = $"{currSkill.CurrentCooldown:F1}";
                Prog.RadialFillDegrees = currSkill.CurrentCooldown / currSkill.Cooldown * 360;
            }
            else
            {
                TimeoutLabel.Text = "";
                Prog.RadialFillDegrees = 0;
            }
        }
    }

    public void SetSkill(Skill skill)
    {
        currSkill = skill;
        if (skill != null)
        {
            Button.TextureNormal = ResourceLoader.Load<CompressedTexture2D>(currSkill.IconPath);
        }
        else
        {
            Button.TextureNormal = null;
            TimeoutLabel.Text = "";
            Prog.RadialFillDegrees = 360;
        }
        _Process(0);
    }
}
