using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WM.SelfLaunchingPods
{
	public class Command_UnloadCaravan_Items : Command_UnloadCaravan
	{
		public Command_UnloadCaravan_Items(WorldTraveler parent) : base(parent)
		{
            this.icon = Resources.GizmoUnloadItems;
		}

		public override string Label
		{
			get
			{
				return ("WM.UnloadCaravanItemsGizmo".Translate());
			}
		}

		protected override IEnumerable<Thing> ThingsToUnload
		{
			get
			{
				return (Parent.AllCarriedItems);
			}
		}
	}
}
