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
         
        private string nickname;
        public string Nickname { get { return nickname; } set { nickname = value; } }

        private string userToken;
        public string UserToken { get { return userToken; } set { userToken = value; } }

        private int gameID;
        public int GameID { get { return gameID; } set { gameID = value; } }

        private int score;
        public int Score { get { return score; } set { score = value; } }

        private List<string> wordsPlayed;
        public List<string> WordsPlayed { get { return copyOfList(wordsPlayed); } }
        public void addWord(string word) { wordsPlayed.Add(word); }

        /// <summary>
        /// The class that backs a game of Boggle;
        /// </summary>
        /// <param name="GameID"></param>
        public Game()
        {
        }

        public IEnumerator<string> EnumBoard()
        {
            char[] letters = board.ToCharArray();
            foreach (var l in letters)
            {
                yield return l.ToString(); 
            }
        }

        public List<string> copyOfList(List<string> original)
        {
            List<string> newList = new List<string>();
            foreach (string s in original)
                newList.Add(s);

            return newList;
        }
    }
}
