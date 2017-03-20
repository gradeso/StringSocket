using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Dynamic;
using Newtonsoft.Json.Linq;

namespace PS8
{
    class BoggleClientController
    {
        private IBoggleClientView ClientView;
        private Game game;
        HttpClient client = null;

        public BoggleClientController(IBoggleClientView view)
        {
            ClientView = view;
            view.registerButtonClicked += handleRegisterClick;
        }

        private void handleRegisterClick(string name, string url)
        {
            CreateClient(url);
            CreateUser(name);
        }

        ///******************* These methods implement the Boggle API ***********************///
        private void CreateUser(string nickname)
        {
            //Create an array object that will be converted to JSON for request body
            dynamic content = new ExpandoObject();
            content.Nickname = nickname;

            //Convert the expando into a JSON array
            StringContent httpContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

           //StringContent httpContent = new StringContent("{\"Nickname\":\"wes\"}");

            using (this.client)
            {
                HttpResponseMessage response = client.PostAsync("users", httpContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;

                    dynamic serverResponse = JsonConvert.DeserializeObject(result);


                }

                else throw new Exception();
            }

            ClientView.Pl
        }

        private void JoinGame(string userToken, int timeLimit)
        {

        }

        private void CancelJoinRequest(string userToken)
        {

        }

        private void PlayWord(string userToken, string wordPlayed)
        {

        }

        private void GameStatus(bool brief)
        {
            using (this.client)
            {
                HttpResponseMessage response = client.GetAsync("games/" + game.GameID.ToString()).Result;
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;

                    dynamic gameState = JsonConvert.DeserializeObject(result);

                    return;
                }

                else throw new Exception();
            }
        }

        /// <summary>
        /// A helper method that creates an HTTP Client with base address of "http://cs3500-boggle-s17.azurewebsites.net".
        /// <exception cref=InvalidHTTP_FormatException>description</exception>  
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private void CreateClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://cs3500-boggle-s17.azurewebsites.net");

            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Add("Content-Type", @"application/json");

            this.client = client;

            return;
        }

        /// <summary>
        /// A helper method that creates an HTTP Client with a string argument that represents the base
        /// address of the HTTP Client instance that is being created. Addtionally this client
        /// requires content in the form of JSON arrays
        /// <exception cref=InvalidHTTP_FormatException>description</exception>  
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private void CreateClient(string url)
        {
            HttpClient client = new HttpClient();

            Uri test;

            //Test to see if the string 'URL' provided is a proper URL with correct HTTP syntax
            if (Uri.TryCreate(url, UriKind.Absolute, out test) && test.Scheme == Uri.UriSchemeHttp)
                client.BaseAddress = test;
            else
                throw new InvalidHTTP_FormatException();

            client.DefaultRequestHeaders.Accept.Clear();
          //  client.DefaultRequestHeaders.Add("Content-Type", @"application/json");

            this.client = client;
        }


    }
    /// <summary>
    /// An exception thrown by CreateClient that 
    /// </summary>
    public class InvalidHTTP_FormatException : Exception { }
}
