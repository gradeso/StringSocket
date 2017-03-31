using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Web;
using System.Text.RegularExpressions;
using static System.Net.HttpStatusCode;
using Newtonsoft.Json;

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

        public BoggleService() { }
		//{
		//	sync = new object();
		//	bigDict = new HashSet<string>(Regex.Split(File.ReadAllText("dictionary.txt"), "\n"));
		//	player1 = null;
		//	player2 = null;
		//}

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
		/// Puts the player in pending game queue. 
		/// </summary>
		/// <param name="player">The player.</param>
		/// <returns>returns true if second player and game is ready.</returns>
		private bool PutPlayerInPendingGameQueue(DetailedPlayerInfo player)
		{
			if (pendingGame.Player1 == null)
			{
				pendingGame.Player1 = player;
				return false;
			}
			else if (pendingGame.Player2 == null)
			{
				pendingGame.Player2 = player;
				return true;
			}
			else
			{
				throw new Exception("queue was full when attempting to add player.");
			}
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
					return null;
				}

				string userID = Guid.NewGuid().ToString();
				SetStatus(Created);
				users.Add(userID, new DetailedPlayerInfo(userID, Nickname.Trim()));
                UserIDInfo t = new UserIDInfo();
                t.UserToken = userID;
                string tyne = JsonConvert.SerializeObject(t);
                return tyne;
			}
		}
		public string AttemptJoin(JoinAttempt ja)
			{
			lock (sync)
			{
				if (pendingGame == null) {
					pendingGame = new DetailedGameState();
				}

                
                    return "";
			}
			}

		public void CancelJoin(string UserToken)
		{
			throw new NotImplementedException();
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
