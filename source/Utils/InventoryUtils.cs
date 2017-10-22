using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace WM.SelfLaunchingPods
{
	public static class InventoryUtils
	{
		public static IEnumerable<Thing> GetItemsFrom(IEnumerable<Thing> things)
		{
			return (things.Where((Thing arg) => !(arg is Pawn)));
		}

		public static IEnumerable<Pawn> GetPawnsFrom(IEnumerable<Thing> things)
		{
			return (things.Where((Thing arg) => (arg is Pawn)).Cast<Pawn>());
		}

		public static IEnumerable<Pawn> GetColonistsFrom(IEnumerable<Thing> things)
		{
			return (GetPawnsFrom(things).Where((Pawn arg) => arg.IsColonist));
		}

		public static IEnumerable<Pawn> GetAnimalsFrom(IEnumerable<Thing> things)
		{
			return (GetPawnsFrom(things).Where((Pawn arg) => arg.RaceProps.Animal));
		}

		public static IEnumerable<Pawn> GetPrisonersFrom(IEnumerable<Thing> things)
		{
			return (GetPawnsFrom(things).Where((Pawn arg) => arg.IsPrisonerOfColony));
		}
	}
}