using Harmony;
using RimWorld;
using Verse;

namespace WM.SelfLaunchingPods
{
	[StaticConstructorOnStartup]
	public static class DefOf
	{
		//internal static void LoadDefs()
		static DefOf()
		{
			foreach (var item in typeof(DefOf).GetFields(AccessTools.all))
			{
				var name = item.Name;
				item.SetValue(null, GenDefDatabase.GetDef(item.FieldType, name));
			}
		}

		public static ThingDef			WM_DropPodLeaving;
		public static ThingDef			WM_DropPodIncoming;
		public static ThingDef			WM_TransportPod;
		public static WorldObjectDef	WM_TravelingTransportPods;
		public static ThingDef			WM_LandingSpot;
		public static ThingDef			Chemfuel;
		public static ThingDef			DropPodIncoming;
		public static ThingDef			Silver;
	}
}
