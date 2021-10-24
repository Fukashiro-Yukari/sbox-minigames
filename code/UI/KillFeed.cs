﻿using Sandbox;
using Sandbox.UI;
using System;

public partial class KillFeed : Sandbox.UI.KillFeed
{
	private bool GetIcon( string method, KillFeedEntry e )
	{
		try
		{
			if ( method != null && method.StartsWith( "mg_" ) )
			{
				var killWeapon = Library.Create<Entity>( method );

				if ( killWeapon is Carriable car )
				{
					if ( !string.IsNullOrEmpty( car.Icon ) )
					{
						e.Icon.Style.BackgroundImage = Texture.Load( car.Icon );
						e.Icon.SetClass( "close", false );
						killWeapon.Delete();

						return true;
					}
				}
			}
		}
		catch ( Exception ) { }

		return false;
	}

	public virtual Panel AddEntry( ulong lsteamid, string left, ulong rsteamid, string right, string method, bool isHeadShot = false )
	{
		Log.Info( $"{left} killed {right} using {method}" );

		var e = Current.AddChild<KillFeedEntry>();

		e.Left.Text = left;
		e.Left.SetClass( "me", lsteamid == (Local.SteamId) );

		if ( !GetIcon( method, e ) )
		{
			e.Method.Text = method;
			e.Icon.SetClass( "close", true );
		}

		e.HeadShotIcon.Style.BackgroundImage = Texture.Load( "ui/headshot.png" );
		e.HeadShotIcon.SetClass( "close", !isHeadShot );

		e.Right.Text = right;
		e.Right.SetClass( "me", rsteamid == (Local.SteamId) );

		return e;
	}

	public virtual Panel AddEntry( string left, ulong rsteamid, string right, string method )
	{
		var e = Current.AddChild<KillFeedEntry>();

		e.Left.Text = left;
		e.Left.SetClass( "me", false );

		if ( !GetIcon( method, e ) )
		{
			e.Method.Text = method;
			e.Icon.SetClass( "close", true );
		}

		e.Right.Text = right;
		e.Right.SetClass( "me", rsteamid == (Local.Client?.SteamId) );

		return e;
	}

	public virtual Panel AddEntry( ulong lsteamid, string left, string right, string method )
	{
		var e = Current.AddChild<KillFeedEntry>();

		e.Left.Text = left;
		e.Left.SetClass( "me", lsteamid == (Local.Client?.SteamId) );

		if ( !GetIcon( method, e ) )
		{
			e.Method.Text = method;
			e.Icon.SetClass( "close", true );
		}

		e.Right.Text = right;
		e.Right.SetClass( "me", false );

		return e;
	}

	public virtual Panel AddEntry( string left, string right, string method )
	{
		var e = Current.AddChild<KillFeedEntry>();

		e.Left.Text = left;
		e.Left.SetClass( "me", false );

		if ( !GetIcon( method, e ) )
		{
			e.Method.Text = method;
			e.Icon.SetClass( "close", true );
		}

		e.Right.Text = right;
		e.Right.SetClass( "me", false );

		return e;
	}
}
