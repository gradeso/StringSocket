using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Web;
using System.Text.RegularExpressions;
using System.Timers;
using static System.Net.HttpStatusCode;

namespace Boggle
{
	public class BoggleService : IBoggleService
	{

		private static readonly object sync = new object();
		private static readonly HashSet<string> bigDict;
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

		private static Timer counter = new Timer(1000); 

        public BoggleService() {
			
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
		public string SaveUserID(string Nickname)
		{

			lock (sync)
			{
				if (Nickname == null || Nickname.Trim() == "")
				{
					SetStatus(Forbidden);
					return "";
				}

				string userID = Guid.NewGuid().ToString();
				SetStatus(Created);
				users.Add(userID, new DetailedPlayerInfo(userID, Nickname.Trim()));

				return userID;
			}
		}
		/// <summary>
		/// Attempts the join.
		/// </summary>
		/// <param name="ja">The join ateempt info.</param>
		/// <returns></returns>
		public string AttemptJoin(JoinAttempt ja)
		{
			lock (sync)
			{
				try
				{

					if (Convert.ToInt32(ja.TimeLimit) > 120 || Convert.ToInt32(ja.TimeLimit) < 5)
					{
						SetStatus(Forbidden);
						return "";
					}
					
				} catch (FormatException) {
					SetStatus(Forbidden);
					return "";
				} catch (NullReferenceException) {

					//triggered when no players in the queue on the line pendingGame.Player1.userID .
					pendingGame = new DetailedGameState();
					pendingGame.gameID = GameIDCounter++;
					DetailedPlayerInfo firstPlayer;
					if (!users.TryGetValue(ja.UserToken, out firstPlayer))
					{
						SetStatus(Forbidden);
						return "";
					}
					pendingGame.Player1 = firstPlayer.DeepClone();
					firstPlayersDesiredTimeLimit = Convert.ToInt32(ja.TimeLimit);
					return pendingGame.gameID.ToString();
				}
				if (ja.UserToken == pendingGame.Player1.userID)
					{
						SetStatus(Conflict);
						return "";
					}
				DetailedPlayerInfo secondPlayer;
				if (!users.TryGetValue(ja.UserToken, out secondPlayer))
				{
					SetStatus(Forbidden);
					return "";
				}
				pendingGame.Player2 = secondPlayer.DeepClone();
				pendingGame.TimeLimit = (firstPlayersDesiredTimeLimit + Convert.ToInt32(ja.TimeLimit))/ 2;
				pendingGame.TimeLeft = pendingGame.TimeLimit;
				pendingGame.GameState = "active";
				currentGames.Add(pendingGame.gameID, pendingGame.DeepClone());
				SetStatus(Created);
				string toReturn = pendingGame.gameID.ToString();
				pendingGame = null;
				return toReturn;
			}
		}

		public void CancelJoin(string UserToken)
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

		

		public string PlayWordInGame(Move moveMade, string GameID)
		{
			throw new NotImplementedException();
		}

		public string gameStatus(string GameID, bool maybeYes)
		{
			throw new NotImplementedException();
		}
	}
}
