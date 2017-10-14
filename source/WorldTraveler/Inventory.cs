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

		public float FuelLevelPerPod
		{
			get
			{
				return (this.FuelLevel / this.PodsCount);
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

		public float MaxCapacity
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

		private void Consume(float amount)
		{
			float newFuelAmountPerPod = (this.FuelLevel - amount) / this.PodsCount;

			foreach (var item in this.pods)
			{
				var compRefuelable = item.PodThing.TryGetComp<CompRefuelable>();

				compRefuelable.ConsumeFuel(Mathf.Ceil(compRefuelable.Fuel - newFuelAmountPerPod));
			}
		}
	}
}