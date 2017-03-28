using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Web;
using static System.Net.HttpStatusCode;

namespace Boggle
{
	public class BoggleService : IBoggleService
	{
		// <summary>
		/// represents the game with the key as the game ID
		/// </summary>
		SortedDictionary<int, BoggleBoard> currentGames;
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

//		/// <summary>
//		Join a game.

//		If UserToken is invalid, TimeLimit< 5, or TimeLimit> 120, responds with status 403 (Forbidden).
//Otherwise, if UserToken is already a player in the pending game, responds with status 409 (Conflict).
//Otherwise, if there is already one player in the pending game, adds UserToken as the second player.The pending game becomes active and a new pending game with no players is created.
//			The active game's time limit is the integer average of the time limits requested by the two players. Returns the new active game's GameID(which should be the same as the old pending game's GameID). Responds with status 201 (Created).
//Otherwise, adds UserToken as the first player of the pending game, and the TimeLimit as the pending game's requested time limit. Returns the pending game's GameID. Responds with status 202 (Accepted).		/// </summary>
//		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public string AttemptJoin()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Starts a new game or stops the join request
		/// </summary>
		/// <exception cref="System.NotImplementedException"></exception>
		public void CancelJoin()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Play a word in a game.
		/// </summary>
		/// <param name="maybeYes"></param>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public string playWordInGame(bool maybeYes)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Plays the word in game with identifier.
		/// </summary>
		/// <param name="GameID">The game identifier.</param>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public string PlayWordInGame(string GameID)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// if Nickname is null, or is empty when trimmed, responds with status 403 (Forbidden).
		/// Otherwise, creates a new user with a unique UserToken and the trimmed Nickname.The returned UserToken should be used to identify the user in subsequent requests.Responds with status 201 (Created).
		/// </summary>
		/// <exception cref="System.NotImplementedException"></exception>
		public void SaveUserID()
		{
			throw new NotImplementedException();
		}

		
	}
}
