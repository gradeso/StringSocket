using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS8
{
    class Game
    {
        private string board;
        public string Board { get { return board; } set { board = value; } } 
         
        private string user;
        public string User { get { return user; } set { user = value; } }

        private string userID;
        public string UserID { get { return userID; } set { userID = value; } }

        private int gameID;
        public int GameID { get { return gameID; } }

        private int score;
        public int Score { get { return score; } set { score = value; } }

        /// <summary>
        /// The class that backs a game of Boggle;
        /// </summary>
        /// <param name="GameID"></param>
        public Game(int GameID)
        {
            gameID = GameID;
        }

        public IEnumerator<string> EnumBoard()
        {
            char[] letters = board.ToCharArray();
            foreach (var l in letters)
            {
                yield return l.ToString(); 
            }
        }
    }
}
