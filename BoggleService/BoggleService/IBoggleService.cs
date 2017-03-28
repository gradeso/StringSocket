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

        ///// <summary>
        ///// Returns the nth word from dictionary.txt.  If there is
        ///// no nth word, responds with code 403. This is a demo;
        ///// you can delete it.
        ///// </summary>
        //[WebGet(UriTemplate = "/word?index={n}")]
        //string WordAtIndex(int n);

        /// <summary>
        /// Registers a new user.
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/users")]
        UserID CreateUser(UserName userName);

        /// <summary>
        /// deals with a join request from a client
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/games")]
        GameCreateResponseInfo JoinGame(GameCreateRequestInfo requestInfo);

        /// <summary>
        /// deals with a cancel join request from a client
        /// </summary>
        /// <param name="userID"></param>
        [WebInvoke(Method = "PUT", UriTemplate = "/games")]
        void CancelJoinRequest(UserID userID);

        /// <summary>
        /// handles a play word request from a client
        /// </summary>
        /// <param name="playWordInfo"></param>
        /// <param name="GameID"></param>
        /// <returns></returns>
        [WebInvoke(Method = "PUT", UriTemplate = "/games/{GameID}")]
        PlayWordResponseInfo PlayWord(PlayWordInfo playWordInfo, string gameID);

        /// <summary>
        /// handles a game status request from the server
        /// </summary>
        /// <param name="brief"></param>
        /// <param name="GameID"></param>
        /// <returns></returns>
        [WebInvoke(Method = "GET", UriTemplate = "/games/{GameID}")]
        GameStatusResponse GameStatus(BriefThing brief, string gameID);
    }
}
