using System.Linq;
using UnityEngine;

namespace WM.SelfLaunchingPods
{
	public static class FuelUtils
	{
		public static float PodFuelUsePerTile
		{
			get
			{
				var t = DefOf.WM_TransportPod.comps.First((obj) => obj.GetType() == typeof(CompProperties_SelfLaunchable)) as CompProperties_SelfLaunchable;

				return (t.fuelUsePerTile * TechUtils.FuelUseMultiplier);
			}
		}

		public static float PodFuelUsePerLaunch
		{
			get
			{
				var t = DefOf.WM_TransportPod.comps.First((obj) => obj.GetType() == typeof(CompProperties_SelfLaunchable)) as CompProperties_SelfLaunchable;

				return (t.fuelUsePerLaunch * TechUtils.FuelUseMultiplier);
			}
		}

		public static float FuelNeededToLaunchAtDistance(int dist, int podsCount)
		{
			return (PodFuelUsePerLaunch + PodFuelUsePerTile * dist) * podsCount;
		}	

		public static int MaxLaunchDistance(float fuelAmount, int podsCount, bool oneway)
		{
			var fuelPerPod = fuelAmount / podsCount;
			var result = ((fuelPerPod - PodFuelUsePerLaunch) / PodFuelUsePerTile) / (oneway ? 1 : 2);

			return (Mathf.FloorToInt(result));
		}
	}
}