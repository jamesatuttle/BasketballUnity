
namespace AssemblyCSharp
{
	public class GamePlay
	{
		public static bool isGamePlayable;
		public bool PlayGame;
		public bool ViewScoreboard;
		public bool ViewLeaderboard;

		public GamePlay ()
		{
			Cameras.StartScreenCameraSetup ();
		}

	}
}
