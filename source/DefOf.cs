using System;
using System.Linq;
using RimWorld;
using Verse;
using Harmony;
using UnityEngine;

namespace WM.SelfLaunchingPods
{
	public static class DefOf
	{
		//static DefOf()
		//{
		//	//var l = DefDatabase<ThingDef>.AllDefs.Where(arg => arg.defName.Contains("P")).Select(arg => arg.defName + " : " + arg.GetType()).ToArray();
		//	var l = DefDatabase<Verse.ThingDef>.AllDefs.Count();

		//	//.Message(string.Join(" ; ", l));
		//	Log.Message(l.ToString());
		//}
		internal static void LoadDefs()
		{
			foreach (var item in typeof(DefOf).GetFields(AccessTools.all))
			{
				var name = item.Name;
				item.SetValue(null, GenDefDatabase.GetDef(item.FieldType, name));
			}
		}
		public static ThingDef WM_DropPodLeaving;
		public static ThingDef WM_DropPodIncoming;

		public static ThingDef WM_TransportPod;
		public static WorldObjectDef WM_TravelingTransportPods;
		//public static WorldObjectDef WM_TravelingTransportPods_Landed;

		public static ThingDef Chemfuel;
	}
}
