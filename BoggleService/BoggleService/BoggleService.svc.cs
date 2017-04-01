using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using static System.Net.HttpStatusCode;


namespace Boggle
{
    public class BoggleService : IBoggleService
    {

        private static readonly object sync = new object();

        private static Dictionary<int, Game> gamesByGameID = new Dictionary<int, Game>();

        private static Dictionary<string, Player> playersByUserToken = new Dictionary<string, Player>();

        private static Dictionary<string, List<Play>> wordsEachPlayerPlayed = new Dictionary<string, List<Play>>();

        private static Game pendingGame;

        private static int gameID = 1000;

        static BoggleService()
        {
            pendingGame = new Game();
            pendingGame.GameID = gameID++;
            gameID++;
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

        public string CreateUser(string Nickname)
        {
            //If Nickname is null, or is empty when trimmed, responds with status 403 (Forbidden).
            if (Nickname == null || Nickname.Trim() == "")
            {
                SetStatus(Forbidden);
                return "";
            }

            string id = Guid.NewGuid().ToString();
            Player player = new Player();
            player.Nickname = Nickname;
            player.UserToken = id;

            playersByUserToken.Add(id, player);

            SetStatus(Created);

            return id;
        }

        public string JoinGame(string UserToken, int TimeLimit)
        {

            //If UserToken is invalid, TimeLimit < 5, or TimeLimit > 120, responds with status 403(Forbidden).
            if (!playersByUserToken.ContainsKey(UserToken) || TimeLimit < 5 || TimeLimit > 120)
            {
                SetStatus(Forbidden);
                return "";
            }
            //Otherwise, if UserToken is already a player in the pending game, responds with status 409 (Conflict).
            else if (pendingGame.Player1 != null && pendingGame.Player1.UserToken == UserToken)
            {

                SetStatus(Conflict);
                return "";
            }
            //Otherwise, if there is already one player in the pending game, adds UserToken as the second player. 
            else if (pendingGame.Player1 != null)
            {
                Player temp = playersByUserToken[UserToken];
                pendingGame.Player2 = temp;

                //? copy pending game
                Game activeGame = pendingGame;
                activeGame.State.gameState = "active";

                // Add games to active games
                gamesByGameID.Add(activeGame.GameID, activeGame);

                //Calulate game time
                int time1 = activeGame.Player1.GameTime;
                int time2 = TimeLimit;
                activeGame.TimeLimit = ((time1 + time2) / 2);

                //Start timer for active game

                //Get Board to play on
                BoggleBoard board = new BoggleBoard();
                activeGame.boggleBoard = board;
                activeGame.Board = board.ToString();

                //Create new pending game
                pendingGame = new Game();
                pendingGame.GameID = gameID;
                gameID++;

                //Respond appropriately
                SetStatus(Created);
                return activeGame.GameID.ToString();
            }
            //Otherwise, adds UserToken as the first player of the pending game, 201 reponse
            else
            {
                Player player = playersByUserToken[UserToken];

                player.GameTime = TimeLimit;
                pendingGame.Player1 = player;

                SetStatus(Accepted);
                return pendingGame.GameID.ToString();
            }

            throw new Exception();

        }

        public void CancelJoin(string UserToken)
        {

        }

          
        public Game gameStatus(string GameID, string brief)
        {
            int id = Convert.ToInt32(GameID);

            //If GameID is invalid, responds with status 403 (Forbidden).
            if (!gamesByGameID.ContainsKey(id))
            {
                SetStatus(Forbidden);
                return null;
            }

            Game game = gamesByGameID[id];

            if (game.State.gameState == "pending")
            {
                SetStatus(OK);
                Game temp = new Game();
                temp.State = new GameState();
                temp.State.gameState = "pending";
                return temp;
            }
            else if (brief == "yes")
            {

                if (game.State.gameState == "active" || game.State.gameState == "completed")
                {
                    Game temp = new Game();
                    temp.State = new GameState();
                    temp.State.gameState = game.State.gameState;
                    temp.TimeLeft = game.TimeLimit;
                    temp.Player1 = new Player();
                    temp.Player2 = new Player();
                    temp.Player1.score = game.Player1.score;
                    temp.Player2.score = game.Player2.score;
                    return temp;
                }
            }
            else
            {
                if (game.State.gameState == "active")
                {
                    SetStatus(OK);
                    return game;
                }
                if (game.State.gameState == "completed")
                {
                    SetStatus(OK);

                    game.Player1.WordsPlayed = wordsEachPlayerPlayed[game.Player1.UserToken];
                    game.Player2.WordsPlayed = wordsEachPlayerPlayed[game.Player2.UserToken];

                    return game;
                }
            }

            SetStatus(HttpVersionNotSupported);
            return null;
        }

        public Score PlayWord(string GameID, string UserToken, string wordPlayed)
        {
            int id;
            if(int.TryParse(GameID, out id))
            {
                SetStatus(Forbidden);
                return null;
            }

            
            //If Word is null or empty when trimmed, 403
            if(wordPlayed == null || wordPlayed.Trim() == "")
            {
                SetStatus(Forbidden);
                return null;
            }

            //If GameID or UserToken is missing or invalid, 403
            if (!playersByUserToken.ContainsKey(UserToken) || gamesByGameID.ContainsKey(Convert.ToInt32(GameID)))
            {
                SetStatus(Forbidden);
                return null;
            }

            Game game = gamesByGameID[id];
            Player player = playersByUserToken[UserToken];
            Score s = new Score();

            //If UserToken is not a player in the game identified by GameID, 403
            if (game.Player1.UserToken == UserToken || game.Player2.UserToken == UserToken)
            {
                SetStatus(Forbidden);
                return null;
            }

            if (game.State.gameState != "active")
            {
                SetStatus(Conflict);
                return null;
            }

            if(game.boggleBoard.CanBeFormed(wordPlayed))
            {
                if (wordPlayed.Length < 3)
                    s.score = 1;
                else
                    s.score = wordPlayed.Length;
            }
            else
                s.score = -1;
            

            player.score += s.score;
            SetStatus(OK);

            return s;
        }

    }
}