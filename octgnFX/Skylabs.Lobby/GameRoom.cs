using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.client;

namespace Skylabs.Lobby
{
    public class GameRoom : IDisposable
    {
        public delegate void dMessageReceived(object sender, Jid from, string message, DateTime rTime, Client.LobbyMessageType mType = Client.LobbyMessageType.Standard);
        public delegate void dUserListChange(object sender, List<Jid> users);

        public event dMessageReceived OnMessageRecieved;
        public event dUserListChange OnUserListChange;

        public long GID { get; private set; }
        public List<Jid> Users { get; set; } 

        private Client _client;
        private XmppClientConnection _xmpp;
        private readonly Jid _groupUser;

        public GameRoom(Client client, XmppClientConnection xmpp,long gid)
        {
            GID = gid;
            _client = client;
            _xmpp = xmpp;
            Users = new List<Jid>();

            _xmpp.OnMessage += XmppOnOnMessage;
            _xmpp.OnPresence += XmppOnOnPresence;

            _groupUser = new Jid(gid + "@conference." + Client.ServerName);
            _client.MucManager.JoinRoom(_groupUser, _client.Me.User.User, true);
            Users.Add(_client.Me.User);
        }

        private void XmppOnOnPresence(object sender , Presence pres)
        {
            switch(pres.Type)
            {
                case PresenceType.available:
                    if(pres.From.Server == "conference." + Client.ServerName)
                    {
                        long rid = -1;
                        if(long.TryParse(pres.From.User,out rid))
                        {
                            if(rid == GID)
                            {
                                if (!Users.Exists(x => x.User.ToLower() == pres.MucUser.Item.Jid.User))
                                {
                                    Users.Add(pres.MucUser.Item.Jid.Bare);
                                    if(OnUserListChange != null)
                                        OnUserListChange.Invoke(this,Users);
                                }
                            }
                        }
                    }
                    break;
                case PresenceType.unavailable:
                    if (pres.From.Server == "conference." + Client.ServerName)
                    {
                        long rid = -1;
                        if (long.TryParse(pres.From.User, out rid))
                        {
                            if (pres.MucUser.Item.Jid == null) break;
                            if (pres.MucUser.Item.Jid.Bare == _client.Me.User.Bare) break;
                            if (Users.Exists(x => x.User.ToLower() == pres.MucUser.Item.Jid.User))
                            {
                                Users.RemoveAll(x => x.User.ToLower() == pres.MucUser.Item.Jid.User.ToLower());
                                if(OnUserListChange != null)
                                    OnUserListChange.Invoke(this,Users);
                            }
                        }
                    }
                    break;
            }
        }

        private void XmppOnOnMessage(object sender , Message msg)
        {
            var rTime = DateTime.Now;
            if (msg.XDelay != null && msg.XDelay.Stamp != null) rTime = msg.XDelay.Stamp.ToLocalTime();
            if(msg.GetAttribute("type") == "gaming")
            {
                
            }
            else
            {
                switch(msg.Type)
                {
                    case MessageType.normal:
                        break;
                    case MessageType.error:
                        break;
                    case MessageType.chat:
                        break;
                    case MessageType.groupchat:
                        if (!String.IsNullOrWhiteSpace(msg.Body))
                        {
                            if (OnMessageRecieved != null)
                                OnMessageRecieved.Invoke(this,
                                                             new Jid(msg.From.Resource + "@" + Client.ServerName),
                                                         msg.Body, rTime);
                        }
                        break;
                    case MessageType.headline:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void Dispose()
        {
            _xmpp.OnMessage -= XmppOnOnMessage;
            _xmpp.OnPresence -= XmppOnOnPresence;
            Users.Clear();
            Users = null;
        }
    }
}
