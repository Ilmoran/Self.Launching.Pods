using System.Collections.Generic;
using System.Linq;
using Harmony;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Sound;

namespace WM.SelfLaunchingPods
{
	//TODO: CompTick() does not work (see tick type)
	//TODO: Fix carried item spawning over pod.
	public class CompSelfLaunchable : ThingComp
	{
		public ActiveDropPodInfo podInfo;
		private int ticksUntilOpen = 100;
		private CompTransporter cachedCompTransporter;

		public CompProperties_SelfLaunchable Props
		{
			get
			{
				return (props as CompProperties_SelfLaunchable);

			}
		}

		private int MaxLaunchDistance
		{
			get
			{
				return FuelUtils.MaxLaunchDistance(this.FuelInLeastFueledFuelingPortSource, 1, true);
			}
		}

		public CompRefuelable Refuelable
		{
			get
			{
				return (this.parent.GetComp<CompRefuelable>());
			}
		}

		public float FuelLevel
		{
			get
			{
				return (Refuelable.Fuel);
			}
		}

		public float MaxFuelLevel
		{
			get
			{
				return (Refuelable.Props.fuelCapacity);
			}
		}

		// RimWorld.CompLaunchable
		public CompTransporter Transporter
		{
			get
			{
				if (this.cachedCompTransporter == null)
				{
					this.cachedCompTransporter = this.parent.GetComp<CompTransporter>();
				}
				return (this.cachedCompTransporter);
			}
		}

		// RimWorld.CompLaunchable
		public List<CompTransporter> TransportersInGroup
		{
			get
			{
				return this.Transporter.TransportersInGroup(this.parent.Map);
			}
		}

		public float FuelInLeastFueledFuelingPortSource
		{
			get
			{
				if (!TransportersInGroup.Any())
					return (0f);

				var result = TransportersInGroup.Min((arg) => arg.parent.TryGetComp<CompRefuelable>().Fuel);
				return (result);
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

		//public override void CompTick()
		//{
		//	base.CompTick();
		//	if (podInfo == null)
		//		return;

		//	if (ticksUntilOpen <= 0)
		//	{
		//		PodOpen();
		//		return;
		//	}
		//	ticksUntilOpen--;
		//}

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

			var transportersInGroup = this.TransportersInGroup;

#if DEBUG
			Log.Message("TryLaunch(): transportersInGroup = " + transportersInGroup.Count);
#endif

			if (transportersInGroup == null)
			{
				Log.Error("Tried to launch " + this.parent + ", but it's not in any group.");
				return;
			}
			if (!this.Transporter.LoadingInProgressOrReadyToLaunch || this.FuelInLeastFueledFuelingPortSource <= 0)
			{
				return;
			}

			Map map = this.parent.Map;
			int num = Find.WorldGrid.TraversalDistanceBetween(map.Tile, target.Tile);

			if (num > this.MaxLaunchDistance)
			{
				return;
			}
			this.Transporter.TryRemoveLord(map);

			int groupID = this.Transporter.groupID;
			float amount = FuelUtils.FuelNeededToLaunchAtDistance(num, 1);

			foreach (CompTransporter compTransporter in transportersInGroup)
			{
				//#if DEBUG
				//				Log.Message("TryLaunch(): i = " + i);
				//#endif
				var dropPodLeaving = (DropPodLeaving)ThingMaker.MakeThing(DefOf.WM_DropPodLeaving, null);

				dropPodLeaving.groupID = groupID;
				dropPodLeaving.destinationTile = target.Tile;
				dropPodLeaving.destinationCell = target.Cell;
				dropPodLeaving.arriveMode = arriveMode;
				dropPodLeaving.attackOnArrival = attackOnArrival;
				dropPodLeaving.landedThing = compTransporter.parent; // MOD

				ThingOwner directlyHeldThings = compTransporter.GetDirectlyHeldThings();
				dropPodLeaving.Contents = new ActiveDropPodInfo();
				dropPodLeaving.Contents.innerContainer.TryAddRangeOrTransfer(directlyHeldThings, true);
				directlyHeldThings.Clear();

				GenSpawn.Spawn(dropPodLeaving, compTransporter.parent.Position, map);
			}

			foreach (CompTransporter compTransporter in transportersInGroup)
			{
				compTransporter.CleanUpLoadingVars(map);
				compTransporter.parent.DeSpawn();
			}
		}

		public override void PostDraw()
		{
			Utils.DrawFuelOverlay(FuelLevel / MaxFuelLevel, this.parent.DrawPos);
		}
	}
}
