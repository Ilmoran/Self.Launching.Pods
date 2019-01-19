using Harmony;
using RimWorld;
using Verse;

namespace WM.SelfLaunchingPods
{
	[DefOf]
	public static class ThingDefOf
    {
		public static ThingDef			WM_DropPodLeaving;
		public static ThingDef			WM_DropPodIncoming;
		public static ThingDef			WM_TransportPod;
		public static ThingDef			WM_LandingSpot;
		public static ThingDef			Chemfuel;
		public static ThingDef			DropPodIncoming;
		public static ThingDef			Silver;
		public static ThingDef			CommsConsole;
		public static ThingDef          ComponentIndustrial;

        static ThingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));
        }
    }

    [DefOf]
    public static class WorldObjectDefOf
    {
        public static WorldObjectDef    WM_TravelingTransportPods;
        public static WorldObjectDef    Caravan;
        public static WorldObjectDef    Settlement;

        static WorldObjectDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(WorldObjectDefOf));
        }
    }

    [DefOf]
    public static class SoundDefOf
    {
        public static SoundDef          DropPod_Leaving;

        static SoundDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SoundDefOf));
        }
    }
}
