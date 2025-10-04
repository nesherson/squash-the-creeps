using Godot;

public partial class Main : Node
{ 
    [Export]
    public PackedScene MobScene { get; set; }

    public void OnMobTimerTimeout()
    {
        var mob = MobScene.Instantiate<Mob>();
        var mobSpawnLocation = GetNode<PathFollow3D>("SpawnPath/SpawnLocation");
        var playerPosition = GetNode<Player>("Player").Position;

        mobSpawnLocation.ProgressRatio = GD.Randf();
        
        mob.Initialize(mobSpawnLocation.Position, playerPosition);
        AddChild(mob);
    }

    public void OnPlayerHit()
    {
        GetNode<Timer>("MobTimer").Stop();
    }
}
