using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Net.HttpStatusCode;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Dynamic;
using System.Security.Cryptography;

namespace Boggle
{
    /// <summary>
    /// Provides a way to start and stop the IIS web server from within the test
    /// cases.  If something prevents the test cases from stopping the web server,
    /// subsequent tests may not work properly until the stray process is killed
    /// manually.
    /// </summary>
    public static class IISAgent
    {
        // Reference to the running process
        private static Process process = null;

        /// <summary>
        /// Starts IIS
        /// </summary>
        public static void Start(string arguments)
        {
            if (process == null)
            {
                ProcessStartInfo info = new ProcessStartInfo(Properties.Resources.IIS_EXECUTABLE, arguments);
                info.WindowStyle = ProcessWindowStyle.Minimized;
                info.UseShellExecute = false;
                process = Process.Start(info);
            }
        }

        /// <summary>
        ///  Stops IIS
        /// </summary>
        public static void Stop()
        {
            if (process != null)
            {
                process.Kill();
            }
        }
    }

    [TestClass]
    public class BoggleTests
    {
        /// <summary>
        /// This is automatically run prior to all the tests to start the server
        /// </summary>
        [ClassInitialize()]
        public static void StartIIS(TestContext testContext)
        {
            IISAgent.Start(@"/site:""BoggleService"" /apppool:""Clr4IntegratedAppPool"" /config:""..\..\..\.vs\config\applicationhost.config""");
        }

        /// <summary>
        /// This is automatically run when all tests have completed to stop the server
        /// </summary>
        [ClassCleanup()]
        public static void StopIIS()
        {
            IISAgent.Stop();
        }

        private RestTestClient client = new RestTestClient("http://localhost:60000/BoggleService.svc/");

        // copy of token generator from defs for testing
        static readonly char[] AvailableCharacters = {
    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
    'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
    'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
    'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
    '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_'
        };

        public string GenerateTokenString(int length)
        {
            char[] identifier = new char[length];
            byte[] randomData = new byte[length];

            //randomize
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomData);
            }

            //use randomized data to create token
            for (int idx = 0; idx < identifier.Length; idx++)
            {
                int pos = randomData[idx] % AvailableCharacters.Length;
                identifier[idx] = AvailableCharacters[pos];
            }

            //return token
            return new string(identifier);
        }

        /// <summary>
        /// Note that DoGetAsync (and the other similar methods) returns a Response object, which contains
        /// the response Stats and the deserialized JSON response (if any).  See RestTestClient.cs
        /// for details.
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            Response r = client.DoGetAsync("word?index={0}", "-5").Result;
            Assert.AreEqual(Forbidden, r.Status);

            r = client.DoGetAsync("word?index={0}", "5").Result;
            Assert.AreEqual(OK, r.Status);

            string word = (string) r.Data;
            Assert.AreEqual("AAL", word);
        }

        [TestMethod]
        public void Test001_CreateUser()
        {
            dynamic user = new ExpandoObject();

            user.Nickname = "@John";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(Created, r.Status);

            user.Nickname = "#John";
            r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(NotImplemented, r.Status);
            
            user.Nickname = "John";
            r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(Created, r.Status);

            user.Nickname = null;
            r = client.DoPostAsync("users", user).Result;
            // bad request
            Assert.AreEqual(Forbidden, r.Status);

            user.Nickname = "";
            r = client.DoPostAsync("users", user).Result;
            // bad request
            Assert.AreEqual(Forbidden, r.Status);
        }

        [TestMethod]
        public void Test002_JoinGame()
        {
            dynamic user = new ExpandoObject();
            dynamic gameInput = new ExpandoObject();

            // token not there
            gameInput.UserToken = GenerateTokenString(40);
            gameInput.TimeLimit = 60;
            Response r = client.DoPostAsync("games", gameInput).Result;
            Assert.AreEqual(Forbidden, r.Status);

            // time limit too low
            user.Nickname = "John";
            r = client.DoPostAsync("users", user).Result;
            gameInput.UserToken = r.Data.UserToken;
            gameInput.TimeLimit = 0;
            r = client.DoPostAsync("games", gameInput).Result;
            Assert.AreEqual(Forbidden, r.Status);

            // time limit too high
            user.Nickname = "John";
            r = client.DoPostAsync("users", user).Result;
            gameInput.UserToken = r.Data.UserToken;
            gameInput.TimeLimit = 100000;
            r = client.DoPostAsync("games", gameInput).Result;
            Assert.AreEqual(Forbidden, r.Status);

            // pending game - created game
            user.Nickname = "John";
            r = client.DoPostAsync("users", user).Result;
            gameInput.UserToken = r.Data.UserToken;
            gameInput.TimeLimit = 60;
            r = client.DoPostAsync("games", gameInput).Result;
            user.Nickname = "Sally";
            r = client.DoPostAsync("users", user).Result;
            gameInput.UserToken = r.Data.UserToken;
            gameInput.TimeLimit = 60;
            r = client.DoPostAsync("games", gameInput).Result;
            Assert.AreEqual(Created, r.Status);

            // not sure how to test a generated token collision, 
            // but it needs to happen for code coverage
        }

        [TestMethod]
        public void Test003_CancelJoin()
        {
            dynamic user = new ExpandoObject();
            dynamic gameInput = new ExpandoObject();

            // null user
            user.Nickname = null;
            Response r = client.DoPutAsync(user, "games").Result;
            Assert.AreEqual(Forbidden, r.Status);

            // no pending game
            user.Nickname = "John";
            r = client.DoPutAsync(user, "games").Result;
            Assert.AreEqual(Forbidden, r.Status);

            // not the same guy
            user.Nickname = "John";
            r = client.DoPostAsync("users", user).Result;
            gameInput.UserToken = r.Data.UserToken;
            gameInput.TimeLimit = 60;
            var userID = r.Data;
            r = client.DoPostAsync("games", gameInput).Result;
            user.Nickname = "notJohn";
            r = client.DoPutAsync(client.DoPostAsync("users", user).Result.Data, "games").Result;
            Assert.AreEqual(Forbidden, r.Status);

            // cancel success
            r = client.DoPutAsync(userID, "games").Result;
            Assert.AreEqual(OK, r.Status);
        }

        [TestMethod]
        public void Test004_PlayWord()
        {
            // not finished making test
            dynamic user = new ExpandoObject();
            dynamic gameInput = new ExpandoObject();

            user.Nickname = "John";
            Response r = client.DoPostAsync("users", user).Result;
            gameInput.UserToken = r.Data.UserToken;
            gameInput.TimeLimit = 60;
            r = client.DoPostAsync("games", gameInput).Result;
            user.Nickname = "Sally";
            r = client.DoPostAsync("users", user).Result;
            gameInput.UserToken = r.Data.UserToken;
            gameInput.TimeLimit = 60;
            r = client.DoPostAsync("games", gameInput).Result;
            Assert.AreEqual(Created, r.Status);
        }

        //[TestMethod]
        //public void TestCreate1()
        //{
        //    Response r = 
        //}
    }
}
