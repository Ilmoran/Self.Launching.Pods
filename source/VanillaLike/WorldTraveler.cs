using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace WM.SelfLaunchingPods
{
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
				return AllCarriedThings.Where(arg => arg.def == DefOf.Chemfuel).Sum(arg => arg.stackCount) +
					   Things.Sum(arg => arg.TryGetComp<CompRefuelable>().Fuel);
			}
		}

		public float FuelLevelPerPod
		{
			get
			{
				return this.FuelLevel / this.PodsCount;
			}
		}

		public int MaxLaunchDistance
		{
			get
			{
				return TravelingPodsUtils.MaxLaunchDistance(this.FuelLevel, this.PodsCount);
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
		public IEnumerable<Thing> Things
		{
			get
			{
				return pods.Select(arg => arg.PodThing);
			}
		}
		public float MaxCapacity
		{
			get
			{
				return Things.Sum(arg => arg.TryGetComp<CompTransporter>().Props.massCapacity);
			}
		}
		public float MassUsage
		{
			get
			{
				//return CollectionsMassCalculator.MassUsage(this.AllCarriedThings.Select(arg => arg.to), false, true, false);
				return this.AllCarriedThings.Sum(arg => arg.GetStatValue(StatDefOf.Mass));
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

			Scribe_Values.Look<float>(ref traveledPct, "traveledPct");
			Scribe_Values.Look<int>(ref departTile, "departTile");
			Scribe_Values.Look<int>(ref destinationTile, "destinationTile");
			Scribe_Values.Look<IntVec3>(ref destinationCell, "destinationCell");
			Scribe_Values.Look<PawnsArriveMode>(ref arriveMode, "arriveMode");
			Scribe_Values.Look<bool>(ref attackOnArrival, "attackOnArrival");
			Scribe_Collections.Look<PodPair>(ref pods, "pods", LookMode.Deep);
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
				Gizmo LaunchGizmo = new Command_Launch_FromWorld(this);
				yield return LaunchGizmo;
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
			//newFuelAmountPerPod = Mathf.Ceil(newFuelAmountPerPod);

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
		public void SpawnDropPodsInMap(Map map, IntVec3 destinationCell, PawnsArriveMode arriveMode, string extraMessagePart = null)
		{
			//tmpFlagDroppedInMap = true;
			TravelingPodsUtils.RemoveAllPawnsFromWorldPawns(AllCarriedPawns);

			IntVec3 intVec;
			if (destinationCell != null && destinationCell.IsValid && destinationCell.InBounds(map))
			{
#if DEBUG
				Log.Message("dropping at targeted cell: " + destinationCell);
#endif
				intVec = destinationCell;
			}
			else if (arriveMode == PawnsArriveMode.CenterDrop)
			{
				if (!DropCellFinder.TryFindRaidDropCenterClose(out intVec, map))
				{
					intVec = DropCellFinder.FindRaidDropCenterDistant(map);
				}
			}
			else
			{
				if (arriveMode != PawnsArriveMode.EdgeDrop && arriveMode != PawnsArriveMode.Undecided)
				{
					Log.Warning("Unsupported arrive mode " + arriveMode);
				}
				intVec = DropCellFinder.FindRaidDropCenterDistant(map);
			}

			// ---------- mod ----------------

			int i = 0;

			//			if (this.def != DefOf.WM_TravelingTransportPods)
			//			{
			//				if (this.arriveMode == PawnsArriveMode.Undecided && this.Faction == Faction.OfPlayer)
			//				{
			//					List<IntVec3> landingPads = Utils.FindLandingPads(map, intVec);

			//#if DEBUG
			//					Log.Message("Found " + landingPads.Count + " pads for landing");
			//#endif

			//					for (; i < this.podsInfo.Count() && landingPads.Count > padNum; i++)
			//					{
			//						RimWorld.DropPodUtility.MakeDropPodAt(landingPads[padNum++], map, this.podsInfo.ElementAt(i));
			//					}
			//				}

			//#if DEBUG
			//				Log.Message(padNum + " pods landed to pads for landing");
			//#endif
			//			}
			// ---------- mod end ------------

			for (; i < this.PodsCount; i++)
			{
				IntVec3 c;
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

			//TODO: dispose objt
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
