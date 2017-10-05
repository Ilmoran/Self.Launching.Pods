using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WM.SelfLaunchingPods
{
	public class Command_UnloadCaravan_Items : Command_UnloadCaravan
	{
		public Command_UnloadCaravan_Items(WorldTraveler parent) : base(parent)
		{
		}

		public override string Label
		{
			get
			{
				return ("WM.UnloadCaravanItemsGizmo".Translate());
			}
		}

		public override string FailMessage
		{
			get
			{
				if (Utils.FindCaravanAt(Parent.Tile) == null)
					return ("WM.MessageCaravanNeeded".Translate());
				return base.FailMessage;
			}
		}

		public override bool CanDoNow
		{
			get
			{
				return Parent.AllCarriedNonPawnThings.Any() && (Utils.FindCaravanAt(Parent.Tile) != null);
			}
		}

		public override IEnumerable<Thing> ThingsToUnload
		{
			get
			{
				return (Parent.AllCarriedNonPawnThings);
			}
		}
	}
}
