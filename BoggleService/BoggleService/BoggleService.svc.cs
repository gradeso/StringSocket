using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.ServiceModel.Web;
using System.Threading;
using static System.Net.HttpStatusCode;

namespace Boggle
{
    public class BoggleService : IBoggleService
    {
        /// <summary>
        /// a dictionary of users to keep track of all registered users
        /// </summary>
        private readonly static Dictionary<string, string> users = new Dictionary<string, string>();

        /// <summary>
        /// a dictionary to keep track of the games
        /// </summary>
        private readonly static Dictionary<string, Game> games = new Dictionary<string, Game>();

        /// <summary>
        /// a game object for holding a pending game
        /// </summary>
        private static Game pendingGame = null;

        /// <summary>
        /// an object to use for locking
        /// </summary>
        private static readonly object sync = new object();

        /// <summary>
        /// a char array to use in generating tokens
        /// code found at http://stackoverflow.com/questions/19298801/generating-random-string-using-rngcryptoserviceprovider
        /// </summary>
        static readonly char[] AvailableCharacters = {
    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
    'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
    'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
    'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
    '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_'
        };

        /// <summary>
        /// a token size constant for easy maintainability if we want to change the size of tokens
        /// </summary>
        private const int NEWTOKENSIZE = 40;

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

        public UserID CreateUser(UserName username)
        {
            //test for special character that will cause the server to slow
            if (username.Nickname.Substring(0, 1) == "@")
            {
                Thread.Sleep(10000);
            }

            lock (sync)
            {
                //if the name is null or nothing after trimming
                if (username.Nickname == null || username.Nickname.Trim().Length == 0)
                {
                    SetStatus(Forbidden);
                    return null;
                }

                //test for purposful failure character
                if (username.Nickname.Substring(0,1) == "#")
                {
                    SetStatus(NotImplemented);
                    return null;
                }

                //generate a random token
                string newToken = GenerateTokenString(NEWTOKENSIZE);

                //make sure the token is unique
                while (users.ContainsKey(newToken))
                {
                    newToken = GenerateTokenString(NEWTOKENSIZE);
                }
                //add the user
                users.Add(newToken, username.Nickname);

                //set the status and return
                SetStatus(Created);
                return new UserID(newToken);
            }           
        }

        /// <summary>
        /// handles a join game request
        /// </summary>
        /// <param name="leRequest"></param>
        /// <returns></returns>
        public GameCreateResponseInfo JoinGame(GameCreateRequestInfo leRequest)
        {
            lock (sync)
            {
                //if invalid arguments are given
                if ((!users.ContainsKey(leRequest.UserToken)) || leRequest.TimeLimit < 5 || leRequest.TimeLimit > 120)
                {
                    SetStatus(Forbidden);
                    return null;
                }
                else if (pendingGame != null)
                {
                    //add the user to the game and add it to the list
                    pendingGame.Player2ID = leRequest.UserToken;
                    string temp = "";
                    users.TryGetValue(leRequest.UserToken, out temp);
                    pendingGame.Player2Nickname = temp;
                    pendingGame.StartGame();
                    games.Add(pendingGame.GameID, pendingGame);


                    //set up response object
                    GameCreateResponseInfo responseData = new GameCreateResponseInfo(pendingGame.GameID);

                    //game is no longer pending so set it to null
                    pendingGame = null;

                    //return response
                    SetStatus(Created);
                    return responseData;
                }
                else
                {
                    //get a new token
                    string newToken = GenerateTokenString(NEWTOKENSIZE);
                    while (games.ContainsKey(newToken))
                    {
                        newToken = GenerateTokenString(NEWTOKENSIZE);
                    }

                    string tempString = "";
                    users.TryGetValue(leRequest.UserToken, out tempString);

                    //create a new pending game
                    pendingGame = new Game(tempString, leRequest.UserToken, newToken, leRequest.TimeLimit);

                    //setup response and send it
                    GameCreateResponseInfo responseData = new GameCreateResponseInfo(pendingGame.GameID);
                    SetStatus(Accepted);
                    return responseData; 
                }
            }
        }

        /// <summary>
        /// cancels a pending game if that ID is in a pending game
        /// </summary>
        /// <param name="userID"></param>
        public void CancelJoin(UserID userID)
        {
            lock (sync)
            {
                //if the userid is null or if pending game is null
                if (userID == null || pendingGame == null)
                {
                    SetStatus(Forbidden);
                    return;
                }
                else if (pendingGame.Player1ID != userID.UserToken)
                {
                    SetStatus(Forbidden);
                    return;
                }
                //otherwise the pending game needs to be canceled
                else
                {

                }
            }
        }

        /// <summary>
        /// helper method that generates a token string to use for user and
        /// game id's
        /// technique idea found at 
        /// http://stackoverflow.com/questions/19298801/generating-random-string-using-rngcryptoserviceprovider
        /// </summary>
        /// <returns></returns>
        private string GenerateTokenString(int length)
        {
            char[] identifier = new char[length];
            byte[] randomData = new byte[length];

            //randomize
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomData);
            }

            //use randomized data to create token
            for (int idx = 0; idx < identifier.Length; idx++)
            {
                int pos = randomData[idx] % AvailableCharacters.Length;
                identifier[idx] = AvailableCharacters[pos];
            }

            //return token
            return new string(identifier);
        }

        ///// <summary>
        ///// Demo.  You can delete this.
        ///// </summary>
        //public string WordAtIndex(int n)
        //{
        //    if (n < 0)
        //    {
        //        SetStatus(Forbidden);
        //        return null;
        //    }

        //    string line;
        //    using (StreamReader file = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + "dictionary.txt"))
        //    {
        //        while ((line = file.ReadLine()) != null)
        //        {
        //            if (n == 0) break;
        //            n--;
        //        }
        //    }

        //    if (n == 0)
        //    {
        //        SetStatus(OK);
        //        return line;
        //    }
        //    else
        //    {
        //        SetStatus(Forbidden);
        //        return null;
        //    }
        //}
    }
}
