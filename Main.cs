using Godot;

namespace SquashTheCreeps;

public partial class Main : Node
{
    [Export]
    public PackedScene MobScene { get; set; }

    public override void _Ready()
    {
        base._Ready();
        
        GetNode<Control>("UserInterface/Retry").Hide();
    }
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_accept") && GetNode<Control>("UserInterface/Retry").Visible)
        {
            GetTree().ReloadCurrentScene();
        }
    }


    private void OnMobTimerTimeout()
    {
        var mob = MobScene.Instantiate<Mob>();

        var mobSpawnLocation = GetNode<PathFollow3D>("SpawnPath/SpawnLocation");
        mobSpawnLocation.ProgressRatio = GD.Randf();

        var playerPosition = GetNode<Player>("Player").Position;
        
        mob.Initialize(mobSpawnLocation.Position, playerPosition);
        AddChild(mob);
        
        mob.Squashed += GetNode<ScoreLabel>("UserInterface/ScoreLabel").OnMobSquashed;
    }
    
    private void OnPlayerHit()
    {
        GetNode<Timer>("MobTimer").Stop();
        
        GetNode<Control>("UserInterface/Retry").Show();
    }
}