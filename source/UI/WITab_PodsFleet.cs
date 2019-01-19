using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace WM.SelfLaunchingPods
{
    /// <summary>
    /// Adapted from <see cref="RimWorld.Planet.WITab_Caravan_Items"/>
    /// RimWorld version 1.0.2096
    /// </summary>
    public class WITab_PodsFleet : WITab
	{
		private const float MassCarriedLineHeight = 22f;
		private Vector2 scrollPosition;
		private float scrollViewHeight;
		private readonly List<Thing> items = new List<Thing>();
        private List<TransferableImmutable> cachedItems = new List<TransferableImmutable>();

        private TransferableSorterDef sorter1;
        private TransferableSorterDef sorter2;

        public WorldTraveler PodsFleet
		{
			get
			{
				return ((WorldTraveler)base.SelObject);
			}
		}

		public WITab_PodsFleet()
		{
			this.labelKey = "WM.FleetInventoryWITab";
		}

		protected override void FillTab()
		{
            float num = 0f;
            this.DrawMassUsage(ref num);
            GUI.BeginGroup(new Rect(0f, num, this.size.x, this.size.y - num));
            this.UpdateItemsList();
            CaravanItemsTabUtility.DoRows(this.size, PodsFleet.AllCarriedItems.ToTransferableImmutables().ToList(), base.SelCaravan, ref this.scrollPosition, ref this.scrollViewHeight);
            this.items.Clear();
            GUI.EndGroup();
        }

		protected override void UpdateSize()
		{
			base.UpdateSize();
			this.UpdateItemsList();
			this.size = CaravanItemsTabUtility.GetSize(this.items.ToTransferableImmutables().ToList(), this.PaneTopY, true);
			this.items.Clear();
		}

		private void DrawMassUsage(ref float curY)
		{
			curY += 10f;
			var rect = new Rect(10f, curY, this.size.x - 10f, 100f);
			float massUsage = PodsFleet.MassUsage;
			float allLandedShipMassCapacity = this.PodsFleet.MassCapacity;
			bool flag = massUsage > allLandedShipMassCapacity;
			if (flag)
			{
				GUI.color = Color.red;
			}
			Text.Font = GameFont.Tiny;
			Widgets.Label(rect, "MassCarried".Translate(new object[]
			{
				massUsage.ToString("0.##"),
				allLandedShipMassCapacity.ToString("0.##")
			}));
			GUI.color = Color.white;
			curY += 22f;
		}

		private void UpdateItemsList()
		{
			this.items.Clear();
			this.items.AddRange(this.PodsFleet.AllCarriedThingsOrdered);
		}

		private void CheckCreateSorters()
        {
            if (this.sorter1 == null)
            {
                this.sorter1 = TransferableSorterDefOf.Category;
            }
            if (this.sorter2 == null)
            {
                this.sorter2 = TransferableSorterDefOf.MarketValue;
            }
        }
    }
}
