using da.Objects;
using da.Scripts.Objects;
using Godot;

public partial class InfHealthPotion : InteractableArea
{
    [Export] Timer timer;
    private bool _canDrink = true;
    public bool CanDrink
    {
        get => _canDrink;
        set
        {
            _canDrink = value;
            if (value)
            {
                (GetParent() as AnimatedSprite2D).PlayBackwards();
            }
            else
            {
                (GetParent() as AnimatedSprite2D).Play();
            }
            //GD.Print($"CanDrink: {value}");
        }
    }

    public override void _Ready()
    {
        base._Ready();
        timer.Timeout += () => CanDrink = true;
        CanDrink = true;
    }

    public override void Interact()
    {
        if (CanDrink)
        {
            CanDrink = false;
            Player player = GameGlobal.Instance.player;
            player.status.Health = player.status.MaxHealth;
            timer.Start();
            GameGlobal.Instance.AddMsg($"喝下了神秘的无限药水，生命回满");
        }
    }
}
