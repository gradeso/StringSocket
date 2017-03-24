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
        private string nickname;
		private IBoggleClientView ClientView;
		private Game game;
		HttpClient client = null;
		private bool pending = true;
		private int gameTime;
        

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
			ClientView = view;
			view.cancel += View_cancel;
			view.passGameTimeAndStart += View_passGameTimeAndStart;
            view.passNameAndUrl += View_passNameAndUrl;
            view.wordPlayed += play_Word;
			timeOutCounter = 0;
			statusCheckTimer = new Timer(delta);
			statusCheckTimer.AutoReset = true;
            
        }

        private void play_Word(string word)
        {
            PlayWord(word);
        }

        private void View_passNameAndUrl(string nickname, Uri url)
        {
            this.nickname = nickname;
            CreateClient(url);
        }

		private void View_passGameTimeAndStart(int time)
		{
            gameTime = time;
            CreateUser(this.nickname, time);
            JoinGame();
            GameStatus(true);
		}

		private void View_cancel()
		{
			
		}

		
		///******************* These methods implement the Boggle API ***********************///
		private void CreateUser(string nickname, int gameTime)
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
					game.TimeLimit = this.gameTime;
					//Read the contents of the POST into a string.
					string result = response.Content.ReadAsStringAsync().Result;

					//Strip the value of the UserToken out of the response.
					string id = result.Substring(14);
					game.UserToken = id.Substring(0, id.Length - 2);

					//JoinGame();

				}

				else if (response.StatusCode == HttpStatusCode.BadRequest)
				{

				}

				else if (response.StatusCode == HttpStatusCode.Forbidden)
				{

				}

				else throw new Exception("Outside of consideration");
			


		}

		private void JoinGame()
		{
			dynamic content = new ExpandoObject();
			content.UserToken = game.UserToken;
			content.TimeLimit = game.TimeLimit;

			StringContent httpContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

			HttpResponseMessage response = client.PostAsync("games", httpContent).Result;
			if (response.StatusCode == HttpStatusCode.Accepted)
			{
				string data = response.Content.ReadAsStringAsync().Result;
				dynamic arg = JsonConvert.DeserializeObject(data);
				int id = (int)(arg.GameID);
				game.GameID = id;
				//GameStatus(true);
			}

			else if (response.StatusCode == HttpStatusCode.Created)
			{
				string data = response.Content.ReadAsStringAsync().Result;
				dynamic arg = JsonConvert.DeserializeObject(data);
				int id = (int)(arg.GameID);
				game.GameID = id;
				//GameStatus(true);
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
						
				    PendingGame(gameState);
			    }
			    else if (status == "active")
			    {
				    ActiveGame(gameState);
			    }
			    else if (status == "completed")
			    {

			    }
			    else
			    {

				    SendCancel();
				    throw new Exception("unexpected termination");
			    }
			    return;
		    }

		    else throw new Exception();
			
		}

		private void SendCancel()
		{

		}

		private void ActiveGame(dynamic gameState)
		{
            //
		}

		private void PendingGame(dynamic gameState)
		{

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
