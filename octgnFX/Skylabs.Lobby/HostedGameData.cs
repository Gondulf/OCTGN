using System;
using System.Collections.Generic;
using System.Globalization;
using Skylabs.Net;
using agsXMPP;
using agsXMPP.Xml.Dom;
using agsXMPP.protocol.component;

namespace Skylabs.Lobby
{        
    [Serializable]
    public enum EHostedGame
    {
        StartedHosting,
        GameInProgress,
        StoppedHosting
    };
    public class HostedGameData :Element
    {
        public HostedGameData() : base("gameitem","gameitem","octgn:gameitem")
        {
            
        }

        public HostedGameData(Guid gameguid, Version gameversion, long id, string name, NewUser huser,
                          DateTime startTime)
            : base("gameitem", "gameitem", "octgn:gameitem")
        {
            GameGuid = gameguid;
            GameVersion = gameversion;
            ID = id;
            Name = name;
            UserHosting = huser;
            GameStatus = EHostedGame.StartedHosting;
            TimeStarted = startTime;
        }

        public HostedGameData(SocketMessage sm)
            : base("gameitem", "gameitem", "octgn:gameitem")
        {
            GameGuid = (Guid) sm["guid"];
            GameVersion = (Version) sm["version"];
            ID = (long) sm["port"];
            Name = (string) sm["name"];
            UserHosting = ((NewUser) sm["hoster"]);
            GameStatus = EHostedGame.StartedHosting;
            TimeStarted = new DateTime(DateTime.Now.ToUniversalTime().Ticks);
        }

        public Guid GameGuid
        {
            get
            {
                Guid ret = Guid.Empty;
                Guid.TryParse(GetTag("guid"),out ret);
                return ret;
            } 
            set{SetTag("guid",value.ToString());}
        }
        public Version GameVersion
        {
            get
            {
                Version v = new Version(0,0);
                Version.TryParse(GetTag("version") , out v);
                return v;
            } 
            set
            {
                SetTag("version",value.ToString());
            }
        }
        public long ID
        {
            get { return long.Parse(GetTag("port")); }
            set{SetTag("port",value.ToString(CultureInfo.InvariantCulture));}
        }
        public String Name
        {
            get { return GetTag("name"); }
            set{SetTag("name",value);}
        }
        public NewUser UserHosting
        {
            get { return new NewUser(GetTagJid("userhosting")); }
            set{SetTag("userhosting",value.User.Bare);}
        }
        public EHostedGame GameStatus
        {
            get
            {
                EHostedGame ret = EHostedGame.StoppedHosting;
                Enum.TryParse(GetTag("gamestatus") , out ret);
                return ret;
            }
            set{SetTag("gamestatus",value.ToString());}
        }
        public DateTime TimeStarted
        {
            get
            {
                DateTime ret = DateTime.Now;
                DateTime.TryParse(GetTag("timestarted") , out ret);
                return ret;
            }
            set{SetTag("timestarted",value.ToString());}
        }
    }
}