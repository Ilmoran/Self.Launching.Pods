using UnityEngine;
using Verse;

namespace WM.SelfLaunchingPods
{
	[StaticConstructorOnStartup]
	public static class Resources
	{
		public static readonly Texture2D LaunchCommandTex = ContentFinder<Texture2D>.Get("UI/Commands/LaunchShip", true);
		public static readonly Texture2D RefuelAndLaunchCommandTex = ContentFinder<Texture2D>.Get("UI/RefuelAndLaunchShip", true);
		public static readonly Texture2D TargeterMouseAttachment = ContentFinder<Texture2D>.Get("UI/Overlays/LaunchableMouseAttachment", true);
		public static readonly Texture2D GizmoLoadEverything = ContentFinder<Texture2D>.Get("UI/LoadEverything", true);
		public static readonly Texture2D GizmoLoadItems = ContentFinder<Texture2D>.Get("UI/LoadItems", true);
		public static readonly Texture2D GizmoUnloadEverything = ContentFinder<Texture2D>.Get("UI/UnloadEverything", true);
		public static readonly Texture2D GizmoUnloadItems = ContentFinder<Texture2D>.Get("UI/UnloadItems", true);
		public static readonly Texture2D GizmoUnloadPawns = ContentFinder<Texture2D>.Get("UI/UnloadPawns", true);
		public static readonly Texture2D GizmoMergeFleets = ContentFinder<Texture2D>.Get("UI/MergeFleets", true);
		public static readonly Texture2D GizmoTrade = ContentFinder<Texture2D>.Get("UI/Commands/Trade", true);
		public static readonly Texture2D GizmoRepair = ContentFinder<Texture2D>.Get("UI/Repair", true);

		// RimWorld.CompRefuelable
		public static readonly Material FuelBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.6f, 0.56f, 0.13f), false);
		public static readonly Material FuelBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f), false);
	}
}
