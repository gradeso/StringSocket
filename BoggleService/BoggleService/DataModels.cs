using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Timers;
using NetSpell;
using System.Runtime.Serialization;

/// <summary>
/// Contains definitions of objects that will be passed to and from the server
/// </summary>

namespace Boggle
{
    /// <summary>
    /// username class just holds the nickname
    /// </summary>
    [DataContract]
    public class UserName
    {
        [DataMember]
        public string Nickname { get; set; }
    }

    /// <summary>
    /// user id class holds a userID
    /// </summary>
    [DataContract]
    public class UserID
    {
        public UserID(string leID)
        {
            UserToken = leID;
        }

        [DataMember]
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
    [DataContract]
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
        [DataMember]
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
    [DataContract]
    public class PlayWordResponseInfo
    {
        [DataMember]
        public int Score { get; set; }
    }

    /// <summary>
    /// holds a game status response data
    /// </summary>
    [DataContract]
    public class GameStatusResponse
    {
        [DataMember]
        public string GameState { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Board { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int TimeLimit { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int TimeLeft { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Player1 Player1;

        [DataMember(EmitDefaultValue = false)]
        public Player2 Player2;
    }
    ///// <summary>
    ///// holds the server reponse to a game status request from a client if the game is pending
    ///// </summary>
    //public class GameStatusResponseIfPendingInfo
    //{
    //    public string GameState { get; set; }
    //}

    ///// <summary>
    ///// holds the server reponse to a game status request from a client if the breif flag is set
    ///// </summary>
    //public class GameStatusResponseIfActiveOrCompletedAndBriefInfo
    //{
    //    public string GameState { get; set; }
    //    public int TimeLeft { get; set; }
    //    public Player1IfBrief Player1;
    //    public Player2IfBrief Player2;

    //}

    ///// <summary>
    /////  holds the server reponse to a game status request from a client if its active
    ///// </summary>
    //public class GameStatusResponseIfActiveInfo
    //{
    //    public string GameState { get; set; }
    //    public string Board { get; set; }
    //    public int TimeLimit { get; set; }
    //    public int TimeLeft { get; set; }
    //    public Player1 Player1;
    //    public Player2 Player2;
    //}

    ///// <summary>
    /////  holds the server reponse to a game status request from a client if its completed
    ///// </summary>
    //public class GameStatusResponseIfCompleted
    //{
    //    public string GameState { get; set; }
    //    public string Board { get; set; }
    //    public int TimeLimit { get; set; }
    //    public int TimeLeft { get; set; }
    //    public Player1Completed Player1;
    //    public Player2Completed Player2;

    //}

    [DataContract]
    public class Player1
    {
        [DataMember]
        public int Score { get; set;}

        [DataMember(EmitDefaultValue = false)]
        public string Nickname { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<WordPlayed> WordsPlayed;
    }

    [DataContract]
    public class Player2
    {
        [DataMember]
        public int Score { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Nickname { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<WordPlayed> WordsPlayed;
    }

    ///// <summary>
    ///// player info if the brief flag is set
    ///// </summary>
    //public class Player1IfBrief
    //{
    //    public int Score { get; set; }
    //}   
    //public class Player2IfBrief
    //{
    //    public int Score { get; set; }
    //}

    ///// <summary>
    ///// player info if the game is active
    ///// </summary>
    //public class Player1f
    //{
    //    public string Nickname { get; set; }
    //    public int Score { get; set; }
    //}
    //public class Player2f
    //{
    //    public string Nickname { get; set; }
    //    public int Score { get; set; }
    //}

    ///// <summary>
    ///// player info if the game is completed
    ///// </summary>
    //public class Player1Completed
    //{
    //    public string Nickname { get; set; }
    //    public int Score { get; set; }
    //    public List<WordPlayed> WordsPlayed;
    //}
    //public class Player2Completed
    //{
    //    public string Nickname { get; set; }
    //    public int Score { get; set; }
    //    public List<WordPlayed> WordsPlayed;
    //}

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
        /// each player has a score
        /// </summary>
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }

        /// <summary>
        /// both players have a list of played words
        /// </summary>
        public List<WordPlayed> player1WordsPlayed;
        public List<WordPlayed> player2WordsPlayed;

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
            Player1Score = 0;
            player1WordsPlayed = new List<WordPlayed>();

            //setup what we can about player 2
            Player2Score = 0;
            player2WordsPlayed = new List<WordPlayed>();

            //set game to pending
            GameStatus = "Pending";
        }

        /// <summary>
        /// method that starts the game
        /// </summary>
        public void StartGame()
        {
            GameStatus = "Active";
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

        /// <summary>
        /// method that tells the caller if the id passed in is a player
        /// in this game
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool isPlayer (string userID)
        {
            if (userID == Player1ID || userID == Player2ID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int PlayWord(string userID, string word)
        {
            //test tried words
            if (userID == Player1ID)
            {
                foreach (WordPlayed w in player1WordsPlayed)
                {
                    if (w.Word == word)
                    {
                        return 0;
                    }
                }
            }
            else
            {
                foreach (WordPlayed w in player2WordsPlayed)
                {
                    if (w.Word == word)
                    {
                        return 0;
                    }
                }
            }

            WordPlayed result = new WordPlayed();
            result.Word = word;

            //if we get here we need to see if the word is playable
            if (board.CanBeFormed(word))
            {
                //determine if the word is in the english dictionary
                //technique found at http://stackoverflow.com/questions/38416265/c-sharp-checking-if-a-word-is-in-an-english-dictionary
                NetSpell.SpellChecker.Dictionary.WordDictionary oDict = new NetSpell.SpellChecker.Dictionary.WordDictionary();

                oDict.DictionaryFile = "en-US.dic";
                oDict.Initialize();
                NetSpell.SpellChecker.Spelling oSpell = new NetSpell.SpellChecker.Spelling();

                oSpell.Dictionary = oDict;
                if (oSpell.TestWord(word))
                {
                    //determine the length and therby the score of the word if we get here
                    if (word.Length < 3)
                    {
                        result.Score = -1;
                    }
                    else if (word.Length >= 3 && word.Length <= 4)
                    {
                        result.Score = 1;
                    }
                    else if (word.Length == 5)
                    {
                        result.Score = 2;
                    }
                    else if (word.Length == 6)
                    {
                        result.Score = 3;
                    }
                    else if (word.Length == 7)
                    {
                        result.Score = 5;
                    }
                    else
                    {
                        result.Score = 11;
                    }
                }
                else
                {
                    result.Score = -1;
                }
            }
            else
            {
                result.Score = -1;
            }

            //log the tried word and score
            if (userID == Player1ID)
            {
                player1WordsPlayed.Add(result);
                Player1Score += result.Score;
            }
            else
            {
                player2WordsPlayed.Add(result);
                Player2Score += result.Score;
            }
            return result.Score;
        }
    }
}