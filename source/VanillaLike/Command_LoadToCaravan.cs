using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using Verse;

namespace WM.SelfLaunchingPods
{
	public abstract class Command_LoadToCaravan : Command_Action
	{
		internal Command_LoadToCaravan(Caravan parent)
		{
			this.Parent = parent;
			action = delegate
			{
				var traveler = Find.WorldObjects.ObjectsAt(parent.Tile).FirstOrDefault(
					(WorldObject arg) => arg.def == DefOf.WM_TravelingTransportPods);

				if (traveler == null)
				{
					Messages.Message("WM.MessageNoPodsToLoad".Translate(), MessageSound.RejectInput);
					return;
				}
				var missingMassCapacity = TravelingPodsUtils.MissingMassCapacity((WorldTraveler)traveler, ThingsToLoad.ToList());

				if (!ThingsToLoad.Any())
				{
					Messages.Message("WM.MessageNothingToLoad".Translate(), MessageSound.RejectInput);
					return;
				}

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
					Messages.Message(string.Format("WM.MessageCaravanPodsFleetCapacityTooLow".Translate(), missingMassCapacity.ToStringMass()), MessageSound.Negative);
				}
			};
		}

		protected Caravan Parent { get; private set; }

		protected abstract IEnumerable<Thing> ThingsToLoad { get; }
	}
}