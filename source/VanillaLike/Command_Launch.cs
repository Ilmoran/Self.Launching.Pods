using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WM.SelfLaunchingPods
{

	public abstract class Command_Launch : Command
	{
		// RimWorld.CompLaunchable
		private static readonly Texture2D TargeterMouseAttachment = ContentFinder<Texture2D>.Get("UI/Overlays/LaunchableMouseAttachment", true);

		public Command_Launch()
		{
			this.icon = Resources.LaunchCommandTex;
			this.hotKey = KeyBindingDefOf.Misc1;
		}

		public override string Label
		{
			get
			{
				return "LaunchCaravanGizmo".Translate();
			}
		}

		public virtual int MaxLaunchDistance
		{
			get
			{
				return 0;
			}
		}
		public virtual int Tile
		{
			get
			{
				return 0;
			}
		}

		public virtual ThingWithComps Parent
		{
			get
			{
				return null;
			}
		}

		internal abstract void Launch(int tile, IntVec3 cell);

		public WorldTraveler Traveler
		{
			get
			{
				return (WorldTraveler)Find.WorldSelector.SingleSelectedObject;
			}
		}


		//bool targetIsMap = false;

		public override void ProcessInput(UnityEngine.Event ev)
		{
			base.ProcessInput(ev);

			try
			{

				CameraJumper.TryJump(CameraJumper.GetWorldTarget(this.Parent));
				Find.WorldSelector.ClearSelection();
				//int tile = this.parent.Map.Tile;

				Find.WorldTargeter.BeginTargeting(new Func<GlobalTargetInfo, bool>(this.ChoseWorldTarget), true, TargeterMouseAttachment, false, delegate
				  {
					  GenDraw.DrawWorldRadiusRing(Tile, MaxLaunchDistance);
					  GenDraw.DrawWorldRadiusRing(Tile, MaxLaunchDistance / 2);
				  }, delegate (GlobalTargetInfo target)
				 {
					 if (!target.IsValid)
					 {
						 return null;
					 }
					 int num = Find.WorldGrid.TraversalDistanceBetween(Tile, target.Tile);
					 if (num <= MaxLaunchDistance)
					 {
						 return null;
					 }
					 //if (num > worldLandedPods.MaxLaunchDistanceEverPossible)
					 //{
					 // return "TransportPodDestinationBeyondMaximumRange".Translate();
					 //}
					 return "TransportPodNotEnoughFuel".Translate();
				 });
			}
			catch (Exception ex)
			{
				Log.Error("Error while trying to target destination: " + ex.Message + "\n" + ex.StackTrace);
			}
		}

		bool ChoseWorldTarget(GlobalTargetInfo target)
		{
			if (!target.IsValid)
			{
				Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageSound.RejectInput);
				return false;
			}
			int num = Find.WorldGrid.TraversalDistanceBetween(this.Tile, target.Tile);
			if (num > this.MaxLaunchDistance)
			{
				Messages.Message("MessageTransportPodsDestinationIsTooFar".Translate(new object[]
				{
					TravelingPodsUtils.FuelNeededToLaunchAtDistance(num,this.Traveler.PodsCount).ToString("0.#")
				}), MessageSound.RejectInput);
				return false;
			}

			MapParent mapParent = target.WorldObject as MapParent;
			if (mapParent != null && mapParent.HasMap)
			{
				if (!CameraJumper.TryHideWorld())
					throw new Exception("Could not hide world()");

				//Verse.CameraJumper.TryJump(target);
				Current.Game.VisibleMap = mapParent.Map;

				Find.Targeter.BeginTargeting(TargetingParameters.ForDropPodsDestination(), (LocalTargetInfo obj) => Launch(target.Tile, obj.Cell), null, null, TargeterMouseAttachment);
			}
			else
			{
				Launch(target.Tile, target.Cell);
			}


			return true;
		}

	}
}
