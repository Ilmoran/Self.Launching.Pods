using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace WM.SelfLaunchingPods
{
	public class DropPodLeaving : Skyfaller, IActiveDropPod, IThingHolder
	{

		public int groupID = -1;
		public int destinationTile = -1;
		public IntVec3 destinationCell = IntVec3.Invalid;
		public PawnsArriveMode arriveMode = PawnsArriveMode.Undecided;
		public bool attackOnArrival;
		private bool alreadyLeft;
		private static List<Thing> tmpActiveDropPods = new List<Thing>();
		public Thing landedThing; // MOD
		ActiveDropPodInfo podInfo;

		public ActiveDropPodInfo Contents
		{
			get
			{
				return podInfo;
			}

			internal set
			{
				podInfo = value;
			}
		}

		// RimWorld.Skyfaller
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.ticksToImpact, "ticksToImpact", 0, false);
			Scribe_Values.Look<float>(ref this.angle, "angle", 0f, false);
			Scribe_Values.Look<float>(ref this.shrapnelDirection, "shrapnelDirection", 0f, false);
			Scribe_References.Look<Thing>(ref landedThing, "landedThing");
			Scribe_Deep.Look<ActiveDropPodInfo>(ref podInfo, "podInfo");
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
			}
		}
	}
}