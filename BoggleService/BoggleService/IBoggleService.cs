using System.Collections.Generic;
using System.IO;
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
		[WebGet(UriTemplate = "/api")]
		Stream API();

		/// <summary>
		///if Nickname is null, or is empty when trimmed, responds with status 403 (Forbidden).
		///Otherwise, creates a new user with a unique UserToken and the trimmed Nickname.The returned UserToken should be used to identify the user in subsequent requests.Responds with status 201 (Created).
		/// </summary>
		[WebInvoke(
			Method = "POST",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json,
			UriTemplate = "users")]
		string SaveUserID(string Nickname);

		/// <summary>
		/// Starts a new game or stops the join request
		/// </summary>
		[WebInvoke(Method = "PUT",
			UriTemplate = "/games")]
		void CancelJoin(string UserToken);

		/// <summary>
		/// Attempts the join a game.
		/// </summary>
		/// <returns></returns>
		[WebInvoke(Method = "POST",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json,
			UriTemplate = "/games")]
		string AttemptJoin(JoinAttempt ja);

		/// <summary>
		/// Plays the word in game with identifier.
		/// </summary>
		/// <param name="GameID">The game identifier.</param>
		/// <returns></returns>
		[WebInvoke(Method = "PUT",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json, UriTemplate = "/games/{GameID}")]
		string PlayWordInGame(Move moveMade, string GameID);

		/// <summary>
		///Play a word in a game.
		/// </summary>
		[WebGet(BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json,
			UriTemplate = "/games/{GameID}?Brief={maybeYes}")]
		string gameStatus(string GameID, bool maybeYes);
	}
}
