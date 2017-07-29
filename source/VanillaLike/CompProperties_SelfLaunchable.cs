using System;
using Verse;

namespace WM.ReusePods
{
	public class CompProperties_SelfLaunchable : CompProperties
	{
		public CompProperties_SelfLaunchable()
		{
			this.compClass = typeof(CompSelfLaunchable);
		}

		//public float maxFuel = 0f;
		public float fuelUsePerLaunch = 0f;
		public float fuelUsePerTile = 0f;
		//public ThingDef leavingPodDef = DefOf.WM_DropPodLeaving;
	}
}
