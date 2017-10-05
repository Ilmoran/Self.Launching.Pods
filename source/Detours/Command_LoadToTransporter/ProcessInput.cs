using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WM.SelfLaunchingPods.Detours.Command_LoadToTransporter
{
	public class Command_LoadToTransporter : RimWorld.Command_LoadToTransporter
	{
		public List<CompTransporter> transporters
		{
			get
			{
				return (List<CompTransporter>)Traverse.Create(this).Field("transporters").GetValue();
			}
			set
			{
				Traverse.Create(this).Field("transporters").SetValue(value);

			}
		}
		public static HashSet<Building> tmpFuelingPortGivers
		{
			get
			{
				return (HashSet<Building>)Traverse.Create(typeof(RimWorld.Command_LoadToTransporter)).Field("tmpFuelingPortGivers").GetValue();
			}
			set
			{
				Traverse.Create(typeof(RimWorld.Command_LoadToTransporter)).Field("tmpFuelingPortGivers").SetValue(value);
			}
		}
	}

	[HarmonyPatch(typeof(RimWorld.Command_LoadToTransporter), "ProcessInput")]
	public static class ProcessInput
	{
		static bool Prefix(Command_LoadToTransporter __instance, Event ev)
		{
			Log.Message("RimWorld.Command_LoadToTransporter.ProcessInput()");
			if (Find.Selector.SelectedObjects.All((arg) => Utils.IsMyClass(arg)))
			{
				Internal(__instance);
				return false;
			}
			return true;

		}
		// RimWorld.Command_LoadToTransporter
		static void Internal(Command_LoadToTransporter __instance)
		{
			if (__instance.transporters == null)
			{
				__instance.transporters = new List<CompTransporter>();
			}
			if (!__instance.transporters.Contains(__instance.transComp))
			{
				__instance.transporters.Add(__instance.transComp);
			}
			RimWorld.CompLaunchable launchable = __instance.transComp.Launchable;
			if (launchable != null)
			{
				Building fuelingPortSource = launchable.FuelingPortSource;
				if (fuelingPortSource != null && !Utils.IsMyClass(fuelingPortSource))
				{
					Map map = __instance.transComp.Map;
					Command_LoadToTransporter.tmpFuelingPortGivers.Clear();
					map.floodFiller.FloodFill(fuelingPortSource.Position, (IntVec3 x) => RimWorld.FuelingPortUtility.AnyFuelingPortGiverAt(x, map), delegate (IntVec3 x)
					{
						Command_LoadToTransporter.tmpFuelingPortGivers.Add(RimWorld.FuelingPortUtility.FuelingPortGiverAt(x, map));
					}, false);

#if DEBUG
					foreach (var item in Command_LoadToTransporter.tmpFuelingPortGivers)
					{
						Log.Message(item.ToString());
					}
#endif

					for (int i = 0; i < __instance.transporters.Count; i++)
					{
						Building fuelingPortSource2 = __instance.transporters[i].Launchable.FuelingPortSource;
						if (fuelingPortSource2 != null && !Command_LoadToTransporter.tmpFuelingPortGivers.Contains(fuelingPortSource2))
						{
							Messages.Message("MessageTransportersNotAdjacent".Translate(), fuelingPortSource2, MessageSound.RejectInput);
							return;
						}
					}
				}
			}
			for (int j = 0; j < __instance.transporters.Count; j++)
			{
				if (__instance.transporters[j] != __instance.transComp)
				{
					if (!__instance.transComp.Map.reachability.CanReach(__instance.transComp.parent.Position, __instance.transporters[j].parent, PathEndMode.Touch, TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false)))
					{
						Messages.Message("MessageTransporterUnreachable".Translate(), __instance.transporters[j].parent, MessageSound.RejectInput);
						return;
					}
				}
			}
			Find.WindowStack.Add(new Dialog_LoadTransporters(__instance.transComp.Map, __instance.transporters));
		}

	}
}
