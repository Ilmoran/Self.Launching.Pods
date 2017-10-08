using System.Linq;

namespace WM.SelfLaunchingPods
{
	public class Config : HugsLib.ModBase
	{
		private static readonly string	modName = "WM_Self_Launching_Pods";
		internal static readonly float	LandingSpotMaxRange = 7f;

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
