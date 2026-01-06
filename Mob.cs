using Godot;

namespace SquashTheCreeps;

public partial class Mob : CharacterBody3D
{
    [Export(PropertyHint.None, "suffix:m/s")]
    public int MinSpeed { get; set; } = 10;
    
    [Export(PropertyHint.None, "suffix:m/s")]
    public int MaxSpeed { get; set; } = 18;
    
    [Signal]
    public delegate void SquashedEventHandler();

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
    }
    
    public void Initialize(Vector3 startPosition, Vector3 playerPosition)
    {
        // We position the mob by placing it at startPosition
        // and rotate it towards playerPosition, so it looks at the player.
        LookAtFromPosition(startPosition, playerPosition, Vector3.Up);
        // Rotate this mob randomly within range of -45 and +45 degrees,
        // so that it doesn't move directly towards the player.
        RotateY(GD.RandRange(-Mathf.Pi / 4.0, Mathf.Pi / 4.0));
        
        var randomSpeed = GD.RandRange(MinSpeed, MaxSpeed);
        Velocity = Vector3.Forward * randomSpeed;
        Velocity = Velocity.Rotated(Vector3.Up, Rotation.Y);
    }
    
    public void Squash()
    {
        EmitSignal(SignalName.Squashed);
        QueueFree();
    }
    
    private void OnScreenExited()
    {
        QueueFree();
    }
}