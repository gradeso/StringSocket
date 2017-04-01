using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
    [DataContract]
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

        [DataMember]
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
			Nicknme = "";
			Score = 0;
		}
		
		[ScriptIgnore]
		public string userID { get; set; }

		public string Nicknme { get; set; }

		public int Score { get; set; }
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
			MovesMade = new List<WordAndScore>();

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
			gameID = -1;
		}
		
		public string GameState { get; set; }

		[ScriptIgnore]
		public int gameID { get; set; }
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

    
	public class DetailedGameState: GameStateActive
	{
		public DetailedGameState()
		{
			TimeLimit = 0;
			Board = null;
			boggleBoard = null;
		}
		public int TimeLimit { get; set; }

		public string Board { get; set; }

		[ScriptIgnore]
		public BoggleBoard boggleBoard { get; set; }
	}
	public static class ExtensionMethods
	{
		// Deep clone
		public static T DeepClone<T>(this T a)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(stream, a);
				stream.Position = 0;
				return (T)formatter.Deserialize(stream);
			}
		}
	}

}