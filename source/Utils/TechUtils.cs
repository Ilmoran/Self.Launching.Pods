using System;
using System.Linq;
using Verse;

namespace WM.SelfLaunchingPods
{
	public static class TechUtils
	{
		public static float FuelUseMultiplier
		{
			get
			{
				return (1 / GetTechMultiplier((PodsStatsModifiers arg) => arg.fuelEfficiency));
			}
		}

		public static float FuelCapacityMultiplier
		{
			get
			{
				return (GetTechMultiplier((PodsStatsModifiers arg) => arg.fuelCapacity));
			}
		}

		//public static float ReliabilityRate
		//{
		//	get
		//	{
		//		return (GetTechMultiplier((PodsStatsModifiers arg) => arg.));
		//	}
		//}

		static float GetTechMultiplier(Func<PodsStatsModifiers, float> fetcher)
		{
			var allDefsListForReading = DefDatabase<PodResearchProjectDef>.AllDefsListForReading;
			var activeTechs = allDefsListForReading.Where((PodResearchProjectDef arg) => arg.IsFinished);
			var totalTechModifier = activeTechs.Aggregate(1, (float arg1, PodResearchProjectDef arg2) => arg1 * (1 + fetcher(arg2.podsStatsModifiers)));

			return (totalTechModifier);
		}
	}
}