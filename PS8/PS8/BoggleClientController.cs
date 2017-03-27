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
using System.Timers;

namespace PS8
{
	
	class BoggleClientController
	{
        protected string nickname;
		private IBoggleClientView ClientView;
		private Game game;
		HttpClient client = null;
		private bool pending = true;
        private bool active = false;
        private bool complete;
		private int gameTime = -1;
        private Task pendingTask;
        private Task activeTask;
        private bool initialize = true;
        /// <summary>
        /// The timer that goes 
        /// </summary>
        private Timer statusCheckTimer;

		/// <summary>
		/// The time out counter
		/// </summary>
		private long timeOutCounter;

		/// <summary>
		/// The time out limit
		/// </summary>
		public const long timeOutLimit = 8000;

		/// <summary>
		/// delta symbolizes the time between input checks on the sever.
		/// </summary>
		public const long delta = 50;

		public BoggleClientController(IBoggleClientView view)
		{
            System.Threading.Thread.CurrentThread.Name = "main";

            ClientView = view;

            view.joinServerRequest += handleJoinRequest;
            view.registerUserRequest += handleRegisterRequest;
            view.playAWord += PlayWord;
            
			timeOutCounter = 0;
			statusCheckTimer = new Timer(delta);
			statusCheckTimer.AutoReset = true;
        }

        private void handleRegisterRequest(string playerName, Uri serverUrl)
        {
            CreateClient(serverUrl);
            CreateUser(playerName);
        }

        private void handleJoinRequest(int gameTime)
        {
            JoinGame(gameTime);

            pendingTask = Task.Run(() =>  handlePendingState());
            
            activeTask = Task.Run(() => ActiveGameState());
        }

        private void handlePendingState()
        {
            
            while (pending)
                GameStatus(true);

            return;
        }

		private void ActiveGameState()
		{
            pendingTask.Wait();

            PendingChange(pending);

            while (active)
                GameStatus(false);

            ActiveChange(active);
            return;
		}

        private void PendingChange(bool state)
        {
            ClientView.Pending = state;
        }

        private void ActiveChange(bool state)
        {
            ClientView.GameActive = state;
        }

        ///******************* These methods implement the Boggle API ***********************///
        private void CreateUser(string nickname)
		{
			//Create an array object that will be converted to JSON for request body
			dynamic content = new ExpandoObject();
			content.Nickname = nickname;

			//Convert the expando into a JSON array
			StringContent httpContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

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
					game.UserToken = id.Substring(0, id.Length - 2);

				}

				else if (response.StatusCode == HttpStatusCode.BadRequest)
				{

				}

				else if (response.StatusCode == HttpStatusCode.Forbidden)
				{

				}

				else throw new Exception("Outside of consideration");
			


		}

		private void JoinGame(int gameTime)
		{
			dynamic content = new ExpandoObject();
			content.UserToken = game.UserToken;
            content.TimeLimit = gameTime;

			StringContent httpContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

			HttpResponseMessage response = client.PostAsync("games", httpContent).Result;
			if (response.StatusCode == HttpStatusCode.Accepted)
			{
				string data = response.Content.ReadAsStringAsync().Result;
				dynamic arg = JsonConvert.DeserializeObject(data);

                int id = (int)(arg.GameID);
				game.GameID = id;

				GameStatus(true);
			}

			else if (response.StatusCode == HttpStatusCode.Created)
			{
				string data = response.Content.ReadAsStringAsync().Result;
				dynamic arg = JsonConvert.DeserializeObject(data);
				int id = (int)(arg.GameID);
				game.GameID = id;

                GameStatus(true);
			}

			else if (response.StatusCode == HttpStatusCode.Forbidden)
			{
               
			}

			else if (response.StatusCode == HttpStatusCode.Conflict)
			{
              
			}
			


		}

		private void CancelJoinRequest()
		{
            dynamic content = new ExpandoObject();
            content.UserToken = game.UserToken;
            

            StringContent httpContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PutAsync("games", httpContent).Result;
            if (response.StatusCode == HttpStatusCode.OK)
            {

            }

            else throw new Exception();

        }

        private void PlayWord(string wordPlayed)
        {
            dynamic content = new ExpandoObject();
            content.UserToken = game.UserToken;
            content.Word = wordPlayed;

            StringContent httpContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PutAsync("games/" + game.GameID, httpContent).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string output = response.Content.ReadAsStringAsync().Result;
                dynamic data = JsonConvert.DeserializeObject(output);

                if (ClientView.Score != data.Score)
                    ClientView.Score = data.Score;

                ClientView.wordPlayed = wordPlayed;
                
                
                //Trigger score change event, fire int
            }
            else if (response.StatusCode == HttpStatusCode.Conflict)
            {

            }



        }

		private void GameStatus(bool brief)
		{
            HttpResponseMessage response = client.GetAsync("games/" + game.GameID).Result;
            if (response.IsSuccessStatusCode)
		    {
			    string result = response.Content.ReadAsStringAsync().Result;
			    dynamic gameState = JsonConvert.DeserializeObject(result);
			    string status = gameState.GameState;
			    if (status == "pending")
			    {	
				    
			    }
			    else if (status == "active")
			    {
                    if(initialize)
                    {
                        ClientView.GameTime = gameState.TimeLimit;

                        string intermediate = gameState.Board;
                        ClientView.Board = intermediate.ToLower();

                        if(gameState.Player1.Nickname == nickname)
                            ClientView.Player2 = gameState.Player2.Nickname;
                        else
                            ClientView.Player2 = gameState.Player1.Nickname;

                        pending = false;
                        active = true;
                        initialize = false;
                    }


			    }
			    else if (status == "completed")
			    {
                    active = false;
                    //Run game over
			    }

			    return;
		    }

		    else throw new Exception();
			
		}


		private void ActiveGame(dynamic gameState)
		{
            string board = gameState.Board;
            ClientView.Board = board; 
		}



		/// <summary>
		/// 444 Client with a Uri argument that represents the base
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
