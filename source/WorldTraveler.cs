using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace WM.SelfLaunchingPods
{
	//TODO: tick pawns
	public class WorldTraveler : WorldObject
	{
		// RimWorld.Planet.TravelingTransportPods
		public override Vector3 DrawPos
		{
			get
			{
				if (Traveling)
					return Vector3.Slerp(this.Start, this.End, this.traveledPct);
				else
					return base.DrawPos;
			}
		}

		// RimWorld.Planet.TravelingTransportPods
		private Vector3 Start
		{
			get
			{
				return Find.WorldGrid.GetTileCenter(this.departTile);
			}
		}

		// RimWorld.Planet.TravelingTransportPods
		private Vector3 End
		{
			get
			{
				return Find.WorldGrid.GetTileCenter(this.destinationTile);
			}
		}

		public bool Traveling
		{
			get
			{
				return this.destinationTile > 0;
			}
		}

		public float FuelLevel
		{
			get
			{
				return AllCarriedThings.Where((Thing arg) => arg.def == DefOf.Chemfuel).Sum(arg => arg.stackCount) +
					   PodsAsThing.Sum(arg => arg.TryGetComp<CompRefuelable>().Fuel);
			}
		}

		public float FuelLevelPerPod
		{
			get
			{
				return this.FuelLevel / this.PodsCount;
			}
		}

		public int PodsCount
		{
			get
			{
				return this.pods.Count;
			}
		}

		public IEnumerable<Thing> AllCarriedThings
		{
			get
			{
				return this.pods.SelectMany((arg) => arg.PodInfo.innerContainer);
			}
		}

		public IEnumerable<Pawn> AllCarriedPawns
		{
			get
			{
				return AllCarriedThings.Where(arg => arg is Pawn).Cast<Pawn>();
			}
		}

		public IEnumerable<Pawn> AllCarriedColonists
		{
			get
			{
				return AllCarriedPawns.Where(arg => arg.IsColonist);
			}
		}

		public IEnumerable<Thing> AllCarriedNonPawnThings
		{
			get
			{
				return AllCarriedThings.Where((Thing arg) => !(arg is Pawn));
			}
		}

		public bool HasPawns
		{
			get
			{
				return AllCarriedPawns.Any();
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

		// RimWorld.Planet.TravelingTransportPods
		private float TraveledPctStepPerTick
		{
			get
			{
				Vector3 start = this.Start;
				Vector3 end = this.End;
				if (start == end)
				{
					return 1f;
				}
				float num = GenMath.SphericalDistance(start.normalized, end.normalized);
				if (num == 0f)
				{
					return 1f;
				}
				return 0.00025f / num;
			}
		}

		List<PodPair> pods = new List<PodPair>();

		float traveledPct;

		int departTile;
		internal int destinationTile;
		internal IntVec3 destinationCell;
		internal PawnsArriveMode arriveMode;
		internal bool attackOnArrival;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<float>			(ref traveledPct, 		"traveledPct");
			Scribe_Values.Look<int>				(ref departTile, 		"departTile");
			Scribe_Values.Look<int>				(ref destinationTile, 	"destinationTile");
			Scribe_Values.Look<IntVec3>			(ref destinationCell, 	"destinationCell");
			Scribe_Values.Look<PawnsArriveMode>	(ref arriveMode, 		"arriveMode");
			Scribe_Values.Look<bool>			(ref attackOnArrival, 	"attackOnArrival");
			Scribe_Collections.Look<PodPair>	(ref pods, 				"pods", LookMode.Deep);
		}

		public override void Tick()
		{
			base.Tick();

			if (Traveling)
			{
				this.traveledPct += this.TraveledPctStepPerTick;
				if (this.traveledPct >= 1f)
				{
					this.traveledPct = 1f;
					this.Arrived();
				}
			}
		}

		public void AddPod(Thing building, ActiveDropPodInfo podInfo)
		{
			var pair = new PodPair(building, podInfo);
			this.pods.Add(pair);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (var item in base.GetGizmos())
				yield return item;

			if (!Traveling)
			{
				yield return new Command_Launch_FromWorld(this);
				yield return new Command_UnloadCaravan_PawnsAndItems(this);
				yield return new Command_UnloadCaravan_Pawns(this);
				yield return new Command_UnloadCaravan_Items(this);
			}
		}

		public virtual void Launch(int destinationTile, IntVec3 destinationCell)
		{
			this.departTile = this.Tile;
			this.destinationTile = destinationTile;
			this.destinationCell = destinationCell;
			this.traveledPct = 0f;

			int distance = Find.WorldGrid.TraversalDistanceBetween(this.Tile, destinationTile);
			float fuelAmount = TravelingPodsUtils.FuelNeededToLaunchAtDistance(distance, this.PodsCount);

			Consume(fuelAmount);
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

		// RimWorld.Planet.TravelingTransportPods
		protected virtual void Arrived()
		{
			this.Tile = destinationTile;
			this.departTile = -1;
			this.destinationTile = -1;

			Map map = Current.Game.FindMap(this.Tile);
			MapParent mapParent = Find.WorldObjects.MapParentAt(this.Tile);

			if (map != null)
			{
				this.SpawnDropPodsInMap(map, this.destinationCell, this.arriveMode);

				Find.WorldObjects.Remove(this);
			}
			//else if (!this.PodsHaveAnyPotentialCaravanOwner)
			//{
			//	Caravan caravan = Find.WorldObjects.PlayerControlledCaravanAt(this.destinationTile);
			//	if (caravan != null)
			//	{
			//		this.GivePodContentsToCaravan(caravan);
			//	}
			//	else
			//	{
			//		for (int i = 0; i < this.pods.Count; i++)
			//		{
			//			this.pods[i].innerContainer.ClearAndDestroyContentsOrPassToWorld(DestroyMode.Vanish);
			//		}
			//		this.RemoveAllPods();
			//		Find.WorldObjects.Remove(this);
			//		Messages.Message("MessageTransportPodsArrivedAndLost".Translate(), new GlobalTargetInfo(this.destinationTile), MessageSound.Negative);
			//	}
			//}
			else if (mapParent != null && mapParent.TransportPodsCanLandAndGenerateMap && this.attackOnArrival)
			{
				LongEventHandler.QueueLongEvent(delegate
				{
					Map orGenerateMap = GetOrGenerateMapUtility.GetOrGenerateMap(mapParent.Tile, null);
					string extraMessagePart = null;
					if (mapParent.Faction != null && !mapParent.Faction.HostileTo(Faction.OfPlayer))
					{
						mapParent.Faction.SetHostileTo(Faction.OfPlayer, true);
						extraMessagePart = "MessageTransportPodsArrived_BecameHostile".Translate(new object[]
						{
						mapParent.Faction.Name
						}).CapitalizeFirst();
					}
					Find.TickManager.CurTimeSpeed = TimeSpeed.Paused;
					this.SpawnDropPodsInMap(orGenerateMap, this.destinationCell, this.arriveMode);
				}, "GeneratingMapForNewEncounter", false, null);

				Find.WorldObjects.Remove(this);
			}
			else
			{
				string text = "MessageTransportPodsArrived".Translate();
				//if (extraMessagePart != null)
				//{
				//	text = text + " " + extraMessagePart;
				//}
				Messages.Message(text, new GlobalTargetInfo(this.Tile), MessageSound.Benefit);
			}
		}

		//TODO: fix bug when traveling inside a map. pods will land on edge.
		public void SpawnDropPodsInMap(Map map, IntVec3 destinationCell, PawnsArriveMode arriveMode, string extraMessagePart = null)
		{
			//tmpFlagDroppedInMap = true;
			TravelingPodsUtils.RemoveAllPawnsFromWorldPawns(AllCarriedPawns);

			IntVec3 intVec;

			intVec = destinationCell;
			if (destinationCell != null && destinationCell.IsValid && destinationCell.InBounds(map))
			{
#if DEBUG
				Log.Message("dropping at targeted cell: " + destinationCell);
#endif
				intVec = destinationCell;
			}
//			else if (arriveMode == PawnsArriveMode.CenterDrop)
//			{
//				if (!DropCellFinder.TryFindRaidDropCenterClose(out intVec, map))
//				{
//					intVec = DropCellFinder.FindRaidDropCenterDistant(map);
//				}
//			}
			else
			{
				if (arriveMode != PawnsArriveMode.EdgeDrop && arriveMode != PawnsArriveMode.Undecided)
				{
					Log.Warning("Unsupported arrive mode " + arriveMode);
				}
				intVec = DropCellFinder.FindRaidDropCenterDistant(map);
			}

			var	landingSpots = Utils.FindLandingSpotsNear(map, intVec);

#if DEBUG
			Log.Message("landingSpots found in range = " + landingSpots.Count());
#endif

			for (int i = 0; i < this.PodsCount; i++)
			{
				IntVec3 c;

				if (landingSpots.Count() > i)
					c = landingSpots.ElementAt(i);
				else
					DropCellFinder.TryFindDropSpotNear(intVec, map, out c, false, true);

				// ---------- mod ----------------
				//if (this.def == DefOf.WM_TravelingTransportPods)
				{
					var pair = this.pods.ElementAt(i);

					DropPodUtility.MakeDropPodAt(c, map, pair.PodInfo, pair.PodThing);
				}
				// ---------- mod end ------------
			}

			string text = "MessageTransportPodsArrived".Translate();

			if (extraMessagePart != null)
			{
				text = text + " " + extraMessagePart;
			}
			Messages.Message(text, new TargetInfo(intVec, map, false), MessageSound.Benefit);

			//TODO: dispose object
		}

		public override string GetInspectString()
		{
			string v = base.GetInspectString();

			v += "\r\n";

			v += string.Format("WM.WorldObjectLandedPodsInspectString".Translate(), this.PodsCount, this.FuelLevel, FuelLevelPerPod, this.MassUsage, this.MaxCapacity);

			return v;
		}

	}
}
