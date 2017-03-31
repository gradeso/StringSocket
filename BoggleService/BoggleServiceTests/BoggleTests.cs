using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Net.HttpStatusCode;
using System.Diagnostics;
using System.Dynamic;
using Newtonsoft.Json;
using System.Net.Http;

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

        

        [TestMethod]
        public void TestCreateUser()
        {
            //Try with valid name, expect 201, created
            dynamic data = new ExpandoObject();
            data.Nickname = "TestName";
            Response r = client.DoPostAsync("users", data).Result;
            Assert.AreEqual(Created, r.Status);
            //dynamic output = r.Content.ReadAsStringAsync().Result;

            //Try with null nickname, expect 403
            data = new ExpandoObject();
            data.Nickname = "";
            r = client.DoPostAsync("users", data).Result;
            Assert.AreEqual(Forbidden, r.Status);

            //Try with empty nickname,excpect 403
            data = new ExpandoObject();
            data.Nickname = " ";
            r = client.DoPostAsync("users", data).Result;
            Assert.AreEqual(Forbidden, r.Status);
        }

        //birdman 24 - 8015825431

        [TestMethod]
        public void TestJoinGameAccepted()
        {
            //Create user
            dynamic data = new ExpandoObject();
            data.Nickname = "TestName";
            HttpResponseMessage r = client.DoPostAsync("users", data).Result;

            string arg1 = r.Content.ReadAsStringAsync().Result;
            dynamic arg = JsonConvert.DeserializeObject(arg1);

            //Submit joinGame with created user, expect Accecpted, because
            //only one user has been created
            dynamic newData = new ExpandoObject();
            newData.UserToken = arg.UserToken;
            newData.TimeLimit = 100;
            r = client.DoPostAsync("users", newData).Result;
            Assert.AreEqual(r.StatusCode, Accepted);

        }

        [TestMethod]
        public void TestJoinGameCreated()
        {
            //Create two users
            dynamic data = new ExpandoObject();
            data.Nickname = "TestName";
            HttpResponseMessage r = client.DoPostAsync("users", data).Result;

            string arg1 = r.Content.ReadAsStringAsync().Result;
            dynamic arg = JsonConvert.DeserializeObject(arg1);

            dynamic data2 = new ExpandoObject();
            data.Nickname = "TestName2";
            r = client.DoPostAsync("users", data2).Result;

            arg1 = r.Content.ReadAsStringAsync().Result;
            dynamic arg3 = JsonConvert.DeserializeObject(arg1);

            //Submit joinGame with created user, expect Accecpted, because
            //only one user has been created
            dynamic newData = new ExpandoObject();
            newData.UserToken = arg.UserToken;
            newData.TimeLimit = 100;
            r = client.DoPostAsync("users", newData).Result;

            //Submit second joinGame request with second user, expect
            //created because two users have joined current pending game
            newData = new ExpandoObject();
            newData.UserToken = arg3.UserToken;
            newData.TimeLimit = 100;
            r = client.DoPostAsync("users", newData).Result;
            Assert.AreEqual(r.StatusCode, Created);
        }

        [TestMethod]
        public void TestJoinGameForbidden()
        {
            //Create user
            dynamic data = new ExpandoObject();
            data.Nickname = "TestName";
            HttpResponseMessage r = client.DoPostAsync("users", data).Result;

            string arg1 = r.Content.ReadAsStringAsync().Result;
            dynamic arg = JsonConvert.DeserializeObject(arg1);

            //Submit joinGame with created user, with time less then 
            //5 seconds, expect forbidden
            dynamic newData = new ExpandoObject();
            newData.UserToken = arg.UserToken;
            newData.TimeLimit = 4;
            r = client.DoPostAsync("users", newData).Result;
            Assert.AreEqual(r.StatusCode, Forbidden);
        }

        [TestMethod]
        public void TestJoinGameForbidden2()
        {
            //Create user
            dynamic data = new ExpandoObject();
            data.Nickname = "TestName";
            HttpResponseMessage r = client.DoPostAsync("users", data).Result;

            string arg1 = r.Content.ReadAsStringAsync().Result;
            dynamic arg = JsonConvert.DeserializeObject(arg1);

            //Submit joinGame with created user, with time less then 
            //5 
            dynamic newData = new ExpandoObject();
            newData.UserToken = arg.UserToken;
            newData.TimeLimit = 4;
            r = client.DoPostAsync("users", newData).Result;
            Assert.AreEqual(r.StatusCode, Accepted);
        }

        [TestMethod]
        public void TestJoinGameForbidden3()
        {
            //Create user
            dynamic data = new ExpandoObject();
            data.Nickname = "TestName";
            HttpResponseMessage r = client.DoPostAsync("users", data).Result;

            string arg1 = r.Content.ReadAsStringAsync().Result;
            dynamic arg = JsonConvert.DeserializeObject(arg1);

            //Submit joinGame with created user, with time greater then 120 seconds
            //expect forbidden
            dynamic newData = new ExpandoObject();
            newData.UserToken = arg.UserToken;
            newData.TimeLimit = 4;
            r = client.DoPostAsync("users", newData).Result;
            Assert.AreEqual(r.StatusCode, Forbidden);
        }

        [TestMethod]
        public void TestJoinGameForbidden4()
        {
            //Create user
            dynamic data = new ExpandoObject();
            data.Nickname = "TestName";
            HttpResponseMessage r = client.DoPostAsync("users", data).Result;

            string arg1 = r.Content.ReadAsStringAsync().Result;
            dynamic arg = JsonConvert.DeserializeObject(arg1);

            //Submit joinGame with created user, with invalid UserToken
            dynamic newData = new ExpandoObject();
            newData.UserToken = "";
            newData.TimeLimit = 4;
            r = client.DoPostAsync("users", newData).Result;
            Assert.AreEqual(r.StatusCode, Forbidden);
        }

        [TestMethod]
        public void CancelJoinRequest()
        {
            //Create user
            dynamic data = new ExpandoObject();
            data.Nickname = "TestName";
            Response r = client.DoPostAsync("users", data).Result;

            string userToken = r.Data.UserToken;
            
            //dynamic arg = JsonConvert.DeserializeObject(arg1);

            //Submit joinGame with created user, gamestatus will be pending

            dynamic newData = new ExpandoObject();
            newData.UserToken = userToken;

            r = client.DoPostAsync("games", newData).Result;
            r = client.DoPutAsync(newData,"games").Result;
            Assert.AreEqual(OK,r.Status);
        }

        [TestMethod]
        public void TestGameStatusPending()
        {

        }

        [TestMethod]
        public void TestGameStatusActive()
        {

        }

        [TestMethod]
        public void TestGameStatusCompleted()
        {

        }

        [TestMethod]
        public void Test()
        {

        }


    }
}
