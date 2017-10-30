using System;
using RimWorld;
using Verse;

namespace WM.SelfLaunchingPods
{
	public class Command_Refuel : Command_Traveler
	{
		public Command_Refuel(WorldTraveler traveler) : base(traveler)
		{
			this.defaultLabel = "WM.RefuelGizmo".Translate();
			this.defaultDesc = string.Format("WM.RefuelGizmoDesc".Translate());
			this.icon = Resources.GizmoRefuel;
			this.action = delegate
			{
				var refuelAmout = Math.Min(this.Parent.CarriedFuelLevel, this.Parent.FuelCapacity - this.Parent.FuelLevel);
				this.Parent.RefuelFromInventory(refuelAmout);
#if DEBUG
				Log.Message("refuelAmout = " + refuelAmout);
#endif
				Messages.Message("WM.MessagePodsRefueled".Translate(), MessageTypeDefOf.NeutralEvent);
			};
		}

		public override bool Visible
		{
			get
			{
				return base.Visible &&
					  	this.Parent.CarriedFuelLevel > 0 &&
					 	this.Parent.FuelLevel < this.Parent.FuelCapacity &&
						InventoryUtils.AnyCapablePawn(this.Parent.AllCarriedColonists);
			}
		}
	}
}
