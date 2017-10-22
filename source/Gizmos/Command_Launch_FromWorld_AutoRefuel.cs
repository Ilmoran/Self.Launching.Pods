using System;
using RimWorld;
using Verse;

namespace WM.SelfLaunchingPods
{
	public class Command_Launch_FromWorld_AutoRefuel : Command_Launch_FromWorld
	{
		public Command_Launch_FromWorld_AutoRefuel(WorldTraveler parent) : base(parent)
		{
			this.icon = Resources.RefuelAndLaunchCommandTex;
		}

		public override int MaxLaunchDistanceOneWay
		{
			get
			{
				var fuelForTrip = Math.Min(this.parent.FuelLevel + parent.CarriedFuelLevel, parent.FuelCapacity);
				var result = FuelUtils.MaxLaunchDistance(fuelForTrip, this.parent.PodsCount, true);

				return (result);
			}
		}

		public override bool CustomCondition()
		{
			if (!InventoryUtils.AnyCapablePawn(this.parent.AllCarriedColonists))
			{
				Messages.Message(string.Format("WM.MessageNoCapableColonistsInFleet".Translate()), MessageSound.Negative);
				return (false);
			}
			return (true);
		}

		internal override void Launch(int tile, IntVec3 cell, PawnsArriveMode arriveMode = 0, bool attackOnArrival = false)
		{
			int distance = Find.WorldGrid.TraversalDistanceBetween(this.parent.Tile, tile);
			float fuelNeedToRefuel = Math.Min(this.parent.MissingFuelLevel, this.parent.CarriedFuelLevel);
#if DEBUG
			Log.Message("this.parent.MissingFuelLevel = " + this.parent.MissingFuelLevel);
			Log.Message("this.parent.CarriedFuelLevel = " + this.parent.CarriedFuelLevel);
#endif
			if (fuelNeedToRefuel > 0)
				this.parent.RefuelFromInventory();
			base.Launch(tile, cell, arriveMode, attackOnArrival);
		}

		public override bool Visible
		{
			get
			{
				return (base.Visible && this.parent.FuelLevel < this.parent.FuelCapacity && this.parent.CarriedFuelLevel > 0f);
			}
		}

		public override string Label
		{
			get
			{
				return ("WM.RefuelAndLaunchCaravanGizmo".Translate());
			}
		}

		public override string Desc
		{
			get
			{
				return ("WM.RefuelAndLaunchCaravanGizmoDesc".Translate());
			}
		}
	}
}
