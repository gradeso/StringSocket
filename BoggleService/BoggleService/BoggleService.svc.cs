using Newtonsoft.Json;
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

		private static readonly object sync = new object();
		private static readonly HashSet<string> bigDict = new HashSet<string>(Regex.Split(File.ReadAllText("dictoionary.txt"), "\n"));
		/// <summary>
		/// The users that have registered.
		/// </summary>
		private static readonly Dictionary<string, DetailedPlayerInfo> users = new Dictionary<string, DetailedPlayerInfo>();

		// <summary>
		/// represents the game with the key as the game ID
		/// </summary>
		private static SortedDictionary<int, DetailedGameState> currentGames =
			new SortedDictionary<int, DetailedGameState>();

		/// <summary>
		/// The pending game to be added to dict. when ready.
		/// </summary>
		private static DetailedGameState pendingGame = null;

		private static int GameIDCounter= 0;

		private static int firstPlayersDesiredTimeLimit = 0;

		private static Timer counter = new Timer(1000)
		{
			AutoReset = true
			

		}; 

        public BoggleService() {
			counter.Elapsed += Counter_Elapsed;
			counter.Start();
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
		public HttpResponseMessage SaveUserID(string Nickname)
		{

			lock (sync)
			{
				if (Nickname == null || Nickname.Trim() == "")
				{
					SetStatus(Forbidden);
					return turnToCorrectFormat("");
				}

				string userID = Guid.NewGuid().ToString();
				SetStatus(Created);
				users.Add(userID, new DetailedPlayerInfo(userID, Nickname.Trim()));

				return turnToCorrectFormat(new UserIDInfo(userID));
			}
		}
				/// <summary>
		/// Attempts the join.
		/// </summary>
		/// <param name="ja">The join ateempt info.</param>
		/// <returns></returns>
		public HttpResponseMessage AttemptJoin(string UserToken, int TimeLimit)
		{
			lock (sync)
			{
				try
				{

					if (TimeLimit > 120 || TimeLimit < 5)
					{
						SetStatus(Forbidden);
						return turnToCorrectFormat("");
					}
					pendingGame.Equals(1);
					
				} catch (FormatException) {
					SetStatus(Forbidden);
					return turnToCorrectFormat("");
				} catch (NullReferenceException) {

					//triggered when no players in the queue on the line pendingGame.Player1.userID .
					pendingGame = new DetailedGameState();
					pendingGame.gameID = GameIDCounter++;
					DetailedPlayerInfo firstPlayer;
					if (!users.TryGetValue(UserToken, out firstPlayer))
					{
						SetStatus(Forbidden);
						return turnToCorrectFormat("");
					}
					pendingGame.Player1 = firstPlayer.DeepClone();
					firstPlayersDesiredTimeLimit = Convert.ToInt32(TimeLimit);
					return turnToCorrectFormat(pendingGame.gameID.ToString());
				}
				if (UserToken == pendingGame.Player1.userID)
					{
						SetStatus(Conflict);
						return turnToCorrectFormat("");
					}
				DetailedPlayerInfo secondPlayer;
				if (!users.TryGetValue(UserToken, out secondPlayer))
				{
					SetStatus(Forbidden);
					return turnToCorrectFormat("");
				}
				pendingGame.Player2 = secondPlayer.DeepClone();
				pendingGame.TimeLimit = (firstPlayersDesiredTimeLimit + Convert.ToInt32(TimeLimit))/ 2;
				pendingGame.TimeLeft = pendingGame.TimeLimit;
				pendingGame.GameState = "active";
				pendingGame.boggleBoard = new BoggleBoard();
				pendingGame.Board = pendingGame.boggleBoard.ToString();
				currentGames.Add(pendingGame.gameID, pendingGame.DeepClone());
				SetStatus(Created);

				string toReturn = pendingGame.gameID.ToString();
				pendingGame = null;
				return turnToCorrectFormat(toReturn);
			}
		}

		public void CancelJoin(string UserToken)
		{
			lock (sync)
			{
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


		public HttpResponseMessage PlayWordInGame(string GameID, string UserToken, string Word)
		{
			lock (sync)
			{
				Word = Word.Trim();
				DetailedGameState gameInQuestion;
				int tempGameID;

				try
				{
					if (!int.TryParse(GameID, out tempGameID) || !currentGames.TryGetValue(tempGameID, out gameInQuestion)
						|| !(gameInQuestion.Player1.userID == UserToken || gameInQuestion.Player2.userID == UserToken) )
					{
						SetStatus(Forbidden);
						return turnToCorrectFormat("");
					}
				}
				catch (NullReferenceException)
				{
					SetStatus(Forbidden);
					return turnToCorrectFormat("");
				}
				if (gameInQuestion.GameState != "active")
				{
					SetStatus(Conflict);
					return turnToCorrectFormat("");
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
				return turnToCorrectFormat(toReturn);

				
			}
		}

		private HttpResponseMessage turnToCorrectFormat(object toReturn)
		{
			HttpResponseMessage response= new HttpResponseMessage(WebOperationContext.Current.OutgoingResponse.StatusCode);
			response.Content = new StringContent(JsonConvert.SerializeObject(toReturn), Encoding.UTF8, "application/json");
			return response;
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

		public HttpResponseMessage gameStatus(string GameID, bool maybeYes)
		{
			lock (sync)
			{
				DetailedGameState gameInQuestion;
				int gameIdNum;
				if (!int.TryParse(GameID, out gameIdNum) || !currentGames.TryGetValue(gameIdNum, out gameInQuestion))
				{
					SetStatus(Forbidden);
					return turnToCorrectFormat("");
				}
				switch (gameInQuestion.GameState)
				{
					case "pending":

						return turnToCorrectFormat((GameStatePending)gameInQuestion.DeepClone());
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

						return turnToCorrectFormat(toReturn);

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
						return turnToCorrectFormat(toReturn2);
					default:
						SetStatus(Forbidden);
						return turnToCorrectFormat("");
				}
			}
		}
	}
}
