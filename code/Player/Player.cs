using Sandbox;

partial class MiniGamesPlayer : Player
{
	private TimeSince timeSinceDropped;
	private TimeSince timeSinceFall;
	private TimeSince timeSinceDied;
	private TimeSince timeSinceTempGod;
	private TimeSince timeSinceLightToggled;

	private DamageInfo lastDamage;
	
	[Net] public BaseTeam Team { get; set; }
	[Net] public PawnController VehicleController { get; set; }
	[Net] public PawnAnimator VehicleAnimator { get; set; }
	[Net, Predicted] public ICamera VehicleCamera { get; set; }
	[Net, Predicted] public Entity Vehicle { get; set; }
	[Net, Predicted] public ICamera MainCamera { get; set; }

	public ICamera LastCamera { get; set; }
	public Clothing.Container Clothing = new();
	public bool SupressPickupNotices { get; private set; }
	public bool InitSpawn { get; set; }
	public bool IsFreeze { get; private set; }
	public bool IsHeadShot { get; private set; }

	[Net, Predicted]
	private SpotLightEntity FlashlightEntity { get; set; }

	[Net]
	private float FlashlightPosOffset { get; set; }

	[Net, Local, Predicted]
	private bool LightEnabled { get; set; } = false;

	MiniGames game;

	public MiniGamesPlayer()
	{
		Inventory = new Inventory( this );
		game = (Game.Current as MiniGames);
	}

	public MiniGamesPlayer( Client cl ) : this()
	{
		// Load clothing from client data
		Clothing.LoadFromClient( cl );
		InitSpawn = true;
	}

	public virtual void Freeze( bool isfreeze )
	{
		IsFreeze = isfreeze;
	}

	public override void Spawn()
	{
		MainCamera = new FirstPersonCamera();
		LastCamera = MainCamera;

		base.Spawn();

		FlashlightEntity = CreateLight();
		FlashlightPosOffset = 30f;
	}

	public override void Respawn()
	{
		if ( !IsServer ) return;
		if ( LifeState != LifeState.Alive || InitSpawn )
		{
			SetModel( "models/citizen/citizen.vmdl" );

			Animator = new StandardPlayerAnimator();

			MainCamera = LastCamera;
			Camera = MainCamera;

			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;
			InitSpawn = false;

			Clothing.DressEntity( this );
		}

		Controller = new MiniGamesWalkController();

		if ( DevController is NoclipController )
		{
			DevController = null;
		}

		EnableAllCollisions = Team is not Spectator;
		EnableDrawing = Team is not Spectator;
		Scale = MapSettings.PlayerScale;
		LightEnabled = false;

		foreach ( var child in Children )
		{
			if ( !child.Tags.Has( "clothes" ) ) continue;
			if ( child is not ModelEntity e ) continue;

			e.EnableAllCollisions = Team is not Spectator;
			e.EnableDrawing = Team is not Spectator;
		}

		if ( !string.IsNullOrEmpty( MapSettings.StartWeapon ) )
		{
			SupressPickupNotices = true;

			var wep = Library.Create<Weapon>( MapSettings.StartWeapon );

			if ( wep != null )
				Inventory.Add( wep );

			SupressPickupNotices = false;
		}

		foreach ( var child in Children )
		{
			if ( !child.Tags.Has( "clothes" ) ) continue;
			if ( child is not ModelEntity e ) continue;

			e.RenderColor = Team.Color.ToColor();
		}

		CreateHull();

		timeSinceTempGod = 0;
		LifeState = LifeState.Alive;
		Health = 100;
		Velocity = Vector3.Zero;
		WaterLevel.Clear();

		Game.Current?.MoveToSpawnpoint( this );
		ResetInterpolation();
	}

	private SpotLightEntity CreateLight()
	{
		var light = new SpotLightEntity
		{
			Enabled = false,
			DynamicShadows = true,
			Range = 3200f,
			Falloff = 0.3f,
			LinearAttenuation = 0.3f,
			Brightness = 25f,
			Color = Color.FromBytes( 200, 200, 200, 230 ),
			InnerConeAngle = 9,
			OuterConeAngle = 32,
			FogStength = 1.0f,
			Owner = this,
			LightCookie = Texture.Load( "materials/effects/lightcookie.vtex" )
		};

		return light;
	}

	public override void CreateHull()
	{
		CollisionGroup = CollisionGroup.Player;
		AddCollisionLayer( CollisionLayer.Player );
		SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, new Vector3( -16, -16, 0 ), new Vector3( 16, 16, 72 ) );

		MoveType = MoveType.MOVETYPE_WALK;
		EnableHitboxes = true;

		if ( MapSettings.PlayersNoCollide )
		{
			//TODO: PlayersNoCollide
			//SetInteractsAs( CollisionLayer.Player );
			//SetInteractsWith( CollisionLayer.WORLD_GEOMETRY );
			//SetInteractsExclude( CollisionLayer.Player );
		}
	}

	public override void OnKilled()
	{
		timeSinceDied = 0;
		IsHeadShot = GetHitboxGroup( lastDamage.HitboxIndex ) == 1;

		base.OnKilled();

		if ( lastDamage.Flags.HasFlag( DamageFlags.Vehicle ) )
		{
			Particles.Create( "particles/impact.flesh.bloodpuff-big.vpcf", lastDamage.Position );
			Particles.Create( "particles/impact.flesh-big.vpcf", lastDamage.Position );
			PlaySound( "kersplat" );
		}

		VehicleController = null;
		VehicleAnimator = null;
		VehicleCamera = null;
		Vehicle = null;

		BecomeRagdollOnClient( Velocity, lastDamage.Flags, lastDamage.Position, lastDamage.Force, GetHitboxBone( lastDamage.HitboxIndex ) );
		LastCamera = MainCamera;
		MainCamera = new SpectateRagdollCamera();
		Camera = MainCamera;
		Controller = null;

		EnableAllCollisions = false;
		EnableDrawing = false;

		if ( MapSettings.DropAfterDeath && Inventory is Inventory inv )
			inv.DropAll();
		
		Inventory.DeleteContents();
	}

	public override void TakeDamage( DamageInfo info )
	{
		if ( IsFreeze ) return;
		if ( Team != null & Team is Spectator ) return;
		if ( MapSettings.God || (MapSettings.TempGod > 0 && timeSinceTempGod < MapSettings.TempGod) ) return;
		if ( !MapSettings.PlayersDamage ) return;
		if ( !MapSettings.TeamDamage && info.Attacker != null && info.Attacker is MiniGamesPlayer ply && ply.Team == Team && ply != this ) return;
		if ( GetHitboxGroup( info.HitboxIndex ) == 1 )
		{
			info.Damage *= 2.0f;
		}

		lastDamage = info;

		TookDamage( lastDamage.Flags, lastDamage.Position, lastDamage.Force );

		base.TakeDamage( info );

		//Log.Info( info.Attacker is MiniGamesPlayer attacker && attacker != this );

		if ( info.Attacker != null && (info.Attacker is MiniGamesPlayer || info.Attacker.Owner is MiniGamesPlayer) )
		{
			MiniGamesPlayer attacker = info.Attacker as MiniGamesPlayer;

			if ( attacker == null )
				attacker = info.Attacker.Owner as MiniGamesPlayer;

			// Note - sending this only to the attacker!
			if ( attacker != this )
				attacker.DidDamage( To.Single( attacker ), info.Position, info.Damage, Health.LerpInverse( 100, 0 ), Health <= 0 );
		}

		TookDamage( To.Single( this ), (info.Weapon != null && info.Weapon.IsValid()) ? info.Weapon.Position : (info.Attacker != null && info.Attacker.IsValid()) ? info.Attacker.Position : Position );
	}

	[ClientRpc]
	public void TookDamage( DamageFlags damageFlags, Vector3 forcePos, Vector3 force )
	{
	}

	[ClientRpc]
	public void DidDamage( Vector3 pos, float amount, float healthinv, bool isdeath )
	{
		Sound.FromScreen( "dm.ui_attacker" )
			.SetPitch( 1 + healthinv * 1 );

		HitIndicator.Current?.OnHit( pos, amount, isdeath );
    }

	[ClientRpc]
	public void TookDamage( Vector3 pos )
	{
		//DebugOverlay.Sphere( pos, 5.0f, Color.Red, false, 50.0f );

		DamageIndicator.Current?.OnHit( pos );
	}

	public override PawnController GetActiveController()
	{
		if ( VehicleController != null ) return VehicleController;
		if ( DevController != null ) return DevController;

		return base.GetActiveController();
	}

	public override PawnAnimator GetActiveAnimator()
	{
		if ( VehicleAnimator != null ) return VehicleAnimator;

		return base.GetActiveAnimator();
	}

	public ICamera GetActiveCamera()
	{
		if ( VehicleCamera != null ) return VehicleCamera;

		return MainCamera;
	}

	public virtual void ToggleFlashlight( bool ison )
	{
		if ( LightEnabled == ison ) return;

		PlaySound( ison ? "flashlight-on" : "flashlight-off" );

		timeSinceLightToggled = 0;
		LightEnabled = ison;
	}

	public bool IsOnGround()
	{
		var tr = Trace.Ray( Position, Position + Vector3.Down * 20 )
				.Radius( 1 )
				.Ignore( this )
				.Run();

		return tr.Hit;
	}

	public override void Simulate( Client cl )
	{
		if ( IsServer && game.Round is not Round.Waiting && LifeState == LifeState.Dead )
		{
			if ( MapSettings.Respawn )
			{
				if ( timeSinceDied > 3 )
				{
					Respawn();
				}

				return;
			}
			else
			{
				if ( timeSinceDied > 3 )
				{
					Respawn();

					Controller = new NoclipController();
					EnableAllCollisions = false;
					EnableDrawing = false;

					foreach ( var child in Children )
					{
						if ( !child.Tags.Has( "clothes" ) ) continue;
						if ( child is not ModelEntity e ) continue;

						e.EnableAllCollisions = false;
						e.EnableDrawing = false;
					}

					ToggleFlashlight( false );
				}

				return;
			}
		}

		var controller = GetActiveController();
		controller?.Simulate( cl, this, GetActiveAnimator() );

		if ( Input.ActiveChild != null )
		{
			ActiveChild = Input.ActiveChild;
		}

		if ( IsServer && game.Round is Round.Waiting && LifeState != LifeState.Alive )
		{
			game.TeamSpectator.Join( this );
			Respawn();
		}

		if ( Team is Spectator )
		{
			Controller = new NoclipController();
			EnableAllCollisions = false;
			EnableDrawing = false;

			foreach ( var child in Children )
			{
				if ( !child.Tags.Has( "clothes" ) ) continue;
				if ( child is not ModelEntity e ) continue;

				e.EnableAllCollisions = false;
				e.EnableDrawing = false;
			}

			ToggleFlashlight( false );
		}

		if ( LifeState != LifeState.Alive )
			return;

		if ( IsServer && FlashlightEntity.IsValid() )
		{
			FlashlightEntity.Position = EyePos + EyeRot.Forward * FlashlightPosOffset;
			FlashlightEntity.Rotation = EyeRot;
			FlashlightEntity.Enabled = LightEnabled;
		}

		if ( VehicleController != null && DevController is NoclipController )
		{
			DevController = null;
		}

		if ( controller != null )
			EnableSolidCollisions = !controller.HasTag( "noclip" );

		if ( !IsFreeze )
		{
			TickPlayerUse();
			SimulateActiveChild( cl, ActiveChild );
		}

		if ( Input.Pressed( InputButton.View ) )
		{
			if ( MainCamera is not FirstPersonCamera )
			{
				MainCamera = new FirstPersonCamera();
			}
			else
			{
				MainCamera = new ThirdPersonCamera();
			}
		}

		Camera = GetActiveCamera();

		if ( Team is not Spectator )
		{
			if ( Input.Pressed( InputButton.Drop ) )
			{
				var dropped = Inventory.DropActive();
				if ( dropped != null )
				{
					dropped.PhysicsGroup.ApplyImpulse( Velocity + EyeRot.Forward * 80.0f + Vector3.Up * 100.0f, true );
					dropped.PhysicsGroup.ApplyAngularImpulse( Vector3.Random * 100.0f, true );

					timeSinceDropped = 0;
				}
			}

			if ( MapSettings.AllowFlashlight && Input.Pressed( InputButton.Flashlight ) && timeSinceLightToggled > 0.1f )
				ToggleFlashlight( !LightEnabled );
		}

		var DownVel = Velocity * Rotation.Down;
		var falldamage = DownVel.z / 50;

		if ( timeSinceFall > 0.02f && DownVel.z > 750 && IsOnGround() && !controller.HasTag( "noclip" ))
		{
			var dmg = new DamageInfo()
			{
				Position = Position,
				Damage = falldamage
			};

			PlaySound( "dm.ui_attacker" );
			TakeDamage( dmg );
			timeSinceFall = 0;
		}

		CanUseEntityGlow();
	}

	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );

		//Update the flashlight position on the client in framesim
		//so the movement is nice and smooth.
		if ( FlashlightEntity.IsValid() )
		{
			FlashlightEntity.Position = EyePos + EyeRot.Forward * FlashlightPosOffset;
			FlashlightEntity.Rotation = EyeRot;
		}
	}

	public void ResetDroppedTime()
	{
		timeSinceDropped = 0;
	}

	public override void StartTouch( Entity other )
	{
		if ( timeSinceDropped < 0.1 ) return;

		base.StartTouch( other );
	}

	[ServerCmd( "inventory_current" )]
	public static void SetInventoryCurrent( string entName )
	{
		var target = ConsoleSystem.Caller.Pawn;
		if ( target == null ) return;

		var inventory = target.Inventory;
		if ( inventory == null )
			return;

		for ( int i = 0; i < inventory.Count(); ++i )
		{
			var slot = inventory.GetSlot( i );
			if ( !slot.IsValid() )
				continue;

			if ( !slot.ClassInfo.IsNamed( entName ) )
				continue;

			inventory.SetActiveSlot( i, false );

			break;
		}
	}

	// TODO

	//public override bool HasPermission( string mode )
	//{
	//	if ( mode == "noclip" ) return true;
	//	if ( mode == "devcam" ) return true;
	//	if ( mode == "suicide" ) return true;
	//
	//	return base.HasPermission( mode );
	//	}
}
