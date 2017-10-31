using System.Collections.Generic;
using System.Linq;
using Harmony;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace WM.SelfLaunchingPods
{
	public class DropPodLeaving : Skyfaller, IActiveDropPod, IThingHolder
	{
		//=========== mod ============

		public Thing landedThing;

		// ============ /mod ============

		public int groupID = -1;
		public int destinationTile = -1;
		public IntVec3 destinationCell = IntVec3.Invalid;
		public PawnsArriveMode arriveMode = PawnsArriveMode.Undecided;
		public bool attackOnArrival;
		private bool alreadyLeft;
		private static List<Thing> tmpActiveDropPods = new List<Thing>();

		public ActiveDropPodInfo Contents
		{
			get; internal set;
			//get
			//{
			//	return ((ActiveDropPod)this.innerContainer[0]).Contents;
			//}
			//set
			//{
			//	((ActiveDropPod)this.innerContainer[0]).Contents = value;
			//}
		}

		public DropPodLeaving()
		{
			this.innerContainer = new ThingOwner<Thing>(this);
		}

		// RimWorld.Skyfaller
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.Look<ThingOwner>(ref this.innerContainer, "innerContainer", new object[]
			{
				this
			});
			Scribe_Values.Look<int>(ref this.ticksToImpact, "ticksToImpact", 0, false);
			Scribe_Values.Look<float>(ref this.angle, "angle", 0f, false);
			Scribe_Values.Look<float>(ref this.shrapnelDirection, "shrapnelDirection", 0f, false);
		}

		public override void PostMake()
		{
			base.PostMake();


		}

		protected override void LeaveMap()
		{
			if (this.alreadyLeft)
			{
				base.LeaveMap();
			}
			else if (this.groupID < 0)
			{
				Log.Error("Drop pod left the map, but its group ID is " + this.groupID);
				this.Destroy(DestroyMode.Vanish);
			}
			else if (this.destinationTile < 0)
			{
				Log.Error("Drop pod left the map, but its destination tile is " + this.destinationTile);
				this.Destroy(DestroyMode.Vanish);
			}
			else
			{
				Lord lord = TransporterUtility.FindLord(this.groupID, base.Map);

				if (lord != null)
				{
					base.Map.lordManager.RemoveLord(lord);
				}

				//TravelingTransportPods travelingTransportPods = (TravelingTransportPods)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.TravelingTransportPods);
				//travelingTransportPods.Tile = base.Map.Tile;
				//travelingTransportPods.SetFaction(Faction.OfPlayer);
				//travelingTransportPods.destinationTile = this.destinationTile;
				//travelingTransportPods.destinationCell = this.destinationCell;
				//travelingTransportPods.arriveMode = this.arriveMode;
				//travelingTransportPods.attackOnArrival = this.attackOnArrival;

				DropPodLeaving.tmpActiveDropPods.Clear();
				DropPodLeaving.tmpActiveDropPods.AddRange(base.Map.listerThings.ThingsInGroup(ThingRequestGroup.ActiveDropPod));

				var dropPods = DropPodLeaving.tmpActiveDropPods.Cast<DropPodLeaving>();
				var podsInfo = dropPods.Select((arg) => arg.Contents);
				var podsThing = dropPods.Select((arg) => arg.landedThing);

				var traveler = TravelingPodsUtils.CreateWorldTraveler(this.Map.Tile, podsInfo, podsThing);

				traveler.Launch(this.destinationTile, this.destinationCell, this.arriveMode, this.attackOnArrival);

				foreach (var item in dropPods)
				{
					if (item != null && item.groupID == this.groupID)
					{
						item.alreadyLeft = true;
						item.Contents = null;
						item.Destroy(DestroyMode.Vanish);
					}
				}

				//for (int i = 0; i < DropPodLeaving.tmpActiveDropPods.Count; i++)
				//{
				//	DropPodLeaving dropPodLeaving = DropPodLeaving.tmpActiveDropPods[i] as DropPodLeaving;
				//	if (dropPodLeaving != null && dropPodLeaving.groupID == this.groupID)
				//	{
				//		dropPodLeaving.alreadyLeft = true;
				//		travelingTransportPods.AddPod(dropPodLeaving.Contents, true);
				//		dropPodLeaving.Contents = null;
				//		dropPodLeaving.Destroy(DestroyMode.Vanish);
				//	}
				//}
			}
		}
	}
}