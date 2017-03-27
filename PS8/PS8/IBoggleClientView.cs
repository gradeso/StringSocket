using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS8
{
    interface IBoggleClientView
    {
        event Action<string, Uri> registerUserRequest;
        event Action<int> joinServerRequest;
        event Action<string> playAWord;
        event Action CancelJoinRequest;

        string Board { set; get; }
        bool Pending { set; get; }
        string wordPlayed { set; }
        bool GameActive { set; get; }
        int GameTime { set; get; }
        string Player2 { set; }
        int Score { set; get; }
    }
}
