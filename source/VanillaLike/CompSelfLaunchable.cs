using System;
using System.Collections;
using System.Collections.Generic;
using Harmony;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WM.SelfLaunchingPods
{
	public class CompSelfLaunchable : RimWorld.CompLaunchable
	{
		public CompSelfLaunchable()
		{
		}

		public ActiveDropPodInfo podInfo;
		int ticksUntilOpen = 100;

		private int _MaxLaunchDistance
		{
			get
			{
				return (int)Traverse.Create(this as RimWorld.CompLaunchable).Property("MaxLaunchDistance").GetValue();
			}
		}

		public CompProperties_SelfLaunchable Props
		{
			get
			{
				return this.props as CompProperties_SelfLaunchable;
			}
		}

		public new bool ConnectedToFuelingPort { get { return true; } }
		public new bool AllInGroupConnectedToFuelingPort { get { return true; } }

		// RimWorld.CompLaunchable
		public new bool FuelingPortSourceHasAnyFuel
		{
			get
			{
				var compRefuelable = ((Building)this.parent).TryGetComp<CompRefuelable>();
				return compRefuelable.Fuel > 0f;
			}
		}
		// RimWorld.CompLaunchable
		public new float FuelingPortSourceFuel
		{
			get
			{
				var compRefuelable = ((Building)this.parent).TryGetComp<CompRefuelable>();
				if (compRefuelable != null)
					return compRefuelable.Fuel;
				else
					return 0f;
			}
		}
		// RimWorld.CompLaunchable
		public new Building FuelingPortSource
		{
			get
			{
				return (Building)this.parent;
			}
		}

		public override void PostExposeData()
		{
			base.PostExposeData();

			Scribe_Values.Look<int>(ref ticksUntilOpen, "ticksUntilOpen");
			Scribe_Deep.Look<ActiveDropPodInfo>(ref podInfo, "podInfo");
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);

			if (respawningAfterLoad)
				return;

			PodOpen();
		}

		//TODO: CompTick() does not work
		public override void CompTick()
		{
			base.CompTick();
			Log.Message("CompTick()");

			if (podInfo == null)
				return;

			if (ticksUntilOpen <= 0)
			{
				PodOpen();
			}
			ticksUntilOpen--;
		}

		// RimWorld.ActiveDropPod
		private void PodOpen()
		{
			Log.Message("PodOpen()");

			if (podInfo == null)
				return;

			for (int i = podInfo.innerContainer.Count - 1; i >= 0; i--)
			{
				Thing thing = podInfo.innerContainer[i];
				Thing thing2;
				GenPlace.TryPlaceThing(thing, this.parent.Position, this.parent.Map, ThingPlaceMode.Near, out thing2, delegate (Thing placedThing, int count)
				{
					if (Find.TickManager.TicksGame < 1200 && TutorSystem.TutorialMode && placedThing.def.category == ThingCategory.Item)
					{
						Find.TutorialState.AddStartingItem(placedThing);
					}
				});
				Pawn pawn = thing2 as Pawn;
				if (pawn != null)
				{
					if (pawn.RaceProps.Humanlike)
					{
						TaleRecorder.RecordTale(TaleDefOf.LandedInPod, new object[]
						{
					pawn
						});
					}
					if (pawn.IsColonist && pawn.Spawned && !this.parent.Map.IsPlayerHome)
					{
						pawn.drafter.Drafted = true;
					}
				}
			}
			podInfo.innerContainer.ClearAndDestroyContents(DestroyMode.Vanish);
			if (podInfo.leaveSlag)
			{
				for (int j = 0; j < 1; j++)
				{
					Thing thing3 = ThingMaker.MakeThing(ThingDefOf.ChunkSlagSteel, null);
					GenPlace.TryPlaceThing(thing3, this.parent.Position, this.parent.Map, ThingPlaceMode.Near, null);
				}
			}
			SoundDef.Named("DropPodOpen").PlayOneShot(new TargetInfo(this.parent.Position, this.parent.Map, false));
			//this.Destroy(DestroyMode.Vanish);

			podInfo = null;
		}

		// RimWorld.CompLaunchable
		internal void TryLaunch(GlobalTargetInfo target, PawnsArriveMode arriveMode, bool attackOnArrival)
		{
#if DEBUG
			Log.Message("TryLaunch()");
#endif
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
			float amount = TravelingPodsUtils.FuelNeededToLaunchAtDistance(num, 1);
			for (int i = 0; i < transportersInGroup.Count; i++)
			{
				CompTransporter compTransporter = transportersInGroup[i];
				Building fuelingPortSource = compTransporter.Launchable.FuelingPortSource;
				if (fuelingPortSource != null)
				{
					fuelingPortSource.TryGetComp<CompRefuelable>().ConsumeFuel(amount);
				}
				var dropPodLeaving = (DropPodLeaving)ThingMaker.MakeThing(DefOf.WM_DropPodLeaving, null);

				dropPodLeaving.groupID = groupID;
				dropPodLeaving.destinationTile = target.Tile;
				dropPodLeaving.destinationCell = target.Cell;
				dropPodLeaving.arriveMode = arriveMode;
				dropPodLeaving.attackOnArrival = attackOnArrival;

				ThingOwner directlyHeldThings = compTransporter.GetDirectlyHeldThings();

				dropPodLeaving.Contents = new ActiveDropPodInfo();
				dropPodLeaving.Contents.innerContainer.TryAddRange(directlyHeldThings, true);

				var compRefuelable = this.parent.TryGetComp<CompRefuelable>();
				//dropPodLeaving.FuelQuantity = compRefuelable.Fuel;
				//compRefuelable.ConsumeFuel(compRefuelable.Fuel);

				directlyHeldThings.Clear();

				compTransporter.CleanUpLoadingVars(map);
				compTransporter.parent.DeSpawn();
				dropPodLeaving.landedThing = compTransporter.parent;

				GenSpawn.Spawn(dropPodLeaving, compTransporter.parent.Position, map);
			}

			//throw new NotImplementedException();
		}


		//public override IEnumerable<Gizmo> CompGetGizmosExtra()
		//{
		//	var launchgizmo = new Command_Action();
		//	launchgizmo.defaultLabel = "Launch";
		//	launchgizmo.action = delegate
		//	{
		//		Harmony.Traverse.Create(this.Transporter).Method("StartChoosingDestination").GetValue();
		//	};
		//	yield return launchgizmo;
		//	//var baseenum = base.CompGetGizmosExtra();
		//	//var hookenum = new CompLaunchGizmoHook(baseenum.GetEnumerator());

		//	//return hookenum;
		//}

		//public override IEnumerable<Gizmo> CompGetGizmosExtra()
		//{
		//	foreach (var item in base.CompGetGizmosExtra())
		//	{
		//		if (item is Command_Action && ((Command_Action)item).Label == "CommandLaunchGroup".Translate())
		//		{
		//			((Command_Action)item).defaultLabel = "test";
		//		}
		//		yield return item;
		//	}
		//}
	}


	//public class CompLaunchGizmoHook : EnumeratorHook<Gizmo>
	//{
	//	CompLaunchable compl;
	//	CompTransporter compt;

	//	public CompLaunchGizmoHook(IEnumerator<Gizmo> test)
	//	{
	//		this.baseenum = test;
	//	}
	//	protected override Gizmo Hook(Gizmo current)
	//	{
	//		if (current is Command_Action)
	//		{
	//			var command = (Command_Action)current;

	//			if (command.Label == "CommandLaunchGroup".Translate())
	//			{
	//				command.action = delegate
	//				{
	//					if (compl.AnyInGroupHasAnythingLeftToLoad)
	//					{
	//						Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmSendNotCompletelyLoadedPods".Translate(new object[]
	//						{
	//				compl.FirstThingLeftToLoadInGroup.LabelCapNoCount
	//						}), new Action(delegate
	//						{
	//							StartChoosingDestination();
	//						}), false, null));
	//					}
	//					else
	//					{
	//						StartChoosingDestination();
	//					}
	//				};
	//			}
	//		}
	//		return base.Hook(current);
	//	}

	//	void StartChoosingDestination()
	//	{
	//		Harmony.Traverse.Create(compl).Method("StartChoosingDestination", null);
	//	}
	//}
}
