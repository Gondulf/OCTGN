using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using Octgn.Server;

namespace Octgn.Networking
{
    public sealed class Client
    {
        #region Private fields

        private readonly Handler _handler; // Message handler
        private bool _disposed; // True when the client has been closed
        private XmlReceiveDelegate _xmlHandler; // Receive delegate when in xml mode

        // Delegates definitions

        #region Nested type: BinaryReceiveDelegate

        private delegate void BinaryReceiveDelegate(byte[] data);

        #endregion

        #region Nested type: XmlReceiveDelegate

        private delegate void XmlReceiveDelegate(string xmlMsg);

        #endregion

        #endregion

        #region Public interface

        // Indicates if this client is connected
        public Client(IPAddress address, int port)
        {
            // Init fields
            _handler = new Handler();
            _xmlHandler = _handler.ReceiveMessage;
            // Create a remote call interface
            Rpc = new XmlSenderStub(new TcpClient());
        }

        // Used to send messages to the server
        internal IServerCalls Rpc { get; private set; }

        public int Muted { get; set; }

        // Gets the underlying windows socket
        //public TcpClient Socket
        //{ get { return tcp; } }

        // C'tor

        // Try to connect the client to the server
        public void Connect()
        {
            // Connect to the give address
            _disposed = false;
            // Start waiting for incoming data
        }

        // Disconnect the client
        public void Disconnect()
        {
            // Lock the disposed field
            lock (this)
            {
                // Quits if this client has already been disposed
                if (_disposed) return;
                // Close the connection
                //TODO Tell the GameRoom we're disconnecting from the room
                // Set disposed to 0
                _disposed = true;
            }
        }

        #endregion

        #region Private implementation

        // Called when the client is unexpectedly disconnected
        internal void Disconnected()
        {
            // Lock the disposed field
            lock (this)
            {
                // Quits if the client is already disconnected
                if (_disposed) return;
                // Disconnect
                Disconnect();
            }

            if (Program.Dispatcher != null)
                Program.Dispatcher.Invoke(new Action<string>(Program.TraceWarning),
                                          "You have been disconnected from server.");
            else
                Program.TraceWarning("You have been disconnected from server.");
        }

        // Handle an xml packet
        private void XmlReceive(int count)
        {
            //Program.Dispatcher.BeginInvoke(DispatcherPriority.Normal, _xmlHandler, xml);
        }

        #endregion
    }

    public class ConnectedEventArgs : EventArgs
    {
        public Exception Exception; // null for success

        public ConnectedEventArgs()
        {
            Exception = null;
        }

        public ConnectedEventArgs(Exception error)
        {
            Exception = error;
        }
    }
}