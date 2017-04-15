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
        private List<ClientConnection> clients = new List<ClientConnection>();
        private readonly ReaderWriterLockSlim sync = new ReaderWriterLockSlim();
        private BoggleService service = null;
        private StringBuilder currentResponse = null;

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
        /// passed to beginAcceptSocket when a connection request arrives
        /// </summary>
        private void ConnectionRequested(IAsyncResult result)
        {
            Socket s = server.EndAcceptSocket(result);
            server.BeginAcceptSocket(ConnectionRequested, null);

            try
            {
                sync.EnterWriteLock();
                clients.Add(new ClientConnection(s, this));
            }
            finally
            {
                sync.ExitWriteLock();
            }
        }

        public void BuildResponse(string s)
        {
            if(currentResponse == null)
            {
                currentResponse = new StringBuilder(s);
            }
            else
            {
                // use service
                var completeResponse = new StringBuilder();
                string body = null;

                HttpStatusCode status;

                // make header
                int lNum = 0;
                int start = 0;
                for (int i = 0; i < currentResponse.Length; i++)
                {
                    if (currentResponse[i] == '\n')
                    {
                        lNum++;
                        String line = currentResponse.ToString(start, i + 1 - start);

                        if(lNum == 1)
                        {
                            string[] parts = line.Split(' ');
                            string verb = parts[0];
                            string url = parts[1];
                            string ver = parts[2];

                            if(verb == "POST")
                            {
                                // Regex r = new Regex(@"^/BoggleService.svc/Games/(\S+)$");
                                // StringBuilder body = new StringBuilder(s);
                                if(url == @"/BoggleService.svc/users")
                                {
                                    // De means deserialized, Se means serialized
                                    Name userDe = JsonConvert.DeserializeObject<Name>(s);
                                    UserIDInfo user = service.SaveUserID(userDe, out status);
                                    string userSe = JsonConvert.SerializeObject(user);
                                    body = userSe;
                                    completeResponse.Append(ver + " ");
                                    completeResponse.Append((int)status + " ");
                                    completeResponse.Append(status.ToString());
                                    completeResponse.Append("\r\n");
                                }
                            }
                        }
                        if(lNum == 2) // type is always json
                        {
                            completeResponse.Append(line);
                        }
                        if(lNum == 3) // host
                        {
                            completeResponse.Append(line);
                        }
                        if (lNum == 4) // content length
                        {
                            completeResponse.Append("Content-Length: " + body.Length + "\r\n");
                        }
                        if (lNum == 5) // expectation
                        {
                            completeResponse.Append(line);
                        }
                        if (lNum == 6) // connection attribute
                        {
                            completeResponse.Append(line);
                        }
                        start = i + 1;
                    }
                }

                // finishing up
                completeResponse.Append(body);
                SendResponse(completeResponse.ToString());
            }
        }
        /// <summary>
        /// sends the completed response. Currently sends response to all servers
        /// </summary>
        public void SendResponse(string s)
        {
            try
            {
                sync.EnterReadLock();
                foreach (ClientConnection c in clients)
                {
                    c.SendMessage(s);
                }
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
                clients.Remove(c);
            }
            finally
            {
                sync.ExitWriteLock();
            }
        }
    }

    /// <summary>
    /// represents a connection with a remote client
    /// </summary>
    public class ClientConnection
    {
        private static System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
        private const int BUFFER_SIZE = 1024;
        private Socket socket;
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
        private StringBuilder re;

        public ClientConnection(Socket s, BoggleServer server)
        {
            this.server = server;
            socket = s;
            incoming = new StringBuilder();
            outgoing = new StringBuilder();

            try
            {
                socket.BeginReceive(incomingBytes, 0, incomingBytes.Length,
                                    SocketFlags.None, MessageReceived, null);
            }
            catch (ObjectDisposedException)
            {
            }
        }

        /// <summary>
        /// Called when message received
        /// </summary>
        private void MessageReceived(IAsyncResult result)
        {
            int bytesRead = socket.EndReceive(result);

            // if there are more then 0 bytes decode them
            if (bytesRead == 0)
            {
                Console.WriteLine("Socket closed");
                server.RemoveClient(this);
                socket.Close();
            }
            else
            {
                int charsRead = decoder.GetChars(incomingBytes, 0, bytesRead, incomingChars, 0, false);
                incoming.Append(incomingChars, 0, charsRead);
                Console.WriteLine(incoming);

                // decode here
                server.BuildResponse(incoming.ToString());

                incoming.Remove(0, incoming.Length);

                try
                {
                    socket.BeginReceive(incomingBytes, 0, incomingBytes.Length,
                        SocketFlags.None, MessageReceived, null);
                }
                catch (ObjectDisposedException)
                {
                }
            }
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
                try
                {
                    socket.BeginSend(pendingBytes, pendingIndex, pendingBytes.Length - pendingIndex,
                                     SocketFlags.None, MessageSent, null);
                }
                catch (ObjectDisposedException)
                {
                }
            }
            else if (outgoing.Length > 0)
            {
                pendingBytes = encoding.GetBytes(outgoing.ToString());
                pendingIndex = 0;
                outgoing.Clear();
                try
                {
                    socket.BeginSend(pendingBytes, 0, pendingBytes.Length,
                                     SocketFlags.None, MessageSent, null);
                }
                catch (ObjectDisposedException)
                {
                }
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
            int bytesSent = socket.EndSend(result);

            lock (sendSync)
            {
                if (bytesSent == 0)
                {
                    socket.Close();
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
    }
}
