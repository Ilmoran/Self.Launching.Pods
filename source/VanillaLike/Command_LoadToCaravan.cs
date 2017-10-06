using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using Verse;

namespace WM.SelfLaunchingPods
{
	public abstract class Command_LoadToCaravan : Command_Action
	{
		public Command_LoadToCaravan(Caravan parent)
		{
			this.Parent = parent;
			action = delegate
			{
				var traveler = Find.WorldObjects.ObjectsAt(parent.Tile).FirstOrDefault(
					(WorldObject arg) => arg.def == DefOf.WM_TravelingTransportPods);

				if (!ThingsToLoad.Any())
				{
					Messages.Message(string.Format("WM.MessageNothingToLoad".Translate(), TravelingPodsUtils.MissingMassCapacity((WorldTraveler)traveler, parent)), MessageSound.RejectInput);
					return;
				}

				if (traveler != null)
				{
					if (TravelingPodsUtils.FromCaravan((WorldTraveler)traveler, parent, ThingsToLoad))
					{
						if (!parent.pawns.Any)
						{
							Find.WorldSelector.ClearSelection();
							Find.WorldSelector.Select(traveler);
						}

						Messages.Message("WM.MessageCaravanLoadedToPods".Translate(), MessageSound.Benefit);
					}
					else
					{
						Messages.Message(string.Format("WM.MessageCaravanPodsFleetCapacityTooLow".Translate(), TravelingPodsUtils.MissingMassCapacity((WorldTraveler)traveler, parent)), MessageSound.Negative);
					}
				}
				else
				{
					Messages.Message("WM.MessageNoPodsToLoad".Translate(), MessageSound.RejectInput);
				}
			};
		}

		public Caravan Parent { get; private set; }

		public abstract IEnumerable<Thing> ThingsToLoad
		{
			get;
		}
	}
}