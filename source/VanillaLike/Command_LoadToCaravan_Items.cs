using System;
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

		protected override IEnumerable<Thing> ThingsToLoad
		{
			get
			{
				return (InventoryUtils.GetItemsFrom(Parent.Goods));
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