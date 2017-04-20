using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Boggle
{
    public class BoggleServer
    {
        static void Main()
        {
            new BoggleServer(60000);
            // This is our way of preventing the main thread from
            // exiting while the server is in use
            Console.ReadLine();
        }

        private TcpListener server;
        private readonly ReaderWriterLockSlim sync = new ReaderWriterLockSlim();
        private static BoggleService service = null;
        private static int clientIndexCount = -1;
        private Dictionary<int, ClientConnection> clients = new Dictionary<int, ClientConnection>();

        /// <summary>
        /// Creates a BoggleServer that listens for incoming reqests
        /// </summary>
        public BoggleServer(int port)
        {
            service = new BoggleService();
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            server.BeginAcceptSocket(ConnectionRequested, null);
        }

        /// <summary>
        /// adds to the outgoing response, if end of message sends the message
        /// </summary>
        public void SendResponse(string s, int clientIndex)
        {
            try
            {
                sync.EnterReadLock();

                ClientConnection c;
                if (!clients.TryGetValue(clientIndex, out c))
                {
                    Console.WriteLine("problem in send response");
                }


                c.SendMessage(s);

            }
            finally
            {
                sync.ExitReadLock();
            }
        }

        /// <summary>
        /// Remove c from the client list.
        /// </summary>
        public void RemoveClient(ClientConnection c)
        {
            try
            {
                sync.EnterWriteLock();
                clients.Remove(c.getClientID());
            }
            finally
            {
                sync.ExitWriteLock();
            }
        }
        /// <summary>
        /// passed to beginAcceptSocket when a connection request arrives
        /// </summary>
        private void ConnectionRequested(IAsyncResult result)
        {
            Socket s = server.EndAcceptSocket(result);
            server.BeginAcceptSocket(ConnectionRequested, null);

            try
            {
                sync.EnterWriteLock();
                var clcon = new ClientConnection(ref s, this, ++clientIndexCount);
                clcon.StartReciving();
                clients.Add(clientIndexCount, clcon);


            }
            finally
            {
                sync.ExitWriteLock();
            }
        }

        internal BoggleService getService()
        {
            return service;
        }


    }

    /// <summary>
    /// represents a connection with a remote client
    /// </summary>
    public class ClientConnection
    {
        private static System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
        private const int BUFFER_SIZE = 1024;
        private Socket sokit;
        private StringBuilder incoming;
        private StringBuilder outgoing;
        private Decoder decoder = encoding.GetDecoder();
        private byte[] incomingBytes = new byte[BUFFER_SIZE];
        private char[] incomingChars = new char[BUFFER_SIZE];
        private BoggleServer server;
        private bool sendIsOngoing = false;
        private readonly object sendSync = new object();
        private byte[] pendingBytes = new byte[0];
        private int pendingIndex = 0;
        private string incompleteMessage = "";
        private int clientID = -1;
        private bool clientIDSet = false;


        public ClientConnection(ref Socket s, BoggleServer server, int clientID)
        {
            this.clientID = clientID;
            this.server = server;
            sokit = s;
            incoming = new StringBuilder();
            outgoing = new StringBuilder();



        }

        internal void StartReciving()
        {

            sokit.BeginReceive(incomingBytes, 0, incomingBytes.Length,
                                SocketFlags.None, MessageReceived, null);

        }
        public void setClientID(int id)
        {
            if (clientIDSet) { return; }
            clientID = id;
            clientIDSet = true;

        }
        internal int getClientID()
        {
            return clientID;
        }
        /// <summary>
        /// Called when message received
        /// </summary>
        private void MessageReceived(IAsyncResult result)
        {
            int bytesRead = 0;


            bytesRead = sokit.EndReceive(result);




            // if there are more then 0 bytes decode them
            if (bytesRead == 0)
            {
                
            }
            else
            {
                // Convert the bytes into characters and appending to incoming

                int charsRead = decoder.GetChars(incomingBytes, 0, bytesRead, incomingChars, 0, true);
                incoming.Append(incomingChars, 0, charsRead);
                Console.WriteLine("------------------------------ START");
                Console.WriteLine(incoming);
                Console.WriteLine("------------------------------ END");
                incompleteMessage += incoming;
                if (checkIfIncomingReady())
                {
                    BuildResponse(incompleteMessage.ToString());
                    incompleteMessage = "";
                }
                else
                {

                }


                // decode here


                incoming.Clear();

                sokit.BeginReceive(incomingBytes, 0, incomingBytes.Length,
                            SocketFlags.None, MessageReceived, null);
            }
 
        }

        private bool checkIfIncomingReady()
        {
            bool isPotentialValidFile = false;
            int lowest = incompleteMessage.IndexOf("\r\n");
            int next = incompleteMessage.IndexOf("\r\n", lowest + 1);
            while (next > lowest)
            {
                if (next - lowest == 1 || next - lowest == 2)
                {


                    isPotentialValidFile = true;
                    break;
                }

                lowest = next;
                next = incompleteMessage.IndexOf("\r\n", lowest + 1);

            }




            if (next != -1 && !string.IsNullOrWhiteSpace(incompleteMessage.Substring(next)))
            {
                return true;
            }
            else if (isPotentialValidFile && incompleteMessage.Contains("Content-Length: 0"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sends a string to client
        /// </summary>
        public void SendMessage(string lines)
        {

            lock (sendSync)
            {
                // Append the message to the outgoing lines
                outgoing.Append(lines);

                // If there's not a send ongoing, start one.
                if (!sendIsOngoing)
                {
                    sendIsOngoing = true;
                    SendBytes();
                }
            }
        }

        private void SendBytes()
        {
            if (pendingIndex < pendingBytes.Length)
            {

                sokit.BeginSend(pendingBytes, pendingIndex, pendingBytes.Length - pendingIndex,
                                 SocketFlags.None, MessageSent, null);

            }
            // If we're not currently dealing with a block of bytes, make a new block of bytes
            // out of outgoing and start sending that.
            else if (outgoing.Length > 0)
            {
                pendingBytes = encoding.GetBytes(outgoing.ToString());
                pendingIndex = 0;
                outgoing.Clear();


                sokit.BeginSend(pendingBytes, 0, pendingBytes.Length,
                                 SocketFlags.None, MessageSent, null);


            }

            // If there's nothing to send, shut down for the time being.
            else
            {
                sendIsOngoing = false;
            }
        }

        /// <summary>
        /// Called when message has successfully sent
        /// </summary>
        private void MessageSent(IAsyncResult result)
        {
            int bytesSent = 0;

            bytesSent = sokit.EndSend(result);

            lock (sendSync)
            {
                if (bytesSent == 0)
                {
                    server.RemoveClient(this);
                    Console.WriteLine("Socket closed");
                }
                else
                {
                    pendingIndex += bytesSent;
                    SendBytes();
                }
            }
        }
        public void BuildResponse(string s)
        {

            StringBuilder currentResponse = new StringBuilder(s);

            // use service
            string completeResponse = "";


            HttpStatusCode status = HttpStatusCode.BadRequest;
            Dictionary<string, object> Headerlines = new Dictionary<string, object>();
            // make header
            int lNum = 0;
            int i = 0;
            int t = 0;
            while (i < currentResponse.Length)
            {
                if (currentResponse[i] == '\n')
                {

                    lNum++;
                    String line = currentResponse.ToString(t, i - t);

                    if (!HandleHeaderLine(line, lNum, ref Headerlines)) break;
                    t = i + 1;
                }
                i++;
            }
            object len;
            Headerlines.TryGetValue("length", out len);
            double lenInt;
            double.TryParse(len.ToString(), out lenInt);
            object verb;
            Headerlines.TryGetValue("verb", out verb);
            object url;
            Headerlines.TryGetValue("url", out url);
            object version;
            Headerlines.TryGetValue("version", out version);

            string body = currentResponse.ToString();
            body = body.Substring(i);

            string responseBody = actionRequested(verb.ToString(), url.ToString(), body, out status);
            if (responseBody == null || responseBody == "null") responseBody = "";
            // finishing up
            
            completeResponse += (generateResponseHeader(version.ToString(), status,encoding.GetByteCount(responseBody), Headerlines));
            completeResponse += "\r\n";
            completeResponse += (responseBody);
            Console.WriteLine(completeResponse);
            server.SendResponse(completeResponse + "\r\n", clientID);
        }


        private string generateResponseHeader(string version, HttpStatusCode status, int length, Dictionary<string, object> headerlines)
        {
            StringBuilder toReturn = new StringBuilder();

            toReturn.Append(version.Substring(0, version.Length - 1) + " " + (int)status + " " + status.ToString() + "\r\n");

            object line2;
            headerlines.TryGetValue("line2", out line2);
            toReturn.Append(line2.ToString() + "\n");

            object line3;
            headerlines.TryGetValue("line3", out line3);
            toReturn.Append(line3.ToString() + "\n");

            toReturn.Append("Content-Length: " + length + "\r\n");

            object line5;
            headerlines.TryGetValue("line5", out line5);
            toReturn.Append(line5.ToString() + "\n");

            object line6;
            headerlines.TryGetValue("line6", out line6);
            // only append if line6 isnt null which can happen when multiple test are run
            if (line6 != null) { toReturn.Append(line6.ToString() + "\n"); }

            return toReturn.ToString();
        }

        /// <summary>
        /// calls the appropriate service method and returns correct object serialized for being used as body.
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private string actionRequested(string verb, string url, string body, out HttpStatusCode status)
        {
            string bassURL = "/BoggleService.svc/";
            BoggleService service = server.getService();

            if (url.Contains(bassURL + "users"))
            {
                //register user
                if (verb == "POST")
                {
                    Name name;
                    try
                    {
                        name = JsonConvert.DeserializeObject<Name>(body);
                    }
                    catch
                    {
                        name = new Name();
                    }
                    UserIDInfo toSerialize = service.SaveUserID(name, out status);
                    return JsonConvert.SerializeObject(toSerialize);
                }
            }
            if (url.Contains(bassURL + "games"))
            {
                //join game
                if (verb == "POST")
                {
                    JoinAttempt ja;
                    try
                    {
                        ja = JsonConvert.DeserializeObject<JoinAttempt>(body);
                    }
                    catch
                    {
                        ja = new JoinAttempt();
                    }

                    GameIDInfo toSerialize = service.AttemptJoin(ja, out status);
                    return JsonConvert.SerializeObject(toSerialize);

                }
                if (verb == "PUT")
                {
                    //play word

                    if (url.Length > (bassURL + "games/").Length)
                    {

                        Move userToken;
                        try
                        {
                            userToken = JsonConvert.DeserializeObject<Move>(body);
                        }
                        catch
                        {
                            userToken = new Move();
                        }

                        ScoreInfo toSerialize = service.PlayWordInGame(url.Substring((bassURL + "games/").Length), userToken, out status);
                        string test = url.Substring((bassURL + "games/").Length);
                        return JsonConvert.SerializeObject(toSerialize);
                    }
                    // cancel game
                    else
                    {
                        UserIDInfo userToken;
                        try
                        {
                            userToken = JsonConvert.DeserializeObject<UserIDInfo>(body);
                        }
                        catch
                        {
                            userToken = new UserIDInfo();
                        }
                        service.CancelJoin(userToken, out status);
                        return "";
                    }
                }
                //game status
                if (verb == "GET")
                {
                    string lastOfURL = url.Substring((bassURL + "games/").Length);
                    string[] partsOfURL = Regex.Split(lastOfURL, "[?breif=]");


                    IGameState toSerialize = service.gameStatus(partsOfURL[0], partsOfURL[1], out status);
                    return JsonConvert.SerializeObject(toSerialize);
                }


            }
            status = HttpStatusCode.BadRequest;
            return "";
        }


        private bool HandleHeaderLine(string line, int lNum, ref Dictionary<string, object> HeaderLines)
        {
            switch (lNum)
            {
                case 1:
                    string[] parts = line.Split(' ');
                    HeaderLines.Add("verb", parts[0]);
                    HeaderLines.Add("url", parts[1]);
                    HeaderLines.Add("version", parts[2]);

                    return true;

                case 2:
                    // type is always json
                    HeaderLines.Add("line2", line);

                    return true;
                case 3:
                    // host
                    HeaderLines.Add("line3", line);

                    return true;
                case 4:
                    // content length
                    HeaderLines.Add("length", line.Substring("Content-Length: ".Length).Trim());

                    return true;
                case 5:
                    // expectation
                    HeaderLines.Add("line5", line);

                    return true;
                case 6:
                    //connection attribute
                    if (line == "\r") { return false; }
                    HeaderLines.Add("line6", line);

                    return true;
                default:

                    return false;

            }

        }

       
    }
}
