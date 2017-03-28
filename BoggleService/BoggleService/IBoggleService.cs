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
			
			BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json,
			UriTemplate = "/users")]
		void SaveUserID();

		/// <summary>
		/// Starts a new game or
		/// </summary>
		[WebInvoke(Method = "PUT",  UriTemplate = "/games")]
		void CancelJoin();

		[WebInvoke(Method = "POST", UriTemplate = "/games")]
		void AttemptJoin();

		[WebGet(UriTemplate = "/games/{GameID}")]
		string AcessUserToken(string GameID);


		/// <summary>
		/// Returns the nth word from dictionary.txt.  If there is
		/// no nth word, responds with code 403. This is a demo;
		/// you can delete it.
		/// </summary>
		[WebGet(UriTemplate = "/word?index={n}")]
        string WordAtIndex(int n);
    }
}
