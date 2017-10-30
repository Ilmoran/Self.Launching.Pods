using System.Collections.Generic;
using System.Linq;
using RimWorld;
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
				var list = ThingsToLoad.ToList();

				if (traveler == null)
				{
					float mass = TravelingPodsUtils.CaravanMass(list);
					Messages.Message(string.Format("WM.MessageNoPodsToLoad".Translate(), mass.ToStringMass(), TravelingPodsUtils.RequiredPodsCountForMass(mass)), MessageTypeDefOf.NeutralEvent);
					return;
				}
				var missingMassCapacity = TravelingPodsUtils.MissingMassCapacity((WorldTraveler)traveler, list);

				if (!ThingsToLoad.Any())
				{
					Messages.Message("WM.MessageNothingToLoad".Translate(), MessageTypeDefOf.NeutralEvent);
					return;
				}

				if (TravelingPodsUtils.FromCaravan((WorldTraveler)traveler, parent, ThingsToLoad))
				{
					if (!parent.pawns.Any)
					{
						Find.WorldSelector.ClearSelection();
						Find.WorldSelector.Select(traveler);
					}

					Messages.Message("WM.MessageCaravanLoadedToPods".Translate(), MessageTypeDefOf.PositiveEvent);
				}
				else
				{
					Messages.Message(string.Format("WM.MessageCaravanPodsFleetCapacityTooLow".Translate(), missingMassCapacity.ToStringMass()), MessageTypeDefOf.NegativeEvent);
				}
			};
		}

		protected Caravan Parent { get; private set; }
		protected abstract IEnumerable<Thing> ThingsToLoad { get; }
	}
}