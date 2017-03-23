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

        

        ///******************* These methods implement the Boggle API ***********************///
        private void CreateUser(string nickname)
        {
            
            //Create an array object that will be converted to JSON for request body
            dynamic content = new ExpandoObject();
            content.Nickname = nickname;

            //Add the nickname to the game object
            

            //Convert the expando into a JSON array
            StringContent httpContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

           //StringContent httpContent = new StringContent("{\"Nickname\":\"wes\"}");

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
                HttpResponseMessage response = client.GetAsync("games/" + game.UserToken).Result;
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
