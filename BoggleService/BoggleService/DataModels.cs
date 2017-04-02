using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// 
/// </summary>
namespace Boggle
{
    [DataContract]
    public class Player
    {
        [DataMember(EmitDefaultValue = false, Order = 1)]
        public string Nickname { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public string UserToken { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 3)]
        public int score { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 4)]
        public List<Play> WordsPlayed;

        public int GameTime { get; set; }

        public void AddPlay(Play play)
        {
            WordsPlayed.Add(play);
        }


    }
    [DataContract]
    public class Game
    {
        public int GameID { get; set; }

        public BoggleBoard boggleBoard { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public GameState State { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public string Board { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 3)]
        public int TimeLimit { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 4)]
        public int TimeLeft { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 5)]
        public Player Player1 { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 6)]
        public Player Player2 { get; set; }
    }

    [DataContract]
    public class Play
    {
        [DataMember(EmitDefaultValue = false, Order = 1)]
        public string startElement = "Word:";

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public string word { get; private set; }

        [DataMember(EmitDefaultValue = false, Order = 3)]
        public string secondElement = "Score:";

        [DataMember(EmitDefaultValue = false, Order = 4)]
        public int score;
        public void Add(string word, int score)
        {
            this.word = word;
            this.score = score;
        }
    }

    public class Score
    {
        public int score { get; set; }
    }

    public class GameState
    {
        public string gameState { get; set; }

    }

}