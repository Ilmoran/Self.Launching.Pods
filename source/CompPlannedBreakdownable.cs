using Harmony;
using RimWorld;
using Verse;

namespace WM.SelfLaunchingPods
{
	public class CompProperties_PlannedBreakdownable : CompProperties
	{
		public int usesCountUntilBreakdown;
		//public float damageRatePerUse;
		public float minHitpointsRateToUse;

		public CompProperties_PlannedBreakdownable()
		{
			this.compClass = typeof(CompPlannedBreakdownable);
		}	}

	public class CompPlannedBreakdownable : RimWorld.CompBreakdownable
	{
		public const string UseSignal = "UsedOnce";
		public const string RepairSignal = "Repaired";
		private int launchCountUntilBreakdownInt;
		private bool initialized;

		public new bool BrokenDown
		{
			get
			{
				return (launchCountUntilBreakdownInt <= 0);
			}
		}

		public int RemainingLaunchesUntilBreakdown
		{
			get
			{
				return (this.launchCountUntilBreakdownInt);
			}
		}

		public CompProperties_PlannedBreakdownable Props
		{
			get
			{
				return (this.props as CompProperties_PlannedBreakdownable);
			}
		}

		//public int DamagePerUse
		//{
		//	get
		//	{
		//		return (Mathf.FloorToInt(this.parent.MaxHitPoints * this.Props.damageRatePerUse));
		//	}
		//}

		public bool DamageLevelAllowsUse
		{
			get
			{
				return (Utils.HitpointsRate(this.parent) >= this.Props.minHitpointsRateToUse);
			}
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (BrokenDown)
			{
				RecordBreakdown();
			}
			if (!respawningAfterLoad && !this.initialized)
			{
				initialized = true;
				this.launchCountUntilBreakdownInt = this.Props.usesCountUntilBreakdown;
			}
		}

		public override void PostExposeData()
		{
			//base.PostExposeData();
			Scribe_Values.Look<int>(ref this.launchCountUntilBreakdownInt, "launchCountUntilBreakdown", this.Props.usesCountUntilBreakdown, false);
			Scribe_Values.Look<bool>(ref this.initialized, "initialized", false, false);
		}

		public override void PostDraw()
		{
			if (this.BrokenDown)
			{
				this.parent.Map.overlayDrawer.DrawOverlay(this.parent, OverlayTypes.BrokenDown);
			}
		}

		public new void Notify_Repaired()
		{
			launchCountUntilBreakdownInt = this.Props.usesCountUntilBreakdown;
		}

		public void Notify_Used()
		{
			//this.parent.TakeDamage(new DamageInfo(DamageDefOf.Deterioration, DamagePerUse));
			launchCountUntilBreakdownInt--;
			if (launchCountUntilBreakdownInt <= 0)
			{
				this.parent.BroadcastCompSignal(BreakdownSignal);
				if (this.parent.Spawned)
					RecordBreakdown();
			}
		}

		public override void ReceiveCompSignal(string signal)
		{
			switch (signal)
			{
				case UseSignal:
					Notify_Used();
					break;
				case RepairSignal:
					Notify_Repaired();
					break;
			}
		}


		private void RecordBreakdown()
		{
			this.parent.Map.GetComponent<BreakdownManager>().Notify_BrokenDown(this.parent);
		}

		public override string CompInspectStringExtra()
		{
			var str = "";

			if (!this.DamageLevelAllowsUse)
			{
				str += "WM.CantUseDamageTooHigh".Translate() + "\n";
			}
			if (this.BrokenDown)
			{
				str += "BrokenDown".Translate();
			}
			else
			{
				str += "WM.RemainingLaunchesUntilBreakdown".Translate(new object[] { RemainingLaunchesUntilBreakdown });
			}

			return (str);
		}
#if DEBUG
		public override System.Collections.Generic.IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			var gizmo = new Command_Action();

			gizmo.defaultLabel = "DEBUG: Breakdown now";
			gizmo.action = delegate
			{
				launchCountUntilBreakdownInt = 0;
				RecordBreakdown();
			};

			yield return (gizmo);
		}
#endif
	}

	[HarmonyPatch(typeof(RimWorld.CompBreakdownable))]
	[HarmonyPatch("Notify_Repaired")]
	static class Notify_Repaired_Detour
	{
		static bool Prefix(RimWorld.CompBreakdownable __instance)
		{
			if (__instance is CompPlannedBreakdownable)
			{
				(__instance as CompPlannedBreakdownable).Notify_Repaired();
				return (false);
			}
			return (true);
		}
	}

	//TODO: this detour is not working. I have to use the one bellow instead.
	//	[HarmonyPatch(typeof(RimWorld.CompBreakdownable))]
	//	[HarmonyPatch("BrokenDown", PropertyMethod.Getter)]
	//	static class BrokenDown_Detour
	//	{
	//		static bool Prefix(RimWorld.CompBreakdownable __instance, ref bool __result)
	//		{
	//#if DEBUG
	//			Log.Message("BrokenDown.Prefix()");
	//#endif
	//			if (__instance is CompPlannedBreakdownable)
	//			{
	//				__result = (__instance as CompPlannedBreakdownable).BrokenDown;
	//				return (false);
	//			}
	//			return (true);
	//		}
	//	}

	[HarmonyPatch(typeof(RimWorld.BreakdownableUtility))]
	[HarmonyPatch("IsBrokenDown")]
	static class IsBrokenDown_Detour
	{
		static bool Prefix(ref bool __result, Thing t)
		{
			var comp = t.TryGetComp<CompPlannedBreakdownable>();

			if (comp != null)
			{
				__result = (comp).BrokenDown;
				return (false);
			}
			return (true);
		}
	}

	[HarmonyPatch(typeof(RimWorld.CompBreakdownable))]
	[HarmonyPatch("CheckForBreakdown")]
	[HarmonyPatch("DoBreakdown")]
	static class DoBreakdown_Detour
	{
		static bool Prefix(RimWorld.CompBreakdownable __instance)
		{
			if (__instance is CompPlannedBreakdownable)
			{
				return (false);
			}
			return (true);
		}	}
}
