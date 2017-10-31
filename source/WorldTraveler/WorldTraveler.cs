using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace WM.SelfLaunchingPods
{
	//TODO: split in partial
	public partial class WorldTraveler : WorldObject
	{
		// RimWorld.Planet.TravelingTransportPods
		public override Vector3 DrawPos
		{
			get
			{
				if (Traveling)
					return (Vector3.Slerp(this.Start, this.End, this.traveledPct));

				return (base.DrawPos);
			}
		}

		// RimWorld.Planet.TravelingTransportPods
		public Vector3 Start
		{
			get
			{
				return (Find.WorldGrid.GetTileCenter(this.departTile));
			}
		}

		// RimWorld.Planet.TravelingTransportPods
		public Vector3 End
		{
			get
			{
				return (Find.WorldGrid.GetTileCenter(this.destinationTile));
			}
		}

		public bool Traveling
		{
			get
			{
				return (this.destinationTile > 0);
			}
		}

		public FactionBase LocalFactionBase
		{
			get
			{
				return (Find.WorldObjects.FactionBaseAt(this.Tile));
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
					return (1f);
				}
				float num = GenMath.SphericalDistance(start.normalized, end.normalized);

				if (System.Math.Abs(num) < float.Epsilon)
				{
					return (1f);
				}
				return 0.00025f / num;
			}
		}

		List<PodPair> pods = new List<PodPair>();
		float traveledPct;
		int departTile;
		int destinationTile;
		IntVec3 destinationCell;
		PawnsArriveMode arriveMode;
		bool attackOnArrival;
		internal readonly RemoteTrader remoteTrader;
		IEnumerable<Gizmo> gizmos;

		public WorldTraveler()
		{
			remoteTrader = new RemoteTrader(this);
		}

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

		public override void SpawnSetup()
		{
			base.SpawnSetup();

			gizmos = MakeGizmos().ToList();
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

		IEnumerable<Gizmo> MakeGizmos()
		{
			yield return new Command_Launch_FromWorld(this);
			yield return new Command_Launch_FromWorld_AutoRefuel(this);

			foreach (var item in typeof(Command_Traveler).AllSubclassesNonAbstract())
			{
				yield return ((Gizmo)Activator.CreateInstance(item, new object[] { this }));
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			return (base.GetGizmos().Concat(gizmos));
		}

		public void Launch(int destinationTile, IntVec3 destinationCell, PawnsArriveMode arriveMode, bool attackOnArrival)
		{
			this.departTile = this.Tile;
			this.destinationTile = destinationTile;
			this.destinationCell = destinationCell;
			this.traveledPct = 0f;
			this.arriveMode = arriveMode;
			this.attackOnArrival = attackOnArrival;

			int distance = Find.WorldGrid.TraversalDistanceBetween(this.Tile, destinationTile);
			float fuelAmount = FuelUtils.FuelNeededToLaunchAtDistance(distance, this.PodsCount);

			Consume(fuelAmount);
		}

		// RimWorld.Planet.TravelingTransportPods
		protected void Arrived()
		{
			this.Tile = destinationTile;
			this.departTile = -1;
			this.destinationTile = -1;

			var map = Current.Game.FindMap(this.Tile);
			var mapParent = Find.WorldObjects.MapParentAt(this.Tile);

			if (map != null)
			{
				this.SpawnDropPodsInMap(map, this.destinationCell, this.arriveMode);
			}
			else
			{
				if (mapParent != null && mapParent.TransportPodsCanLandAndGenerateMap && this.attackOnArrival)
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
						this.SpawnDropPodsInMap(orGenerateMap, this.destinationCell, this.arriveMode, extraMessagePart);
					}, "GeneratingMapForNewEncounter", false, null);
				}
				else
				{
					DamagePods();
					string text = "MessageTransportPodsArrived".Translate();
					Messages.Message(text, this, MessageTypeDefOf.NeutralEvent);
				}
			}
		}

		public void SpawnDropPodsInMap(Map map, IntVec3 destinationCell, PawnsArriveMode arriveMode, string extraMessagePart = null)
		{
			//tmpFlagDroppedInMap = true;
			TravelingPodsUtils.RemoveAllPawnsFromWorldPawns(AllCarriedPawns);

			IntVec3 intVec;

			intVec = destinationCell;
			if (destinationCell.IsValid && destinationCell.InBounds(map))
			{
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

			var landingSpots = Utils.FindLandingSpotsNear(map, intVec);


			for (int i = 0; i < this.PodsCount; i++)
			{
				IntVec3 c;
				if (landingSpots.Count > i)
				{
					c = landingSpots.ElementAt(i);
					if (DebugViewSettings.drawDestSearch)
					{
						map.debugDrawer.FlashCell(c, 3f, "spot");
					}
				}
				else
				{
					VanillaLikeUtils.TryFindDropSpotNear(intVec, map, out c, false, true);
				}

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
			Messages.Message(text, new TargetInfo(intVec, map, false), MessageTypeDefOf.NeutralEvent);

			Find.WorldObjects.Remove(this);

			//TODO: dispose object
		}

		public override string GetInspectString()
		{
			string v = base.GetInspectString();

			v += "\n";
			string remainingLaunchesStr = string.Format("WM.RemainingLaunchesUntilBreakdown".Translate(), RemainingLaunches);
			v += string.Format("WM.WorldObjectLandedPodsInspectString".Translate(),
							   this.PodsCount, this.FuelLevel, FuelLevelPerPod,
							   this.MassUsage, this.MassCapacity,
							   remainingLaunchesStr,
							   this.CarriedFuelLevel, this.CarriedFuelLevel / this.PodsCount);

			return (v);
		}
	}
}
