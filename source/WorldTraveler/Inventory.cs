using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace WM.SelfLaunchingPods
{
	public partial class WorldTraveler : WorldObject
	{
		public float FuelLevel
		{
			get
			{
				return (PodsAsThing.Sum(arg => arg.TryGetComp<CompRefuelable>().Fuel));

			}
		}

		public float MissingFuelLevel
		{
			get
			{
				return (PodsAsThing.Sum(arg => { var compRefuelable = arg.TryGetComp<CompRefuelable>(); return compRefuelable.Props.fuelCapacity - compRefuelable.Fuel; }));
			}
		}

		public float FuelCapacity
		{
			get
			{
				return (PodsAsThing.Sum(arg => arg.TryGetComp<CompRefuelable>().Props.fuelCapacity));
			}
		}

		public float FuelLevelPerPod
		{
			get
			{
				return (this.FuelLevel / this.PodsCount);
			}
		}

		public float CarriedFuelLevel
		{
			get
			{
				return (this.AllCarriedFuelThingsOrdered.Sum((Thing arg) => arg.stackCount));
			}
		}

		public int PodsCount
		{
			get
			{
				return (this.pods.Count);
			}
		}

		public IEnumerable<Thing> AllCarriedThings
		{
			get
			{
				return
					this.pods.SelectMany((arg) => arg.PodInfo.innerContainer)
						//.Concat(AllCarriedPawns.SelectMany((Pawn arg) => arg.inventory.innerContainer))
						;
			}
		}

		public IEnumerable<Thing> AllCarriedGoods
		{
			get
			{
				return
					this.pods.SelectMany((arg) => arg.PodInfo.innerContainer)
						.Where(delegate (Thing arg)
						{
							var pawn = arg as Pawn;
							return (pawn == null || !pawn.RaceProps.Humanlike || pawn.IsPrisoner);
						});
			}
		}

		public IEnumerable<Thing> AllCarriedThingsOrdered
		{
			get
			{
				return (from item in AllCarriedThings
						orderby item.MarketValue descending, item.stackCount descending
						select item);
			}
		}

		public IEnumerable<Pawn> AllCarriedPawns
		{
			get
			{
				return InventoryUtils.GetPawnsFrom(AllCarriedThings);
			}
		}

		public IEnumerable<Pawn> AllCarriedColonists
		{
			get
			{
				return InventoryUtils.GetColonistsFrom(AllCarriedThings);
			}
		}

		public IEnumerable<Thing> AllCarriedItems
		{
			get
			{
				return InventoryUtils.GetItemsFrom(AllCarriedThings);
			}
		}

		public IEnumerable<Thing> AllCarriedFuelThingsOrdered
		{
			get
			{
				return (this.AllCarriedThings.Where((Thing arg) => arg.def == ThingDefOf.Chemfuel)
						.OrderByDescending((Thing arg) => arg.stackCount));
			}
		}

		public bool HasPawns
		{
			get
			{
				return (AllCarriedPawns.Any());
			}
		}

		public IEnumerable<PodPair> Pods
		{
			get
			{
				return pods;
			}
		}

		public IEnumerable<Thing> PodsAsThing
		{
			get
			{
				return pods.Select(arg => arg.PodThing);
			}
		}

		public IEnumerable<ActiveDropPodInfo> PodsInfo
		{
			get
			{
				return pods.Select(arg => arg.PodInfo);
			}
		}

		public float MassCapacity
		{
			get
			{
				return PodsAsThing.Sum(arg => arg.TryGetComp<CompTransporter>().Props.massCapacity);
			}
		}

		public float MassUsage
		{
			get
			{
				return (CollectionsMassCalculator.MassUsage<Thing>(this.AllCarriedThings.ToList(), IgnorePawnsInventoryMode.DontIgnore, true, false));
			}
		}

		public void AddPod(Thing building, ActiveDropPodInfo podInfo)
		{
			var pair = new PodPair(building, podInfo);
			AddPod(pair);
		}

		public void AddPod(PodPair podPair)
		{
			this.pods.Add(podPair);
		}

		public void RemovePod(PodPair podPair)
		{
			if (!this.pods.Remove(podPair))
				Log.Warning("Tried to remove pod from fleet but pod is not in fleet.");
		}

		void DiscardThingsOfDefCountOf(ThingDef thingDef, int componentsCountNeeded)
		{
			var list =
				(from item in this.AllCarriedThings
				 where item.def == thingDef
				 orderby item.stackCount
				 select item);

			foreach (var item in list)
			{
				int num = Math.Min(componentsCountNeeded, item.stackCount);

				item.stackCount -= num;
				componentsCountNeeded -= num;
				if (item.stackCount <= 0)
					item.Destroy();
				if (componentsCountNeeded <= 0)
					break;
			}
		}

		internal void RefuelFromInventory(float arg_totalamountToRefuel = -1)
		{
			float totalAmountToRefuel;

			if (arg_totalamountToRefuel < 0)
				totalAmountToRefuel = this.MissingFuelLevel;
			else
				totalAmountToRefuel = arg_totalamountToRefuel;

			float maxFuelPerPod = Mathf.FloorToInt(Math.Min(totalAmountToRefuel, this.CarriedFuelLevel / this.PodsCount)) + this.FuelLevelPerPod;

			int n = 0;
			var neededThings = this.AllCarriedFuelThingsOrdered.TakeWhile(
					delegate (Thing arg)
					{
						bool flag = (n < totalAmountToRefuel);
						if (!flag)
							return (false);
						n += arg.stackCount;
						return (true);
					})
					.ToList();

			var fuelEnum = neededThings.GetEnumerator();
			fuelEnum.MoveNext();

			foreach (var item in this.PodsAsThing)
			{
				var compRefuelable = item.TryGetComp<CompRefuelable>();
				var fuelPerPod = Math.Min(compRefuelable.Props.fuelCapacity, maxFuelPerPod);
#if DEBUG
				Log.Message(string.Format("pod #{0} has {1}/{2} fuel", this.PodsAsThing.FirstIndexOf((Thing arg) => arg == item), compRefuelable.Fuel, fuelPerPod));
#endif
				while (fuelPerPod > compRefuelable.Fuel)
				{
					var amountToRefuel = Math.Min(fuelPerPod - compRefuelable.Fuel, fuelEnum.Current.stackCount);
					if (amountToRefuel <= 0)
						throw new Exception("Could not refuel pods: amount to refuel = 0.");
#if DEBUG
					Log.Message(string.Format("refueling pod #{0} with {1} ({2})", this.PodsAsThing.FirstIndexOf((Thing arg) => arg == item), fuelEnum.Current.Label, amountToRefuel));
#endif
					compRefuelable.Refuel(amountToRefuel);
					fuelEnum.Current.stackCount -= Mathf.CeilToInt(amountToRefuel);
					if (fuelEnum.Current.Destroyed || fuelEnum.Current.stackCount <= 0)
					{
						if (!fuelEnum.Current.Destroyed)
							fuelEnum.Current.Destroy();
						if (!fuelEnum.MoveNext() && this.PodsAsThing.Last() != item)
							throw new Exception("Could not refuel pods: fuel missing.");
					}
				}
			}
		}

		private void Consume(float amount)
		{
			float newFuelAmountPerPod = (this.FuelLevel - amount) / this.PodsCount;

			foreach (var item in this.pods)
			{
				var compRefuelable = item.PodThing.TryGetComp<CompRefuelable>();

				compRefuelable.ConsumeFuel(Mathf.Ceil(compRefuelable.Fuel - newFuelAmountPerPod));
			}
#if DEBUG
			Log.Message("Consume(" + amount + ")");
#endif
		}
	}
}