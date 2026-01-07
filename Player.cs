using Godot;

namespace SquashTheCreeps;

public partial class Player : CharacterBody3D
{
    /// <summary>
    /// Player ground movement speed
    /// </summary>
    [Export(PropertyHint.None, "suffix:m/s")]
    public int Speed { get; set; } = 14;
    
    /// <summary>
    /// Downward acceleration when in the air
    /// </summary>
    [Export(PropertyHint.None, "suffix:m/s\u00b2")]
    public int FallAcceleration { get; set; } = 75;
    
    /// <summary>
    /// Vertical velocity applied when jumping
    /// </summary>
    [Export(PropertyHint.None, "suffix:m/s")]
    public int JumpImpulse { get; set; } = 20;
    
    /// <summary>
    /// Vertical velocity applied when bouncing off a mob
    /// </summary>
    [Export(PropertyHint.None, "suffix:m/s")]
    public int BounceImpulse { get; set; } = 16;
    
    [Signal]
    public delegate void HitEventHandler();

    private Vector3 _targetVelocity = Vector3.Zero;
    
    public override void _PhysicsProcess(double delta)
    {
        var direction = Vector3.Zero;

        if (Input.IsActionPressed("move_right"))
        {
            direction += Vector3.Right;
        }
        if (Input.IsActionPressed("move_left"))
        {
            direction += Vector3.Left;
        }
        if (Input.IsActionPressed("move_back"))
        {
            direction += Vector3.Back;
        }
        if (Input.IsActionPressed("move_forward"))
        {
            direction += Vector3.Forward;
        }
        
        if (direction != Vector3.Zero)
        {
            direction = direction.Normalized();
            GetNode<Node3D>("Pivot").Basis = Basis.LookingAt(direction);
        }
        
        _targetVelocity.X = direction.X * Speed;
        _targetVelocity.Z = direction.Z * Speed;

        if (!IsOnFloor())
        {
            _targetVelocity.Y -= FallAcceleration * (float)delta;
        }
        else if (IsOnFloor() && Input.IsActionJustPressed("jump"))
        {
            _targetVelocity.Y = JumpImpulse;
        }

        Velocity = _targetVelocity;
        MoveAndSlide();
        
        for (var index = 0; index < GetSlideCollisionCount(); index++)
        {
            KinematicCollision3D collision = GetSlideCollision(index);

            if (collision.GetCollider() is Mob mob)
            {
                if (Vector3.Up.Dot(collision.GetNormal()) > 0.1f)
                {
                    mob.Squash();
                    _targetVelocity.Y = BounceImpulse;
                    // Prevent further duplicate calls.
                    break;
                }
            }
        }
    }
    
    private void Die()
    {
        EmitSignal(SignalName.Hit);
        QueueFree();
    }

    private void OnMobDetectorBodyEntered(Node3D body)
    {
        Die();
    }
}