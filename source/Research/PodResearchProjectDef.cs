using Verse;

namespace WM.SelfLaunchingPods
{
	public class PodResearchProjectDef : ResearchProjectDef
	{
		public PodsStatsModifiers podsStatsModifiers;
	}

	public class PodsStatsModifiers
	{
		public float fuelEfficiency;
		public float fuelCapacity;
		//public float reliabilityGain;
	}
}
