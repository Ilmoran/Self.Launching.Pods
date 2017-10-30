using System.Collections.Generic;
using Harmony;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Sound;

namespace WM.SelfLaunchingPods
{
	//TODO: CompTick() does not work (see tick type)
	//TODO: Fix carried item spawning over pod.
	public class CompSelfLaunchable : RimWorld.CompLaunchable
	{
		public ActiveDropPodInfo podInfo;
		int ticksUntilOpen = 100;

		private int _MaxLaunchDistance
		{
			get
			{
				return FuelUtils.MaxLaunchDistance(this._FuelInLeastFueledFuelingPortSource, 1, true);
			}
		}

		public float _FuelInLeastFueledFuelingPortSource
		{
			get
			{
				return ((float)Traverse.Create(this as RimWorld.CompLaunchable).Property("FuelInLeastFueledFuelingPortSource").GetValue());
			}
		}

		public CompProperties_SelfLaunchable Props
		{
			get
			{
				return props as CompProperties_SelfLaunchable;
			}
		}

		public new bool ConnectedToFuelingPort { get { return (true); } }
		public new bool AllInGroupConnectedToFuelingPort { get { return (true); } }

		// RimWorld.CompLaunchable
		public new bool FuelingPortSourceHasAnyFuel
		{
			get
			{
				var compRefuelable = ((Building)parent).TryGetComp<CompRefuelable>();
				return (compRefuelable.Fuel > 0f);
			}
		}
		// RimWorld.CompLaunchable
		public new float FuelingPortSourceFuel
		{
			get
			{
				var compRefuelable = ((Building)parent).TryGetComp<CompRefuelable>();
				if (compRefuelable != null)
					return (compRefuelable.Fuel);
				return (0f);
			}
		}
		// RimWorld.CompLaunchable
		public new Building FuelingPortSource
		{
			get
			{
				return ((Building)parent);
			}
		}

		public float MaxFuelLevel
		{
			get
			{
				return (this.parent.TryGetComp<CompRefuelable>().Props.fuelCapacity);
			}
		}

		public override void PostExposeData()
		{
			base.PostExposeData();

			Scribe_Values.Look(ref ticksUntilOpen, "ticksUntilOpen");
			Scribe_Deep.Look(ref podInfo, "podInfo");
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);

			if (respawningAfterLoad)
				return;

			PodOpen();
		}

		public override void CompTick()
		{
			base.CompTick();
			if (podInfo == null)
				return;

			if (ticksUntilOpen <= 0)
			{
				PodOpen();
				return;
			}
			ticksUntilOpen--;
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (parent.TryGetComp<CompTransporter>().LoadingInProgressOrReadyToLaunch)
				yield return new Command_Launch_FromMap(parent);
		}

		// RimWorld.ActiveDropPod
		private void PodOpen()
		{
			if (podInfo == null)
				return;

			for (int i = podInfo.innerContainer.Count - 1; i >= 0; i--)
			{
				Thing thing = podInfo.innerContainer[i];
				Thing thing2;

				GenPlace.TryPlaceThing(thing, parent.Position, this.parent.Map, ThingPlaceMode.Near, out thing2,
#pragma warning disable RECS0154 // Parameter is never used
					delegate (Thing placedThing, int count)
#pragma warning restore RECS0154 // Parameter is never used
					{
						if (Find.TickManager.TicksGame < 1200 && TutorSystem.TutorialMode && placedThing.def.category == ThingCategory.Item)
						{
							Find.TutorialState.AddStartingItem(placedThing);
						}
					});
				//GenPlace.TryPlaceThing(thing, this.parent.Position, this.parent.Map, ThingPlaceMode.Near, out thing2);
				var pawn = thing2 as Pawn;

				if (pawn != null)
				{
					if (pawn.RaceProps.Humanlike)
					{
						TaleRecorder.RecordTale(TaleDefOf.LandedInPod, new object[]
						{
					pawn
						});
					}
					if (pawn.IsColonist && pawn.Spawned && !parent.Map.IsPlayerHome)
					{
						pawn.drafter.Drafted = true;
					}
				}
			}

			podInfo.innerContainer.ClearAndDestroyContents(DestroyMode.Vanish);
			//if (podInfo.leaveSlag)
			//{
			//	for (int j = 0; j < 1; j++)
			//	{
			//		Thing thing3 = ThingMaker.MakeThing(ThingDefOf.ChunkSlagSteel, null);
			//		GenPlace.TryPlaceThing(thing3, parent.Position, parent.Map, ThingPlaceMode.Near, null);
			//	}
			//}
			SoundDef.Named("DropPodOpen").PlayOneShot(new TargetInfo(parent.Position, parent.Map, false));

			podInfo = null;
		}

		// RimWorld.CompLaunchable
		internal void TryLaunch(GlobalTargetInfo target, PawnsArriveMode arriveMode, bool attackOnArrival)
		{
			if (!this.parent.Spawned)
			{
				Log.Error("Tried to launch " + this.parent + ", but it's unspawned.");
				return;
			}

			List<CompTransporter> transportersInGroup = this.TransportersInGroup;

			if (transportersInGroup == null)
			{
				Log.Error("Tried to launch " + this.parent + ", but it's not in any group.");
				return;
			}
			if (!this.LoadingInProgressOrReadyToLaunch || !this.AllInGroupConnectedToFuelingPort || !this.AllFuelingPortSourcesInGroupHaveAnyFuel)
			{
				return;
			}

			Map map = this.parent.Map;
			int num = Find.WorldGrid.TraversalDistanceBetween(map.Tile, target.Tile);

			if (num > this._MaxLaunchDistance)
			{
				return;
			}
			this.Transporter.TryRemoveLord(map);

			int groupID = this.Transporter.groupID;
			float amount = FuelUtils.FuelNeededToLaunchAtDistance(num, 1);

			for (int i = 0; i < transportersInGroup.Count; i++)
			{
				CompTransporter compTransporter = transportersInGroup[i];
				var dropPodLeaving = (DropPodLeaving)ThingMaker.MakeThing(DefOf.WM_DropPodLeaving, null);

				dropPodLeaving.groupID = groupID;
				dropPodLeaving.destinationTile = target.Tile;
				dropPodLeaving.destinationCell = target.Cell;
				dropPodLeaving.arriveMode = arriveMode;
				dropPodLeaving.attackOnArrival = attackOnArrival;

				ThingOwner directlyHeldThings = compTransporter.GetDirectlyHeldThings();

				dropPodLeaving.Contents = new ActiveDropPodInfo();
				dropPodLeaving.Contents.innerContainer.TryAddRangeOrTransfer(directlyHeldThings, true);

				directlyHeldThings.Clear();

				compTransporter.CleanUpLoadingVars(map);
				compTransporter.parent.DeSpawn();
				dropPodLeaving.landedThing = compTransporter.parent;

				GenSpawn.Spawn(dropPodLeaving, compTransporter.parent.Position, map);
			}
		}

		public override void PostDraw()
		{
			Utils.DrawFuelOverlay(FuelingPortSourceFuel / MaxFuelLevel, this.parent.DrawPos);
		}
	}
}
