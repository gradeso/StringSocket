using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS8
{
    interface IBoggleClientView
    {
		event Action<string, Uri> passNameAndUrl;
		event Action<int> passGameTimeAndStart;
		event Action cancel;
        event Action<string> wordPlayed;
    }
}
