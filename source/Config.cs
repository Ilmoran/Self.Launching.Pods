using System;
using System.Linq;
using HugsLib.Settings;
using Verse;

namespace WM.SelfLaunchingPods
{
	public class Config : HugsLib.ModBase
	{
		internal static float controlerRange = 20;
		internal static float LandingDesignatorRange = 20;

		private string modName = "WM_Reuse_pods";

		public override string ModIdentifier
		{
			get
			{
				return (modName);
			}
		}

		public static float PodFuelUsePerTile
		{
			get
			{
				var t = DefOf.WM_TransportPod.comps.First((obj) => obj.GetType() == typeof(CompProperties_SelfLaunchable)) as CompProperties_SelfLaunchable;

				return (t.fuelUsePerTile);
			}
		}
		public static float PodFuelUsePerLaunch
		{
			get
			{
				var t = DefOf.WM_TransportPod.comps.First((obj) => obj.GetType() == typeof(CompProperties_SelfLaunchable)) as CompProperties_SelfLaunchable;

				return (t.fuelUsePerLaunch);
			}
		}

		public override void WorldLoaded()
		{
			base.WorldLoaded();
		}
		public override void DefsLoaded()
		{
			DefOf.LoadDefs();
		}
	}
}
