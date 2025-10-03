using Godot;

public partial class Player : CharacterBody3D
{
	[Export] 
	public int Speed { get; set; } = 14;
	[Export] 
	public int FallAcceleration { get; set; } = 75;
	[Export]
	public int JumpImpulse { get; set; } = 20;
	[Export]
	public int BounceImpulse { get; set; } = 16;
	
	[Signal]
	public delegate void HitEventHandler();
	
	private Vector3 _targetVelocity = Vector3.Zero;

	public override void _PhysicsProcess(double delta)
	{
		var direction = Vector3.Zero;

		if (Input.IsActionPressed("move_right"))
		{
			direction.X += 1.0f;
		}
		
		if (Input.IsActionPressed("move_left"))
		{
			direction.X -= 1.0f;
		}
		
		if (Input.IsActionPressed("move_forward"))
		{
			direction.Z -= 1.0f;
		}
		
		if (Input.IsActionPressed("move_back"))
		{
			direction.Z += 1.0f;
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

		if (IsOnFloor() && Input.IsActionPressed("jump"))
		{
			_targetVelocity.Y = JumpImpulse;
		}

		for (var i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision = GetSlideCollision(i);

			if (collision.GetCollider() is not Mob mob) 
				continue;
			
			if (!(Vector3.Up.Dot(collision.GetNormal()) > 0.1f)) 
				continue;
			
			mob.Squash();
			
			_targetVelocity.Y = BounceImpulse;

			break;
		}
		
		Velocity = _targetVelocity;
		MoveAndSlide();
	}

	private void OnMobDetectorBodyEntered()
	{
		GD.Print("Trigger");
		Die();
	}

	private void Die()
	{
		EmitSignal(SignalName.Hit);
		QueueFree();
	}
}
