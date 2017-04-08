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
using System.Data.SqlTypes;
using System.Data;

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

            // this clears and reseeds the database on startup for testing
            // using guarentees connection will drop after leaving code
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                // transaction that contains all commands
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    using (SqlCommand command = new SqlCommand("DELETE FROM Words DELETE FROM Games DELETE FROM Users DBCC CHECKIDENT (Games, RESEED, 0)", conn, trans))
                    {

                        command.ExecuteNonQuery();

                        // commit transaction, otherwise abort
                        trans.Commit();
                    }
                }
            }
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
            lock (sync)
			{
                string n = Nickname.Nickname;
                
                if (n == null || n.Trim() == "")
                {
                    SetStatus(Forbidden);
                    return null;
                }

                // using guarentees connection will drop after leaving code
                using (SqlConnection conn = new SqlConnection(BoggleDB))
				{
					conn.Open();
					// transaction that contains all commands
					using (SqlTransaction trans = conn.BeginTransaction())
					{
						using (SqlCommand command = new SqlCommand("INSERT INTO Users (UserID, Nickname) values (@UserID, @Nickname)", conn, trans))
						{
                            // todolist generates id within command
                            string userID = Guid.NewGuid().ToString();

                            command.Parameters.AddWithValue("@UserID", userID);
							command.Parameters.AddWithValue("@Nickname", n);

							command.ExecuteNonQuery();
                            SetStatus(Created);

                            // commit transaction, otherwise abort
                            trans.Commit();
                            return new UserIDInfo(userID);
                        }
					}
				}
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
                // check if invalid game arguments are given
                if (TimeLimit < 5 || TimeLimit > 120 || ja == null || ja.UserToken == null)
                {
                    SetStatus(Forbidden);
                    return null;
                }

                // need to check if user is in the Users sql table, otherwise we can't make a new game
                PlayerInfo foundPlayer = findPlayer(UserToken);
                if (foundPlayer == null)
                {
                    return null;
                }

                // now we know valid arguments are given
                // this is like a thing that only works if there are pending games?
                try
				{
					var pendingGame = getGameWithID(GameIDCounter);
					if (UserToken == pendingGame.Player1.userID)
					{
                    }
                }
                // this is thrown by the getGameWithID method
                catch (FormatException)
                {
					SetStatus(Forbidden);
					return null;
				}
                // this is if there isnt a pending game and I try to get a userID from it
                // so if there isnt a new pending game we make one with the player
                catch (NullReferenceException)
                {
					DetailedGameState pendingGame = new DetailedGameState(GameIDCounter++);
                    pendingGame.Player1 = foundPlayer;
					pendingGame.TimeLimit = Convert.ToInt32(TimeLimit);
					addGameToDB(pendingGame);
                    SetStatus(Accepted);
					return new GameIDInfo(pendingGame.gameID.ToString());
				}

				DetailedGameState savedGame = getGameWithID(GameIDCounter);
				if (UserToken == savedGame.Player1.userID)
				{
						SetStatus(Conflict);
						return new GameIDInfo("user already in game");
				}

				savedGame.Player2 = foundPlayer;
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
				return toReturn;
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

        /// <summary>
        /// helper method for getting game with a certain id
        /// </summary>
		private DetailedGameState getGameWithID(int gameID)
		{
            // object array that will hold all the parts
            // of a game from the sql table
            // if there is an error while 
			object[] toChange = new object[6];
			toChange[0] = gameID;
			using (SqlConnection conn = new SqlConnection(BoggleDB))
			{
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Games WHERE GameID = @gameID", conn);

                cmd.Parameters.AddWithValue("@gameID", gameID);
                try
				{
					SqlDataReader rdr = cmd.ExecuteReader();

					while (rdr.Read())
					{
                        
						toChange[1] = rdr["Player1"];
                        toChange[2] = rdr["Player2"];
                        toChange[3] = rdr["Board"];
                        toChange[4] = rdr["TimeLimit"];
                        toChange[5] = rdr["StartTime"];

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
                conn.Open();

				SqlCommand cmd = new SqlCommand("IF NOT EXISTS(SELECT 1 FROM Games WHERE GameID=@GameID)" +
			   " Insert INTO Games (Player1,Player2,Board,TimeLimit,StartTime) VALUES(@Player1,@Player2,@Board,@TimeLimit,@StartTime)" +
			   " else" +
			   " UPDATE Games SET Player1=@Player1,Player2=@Player2,Board=@Board,TimeLimit=@TimeLimit,StartTime=@StartTime WHERE GameID=@GameID"
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
			toReturn.Board = input[3].GetType() == typeof(DBNull) ? null : (string)input[3];
            // input 4 will be of type dbnull but not likely null
			toReturn.TimeLimit = input[4] == null ? 0 : (int)input[4];
			toReturn.boggleBoard = input[3].GetType() == typeof(DBNull) ? null : new BoggleBoard((string)input[3]);
            if(input[5].GetType() != typeof(DBNull))
            {
                toReturn.TimeLeft = (DateTime.Now - (DateTime)input[5]).Seconds > 0 ? (DateTime.Now - (DateTime)input[5]).Seconds : 0;
            }
			
			toReturn.GameState = (input[2].GetType() == typeof(DBNull) ? "pending" : toReturn.TimeLeft == 0 ? "completed" : "active");

			return toReturn;
		}

		private PlayerInfo findPlayer(object v)
		{
			if (v == null || v.GetType() == typeof(DBNull)) return null;

			using (SqlConnection conn = new SqlConnection(BoggleDB))
			{
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Users WHERE UserID = @ID", conn))
                {
                    cmd.Parameters.AddWithValue("@ID", (string)v);

                    SqlDataReader rdr = cmd.ExecuteReader();

                    // checks if the user is there
                    if (!rdr.HasRows)
                    {
                        SetStatus(Forbidden);
                        return null;
                    }

                    while (rdr.Read())
                    {
                        return new DetailedPlayerInfo((string)rdr["UserID"], (string)rdr["Nickname"]);
                    }

                    return null;
                }
            }
		}

		private SqlParameter[] simplifyGameState(DetailedGameState pendingGame)
		{
            var spcArray = new SqlParameter[6];
            spcArray[0] = new SqlParameter("@GameID", pendingGame.gameID);
            spcArray[1] = new SqlParameter("@Player1", pendingGame.Player1.userID);

            // might change later, needed to prevent null reference
            // player 2 can be null which is why we need this if statement
            if (pendingGame.Player2 != null) { spcArray[2] = new SqlParameter("@Player2", pendingGame.Player2.userID); }
            else { spcArray[2] = new SqlParameter("@Player2", SqlChars.Null); }

            // same with the board
            if (pendingGame.Board != null) { spcArray[3] = new SqlParameter("@Board", pendingGame.Board); }
            else { spcArray[3] = new SqlParameter("@Board", SqlChars.Null); }

            spcArray[4] = new SqlParameter("@TimeLimit", pendingGame.TimeLimit);
            spcArray[5] = new SqlParameter("@StartTime", DateTime.Now);
            /*
            object[] toSave = new object[6];
            toSave[0] = pendingGame.gameID;
			toSave[1] = pendingGame.Player1.userID;
            toSave[2] = pendingGame.Player2.userID;
			toSave[3] = pendingGame.Board;
			toSave[4] = pendingGame.TimeLimit;
			toSave[5] = DateTime.Now;
            */
			return spcArray;

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
