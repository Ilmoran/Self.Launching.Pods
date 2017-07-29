using System;
using UnityEngine;
using Verse;

namespace WM.SelfLaunchingPods
{
	public static class Resources
	{
		public static readonly Texture2D MK1Pod = ContentFinder<Texture2D>.Get("Things/MK1pod");
		internal static Texture2D LaunchIcon;
	}
}
