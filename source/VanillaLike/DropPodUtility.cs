using RimWorld;
using Verse;

namespace WM.SelfLaunchingPods
{
	public static class DropPodUtility
	{
		// RimWorld.DropPodUtility
		public static void MakeDropPodAt(IntVec3 c, Map map, ActiveDropPodInfo info, Thing landedThing)
		{
			DropPodIncoming dropPodIncoming = (DropPodIncoming)ThingMaker.MakeThing(ThingDefOf.WM_DropPodIncoming, null);
			dropPodIncoming.podInfo = info;
			dropPodIncoming.landedThing = landedThing;
			GenSpawn.Spawn(dropPodIncoming, c, map);
		}
	}
}
