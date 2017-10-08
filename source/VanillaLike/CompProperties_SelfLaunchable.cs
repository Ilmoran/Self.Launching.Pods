using Verse;

namespace WM.SelfLaunchingPods
{
	public class CompProperties_SelfLaunchable : CompProperties
	{
		public CompProperties_SelfLaunchable()
		{
			this.compClass = typeof(CompSelfLaunchable);
		}

		public float fuelUsePerLaunch = 0f;
		public float fuelUsePerTile = 0f;
	}
}
