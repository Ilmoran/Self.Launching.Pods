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
				return (this.AllCarriedThings.Where((Thing arg) => arg.def == DefOf.Chemfuel)
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

		internal void RefuelFromInventory()
		{
#if DEBUG
			Log.Message("RefuelFromInventory()");
#endif
			float totalAmountToRefuel = this.MissingFuelLevel;
			float fuelPerPod = Mathf.FloorToInt((Math.Min(this.CarriedFuelLevel, totalAmountToRefuel) / this.PodsCount));

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

				while (fuelPerPod > compRefuelable.Fuel)
				{
					var amountToRefuel = Math.Min(fuelPerPod - compRefuelable.Fuel, fuelEnum.Current.stackCount);
#if DEBUG
					Log.Message(string.Format("refueling pod #{0} with {1} ({2})", this.PodsAsThing.FirstIndexOf((Thing arg) => arg == item), fuelEnum.Current.Label, amountToRefuel));
#endif
					if (fuelEnum.Current.stackCount <= amountToRefuel)
					{
						compRefuelable.Refuel(fuelEnum.Current);
						if (!fuelEnum.MoveNext() && this.PodsAsThing.Last() != item)
							throw new Exception("Could not refuel pods");
					}
					else
					{
						compRefuelable.Refuel(amountToRefuel);
						fuelEnum.Current.stackCount -= Mathf.CeilToInt(amountToRefuel);
					}
				}
			}
		}
		//		internal void RefuelFromInventory(float amount)
		//		{
		//			float fuelPerPod = Mathf.FloorToInt((Math.Min(this.CarriedFuelLevel, amount) / this.PodsCount));

		//			foreach (var item in this.PodsAsThing)
		//			{
		//				int n = 0;
		//				var neededThings = this.AllCarriedFuelThings.TakeWhile(
		//						delegate (Thing arg)
		//						{
		//							bool flag = (n < fuelPerPod);
		//							if (!flag)
		//								return (false);
		//							n += arg.stackCount;
		//							return (true);
		//						})
		//						.ToList();
		//
		//#if DEBUG
		//				Log.Message("fuelneed = " + amount + " neededThings count = " + neededThings.Count());
		//#endif

		//				var compRefuelable = item.TryGetComp<CompRefuelable>();
		//				Thing finalFuelThing = neededThings.Last();

		//				foreach (var item2 in neededThings.Take(neededThings.Count() - 1))
		//				{
		//					if (item2.stackCount > compRefuelable.GetFuelCountToFullyRefuel())
		//					{
		//						finalFuelThing = item2;
		//						break;
		//					}
		//					compRefuelable.Refuel(item2);
		//				}

		//				var v = n - (int)amount;

		//				compRefuelable.Refuel(v);
		//				finalFuelThing.stackCount -= v;
		//				if (finalFuelThing.stackCount <= 0)
		//					finalFuelThing.Destroy();
		//			}
		//		}

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