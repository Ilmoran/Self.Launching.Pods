using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace WM.SelfLaunchingPods
{

	public abstract class Command_Launch : Command_Action
	{
		public override string Label
		{
			get
			{
				return ("WM.LaunchCaravanGizmo".Translate());

			}
		}

		public virtual int MaxLaunchDistanceEver
		{
			get
			{
				return (-1);
			}
		}

		public virtual int MaxLaunchDistanceOneWay
		{
			get
			{
				return (FuelUtils.MaxLaunchDistance(ParentLeastFueledPodFuelLevel, 1, true));
			}
		}

		public virtual bool HideWorldAfterLaunch
		{
			get
			{
				return (true);
			}
		}

		public int MaxLaunchDistanceRoundTrip
		{
			get
			{
				return (Mathf.FloorToInt(MaxLaunchDistanceOneWay / 2));
			}
		}

		public abstract float ParentLeastFueledPodFuelLevel
		{
			get;
		}

		public abstract int ParentTile
		{
			get;
		}

		public abstract int ParentPodsCount
		{
			get;
		}

		public abstract IEnumerable<ThingWithComps> PodsList
		{
			get;
		}

		public virtual bool CustomCondition()
		{
			return (true);
		}

		internal abstract void Launch(int tile, IntVec3 cell, PawnsArrivalModeDef arriveMode = null, bool attackOnArrival = false);
        
        //TODO: rearrange destination validation
        internal Command_Launch()
		{
			this.icon = Resources.LaunchCommandTex;
			action = delegate
			{
				try
				{
					if (!CustomCondition())
					{
						return;
					}

					if (!DamageLevelAllowsLaunch())
					{
						Messages.Message("WM.MessageRepairPodsFirst".Translate(), MessageTypeDefOf.RejectInput);
						return;
					}
					CameraJumper.TryJump(CameraJumper.GetWorldTarget(new GlobalTargetInfo(ParentTile)));
					Find.WorldSelector.ClearSelection();
					Find.WorldTargeter.BeginTargeting(new Func<GlobalTargetInfo, bool>(this.ChoseWorldTarget), true, Resources.LaunchCommandTex, false,
														delegate
														{
															GenDraw.DrawWorldRadiusRing(ParentTile, MaxLaunchDistanceOneWay);
															GenDraw.DrawWorldRadiusRing(ParentTile, MaxLaunchDistanceRoundTrip);
														},
														delegate (GlobalTargetInfo target)
														{
															if (!target.IsValid)
															{
																return (null);
															}
															int num = Find.WorldGrid.TraversalDistanceBetween(ParentTile, target.Tile);

															if (num <= MaxLaunchDistanceOneWay)
															{
																return (null);
															}
															return ("TransportPodNotEnoughFuel".Translate());
														});
				}
				catch (Exception ex)
				{
					Log.Error("Error while trying to target destination: " + ex.Message + "\n" + ex.StackTrace);
				}
			};
		}

		bool DamageLevelAllowsLaunch()
		{
			return (this.PodsList.All((ThingWithComps arg) => !Utils.PodIsBrokenDown(arg, true)));
		}

        bool ChoseWorldTarget(GlobalTargetInfo target)
        {
            if (!target.IsValid || Find.World.Impassable(target.Tile))
            {
                Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageTypeDefOf.RejectInput);
                return (false);
            }
            int num = Find.WorldGrid.TraversalDistanceBetween(this.ParentTile, target.Tile);

            if (num > this.MaxLaunchDistanceOneWay)
            {
                Messages.Message(
                            string.Format(
                                    "MessageTransportPodsDestinationIsTooFar".Translate(),
                                    FuelUtils.FuelNeededToLaunchAtDistance(num, this.ParentPodsCount)),
                                    MessageTypeDefOf.RejectInput);

                return (false);
            }
            var mapParent = Find.WorldObjects.MapParentAt(target.Tile);

            if (mapParent == null)
            {
                Launch(target.Tile, target.Cell);
            }
            else
            {
                if (mapParent.HasMap)
                {
                    Current.Game.CurrentMap = mapParent.Map;
                    CameraJumper.TryHideWorld();
                    Action<LocalTargetInfo> launchAction = delegate (LocalTargetInfo localTarget)
                    {
                        Launch(target.Tile, localTarget.Cell);
                        if (HideWorldAfterLaunch)
                            CameraJumper.TryHideWorld();
                    };
                    Find.Targeter.BeginTargeting(TargetingParameters.ForDropPodsDestination(), launchAction, null, null, Resources.LaunchCommandTex);
                }
                else
                {
                    var list = new List<FloatMenuOption>();
                    var settlement = mapParent as Settlement;

                    if (settlement != null && settlement.Visitable)
                    {
                        list.Add(new FloatMenuOption("VisitSettlement".Translate(new object[] { settlement.Label }), delegate
                                                {
                                                    this.Launch(target.Tile, target.Cell, null, false);
                                                    if (HideWorldAfterLaunch)
                                                        CameraJumper.TryHideWorld();
                                                }, MenuOptionPriority.Default, null, null, 0f, null, null));
                    }
                    list.Add(new FloatMenuOption("DropAtEdge".Translate(), delegate
                    {
                        this.Launch(target.Tile, target.Cell, PawnsArrivalModeDefOf.EdgeDrop, true);
                    }, MenuOptionPriority.Default, null, null, 0f, null, null));

                    list.Add(new FloatMenuOption("DropInCenter".Translate(), delegate
                    {
                        this.Launch(target.Tile, target.Cell, PawnsArrivalModeDefOf.CenterDrop, true);
                    }, MenuOptionPriority.Default, null, null, 0f, null, null));
                    if (!list.Any())
                    {
                        throw new Exception("Error when choosing pods destination.");
                    }

                    Find.WindowStack.Add(new FloatMenu(list));
                }
            }

            return (true);
        }
    }
}
