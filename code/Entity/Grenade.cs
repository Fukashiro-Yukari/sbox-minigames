using Sandbox;
using System.Threading.Tasks;

public partial class Grenade : Prop
{
	public virtual string ModelPath => "";

	public float Speed;
	public float ExplosionDelay;
	public string BounceSound;

	public override void Spawn()
	{
		base.Spawn();

		MoveType = MoveType.Physics;
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

		_ = DelayedExplode();
	}

	async Task DelayedExplode()
	{
		await GameTask.DelaySeconds( ExplosionDelay );
		Explode();
	}

	public virtual void Explode()
	{
		Delete();
	}

	protected override void OnPhysicsCollision( CollisionEventData eventData )
	{
		if ( eventData.Entity is not Player && eventData.Speed > 50 && !string.IsNullOrEmpty( BounceSound ) )
			PlaySound( BounceSound );
	}
}
