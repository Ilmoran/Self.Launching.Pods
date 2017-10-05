using Verse;

namespace WM.SelfLaunchingPods
{
	public class Command_Traveler : Command_Action
	{
		public Command_Traveler(WorldTraveler parent)
		{
			Parent = parent;
		}

		public WorldTraveler Parent
		{
			get; private set;
		}
	}
}