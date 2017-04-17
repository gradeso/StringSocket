using Boggle;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using static System.Net.HttpStatusCode;
using System.Data.SqlTypes;
using System.Data;

namespace Boggle
{
    public class BoggleService
    {

        private static readonly object sync;
        private static readonly HashSet<string> bigDict;
        private static int GameIDCounter;
        private static string BoggleDB;


        static BoggleService()
        {
            // MSSQLLocalDB or ProjectsV13
            BoggleDB = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = |DataDirectory|\BoggleDB.mdf; Integrated Security = True";
            // might need to change path to just "dictionary.txt"
            bigDict = new HashSet<string>(File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "dictionary.txt"));
            sync = new object();
            GameIDCounter = 0;
            clearDB();
        }

        /// <summary>
        /// SERVICE METHOD
        /// if Nickname is null, or is empty when trimmed, responds with status 403 (Forbidden).
        /// Otherwise, creates a new user with a unique UserToken and the trimmed Nickname.The returned UserToken should be used to identify the user in subsequent requests.Responds with status 201 (Created).
        /// </summary>
        /// <param name="Nickname"></param>
        /// <returns></returns>
        public UserIDInfo SaveUserID(Name Nickname, out HttpStatusCode status)
        {
            if(Nickname == null)
            {
                status = Forbidden;
                return null;
            }
            lock (sync)
            {
                string n = Nickname.Nickname;

                if (n == null || n.Trim() == "")
                {
                    status = Forbidden;
                    return null;
                }

                // using guarentees connection will drop after leaving code
                using (SqlConnection conn = new SqlConnection(BoggleDB))
                {
                    conn.Open();
                    // transaction that contains all commands
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        using (SqlCommand command = new SqlCommand("INSERT INTO Users (UserID, Nickname) values (@UserID, @Nickname)", conn, trans))
                        {
                            // todolist generates id within command
                            string userID = Guid.NewGuid().ToString();

                            command.Parameters.AddWithValue("@UserID", userID);
                            command.Parameters.AddWithValue("@Nickname", n);

                            command.ExecuteNonQuery();
                            status = Created;

                            // commit transaction, otherwise abort
                            trans.Commit();
                            return new UserIDInfo(userID);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// SERVICE METHOD
        /// Attempts the join.
        /// method that initiates the joining process uses multiple helper methods to get the job done
        /// </summary>
        /// <param name="ja">The join ateempt info.</param>
        /// <returns></returns>
        public GameIDInfo AttemptJoin(JoinAttempt ja, out HttpStatusCode status)
        {
            if(ja == null || ja.UserToken == null)
            {
                status = HttpStatusCode.Forbidden;
                return null;
            }
            int TimeLimit = ja.TimeLimit;
            string UserToken = ja.UserToken;

            lock (sync)
            {
                // check if invalid game arguments are given
                if (TimeLimit < 5 || TimeLimit > 120 || ja == null || ja.UserToken == null)
                {
                    status = Forbidden;
                    return null;
                }

                // need to check if user is in the Users sql table, otherwise we can't make a game
                PlayerInfo foundPlayer = findPlayer(UserToken);
                if (foundPlayer == null)
                {
                    status = Forbidden;
                    return null;
                }
                var pendingGame = getGameWithID(GameIDCounter);
                // now we know valid arguments are given
                // this is like a thing that only works if there are pending games?
                try
                {

                    if (UserToken == pendingGame.Player1.userID)
                    {
                    }
                }
                // this is thrown by the getGameWithID method
                catch (FormatException)
                {
                    status = Forbidden;
                    return null;
                }
                // this is if there isnt a pending game and I try to get a userID from it
                // so if there isnt a new pending game we make one with the player
                catch (NullReferenceException)
                {
                    pendingGame = new DetailedGameState(GameIDCounter);
                    pendingGame.Player1 = foundPlayer;
                    pendingGame.TimeLimit = Convert.ToInt32(TimeLimit);
                    addGameToDB(pendingGame);
                    status = Accepted;
                    return new GameIDInfo(pendingGame.gameID.ToString());
                }

                DetailedGameState savedGame = pendingGame;
                if (UserToken == savedGame.Player1.userID)
                {
                    status = Conflict;
                    return new GameIDInfo("user already in game");
                }

                savedGame.Player2 = foundPlayer;
                if (savedGame.Player2 == null)
                {
                    status = Forbidden;
                    return null;
                }
                savedGame.TimeLimit = (savedGame.TimeLimit + TimeLimit) / 2;
                savedGame.TimeLeft = savedGame.TimeLimit;
                savedGame.GameState = "active";
                savedGame.boggleBoard = new BoggleBoard();
                savedGame.Board = savedGame.boggleBoard.ToString();

                addGameToDB(savedGame);
                string toReturn = savedGame.gameID.ToString();
                GameIDCounter++;
                status = Created;
                return new GameIDInfo(toReturn);
            }
        }

        /// <summary>
        /// SERVICE METHOD
        /// Cancel a pending game
        /// </summary>
        /// <param name="ut"></param>
        public void CancelJoin(UserIDInfo ut, out HttpStatusCode status)
        {
            if(ut == null)
            {
                status = Forbidden;
                return;
            }
            lock (sync)
            {
                string UserToken = ut.UserToken;
                var player = findPlayer(UserToken);
                var pendingGame = getGameWithID(GameIDCounter);
                try
                {
                    if (pendingGame.Player1.userID == UserToken)
                    {
                        pendingGame = new DetailedGameState(GameIDCounter);
                        addGameToDB(pendingGame);
                        status = OK;
                        return;
                    }
                }
                catch
                {

                }
                status = Forbidden;
            }
        }
        
        /// <summary>
        /// SERVICE METHOD
        /// Plays the word in game.
        /// </summary>
        /// <param name="GameID">The game identifier.</param>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        public ScoreInfo PlayWordInGame(string GameID, Move m, out HttpStatusCode status)
        {
            if(m == null)
            {
                status = Forbidden;
                return null;
            }
            m.Word = m.Word.Trim();

            int tempGameId;
            if (!int.TryParse(GameID, out tempGameId) || m.Word == null || m.Word == "")
            {
                status = Forbidden;
                return null;
            }
            lock (sync)
            {
                DetailedGameState gameInQuestion = getGameWithID(tempGameId);
                if (gameInQuestion.Player1.userID != m.UserToken && gameInQuestion.Player2.userID != m.UserToken)
                {
                    status = Forbidden;
                    return null;
                }
                if (gameInQuestion.GameState != "active")
                {
                    status = Forbidden;
                    return null;
                }
                int toReturn = calculateScore(gameInQuestion.boggleBoard, m.Word);

                if (toReturn == -2)
                {
                    status = Conflict;
                    return new ScoreInfo();
                }
                if (!addMove(tempGameId, m.Word, m.UserToken, toReturn))
                {
                    toReturn = -1;
                }

                status = OK;
                return new ScoreInfo(toReturn);
            }

        }

        /// <summary>
        /// SERVICE METHOD
        /// gets the detailed game state
        /// </summary>
        /// <param name="GameID"></param>
        /// <param name="brief"></param>
        /// <returns></returns>
		public IGameState gameStatus(string GameID, string brief, out HttpStatusCode status)
        {
            lock (sync)
            {

                DetailedGameState gameInQuestion;
                int gameIdNum;
                if (!int.TryParse(GameID, out gameIdNum))
                {
                    status = Forbidden;
                    return null;
                }
                gameInQuestion = getGameWithID(gameIdNum);
                switch (gameInQuestion.GameState)
                {
                    case "pending":
                        status = OK;
                        return (GameStatePending)gameInQuestion;
                    case "active":

                        DetailedPlayerInfo tempPlr1 = (DetailedPlayerInfo)(gameInQuestion.Player1);
                        DetailedPlayerInfo tempPlr2 = (DetailedPlayerInfo)(gameInQuestion.Player2);
                        GameStateActive toReturn;
                        if (brief == "yes")
                        {
                            toReturn = gameInQuestion;
                            tempPlr1.Nickname = null;
                            tempPlr2.Nickname = null;
                            tempPlr1.Score = getPlayerScore(gameInQuestion, tempPlr1.userID);
                            tempPlr2.Score = getPlayerScore(gameInQuestion, tempPlr2.userID);

                            ((GameStateActive)toReturn).Player1 = (PlayerInfo)tempPlr1;
                            ((GameStateActive)toReturn).Player2 = (PlayerInfo)tempPlr2;


                        }
                        else
                        {
                            toReturn = gameInQuestion;
                            ((DetailedGameState)toReturn).Player1 = tempPlr1;
                            ((DetailedGameState)toReturn).Player2 = tempPlr2;

                        }
                        status = OK;
                        return toReturn;

                    case "completed":
                        DetailedGameState toReturn2;
                        DetailedPlayerInfo tempPlr10 = (DetailedPlayerInfo)(gameInQuestion.Player1);
                        DetailedPlayerInfo tempPlr20 = (DetailedPlayerInfo)(gameInQuestion.Player2);


                        tempPlr10.Score = getPlayerScore(gameInQuestion, tempPlr10.userID);
                        tempPlr20.Score = getPlayerScore(gameInQuestion, tempPlr20.userID);

                        if (brief == "yes")

                        {
                            tempPlr10.Nickname = null;
                            tempPlr20.Nickname = null;

                            GameStateActive toReturn20 = (GameStateActive)gameInQuestion;
                            ((GameStateActive)toReturn20).Player1 = (PlayerInfo)tempPlr10;
                            ((GameStateActive)toReturn20).Player2 = (PlayerInfo)tempPlr20;
                            status = OK;
                            return toReturn20;
                        }
                        else
                        {
                            tempPlr10.WordsPlayed.AddRange(getPlayerMoves(tempPlr10.userID));
                            tempPlr20.WordsPlayed.AddRange(getPlayerMoves(tempPlr20.userID));

                            toReturn2 = gameInQuestion;
                            toReturn2.Player1 = tempPlr10;
                            toReturn2.Player2 = tempPlr20;

                        }
                        status = OK;
                        return toReturn2;
                    default:
                        status = Forbidden;
                        return null;
                }
            }
        }

        /// <summary>
        ///adds the move to the data base returns true if it succeeded.
        ///helper method that puts the played move into the database
        /// </summary>
        /// <param name="gameID">The game identifier.</param>
        /// <param name="word">The word.</param>
        /// <param name="userToken">The user token.</param>
        /// <returns></returns>
        private bool addMove(int gameID, string word, string userToken, int score)
        {

            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {

                conn.Open();

                var sql = "INSERT INTO Words VALUES (@Player_ID, @Word, @Game_ID, @Score)";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Player_ID", userToken);
                cmd.Parameters.AddWithValue("@Word", word);
                cmd.Parameters.AddWithValue("@Game_ID", gameID);
                cmd.Parameters.AddWithValue("@Score", score);
                try
                {
                    return cmd.ExecuteNonQuery() >= 0;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// helper method for gameStatus that get's scores
        /// </summary>
        /// <param name="game"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
		private int getPlayerScore(DetailedGameState game, string userToken)
        {
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {

                conn.Open();
                var sql = "SELECT * FROM Words WHERE GameID=@Game_ID AND PlayerID=@Player_ID";

                //get the words where id and game match
                //divi the scores between players
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Game_ID", game.gameID);
                cmd.Parameters.AddWithValue("@Player_ID", userToken);

                int toReturn = 0;
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    toReturn += (int)rdr["Score"];
                }
                return toReturn;
            }
        }

        private IEnumerable<WordAndScore> getPlayerMoves(string userId)
        {
            List<WordAndScore> toReturn = new List<WordAndScore>();
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {

                conn.Open();
                var sql = "SELECT * FROM Words WHERE PlayerID=@Player_ID";

                //get the words where id and game match
                //divi the scores between players
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Player_ID", userId);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    toReturn.Add(new WordAndScore(userId, (string)rdr["Word"], (int)rdr["Score"]));
                }
                return toReturn;
            }

        }

        /// <summary>
        /// helper method for getting game with a certain id
        /// </summary>
        private DetailedGameState getGameWithID(int gameID)
        {
            // object array that will hold all the parts
            // of a game from the sql table
            // if there is an error while 
            object[] toChange = new object[6];

            toChange[0] = gameID;
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                var sql = string.Format("SELECT * FROM Games where GameId='{0}'", gameID.ToString());

                SqlCommand cmd = new SqlCommand(sql, conn);

                try
                {
                    var rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {

                        toChange[1] = rdr["Player1"];
                        toChange[2] = rdr["Player2"];
                        toChange[3] = rdr["Board"];
                        toChange[4] = rdr["TimeLimit"];
                        toChange[5] = rdr["StartTime"];

                    }
                    return detailGame(toChange);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// helper method that finally adds the pending game to the database
        /// </summary>
        /// <param name="pendingGame"></param>
		private void addGameToDB(DetailedGameState pendingGame)
        {
            if (pendingGame.Player1 == null)
            {
                using (SqlConnection conn = new SqlConnection(BoggleDB))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("DElETE FROM Games WHERE GameID=@Game_ID", conn);
                    cmd.Parameters.AddWithValue("@Game_ID", pendingGame.gameID);
                    cmd.ExecuteNonQuery();
                }
                return;
            }
            SqlParameter[] contents = simplifyGameState(pendingGame);
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("IF NOT EXISTS(SELECT 1 FROM Games WHERE GameID=@GameID)" +
               " Insert INTO Games (GameId,Player1,Player2,Board,TimeLimit,StartTime) VALUES(@GameID,@Player1,@Player2,@Board,@TimeLimit,@StartTime)" +
               " else" +
               " UPDATE Games SET Player1=@Player1,Player2=@Player2,Board=@Board,TimeLimit=@TimeLimit,StartTime=@StartTime WHERE GameID=@GameID"
               , conn);

                cmd.Parameters.AddRange(contents);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// usefull helper method that returns player info of a given player id if it exists
        /// might need to get rid of status out don't know yet. helper should probably
        /// not have status setting responsibility
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
		private PlayerInfo findPlayer(object v)
        {
            if (v is DBNull || v == null) return null;

            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Users WHERE UserID = @ID", conn))
                {
                    cmd.Parameters.AddWithValue("@ID", (string)v);

                    SqlDataReader rdr = cmd.ExecuteReader();

                    // checks if the user is there
                    if (!rdr.HasRows)
                    {
                        // status = Forbidden;
                        return null;
                    }

                    while (rdr.Read())
                    {
                        return new DetailedPlayerInfo((string)rdr["UserID"], (string)rdr["Nickname"]);
                    }

                    return null;
                }
            }
        }


        /// <summary>
        /// helper method that breaks apart a 
        /// </summary>
        /// <param name="pendingGame"></param>
        /// <returns></returns>
		private SqlParameter[] simplifyGameState(DetailedGameState pendingGame)
        {
            var spcArray = new SqlParameter[6];
            spcArray[0] = new SqlParameter("@GameID", pendingGame.gameID);
            spcArray[1] = new SqlParameter("@Player1", pendingGame.Player1.userID);

            // might change later, needed to prevent null reference
            // player 2 can be null which is why we need this if statement
            if (pendingGame.Player2 != null) { spcArray[2] = new SqlParameter("@Player2", pendingGame.Player2.userID); }
            else { spcArray[2] = new SqlParameter("@Player2", SqlChars.Null); }

            // same with the board
            if (pendingGame.Board != null) { spcArray[3] = new SqlParameter("@Board", pendingGame.Board); }
            else { spcArray[3] = new SqlParameter("@Board", SqlChars.Null); }

            spcArray[4] = new SqlParameter("@TimeLimit", pendingGame.TimeLimit);
            spcArray[5] = new SqlParameter("@StartTime", DateTime.Now);

            return spcArray;

        }

        /// <summary>
        /// helper method that takes an array of objects and turns it into a game state that can be used
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
		private DetailedGameState detailGame(object[] input)
        {
            var toReturn = new DetailedGameState();

            toReturn.gameID = (int)input[0];
            toReturn.Player1 = findPlayer(input[1]);
            toReturn.Player2 = findPlayer(input[2]);
            toReturn.Board = input[3] == null || input[3] is DBNull ? null : (string)input[3];
            toReturn.TimeLimit = input[4] == null || input[4] is DBNull ? 0 : (int)input[4];
            toReturn.boggleBoard = toReturn.Board == null ? null : new BoggleBoard((string)input[3]);
            if (input[5] != null && !(input[5] is DBNull))
            {

                // pulled this out so it's easier to read
                int elapsedTime = (int)(DateTime.Now - (DateTime)input[5]).TotalSeconds;
                // gets time left
                toReturn.TimeLeft = toReturn.TimeLimit - elapsedTime > 0 ? toReturn.TimeLimit - elapsedTime : 0;
            }
            toReturn.GameState = (toReturn.Player2 == null ? "pending" : toReturn.TimeLeft == 0 ? "completed" : "active");

            return toReturn;
        }

        /// <summary>
        /// helper method for calculating the score
        /// </summary>
        /// <param name="boggleBoard"></param>
        /// <param name="word"></param>
        /// <returns></returns>
		private int calculateScore(BoggleBoard boggleBoard, string word)
        {
            if (word == null || boggleBoard == null) { return -2; }
            //here you need to add the word to the database,
            //if the word is already there set the score to zero.

            return (boggleBoard.CanBeFormed(word) && bigDict.Contains(word.ToUpper())) ?
                word.Length < 3 ?
                0 : word.Length < 5 ?
                1 : word.Length < 6 ?
                2 : word.Length < 7 ?
                3 : word.Length < 8 ?
                5 : 11
                  : -1;
        }

        /// <summary>
        /// helper method that deletes all the rows in each table of the database. Useful for consistent testing
        /// and runs in the service constructor
        /// </summary>
		private static void clearDB()
        {
            bool anyFailed = false;
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                try
                {
                    var cmd = new SqlCommand();

                    cmd.CommandText = "DELETE FROM Words; DELETE FROM Games; DELETE FROM Users";
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.ExecuteNonQuery();  // all rows deleted
                }
                catch
                {
                    try
                    {
                        anyFailed = true;
                        var cmd = new SqlCommand();

                        cmd.CommandText = "DELETE FROM Users";
                        cmd.Connection = conn;
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch
                    {
                        try
                        {
                            anyFailed = true;
                            var cmd = new SqlCommand();

                            cmd.CommandText = "DELETE FROM Games";
                            cmd.Connection = conn;
                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            anyFailed = true;
                        }
                    }
                }
            }
            if (anyFailed) clearDB();
        }
    }
}

