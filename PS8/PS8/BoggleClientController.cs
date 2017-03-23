using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using System.Net;

namespace PS8
{
    class BoggleClientController
    {
        private IBoggleClientView ClientView;
        private Game game;
        HttpClient client = null;
        private bool pending = true;
        

        public BoggleClientController(IBoggleClientView view)
        {
            ClientView = view;
            view.registerButtonClicked += handleRegisterClick;
        }

        private void handleRegisterClick(string name, Uri url)
        {
            CreateClient(url);
            CreateUser(name);
        }

        private void Game()
        {
            JoinGame();
            GameStatus(true);
            //while (pending)
            //{

            //    Task checkStatus = new Task(() => GameStatus(true));
            //}
        }

        ///******************* These methods implement the Boggle API ***********************///
        private void CreateUser(string nickname)
        {
            //Create an array object that will be converted to JSON for request body
            dynamic content = new ExpandoObject();
            content.Nickname = nickname;

            //Convert the expando into a JSON array
            StringContent httpContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");


            using (client)
            {
                HttpResponseMessage response = client.PostAsync("users", httpContent).Result;
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    //Because we successfully connected to the server, the URL is correct, so 
                    //create the game object that will act as the model.
                    game = new Game();
                    game.Nickname = nickname;
                    //Read the contents of the POST into a string.
                    string result = response.Content.ReadAsStringAsync().Result;

                    //Strip the value of the UserToken out of the response.
                    string id = result.Substring(14);
                    game.UserToken = id.Substring(0,id.Length - 2);

                    Game();
                }

                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {

                }

                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {

                }

                else throw new Exception();
            }

            
        }

        private void JoinGame()
        {
            dynamic content = new ExpandoObject();
            content.UserToken = game.UserToken;
            content.TimeLimit = 120;

            StringContent httpContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            using (client)
            {
                HttpResponseMessage response = client.PostAsync("games", httpContent).Result;
                if (response.StatusCode == HttpStatusCode.Accepted)
                {

                }

                else if (response.StatusCode == HttpStatusCode.Created)
                {

                }

                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {

                }

                else if (response.StatusCode == HttpStatusCode.Conflict)
                {

                }
            }
            

        }

        private void CancelJoinRequest()
        {

        }

        private void PlayWord(string wordPlayed)
        {

        }

        private void GameStatus(bool brief)
        {
            using (client)
            {
                HttpResponseMessage response = client.GetAsync("games/132").Result;
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;

                    dynamic gameState = JsonConvert.DeserializeObject(result);

                    StringBuilder gameStatus = gameState.GameState.ToString();
                    gameStatus.Replace("{", "");
                    gameStatus.Replace("}", "");
                    bool temp;
                    bool resutl = bool.TryParse(gameStatus.ToString(), out temp);

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

            this.client = client;

            return;
        }

        /// <summary>
        /// A helper method that creates an HTTP Client with a Uri argument that represents the base
        /// address of the HTTP Client instance that is being created. Addtionally this client
        /// requires content in the form of JSON arrays
        /// <exception cref=InvalidHTTP_FormatException>description</exception>  
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private void CreateClient(Uri url)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = url;
            client.DefaultRequestHeaders.Accept.Clear();

            this.client = client;
        }


    }
    /// <summary>
    /// An exception thrown by CreateClient that 
    /// </summary>
    public class InvalidHTTP_FormatException : Exception { }
}
