using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PS8
{
    class BoggleClientController
    {
        private IBoggleClientView ClientView;

        private Game game;

        public BoggleClientController(IBoggleClientView view)
        {
            ClientView = view;
            
        }

        private static HttpClient CreateClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://cs3500-boggle-s17.azurewebsites.net");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Content-Type", @"application/json");

            return client;
        }
    }
}
