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
	/// <summary>
	/// call POST /users
	/// </summary>
	public class UserNckname
	{
		public string Nickname { get; set; }

	}
	/// <summary>
	/// call POST /games
	/// </summary>
	public class UserTokenAndTimeLimit
	{
		/// <summary>
		/// Gets or sets the user token.
		/// </summary>
		/// <value>
		/// The user token.
		/// </value>
		public string UserToken { get; set; }

			public int TimeLimit { get; set; }
		}

	/// <summary>
	/// response POST games
	/// </summary>
	public class GameIDInfo
	{
		public string GameID { get; set; }
	}
	/// <summary>
	/// used in /games response.
	/// </summary>
	public class ScoreInfo
	{
		public int Score { get; set; }
	}

	/// <summary>
	/// Holds scores and names of player
	/// </summary>
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
		public string GameState { get; set; }
	}
	public class GameStateActive : GameStatePending
	{
		public int TimeLeft { get; set; }

		public PlayerInfo Player1 { get; set; }

		public PlayerInfo Player2 { get; set; }
	}
	public class GameState 
	{
		public int TimeLimit { get; set; }

		public string Board { get; set; }

		[ScriptIgnore]
		public BoggleBoard boggleBoard { get; set; }
	}
	

}