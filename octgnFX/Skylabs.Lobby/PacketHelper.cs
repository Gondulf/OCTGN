using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Octgn.Data;
using agsXMPP;
using agsXMPP.Xml.Dom;
using agsXMPP.protocol.client;

namespace Skylabs.Lobby
{
    public static class PacketHelper
    {
        public static Element Gaming_CreateGame(Game game, string name, Jid me)
        {
            var m = new Message(new Jid("game.skylabsonline.com"), me.Bare, MessageType.chat, "");
            var ge = new Element("gaming");
            ge.SetAttribute("type", "create");
            ge.AddTag("guid", game.Id.ToString());
            ge.AddTag("version", game.Version.ToString());
            ge.AddTag("gamename", game.Name);
            ge.AddTag("name", name);
            m.AddChild(ge);
            m.GenerateId();
            m.SetAttribute("type", "gaming");
            return m;
        }
    }
}
