using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Boggle
{
	[ServiceContract]
	public interface IBoggleService
	{

		/// <summary>
		/// Sends back index.html as the response body.
		/// </summary>
		
		[WebGet(UriTemplate = "api")]
		Stream API();

        /// <summary>
        ///if Nickname is null, or is empty when trimmed, responds with status 403 (Forbidden).
        ///Otherwise, creates a new user with a unique UserToken and the trimmed Nickname.The returned UserToken should be used to identify the user in subsequent requests.Responds with status 201 (Created).
        /// </summary>
		
        [WebInvoke(
            Method = "POST",
			RequestFormat = WebMessageFormat.Json,
			UriTemplate = "users")]
		UserIDInfo SaveUserID(Name name);

		/// <summary>
		/// Starts a new game or stops the join request
		/// </summary>
		[WebInvoke(Method = "PUT",

			UriTemplate = "games")]

		void CancelJoin(UserIDInfo UserToken);

		/// <summary>
		/// Attempts the join a game.
		/// </summary>
		/// <returns></returns>
		[WebInvoke(Method = "POST",
								ResponseFormat = WebMessageFormat.Json,

		UriTemplate = "games")]

		GameIDInfo AttemptJoin(JoinAttempt ja);

		/// <summary>
		/// Plays the word in game with identifier.
		/// </summary>
		/// <param name="GameID">The game identifier.</param>
		/// <returns></returns>
		[WebInvoke(Method = "PUT",
								ResponseFormat = WebMessageFormat.Json,

			UriTemplate = "games/{GameID}")]

		ScoreInfo PlayWordInGame(string GameID, Move m);

        /// <summary>
        ///gets Stats of game.
        /// </summary>
        [WebGet(
						ResponseFormat = WebMessageFormat.Json,

		UriTemplate = "games/{GameID}?brief={yes}")]
		GameStatePending gameStatus(string GameID, string yes);
                
        
	}
}
