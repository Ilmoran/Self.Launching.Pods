using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using RimWorld;
using Verse;

namespace WM.SelfLaunchingPods.Detours.CaravanUIUtility
{
	[HarmonyPatch(typeof(RimWorld.CaravanUIUtility), "AddPawnsSections")]
	static class AddPawnsSections
	{
		public static void Postfix(TransferableOneWayWidget widget, List<TransferableOneWay> transferables)
		{
			Func<TransferableOneWay, bool> validator = delegate (TransferableOneWay item)
			{
				var pawn = item.AnyThing as Pawn;
				return (pawn != null && !pawn.IsFreeColonist && !pawn.IsPrisoner && !pawn.RaceProps.Animal && pawn.HostFaction == Faction.OfPlayer);
			};
			var list = transferables.Where(validator);
			widget.AddSection("WM.RefugeeSection".Translate(), list);
		}
	}
}
