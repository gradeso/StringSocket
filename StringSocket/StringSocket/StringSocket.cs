// Written by Joe Zachary for CS 3500, November 2012
// Revised by Joe Zachary April 2016
// Revised extensively by Joe Zachary April 2017

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace CustomNetworking
{
    /// <summary>
    /// The type of delegate that is called when a StringSocket send has completed.
    /// </summary>
    public delegate void SendCallback(bool wasSent, object payload);

    /// <summary>
    /// The type of delegate that is called when a receive has completed.
    /// </summary>
    public delegate void ReceiveCallback(String s, object payload);

    /// <summary> 
    /// A StringSocket is a wrapper around a Socket.  It provides methods that
    /// asynchronously read lines of text (strings terminated by newlines) and 
    /// write strings. (As opposed to Sockets, which read and write raw bytes.)  
    ///
    /// StringSockets are thread safe.  This means that two or more threads may
    /// invoke methods on a shared StringSocket without restriction.  The
    /// StringSocket takes care of the synchronization.
    /// 
    /// Each StringSocket contains a Socket object that is provided by the client.  
    /// A StringSocket will work properly only if the client refrains from calling
    /// the contained Socket's read and write methods.
    /// 
    /// We can write a string to a StringSocket ss by doing
    /// 
    ///    ss.BeginSend("Hello world", callback, payload);
    ///    
    /// where callback is a SendCallback (see below) and payload is an arbitrary object.
    /// This is a non-blocking, asynchronous operation.  When the StringSocket has 
    /// successfully written the string to the underlying Socket, or failed in the 
    /// attempt, it invokes the callback.  The parameter to the callback is the payload.  
    /// 
    /// We can read a string from a StringSocket ss by doing
    /// 
    ///     ss.BeginReceive(callback, payload)
    ///     
    /// where callback is a ReceiveCallback (see below) and payload is an arbitrary object.
    /// This is non-blocking, asynchronous operation.  When the StringSocket has read a
    /// string of text terminated by a newline character from the underlying Socket, or
    /// failed in the attempt, it invokes the callback.  The parameters to the callback are
    /// a string and the payload.  The string is the requested string (with the newline removed).
    /// </summary>

    public class StringSocket : IDisposable
    {
        // representation
        // Underlying socket
        private Socket socket;

        // Encoding used for sending and receiving
        private Encoding encoding;

        // holds the outgoing message
        private string outgoing;

        // holds the incoming message
        private string incoming;

        // holds recieved requests
        private LinkedList<Request> rRequests;

        // holds good to go messages
        private LinkedList<string> messages;

        // holds send requests
        private LinkedList<Request> sRequests;

        // lock for asyncronization
        private readonly object sync;

        /// <summary>
        /// Creates a StringSocket from a regular Socket, which should already be connected.  
        /// The read and write methods of the regular Socket must not be called after the
        /// StringSocket is created.  Otherwise, the StringSocket will not behave properly.  
        /// The encoding to use to convert between raw bytes and strings is also provided.
        /// </summary>
        internal StringSocket(Socket s, Encoding e)
        {
            socket = s;
            encoding = e;
            outgoing = "";
            incoming = "";
            rRequests = new LinkedList<Request>();
            messages = new LinkedList<string>();
            sRequests = new LinkedList<Request>();
            sync = new object();
            // TODO: Complete implementation of StringSocket
        }

        /// <summary>
        /// Shuts down this StringSocket.
        /// </summary>
        public void Shutdown(SocketShutdown mode)
        {
            socket.Shutdown(mode);
        }

        /// <summary>
        /// Closes this StringSocket.
        /// </summary>
        public void Close()
        {
            socket.Close();
        }

        /// <summary>
        /// We can write a string to a StringSocket ss by doing
        /// 
        ///    ss.BeginSend("Hello world", callback, payload);
        ///    
        /// where callback is a SendCallback (see below) and payload is an arbitrary object.
        /// This is a non-blocking, asynchronous operation.  When the StringSocket has 
        /// successfully written the string to the underlying Socket it invokes the callback.  
        /// The parameters to the callback are true and the payload.
        /// 
        /// If it is impossible to send because the underlying Socket has closed, the callback 
        /// is invoked with false and the payload as parameters.
        ///
        /// This method is non-blocking.  This means that it does not wait until the string
        /// has been sent before returning.  Instead, it arranges for the string to be sent
        /// and then returns.  When the send is completed (at some time in the future), the
        /// callback is called on another thread.
        /// 
        /// This method is thread safe.  This means that multiple threads can call BeginSend
        /// on a shared socket without worrying around synchronization.  The implementation of
        /// BeginSend must take care of synchronization instead.  On a given StringSocket, each
        /// string arriving via a BeginSend method call must be sent (in its entirety) before
        /// a later arriving string can be sent.
        /// </summary>
        public void BeginSend(String s, SendCallback callback, object payload)
        {
            lock (sync)
            {
                sRequests.AddLast(new Request { Message = s, Callback = callback, Payload = payload });
                if (sRequests.Count == 1)
                {
                    //more
                    if(sRequests.Count > 0)
                    {
                        byte[] bytes = encoding.GetBytes(sRequests.First().Message);
                        socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, SendAsyncCallback, bytes);
                    }
                }
            }
        }

        /// <summary>
        /// this will be called when a message is sent
        /// </summary>
        private void SendAsyncCallback(IAsyncResult result)
        {
            byte[] bytes = (byte[])result.AsyncState;
            int totalLength = bytes.Length;
            int currentLength = socket.EndSend(result);

            if(currentLength == totalLength)
            {
                lock (sync)
                {
                    Request r = sRequests.First();
                    sRequests.RemoveFirst();
                    var c = (SendCallback)r.Callback;
                    ThreadPool.QueueUserWorkItem(x => c(true, r.Payload));

                    //more
                    if(sRequests.Count > 0)
                    {
                        byte[] moreBytes = encoding.GetBytes(sRequests.First().Message);
                        socket.BeginSend(moreBytes, 0, moreBytes.Length, SocketFlags.None, SendAsyncCallback, moreBytes);
                    }
                }
            }
            else
            {
                socket.BeginSend(bytes, currentLength, totalLength - currentLength, SocketFlags.None, SendAsyncCallback, bytes);
            }
        }

        /// <summary>
        /// We can read a string from the StringSocket by doing
        /// 
        ///     ss.BeginReceive(callback, payload)
        ///     
        /// where callback is a ReceiveCallback (see below) and payload is an arbitrary object.
        /// This is non-blocking, asynchronous operation.  When the StringSocket has read a
        /// string of text terminated by a newline character from the underlying Socket, it 
        /// invokes the callback.  The parameters to the callback are a string and the payload.  
        /// The string is the requested string (with the newline removed).
        /// 
        /// Alternatively, we can read a string from the StringSocket by doing
        /// 
        ///     ss.BeginReceive(callback, payload, length)
        ///     
        /// If length is negative or zero, this behaves identically to the first case.  If length
        /// is positive, then it reads and decodes length bytes from the underlying Socket, yielding
        /// a string s.  The parameters to the callback are s and the payload
        ///
        /// In either case, if there are insufficient bytes to service a request because the underlying
        /// Socket has closed, the callback is invoked with null and the payload.
        /// 
        /// This method is non-blocking.  This means that it does not wait until a line of text
        /// has been received before returning.  Instead, it arranges for a line to be received
        /// and then returns.  When the line is actually received (at some time in the future), the
        /// callback is called on another thread.
        /// 
        /// This method is thread safe.  This means that multiple threads can call BeginReceive
        /// on a shared socket without worrying around synchronization.  The implementation of
        /// BeginReceive must take care of synchronization instead.  On a given StringSocket, each
        /// arriving line of text must be passed to callbacks in the order in which the corresponding
        /// BeginReceive call arrived.
        /// 
        /// Note that it is possible for there to be incoming bytes arriving at the underlying Socket
        /// even when there are no pending callbacks.  StringSocket implementations should refrain
        /// from buffering an unbounded number of incoming bytes beyond what is required to service
        /// the pending callbacks.
        /// </summary>
        public void BeginReceive(ReceiveCallback callback, object payload, int length = 0)
        {
            lock (sync)
            {
                rRequests.AddLast(new Request { Message = null, Callback = callback, Payload = payload });
                if(rRequests.Count == 1)
                {
                    //more
                    while (rRequests.Count > 0 && messages.Count > 0)
                    {
                        Request r = rRequests.First();
                        rRequests.RemoveFirst();
                        string m = messages.First();
                        messages.RemoveFirst();
                        var c = (ReceiveCallback)r.Callback;
                        ThreadPool.QueueUserWorkItem(x => c(m, r.Payload));
                    }
                    if(rRequests.Count > 0)
                    {
                        byte[] bytes = new byte[1024];
                        if (length > 0) { bytes = new byte[length]; }
                        socket.BeginReceive(bytes, 0, bytes.Length, SocketFlags.None, ReceiveAsyncCallback, bytes);
                    }
                }
            }
        }

        /// <summary>
        /// this is a mothod that will be called when a message is sent
        /// </summary>
        private void ReceiveAsyncCallback(IAsyncResult result)
        {
            byte[] bytes = (byte[]) result.AsyncState;
            int totalLength = bytes.Length;
            int currentLength = socket.EndSend(result);

            if(currentLength == 0)
            {
                string n = null;
                messages.AddLast(n);
                // socket.BeginReceive(bytes, 0, bytes.Length, SocketFlags.None, ReceiveAsyncCallback, bytes);
            }
            else
            {
                incoming += encoding.GetString(bytes, 0, currentLength);
                for(int i = 0; i < incoming.Length; i++)
                {
                    if(incoming[i] == '\n')
                    {
                        messages.AddLast(incoming.Substring(0, i));
                        incoming = incoming.Substring(0, i + 1);
                        break;
                    }
                }

                // more
                while (rRequests.Count > 0 && messages.Count > 0)
                {
                    Request r = rRequests.First();
                    rRequests.RemoveFirst();
                    string m = messages.First();
                    messages.RemoveFirst();
                    var c = (ReceiveCallback)r.Callback;
                    ThreadPool.QueueUserWorkItem(x => c(m, r.Payload));
                }
                if(rRequests.Count > 0)
                {
                    byte[] moreBytes = new byte[totalLength];
                    socket.BeginReceive(moreBytes, 0, moreBytes.Length, SocketFlags.None, ReceiveAsyncCallback, moreBytes);
                }
            }
        }

        /// <summary>
        /// Frees resources associated with this StringSocket.
        /// </summary>
        public void Dispose()
        {
            Shutdown(SocketShutdown.Both);
            Close();
        }

        private class Request
        {
            public string Message { get; set; }
            public object Callback { get; set; }
            public object Payload { get; set; }
        }
    }
}
