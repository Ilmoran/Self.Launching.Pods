using System;
using System.Linq;
using RimWorld;
using Verse;

namespace WM.SelfLaunchingPods
{
	public class PodPair : IExposable
	{
		public ThingOwner<Thing> ThingOwner;
		public ActiveDropPodInfo PodInfo;

		public Thing PodThing
		{
			get
			{
				return ThingOwner.First();
			}
		}

		public PodPair()
		{
		}
		public PodPair(Thing t, ActiveDropPodInfo podinfo)
		{
			this.ThingOwner = new ThingOwner<Thing>();
			this.ThingOwner.TryAdd(t, 1, false);
			this.PodInfo = podinfo;
		}

		public void ExposeData()
		{
			//Scribe_References.Look<Thing>(ref Thing, "Thing", true);
			Scribe_Deep.Look<ThingOwner<Thing>>(ref ThingOwner, true, "ThingOwner", new object[0]);
			Scribe_Deep.Look<ActiveDropPodInfo>(ref PodInfo, "PodInfo");
		}
	}
}
