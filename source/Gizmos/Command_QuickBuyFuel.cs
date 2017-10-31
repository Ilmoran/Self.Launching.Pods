using System;
using System.Linq;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WM.SelfLaunchingPods
{
	public class Command_QuickBuyFuel : Command_Traveler
	{
		public Command_QuickBuyFuel(WorldTraveler worldTraveler) : base(worldTraveler)
		{
			this.defaultLabel = "WM.QuickBuyFuelGizmo".Translate();
			this.defaultDesc = string.Format("WM.QuickBuyFuelGizmoDesc".Translate());
			this.icon = Resources.GizmoQuickBuyFuel;
			this.action = delegate
			{
				//var freeMassCapacity = this.Parent.MassCapacity - this.Parent.MassUsage + this.Parent.FuelCapacity - this.Parent.FuelLevel;
				var freeMassCapacity = this.Parent.MassCapacity - this.Parent.MassUsage;
				var bestNegociator = TradeUtils.BestNegociator(Parent.AllCarriedColonists);
				var dummyCaravan = TradeTweakUtils.PrepareTrade(bestNegociator, Parent.LocalFactionBase, this.Parent, false);

				TradeSession.SetupWith(Parent.LocalFactionBase, bestNegociator);

				var fuelTradeable = TradeSession.deal.AllTradeables.First((Tradeable arg) => arg.ThingDef == DefOf.Chemfuel);
				var presentFuel = TradeUtils.FuelAvailableAt(Parent.LocalFactionBase);
				var colonyMoney = TradeSession.deal.SilverTradeable.CountHeldBy(Transactor.Colony);

				var fuelPrice = fuelTradeable.GetPriceFor(TradeAction.PlayerBuys);
				int maxFuel = InventoryUtils.CalculateMaxBuyableFuel(freeMassCapacity, presentFuel, colonyMoney, fuelPrice);

				//TODO: ducktape. 
				//I can't check that the trade is not possible in Visible because I need to call TradeSession.SetupWith() and TradeTweakUtils.PrepareTrade() to get the prices.
				if (maxFuel <= 0)
				{
					TradeSession.Close();
					TradeTweakUtils.FinishTrade();
					Messages.Message(string.Format("WM.MessageCannotQuickBuyNow".Translate(), DefOf.Chemfuel.label), MessageTypeDefOf.RejectInput);
					return;
				}

				Func<int, string> textGetter =
					delegate (int n)
					{
						if (!fuelTradeable.CanAdjustTo(n).Accepted)
							return ("Error.");
						return (string.Format("WM.QuickBuyFuelGizmoDialog".Translate(),
											  n,
											  presentFuel,
											  DefOf.Chemfuel.label,
											  Mathf.Floor(n * fuelPrice),
											  colonyMoney,
											  DefOf.Silver.label));
					};

				Action<int> confirmAction =
					delegate (int n)
					{
						bool result;

						fuelTradeable.AdjustTo(n);
						TradeSession.deal.TryExecute(out result);
						if (!result)
						{
							Log.Error("TryExecute() failed.");
							return;
						}
						SoundDefOf.ExecuteTrade.PlayOneShotOnCamera();
					};

				Action finalAction =
					delegate ()
					{
						TradeSession.Close();
						TradeTweakUtils.FinishTrade();
					};

				var dialog = new Dialog_Slider_QuickBuyFuel(textGetter, 0, maxFuel, confirmAction, finalAction)
				{
					closeOnEscapeKey = true,
					forcePause = true,
					soundAppear = SoundDefOf.CommsWindow_Open,
					soundClose = SoundDefOf.CommsWindow_Close
				};

				Find.WindowStack.Add(dialog);
			};
		}

		public override bool Visible
		{
			get
			{
				return (base.Visible &&
						this.Parent.LocalFactionBase != null &&
						TradeUtils.FuelAvailableAt(this.Parent.LocalFactionBase) > 0 &&
						InventoryUtils.AnyCapablePawn(this.Parent.AllCarriedColonists));
			}
		}

		private class Dialog_Slider_QuickBuyFuel : Dialog_Slider
		{
			Action finalAction;

			// Verse.Dialog_Slider
			private Action<int> confirmAction
			{
				get
				{
					return (Action<int>)Traverse.Create(this).Field("confirmAction").GetValue();
				}
			}
			private int curValue
			{
				get
				{
					return (int)Traverse.Create(this).Field("curValue").GetValue();
				}
				set
				{
					Traverse.Create(this).Field("curValue").SetValue(value);
				}
			}

			public Dialog_Slider_QuickBuyFuel(Func<int, string> textGetter, int v, int max, Action<int> confirmAction, Action finalAction) : base(textGetter, v, max, confirmAction, max)
			{
				this.finalAction = finalAction;
			}

			public override void PostClose()
			{
				base.PostClose();
				finalAction();
			}

			// Verse.Dialog_Slider
			public override void DoWindowContents(Rect inRect)
			{
				Rect rect = new Rect(inRect.x, inRect.y + 15f, inRect.width, 30f);
				this.curValue = (int)Widgets.HorizontalSlider(rect, this.curValue, this.from, this.to, true, this.textGetter(this.curValue), null, null, 1f);
				Text.Font = GameFont.Small;
				Rect rect2 = new Rect(inRect.x, inRect.yMax - 30f, inRect.width / 2f, 30f);
				if (Widgets.ButtonText(rect2, "CancelButton".Translate(), true, false, true))
				{
					this.Close(true);
				}
				Rect rect3 = new Rect(inRect.x + inRect.width / 2f, inRect.yMax - 30f, inRect.width / 2f, 30f);
				if (Widgets.ButtonText(rect3, "OK".Translate(), true, false, true))
				{
					// Swaped this.
					this.confirmAction(this.curValue);
					this.Close(true);
				}
			}
		}
	}
}
