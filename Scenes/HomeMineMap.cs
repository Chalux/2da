using da.Scripts;

public partial class HomeMineMap : Map
{
    public override void _Ready()
    {
        base._Ready();
        player.Light.Enabled = true;
    }
}
