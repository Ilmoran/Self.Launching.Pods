using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
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

		public static bool AnyCapablePawn(IEnumerable<Pawn> pawns)
		{
			return pawns.Any(delegate (Pawn arg)
			{
				return (!arg.Downed && arg.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation));
			});
		}

		public static int CalculateMaxBuyableFuel(float freeMassCapacity, int fuelForSale, int colonyMoney, float fuelPrice)
		{
			int buyableFuelByMoney;
			int buyableFuelByMass;
			int maxFuel = 0;
			int previousMaxFuel = int.MinValue;

			do
			{
				previousMaxFuel = maxFuel;
				buyableFuelByMoney = Mathf.FloorToInt(colonyMoney / fuelPrice);
				buyableFuelByMass = Mathf.FloorToInt(freeMassCapacity / DefOf.Chemfuel.BaseMass);
				maxFuel = (new int[]
				{
					fuelForSale,
					buyableFuelByMoney,
					buyableFuelByMass
				}).Min();

				freeMassCapacity += fuelPrice * (maxFuel - previousMaxFuel) * DefOf.Silver.BaseMass;
			} while (previousMaxFuel < maxFuel);
			return (maxFuel);
		}
	}
}