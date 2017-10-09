using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WM.SelfLaunchingPods
{
	public abstract class Command_UnloadCaravan : Command_Traveler
	{
		internal Command_UnloadCaravan(WorldTraveler parent) : base(parent)
		{
			this.action = delegate
			{
				var thingsToUnload = ThingsToUnload.ToList();

				if (!thingsToUnload.Any())
				{
					Messages.Message("WM.MessageNothingToUnload".Translate(), MessageSound.RejectInput);
					return;
				}
				Caravan caravan;

				caravan = Utils.FindCaravanAt(Parent.Tile);
				if (caravan == null)
				{
					var pawns = InventoryUtils.GetPawnsFrom(thingsToUnload);

					if (!pawns.Any())
					{
						Messages.Message("WM.MessageCaravanNeeded".Translate(), MessageSound.RejectInput);
						return;
					}

					var colonists = InventoryUtils.GetColonistsFrom(thingsToUnload);

					if (!colonists.Any())
					{
						Messages.Message("WM.MessageColonistsNeeded".Translate(), MessageSound.RejectInput);
						return;
					}

					foreach (var item in pawns)
					{
						var holdingOwner = item.holdingOwner;
						if (holdingOwner != null)
						{
							holdingOwner.Remove(item);
						}
					}

					caravan = CaravanMaker.MakeCaravan(pawns, Faction.OfPlayer, Parent.Tile, true);
				}

				TravelingPodsUtils.ToCaravan(caravan, thingsToUnload);
				Find.WorldSelector.ClearSelection();
				Find.WorldSelector.Select(caravan);
				Messages.Message(SuccessMessage, MessageSound.Benefit);
			};
		}

		public override string Label
		{
			get
			{
				return ("WM.UnloadCaravanGizmo".Translate());
			}
		}

		protected abstract IEnumerable<Thing> ThingsToUnload
		{
			get;
		}

		protected virtual string SuccessMessage
		{
			get
			{
				return ("WM.MessageCaravanUnloadedFromPods".Translate());
			}
		}
	}
}
