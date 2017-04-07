using Boggle;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using static System.Net.HttpStatusCode;

namespace Boggle
{
	public class BoggleService : IBoggleService
	{

		private static readonly object sync;
		private static readonly HashSet<string> bigDict;
		private static int GameIDCounter;

		private static string BoggleDB;
		private static Timer globalTimer;

		static BoggleService()
		{
			BoggleDB = ConfigurationManager.ConnectionStrings["BoggleDB"].ConnectionString;
			bigDict = new HashSet<string>(File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "dictionary.txt"));
			globalTimer = new Timer(1000) { AutoReset = true};	
			sync = new object();
			GameIDCounter = 0;
			globalTimer.Elapsed += GlobalTimer_Elapsed;
		}

		/// <summary>
		/// Handles the Elapsed event of the GlobalTimer control.
		/// decrements the time left in all games marked active.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
		/// <exception cref="System.NotImplementedException"></exception>
		private static void GlobalTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			for (int i = GameIDCounter; i > -1; i--)
			{
				
			}
		}


		/// <summary>
		/// The most recent call to SetStatus determines the response code used when
		/// an http response is sent.
		/// </summary>
		/// <param name="status"></param>
		private static void SetStatus(HttpStatusCode status)
		{
			WebOperationContext.Current.OutgoingResponse.StatusCode = status;
		}

		/// <summary>
		/// Returns a Stream version of index.html.
		/// </summary>
		/// <returns></returns>
		public Stream API()
		{
			SetStatus(OK);
			WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
			return File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + "index.html");
		}

		/// <summary>
		/// if Nickname is null, or is empty when trimmed, responds with status 403 (Forbidden).
		/// Otherwise, creates a new user with a unique UserToken and the trimmed Nickname.The returned UserToken should be used to identify the user in subsequent requests.Responds with status 201 (Created).
		/// </summary>
		/// <param name="Nickname"></param>
		/// <returns></returns>
		public UserIDInfo SaveUserID(Name Nickname)
		{
			string n = Nickname.Nickname;
			lock (sync)
			{
				if (n == null || n.Trim() == "")
				{
					SetStatus(Forbidden);
					return new UserIDInfo();
				}

				string userID = Guid.NewGuid().ToString();
				SetStatus(Created);
				// using guarentees connection will drop after leaving code
				using (SqlConnection conn = new SqlConnection(BoggleDB))
				{
					conn.Open();
					// transaction that contains all commands
					using (SqlTransaction trans = conn.BeginTransaction())
					{
						using (SqlCommand command = new SqlCommand("INSERT INTO Users (UserID, Nickname) values (@UserID, @Nickname)", conn, trans))
						{
							command.Parameters.AddWithValue("@UserID", userID);
							command.Parameters.AddWithValue("@Nickname", n);

							command.ExecuteNonQuery();
							// commit transaction, otherwise abort
							trans.Commit();
							
						}
					}
					conn.Close();
				}

				return new UserIDInfo(userID);
			}
		}
				/// <summary>
		/// Attempts the join.
		/// </summary>
		/// <param name="ja">The join ateempt info.</param>
		/// <returns></returns>
		public GameIDInfo AttemptJoin(JoinAttempt ja)
		{
			int TimeLimit = ja.TimeLimit;
			string UserToken = ja.UserToken;
			lock (sync)
			{
				try
				{

					if (TimeLimit > 120 || TimeLimit < 5)
					{
						SetStatus(Forbidden);
						return new GameIDInfo("");
					}
					var pendingGame = getGameWithID(GameIDCounter);
					if (UserToken == pendingGame.Player1.userID)
					{
					}
					} catch (FormatException) {
					SetStatus(Forbidden);
					return new GameIDInfo("");
				} catch (NullReferenceException) {
					DetailedGameState pendingGame = new DetailedGameState(GameIDCounter++);
					pendingGame.Player1 = findPlayer(UserToken);
					pendingGame.TimeLimit = Convert.ToInt32(TimeLimit);
					addGameToDB(pendingGame);
					return new GameIDInfo(pendingGame.gameID.ToString());
				}
				DetailedGameState savedGame = getGameWithID(GameIDCounter);
				if (UserToken == savedGame.Player1.userID)
				{
						SetStatus(Conflict);
						return new GameIDInfo("user already in game");
				}

				savedGame.Player2 = findPlayer(UserToken);
				if (savedGame.Player2 == null)
				{
					SetStatus(Forbidden);
					return new GameIDInfo("bad id for player 2");
				}
				savedGame.TimeLimit = (savedGame.TimeLimit + TimeLimit)/ 2;
				savedGame.TimeLeft = savedGame.TimeLimit;
				savedGame.GameState = "active";
				savedGame.boggleBoard = new BoggleBoard();
			    savedGame.Board = savedGame.boggleBoard.ToString();
				
				addGameToDB(savedGame);
				SetStatus(Created);
				string toReturn = savedGame.gameID.ToString();
				return new GameIDInfo(toReturn);
			}
		}
		

		public void CancelJoin(UserIDInfo ut)
		{
			lock (sync)
			{
				string UserToken = ut.UserToken;
				var player = findPlayer(UserToken);
				var pendingGame = getGameWithID(GameIDCounter);
				try
				{
					if (pendingGame.Player1.userID == UserToken)
					{
						pendingGame = null;
						SetStatus(OK);
					}
				}
				catch
				{

				}
			SetStatus(Forbidden);
			}
		}


		public ScoreInfo PlayWordInGame(string GameID, Move m)
		{
			lock (sync)
			{
				string UserToken = m.UserToken;
				string Word = m.Word;
				Word = Word.Trim();
				DetailedGameState gameInQuestion;
				int tempGameID;

				try
				{
					if (!int.TryParse(GameID, out tempGameID))
					{
						SetStatus(Forbidden);
						return new ScoreInfo();
					}
					gameInQuestion = getGameWithID(tempGameID);

					if (!(gameInQuestion.Player1.userID == UserToken || gameInQuestion.Player2.userID == UserToken))
					{
						SetStatus(Forbidden);
						return new ScoreInfo();
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
					SetStatus(Forbidden);
					return new ScoreInfo();
				}
				if (gameInQuestion.GameState != "active")
				{
					SetStatus(Conflict);
					return new ScoreInfo();
				}
				//needs finnishing
			//int scoreOfWord = calculateScore(gameInQuestion.boggleBoard, Word);
			ScoreInfo toReturn = new ScoreInfo();
			
			//return toReturn;

			//
			//if (gameInQuestion.Player1.userID == UserToken)
			//{
			//	gameInQuestion.Player1.Score += scoreOfWord;
			//	((DetailedPlayerInfo)gameInQuestion.Player1).MovesMade.Add(wac);
			//}
			//else
			//{

			//	gameInQuestion.Player2.Score += scoreOfWord;
			//	((DetailedPlayerInfo)gameInQuestion.Player2).MovesMade.Add(wac);

			//}
		

	}
		
		

		public GameStatePending gameStatus(string GameID, bool maybeYes)
		{
			lock (sync)
			{
				DetailedGameState gameInQuestion;
				int gameIdNum;
				if (!int.TryParse(GameID, out gameIdNum))
				{
					SetStatus(Forbidden);
					return new GameStatePending();
				}
				gameInQuestion = getGameWithID(gameIdNum);
				switch (gameInQuestion.GameState)
				{
					case "pending":

						return (GameStatePending)gameInQuestion.DeepClone();
					case "active":

						PlayerInfo tempPlr1 = (PlayerInfo)(gameInQuestion.Player1.DeepClone());
						PlayerInfo tempPlr2 = (PlayerInfo)(gameInQuestion.Player2.DeepClone());
						GameStatePending toReturn;
						if (maybeYes)
						{
							toReturn = (GameStateActive)gameInQuestion.DeepClone();
							((GameStateActive)toReturn).Player1 = tempPlr1;
							((GameStateActive)toReturn).Player2 = tempPlr2;

						}
						else
						{
							toReturn = gameInQuestion.DeepClone();
							((DetailedGameState)toReturn).Player1 = tempPlr1;
							((DetailedGameState)toReturn).Player2 = tempPlr2;

						}

						return toReturn;

					case "completed":
						GameStatePending toReturn2;
						if (maybeYes)
						{
							PlayerInfo tempPlr11 = (PlayerInfo)(gameInQuestion.Player1.DeepClone());
							PlayerInfo tempPlr22 = (PlayerInfo)(gameInQuestion.Player2.DeepClone());
							toReturn2 = (GameStateActive)gameInQuestion.DeepClone();
							((GameStateActive)toReturn2).Player1 = tempPlr11;
							((GameStateActive)toReturn2).Player2 = tempPlr22;

						}
						else
						{
							toReturn2 = gameInQuestion.DeepClone();
						}
						return toReturn2;
					default:
						SetStatus(Forbidden);
						return new GameStatePending() ;
				}
			}
		}

		private DetailedGameState getGameWithID(int gameID)
		{
			object[] toChange = new object[6];
			int i = 0;
			toChange[0] = gameID;
			using (SqlConnection conn = new SqlConnection(BoggleDB))
			{
				SqlCommand cmd = new SqlCommand("SELECT * FROM Games WHERE ID = @ID", conn);

				try
				{
					conn.Open();
					cmd.Parameters.AddWithValue("@ID", gameID);
					var rdr = cmd.ExecuteReader();
					while (rdr.Read())
					{
						toChange[++i] = rdr["Player1"];
						toChange[++i] = rdr["Player2"];
						toChange[++i] = rdr["Board"];
						toChange[++i] = rdr["TimeLimit"];
						toChange[++i] = rdr["StartTime"];

					}
					return detailGame(toChange);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					return null;
				}
			}
		}

		private void addGameToDB(DetailedGameState pendingGame)
		{
			object[] contents = simplifyGameState(pendingGame);
			using (SqlConnection conn = new SqlConnection(BoggleDB))
			{

				SqlCommand cmd = new SqlCommand("IF NOT EXISTS(SELECT 1 from Games WHERE GameID=@GameID)" +
			   " Insert INTO Games (GameID,Player1,Player2,Board,TimeLimit,StartTime) VALUES(@GameID,@Player1,@Player2,@Board,@TimeLimit,@StartTime)" +
			   " else" +
			   " UPDATE  SET GameID=@GameID,Player1=@Player1,Player2=@Player2,Board=@Board,TimeLimit=@TimeLimit,StartTime=@StartTime WHERE GameID=@GameID"
			   , conn);
				cmd.Parameters.AddRange(contents);
				cmd.ExecuteNonQuery();
			}
		}
		private DetailedGameState detailGame(object[] input)
		{
			var toReturn = new DetailedGameState();

			toReturn.gameID = (int)input[0];
			toReturn.Player1 = findPlayer(input[1]);
			toReturn.Player2 = findPlayer(input[2]);
			toReturn.Board = (string)input[3];
			toReturn.TimeLimit = (int)input[4];
			toReturn.boggleBoard = new BoggleBoard((string)input[3]);
			toReturn.TimeLeft = (DateTime.Now - (DateTime)input[5]).Seconds > 0 ? (DateTime.Now - (DateTime)input[5]).Seconds : 0;
			toReturn.GameState = (input[2] == null ? "pending" : toReturn.TimeLeft == 0 ? "completed" : "active");

			return toReturn;
		}

		private PlayerInfo findPlayer(object v)
		{
			if (v == null) return null;

			using (SqlConnection conn = new SqlConnection(BoggleDB))
			{
				SqlCommand cmd = new SqlCommand("SELECT * FROM Users WHERE id = @ID", conn);

				try
				{
					conn.Open();
					cmd.Parameters.AddWithValue("@ID", (string)v);
					SqlDataReader rdr = cmd.ExecuteReader();
					while (rdr.Read())
					{
						return new DetailedPlayerInfo((string)rdr["UserID"], (string)rdr["Nickname"]);

					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message + "or user was not registerd");
					return null;

				}
			}
			return null;

		}

		private object[] simplifyGameState(DetailedGameState pendingGame)
		{
			object[] toSave = new object[6];
			toSave[0] = pendingGame.gameID;
			toSave[1] = pendingGame.Player1.userID;
			toSave[2] = pendingGame.Player2.userID;
			toSave[3] = pendingGame.Board;
			toSave[4] = pendingGame.TimeLimit;
			toSave[5] = DateTime.Now;
			return toSave;

		}

		private int calculateScore(BoggleBoard boggleBoard, string word)
		{

			//here you need to add the word to the database,
			//if the word is already there set the score to zero.
			
			return (boggleBoard.CanBeFormed(word) && bigDict.Contains(word)) ?
				word.Length < 3 ?
				0 : word.Length < 5 ?
				1 : word.Length < 6 ?
				2 : word.Length < 7 ?
				3 : word.Length < 8 ?
				5 : 11
				  : 0;
		}

	}
}
