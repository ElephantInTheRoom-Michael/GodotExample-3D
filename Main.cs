using Godot;

namespace SquashTheCreeps;

public partial class Main : Node
{
    [Export]
    public PackedScene MobScene { get; set; }
    
    private void OnMobTimerTimeout()
    {
        var mob = MobScene.Instantiate<Mob>();

        var mobSpawnLocation = GetNode<PathFollow3D>("SpawnPath/SpawnLocation");
        mobSpawnLocation.ProgressRatio = GD.Randf();

        var playerPosition = GetNode<Player>("Player").Position;
        
        mob.Initialize(mobSpawnLocation.Position, playerPosition);
        AddChild(mob);
    }
}