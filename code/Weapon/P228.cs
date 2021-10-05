using Sandbox;

[Library( "mg_p228", Title = "P228", Spawnable = true )]
[Hammer.EditorModel( "weapons/css_p228/w_css_pist_p228.vmdl" )]
partial class P228 : Weapon
{
	public override string ViewModelPath => "weapons/css_p228/v_css_pist_p228.vmdl";
	public override string WorldModelPath => "weapons/css_p228/w_css_pist_p228.vmdl";

	public override int ClipSize => 13;
	public override int Bucket => 1;
	public override int AmmoMultiplier => 5;
	public override bool Automatic => false;
	public override float PrimaryRate => 10f;
	public override float ReloadTime => 2.71f;
	public override CType Crosshair => CType.Pistol;
	public override string Icon => "ui/weapons/weapon_p228.png";
	public override string ShootSound => "css_p228.fire";
	public override float Spread => 0.2f;
	public override float Force => 5f;
	public override float Damage => 40f;
	public override float BulletSize => 3f;
	public override float FOV => 75;
	public override ScreenShake ScreenShake => new ScreenShake
	{
		Length = 0.5f,
		Speed = 4.0f,
		Size = 1.0f,
		Rotation = 0.5f
	};

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 1 ); // TODO this is shit
		anim.SetParam( "holdtype_handedness", 0 );
		anim.SetParam( "aimat_weight", 1.0f );
	}
}
