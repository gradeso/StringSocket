using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Timers;


/// <summary>
/// Contains definitions of objects that will be passed to and from the server
/// </summary>

namespace Boggle
{
    /// <summary>
    /// username class just holds the nickname
    /// </summary>
    public class UserName
    {
        public string Nickname { get; set; }
    }

    /// <summary>
    /// user id class holds a userID
    /// </summary>
    public class UserID
    {
        public UserID(string leID)
        {
            UserToken = leID;
        }
        public string UserToken { get; set; }
    }

    /// <summary>
    /// holds info for a join game request
    /// </summary>
    public class GameCreateRequestInfo
    {
        public string UserToken { get; set; }
        public int TimeLimit { get; set; }
    }

    /// <summary>
    /// holds the server response to a join game request
    /// </summary>
    public class GameCreateResponseInfo
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="id"></param>
        public GameCreateResponseInfo(string id)
        {
            GameID = id;
        }
        public string GameID { get; set; }
    }

    /// <summary>
    /// holds the info for a play word request from a client
    /// </summary>
    public class PlayWordInfo
    {
        public string UserToken { get; set; }
        public string Word { get; set; }
    }

    /// <summary>
    /// holds the server response to a play word request
    /// </summary>
    public class PlayWordResponseInfo
    {
        public int Score { get; set; }
    }

    /// <summary>
    /// holds the server reponse to a game status request from a client if the game is pending
    /// </summary>
    public class GameStatusResponseIfPendingInfo
    {
        public string GameState { get; set; }
    }

    /// <summary>
    /// holds the server reponse to a game status request from a client if the breif flag is set
    /// </summary>
    public class GameStatusResponseIfActiveOrCompletedAndBriefInfo
    {
        public string GameState { get; set; }
        public int TimeLeft { get; set; }
        public Player1IfBrief Player1;
        public Player2IfBrief Player2;

    }

    /// <summary>
    ///  holds the server reponse to a game status request from a client if its active
    /// </summary>
    public class GameStatusResponseIfActiveInfo
    {
        public string GameState { get; set; }
        public string Board { get; set; }
        public int TimeLimit { get; set; }
        public int TimeLeft { get; set; }
        public Player1 Player1;
        public Player2 Player2;
    }

    /// <summary>
    ///  holds the server reponse to a game status request from a client if its completed
    /// </summary>
    public class GameStatusResponseIfCompleted
    {
        public string GameState { get; set; }
        public string Board { get; set; }
        public int TimeLimit { get; set; }
        public int TimeLeft { get; set; }
        public Player1Completed Player1;
        public Player2Completed Player2;

    }
    
    /// <summary>
    /// holds a breif string parameter
    /// </summary>
    public class BriefThing
    {
        public string Brief { get; set; }
    }  

    /// <summary>
    /// player info if the brief flag is set
    /// </summary>
    public class Player1IfBrief
    {
        public int Score { get; set; }
    }   
    public class Player2IfBrief
    {
        public int Score { get; set; }
    }

    /// <summary>
    /// player info if the game is active
    /// </summary>
    public class Player1
    {
        public string Nickname { get; set; }
        public int Score { get; set; }
    }
    public class Player2
    {
        public string Nickname { get; set; }
        public int Score { get; set; }
    }

    /// <summary>
    /// player info if the game is completed
    /// </summary>
    public class Player1Completed
    {
        public string Nickname { get; set; }
        public int Score { get; set; }
        public List<WordPlayed> WordsPlayed;
    }
    public class Player2Completed
    {
        public string Nickname { get; set; }
        public int Score { get; set; }
        public List<WordPlayed> WordsPlayed;
    }

    /// <summary>
    /// an object that holds words play info
    /// </summary>
    public class WordPlayed
    {
        public string Word { get; set; }
        public int Score { get; set; }
    }

    /// <summary>
    /// the class that represents a game in any state
    /// </summary>
    public class Game
    {
        /// <summary>
        /// each game has one boggle board
        /// </summary>
        public BoggleBoard board;

        /// <summary>
        /// each game has a timer
        /// </summary>
        public Timer gameTimer;

        /// <summary>
        /// string that keeps track of the game id
        /// </summary>
        public string GameID { get; set; }

        /// <summary>
        /// an int that keeps track of the total time of the game
        /// </summary>
        public int TotalTime { get; set; }

        /// <summary>
        /// keeps track of how much time is left
        /// </summary>
        public int TimeLeft { get; set; }

        /// <summary>
        /// each game has a status
        /// </summary>
        public string GameStatus { get; private set; }

        /// <summary>
        /// both players hve nicknames
        /// </summary>
        public string Player1Nickname { get; set; }
        public string Player2Nickname { get; set; }

        /// <summary>
        /// both players have an ID
        /// </summary>
        public string Player1ID { get; set; }
        public string Player2ID { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public Game(string name, string playerid, string gameid, int time)
        {
            //initiate game board
            board = new BoggleBoard();
            GameID = gameid;

            //first person gets to choose the time but just prime the timer without starting it
            TotalTime = time;
            gameTimer = new Timer();
            gameTimer.Interval = 1000;
            gameTimer.Enabled = true;
            gameTimer.Elapsed += TimerTick;

            //set up player1
            Player1Nickname = name;
            Player1ID = playerid;

            //set game to pending
            GameStatus = "Pending";
        }

        /// <summary>
        /// method that starts the game
        /// </summary>
        public void StartGame()
        {
            gameTimer.Start();
        }

        /// <summary>
        /// fired when the time runs out
        /// </summary>
        private void TimerTick(object source, EventArgs e)
        {
            if (TimeLeft > 0)
            {
                TimeLeft--;
            }
            else
            {
                gameTimer.Stop();
                GameStatus = "Completed";
            }
        }
    }
}