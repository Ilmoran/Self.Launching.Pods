using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace WM.SelfLaunchingPods
{
	public abstract class Command_UnloadCaravan : Command_Traveler
	{
		public Command_UnloadCaravan(WorldTraveler parent) : base(parent)
		{
			this.action = delegate
			{
				if (CanDoNow)
				{
					Caravan caravan;

					caravan = Utils.FindCaravanAt(Parent.Tile);
					if (caravan == null)
					{
						IEnumerable<Pawn> pawns = ThingsToUnload.Where((Thing arg) => arg is Pawn).Cast<Pawn>().ToList();

						if (!pawns.Any())
							throw new Exception("Caravan creation needed but no pawns available");

						foreach (var item in pawns)
						{
							var holdingOwner = item.holdingOwner;
							if (holdingOwner != null)
								holdingOwner.Remove(item);
						}

						caravan = CaravanMaker.MakeCaravan(pawns, Faction.OfPlayer, Parent.Tile, true);
					}

					TravelingPodsUtils.ToCaravan(this.Parent, caravan, ThingsToUnload);
					Find.WorldSelector.ClearSelection();
					Find.WorldSelector.Select(caravan);
					Messages.Message(SuccessMessage, MessageSound.Benefit);
				}
				else
				{
					Messages.Message(FailMessage, MessageSound.RejectInput);
				}

			};
		}

		public override string Label
		{
			get
			{
				return ("WM.UnloadCaravanGizmo".Translate());
			}
		}

		public abstract IEnumerable<Thing> ThingsToUnload
		{
			get;
		}

		//public abstract Texture2D Icon
		//{ 
		//	get;
		//}

		public virtual bool CanDoNow
		{
			get
			{
				return (Utils.FindCaravanAt(Parent.Tile) != null);
			}
		}

		public virtual string FailMessage
		{
			get
			{
				if (!CanDoNow)
					return ("WM.MessageCaravanNeeded".Translate());
				else
					return ("WM.MessageCannotUnload".Translate());
			}
		}

		public virtual string SuccessMessage
		{
			get
			{
				return ("WM.MessageCaravanUnloadedFromPods".Translate());
			}
		}
	}
}
