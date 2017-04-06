using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Script.Serialization;

namespace Boggle
{

	[DataContract]
	public class Move
	{
		[DataMember]
		public string UserToken { get; set; }
		[DataMember]
		public string Word { get; set; }
	}

	[DataContract]
	public class JoinAttempt
	{
		[DataMember]
		public string UserToken { get; set; }
		[DataMember]
		public int TimeLimit { get; set; }

	}
	[DataContract]
	public class Name
	{
		[DataMember]
		public string Nickname { get; set; }
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
	[DataContract]
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
		[DataMember]
		public string GameID { get; set; }
	}

	/// <summary>
	/// used in PUT /games/{GameID} response.
	/// </summary>
	[DataContract]
	public class ScoreInfo
	{
		public ScoreInfo()
		{
			Score = 0;
		}
		[DataMember]
		public int Score { get; set; }
	}

	/// <summary>
	/// Holds scores and names of player. Used in game status GET.
	/// </summary>
	[Serializable]
	[DataContract]
	public class PlayerInfo
	{
		public PlayerInfo()
		{
			userID = "";
			Nicknme = "";
			Score = 0;
		}


		public string userID { get; set; }
		[DataMember]
		public string Nicknme { get; set; }
		[DataMember]
		public int Score { get; set; }
	}

	//i will make a user struct to save wrd and score




	[Serializable]
	[DataContract]
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

		[DataMember]
		public List<WordAndScore> MovesMade;

	}
	[Serializable]
	[DataContract]
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

		public string ownersToken { get; set; }
		[DataMember]
		public string Word { get; set; }
		[DataMember]
		public int Score { get; set; }
	}

	/// <summary>
	/// these are for the gamestate object
	/// </summary>
	[Serializable]
	[DataContract]
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
		[DataMember]
		public string GameState { get; set; }

		[DataMember]
		public int gameID { get; set; }

	}
	[Serializable]
[DataContract]
	public class GameStateActive : GameStatePending
	{
		public GameStateActive()
		{
			TimeLeft = 0;
			Player1 = null;
			Player2 = null;
		}
		[DataMember]
        public int TimeLeft { get; set; }
		[DataMember]
		public PlayerInfo Player1 { get; set; }
		[DataMember]
		public PlayerInfo Player2 { get; set; }
	}
	[Serializable]
	[DataContract]

	public class DetailedGameState : GameStateActive
	{
		public DetailedGameState()
		{
			TimeLimit = 0;
			Board = null;
			boggleBoard = null;
			MovesMade = new List<WordAndScore>();
		}
		public DetailedGameState(int gameID)
		{
			this.gameID = gameID;
			TimeLimit = 0;
			Board = null;
			boggleBoard = null;
			MovesMade = new List<WordAndScore>();
		}


		[DataMember]
		public int TimeLimit { get; set; }
		[DataMember]
		public string Board { get; set; }

		public List<WordAndScore> MovesMade { get; set; }

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