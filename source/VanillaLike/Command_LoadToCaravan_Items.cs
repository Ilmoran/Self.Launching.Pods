using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using Verse;

namespace WM.SelfLaunchingPods
{
	public class Command_LoadToCaravan_Items : Command_LoadToCaravan
	{
		public Command_LoadToCaravan_Items(Caravan parent) : base(parent)
		{
			this.icon = Resources.GizmoLoadItems;
		}

		public override IEnumerable<Thing> ThingsToLoad
		{
			get
			{
				return (Parent.Goods);
			}
		}

		public override string Label
		{
			get
			{
				return ("WM.LoadCaravanItemsGizmo".Translate());
			}
		}
	}
}