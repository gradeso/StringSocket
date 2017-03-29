using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// 
/// </summary>
namespace Boggle
{


	public class Move
	{
		public string UserToken { get; set; }
		public string Word { get; set; }

	}
	public class JoinAttempt
	{
		public string UserToken { get; set; }
		public string TimeLimit { get; set; }

	}


	/// <summary>
	/// response POST users
	/// </summary>
	public class UserIDInfo
	{
		public UserIDInfo()
		{
			UserToken = "";
		}
		public UserIDInfo(string id)
		{
			UserToken = id;
		}
		public string UserToken { get; set; }
	}




	/// <summary>
	/// response POST games
	/// </summary>
	public class GameIDInfo
	{
		public GameIDInfo()
		{
			GameID = "";
		}
		public GameIDInfo(string id)
		{
			GameID = id;
		}
		public string GameID { get; set; }
	}

	/// <summary>
	/// used in PUT /games/{GameID} response.
	/// </summary>
	public class ScoreInfo
	{
		public int Score { get; set; }
	}

	/// <summary>
	/// Holds scores and names of player. Used in game status GET.
	/// </summary>
	public class PlayerInfo
	{
		public PlayerInfo()
		{
			userID = "";
		}
		
		[ScriptIgnore]
		public string userID { get; set; }

		public string Nicknme { get; set; }

		public ScoreInfo Score { get; set; }
	}
	public class DetailedPlayerInfo : PlayerInfo
	{
		public DetailedPlayerInfo()
		{
			MovesMade = new List<WordAndScore>();
		}


		public DetailedPlayerInfo(string userID, string nn)
		{
			this.userID = userID;
			Nicknme = nn;
		}

		public List<WordAndScore> MovesMade;
		
	}
	public class WordAndScore
	{
		public WordAndScore()
		{
			ownersToken = "";
			Word = "";
			Score = 0;
		}
		public WordAndScore(string ot, string w, int s)
		{
			ownersToken = ot;
			Word = w;
			Score = s;
		}
		/// <summary>
		/// Gets or sets the owners token. eg. player 1 s user Id
		/// </summary>
		/// <value>
		/// The owners token.
		/// </value>
		[ScriptIgnore]
		public string ownersToken { get; set; }

		public string Word { get; set; }

		public int Score { get; set; }
	}

	/// <summary>
	/// these are for the gamestate object
	/// </summary>
	public class GameStatePending
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GameStatePending"/> class.
		/// </summary>
		public GameStatePending()
		{
			GameState = "pending";
		}
		

		public string GameState { get; set; }
	}
	public class GameStateActive : GameStatePending
	{
		public GameStateActive()
		{
			TimeLeft = 0;
			Player1 = null;
			Player2 = null;
		}

		public int TimeLeft { get; set; }

		public PlayerInfo Player1 { get; set; }

		public PlayerInfo Player2 { get; set; }
	}
	public class DetailedGameState : GameStateActive
	{
		DetailedGameState()
		{
			TimeLimit = 0;
			Board = null;
		}
		public int TimeLimit { get; set; }

		public string Board { get; set; }

		[ScriptIgnore]
		public BoggleBoard boggleBoard { get; set; }
	}

	
}