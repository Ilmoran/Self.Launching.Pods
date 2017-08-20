using System;
using UnityEngine;
using Verse;

namespace WM.SelfLaunchingPods
{
	public static class Resources
	{
		//public static readonly Texture2D MK1Pod = ContentFinder<Texture2D>.Get("Things/MK1pod");
		public static readonly Texture2D LaunchCommandTex = ContentFinder<Texture2D>.Get("UI/Commands/LaunchShip", true);
		// RimWorld.CompLaunchable
		public static readonly Texture2D TargeterMouseAttachment = ContentFinder<Texture2D>.Get("UI/Overlays/LaunchableMouseAttachment", true);
	}
}
