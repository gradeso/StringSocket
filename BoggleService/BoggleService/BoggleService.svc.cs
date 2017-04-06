using Boggle;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
		/// <summary>
		/// The users that have registered.
		/// </summary>
		private static readonly Dictionary<string, DetailedPlayerInfo> users;

		// <summary>
		/// represents the game with the key as the game ID
		/// </summary>
		private static SortedDictionary<int, DetailedGameState> currentGames;

		/// <summary>
		/// The pending game to be added to dict. when ready.
		/// </summary>
		private static DetailedGameState pendingGame; 

		private static int GameIDCounter;

		private static int firstPlayersDesiredTimeLimit;

		static BoggleService()
		{
			try
			{
				bigDict = new HashSet<string>(Regex.Split(File.ReadAllText("Boggle.dictionary.txt"), "\n"));
			}
			catch
			{
				bigDict = null;
			}
			sync = new object();
			users = new Dictionary<string, DetailedPlayerInfo>();
			currentGames = new SortedDictionary<int, DetailedGameState>();
			pendingGame = null;
			GameIDCounter = 0;
			firstPlayersDesiredTimeLimit = 0;
		}
		private void Counter_Elapsed(object sender, ElapsedEventArgs e)
		{
			foreach (DetailedGameState dgs in currentGames.Values)
			{
				if (dgs.GameState == "active")
				{
					dgs.TimeLeft--;
					if (dgs.TimeLeft <= 0)
					{
						dgs.GameState = "completed";
					}
				}
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
				users.Add(userID, new DetailedPlayerInfo(userID, n.Trim()));

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
					pendingGame.Equals(1);
					if (UserToken == pendingGame.Player1.userID)
					{
					}
					} catch (FormatException) {
					SetStatus(Forbidden);
					return new GameIDInfo("");
				} catch (NullReferenceException) {

					//triggered when no players in the queue on the line pendingGame.Player1.userID .
					pendingGame = new DetailedGameState();
					pendingGame.gameID = GameIDCounter++;
					DetailedPlayerInfo firstPlayer;
					if (!users.TryGetValue(UserToken, out firstPlayer))
					{
						SetStatus(Forbidden);
						return new GameIDInfo("");
					}
					pendingGame.Player1 = firstPlayer.DeepClone();
					firstPlayersDesiredTimeLimit = Convert.ToInt32(TimeLimit);
					return new GameIDInfo(pendingGame.gameID.ToString());
				}
				if (UserToken == pendingGame.Player1.userID)
					{
						SetStatus(Conflict);
						return new GameIDInfo("");
					}
				DetailedPlayerInfo secondPlayer;
				if (!users.TryGetValue(UserToken, out secondPlayer))
				{
					SetStatus(Forbidden);
					return new GameIDInfo("");
				}
				pendingGame.Player2 = secondPlayer.DeepClone();
				pendingGame.TimeLimit = (firstPlayersDesiredTimeLimit + Convert.ToInt32(TimeLimit))/ 2;
				pendingGame.TimeLeft = pendingGame.TimeLimit;
				pendingGame.GameState = "active";
				pendingGame.boggleBoard = new BoggleBoard();
				pendingGame.Board = pendingGame.boggleBoard.ToString();
				pendingGame.startTimer();
				currentGames.Add(pendingGame.gameID, pendingGame.DeepClone());
				SetStatus(Created);
				
				string toReturn = pendingGame.gameID.ToString();
				pendingGame = null;
				return new GameIDInfo(toReturn);
			}
		}

		public void CancelJoin(UserIDInfo ut)
		{
			lock (sync)
			{
				string UserToken = ut.UserToken;
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


		public ScoreInfo PlayWordInGame(string GameID,Move m)
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
					if (!int.TryParse(GameID, out tempGameID) || !currentGames.TryGetValue(tempGameID, out gameInQuestion)
						|| !(gameInQuestion.Player1.userID == UserToken || gameInQuestion.Player2.userID == UserToken) )
					{
						SetStatus(Forbidden);
						return new ScoreInfo();
					}
				}
				catch (NullReferenceException)
				{
					SetStatus(Forbidden);
					return new ScoreInfo();
				}
				if (gameInQuestion.GameState != "active")
				{
					SetStatus(Conflict);
					return new ScoreInfo();
				}
				int scoreOfWord = calculateScore(gameInQuestion.boggleBoard, Word);
				if (scoreOfWord != 0)
				{
					WordAndScore wac = new WordAndScore(UserToken, Word, scoreOfWord);
					gameInQuestion.MovesMade.Add(wac);
					if (gameInQuestion.Player1.userID == UserToken)
					{
						gameInQuestion.Player1.Score += scoreOfWord;
						((DetailedPlayerInfo)gameInQuestion.Player1).MovesMade.Add(wac);
					}
					else
					{

						gameInQuestion.Player2.Score += scoreOfWord;
						((DetailedPlayerInfo)gameInQuestion.Player2).MovesMade.Add(wac);

					}
				}
				ScoreInfo toReturn = new ScoreInfo();
				toReturn.Score = scoreOfWord;
				return toReturn;

				
			}
		}
		

		private int calculateScore(BoggleBoard boggleBoard, string word)
		{
			return (boggleBoard.CanBeFormed(word) && bigDict.Contains(word)) ?
				word.Length < 3 ?
				0 : word.Length < 5 ?
				1 : word.Length < 6 ?
				2 : word.Length < 7 ?
				3 : word.Length < 8 ?
				5 : 11
				  : 0;
		}

		public GameStatePending gameStatus(string GameID, bool maybeYes)
		{
			lock (sync)
			{
				DetailedGameState gameInQuestion;
				int gameIdNum;
				if (!int.TryParse(GameID, out gameIdNum) || !currentGames.TryGetValue(gameIdNum, out gameInQuestion))
				{
					SetStatus(Forbidden);
					return new GameStatePending();
				}
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
	}
}
