using System.Linq;
using RimWorld;
using Verse;

namespace WM.SelfLaunchingPods
{
	public class PodPair : IExposable
	{
		private ThingOwner<Thing> thingOwner;
		private ActiveDropPodInfo podInfo;

		public Thing PodThing
		{
			get
			{
				return (thingOwner.First());
			}
		}

		public ActiveDropPodInfo PodInfo
		{
			get
			{
				return podInfo;
			}
		}

		public PodPair()
		{
		}
		public PodPair(Thing t, ActiveDropPodInfo podinfo)
		{
           	this.thingOwner = new ThingOwner<Thing>();
           	this.thingOwner.TryAdd(t, 1, false);
			this.podInfo = podinfo;
		}

		public void ExposeData()
		{
			Scribe_Deep.Look(ref thingOwner, true, "ThingOwner", new object[0]);
			Scribe_Deep.Look(ref podInfo, "PodInfo");
		}
	}
}
