using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boggle
{
	public class UserNckname
	{
		public string Nickname { get; set; }

	}
	public class UserTokenAndTimeLimit
	{
			public string UserToken { get; set; }

			public int TimeLimit { get; set; }
		}
	
	public class GameIDInfo
	{
		public string GameID { get; set; }
	}
	public class ScoreInfo
	{
		public int Score { get; set; }
	}

	public class PlayerInfo
	{
		public UserNckname Nicknme { get; set; }

		public ScoreInfo Score { get; set; }
	}
	public class DetailedPlayerInfo : PlayerInfo
	{
		public WordAndScore[] MovesMade;
	}
	public class WordAndScore
	{
		public string Word { get; set; }

		public int Score { get; set; }
	}
	public class GameStatePending
	{
		public string GameState { get; set; }
	}
	public class GameStateActive : GameStatePending
	{
		public int TimeLeft { get; set; }

		public PlayerInfo Player1 { get; set; }

		public PlayerInfo Player2 { get; set; }
	}
	public class DetailedGameState : GameStateActive
	{
		public int TimeLimit { get; set; }

		public string Board { get; set; }

	}

}