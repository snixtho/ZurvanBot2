using Newtonsoft.Json;
using ZurvanBot.Discord.Resources.Objects;
using ZurvanBot.Discord.Resources.Params;

namespace ZurvanBot.Discord.Resources
{
    public class User: Resource
    {
        public UserObject GetCurrentUser()
        {
            var re = _request.GetRequestAsync("/users/@me").Result;
            if (re.Code != 200)
                return null; // handle these errors ?
            var r = JsonConvert.DeserializeObject<UserObject>(re.Contents);
            return r;
        }

        public UserObject GetUser(ulong userId)
        {
            var re = _request.GetRequestAsync("/users/" + userId).Result;
            if (re.Code != 200)
                return null; // handle these errors ?
            var r = JsonConvert.DeserializeObject<UserObject>(re.Contents);
            return r;
        }

        public UserObject ModifyCurrentUser(ModifyChannelParams pars)
        {
            var re = _request.PatchRequestAsync("/users/@me", pars).Result;
            if (re.Code != 200)
                return null; // handle these errors ?
            var r = JsonConvert.DeserializeObject<UserObject>(re.Contents);
            return r;
        }

        public UserGuildObject[] GetCurrentUserGuilds(GetCurrentUserGuildsParams pars)
        {
            var q = "";
            if (pars.after != null || pars.before != null || pars.limit != null)
            {
                q += "?";
                if (pars.after != null)
                    q  += "after=" + pars.after;
                else if (pars.before != null)
                    q += "before=" + pars.before;
                if (pars.limit != null)
                    q += "limit=" + pars.limit;
            }
            
            var re = _request.GetRequestAsync("/users/@me/guilds" + q).Result;
            if (re.Code != 200)
                return null; // handle these errors ?
            var r = JsonConvert.DeserializeObject<UserGuildObject[]>(re.Contents);
            return r;
        }

        public bool LeaveGuild(ulong guildId)
        {
            var re = _request.DeleteRequestAsync("users/@me/guilds/" + guildId).Result;
            if (re.Code == 204 || re.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public DMChannelObject[] GetUserDMs()
        {
            var re = _request.GetRequestAsync("/users/@me/channels").Result;
            if (re.Code != 200)
                return null; // handle these errors ?
            var r = JsonConvert.DeserializeObject<DMChannelObject[]>(re.Contents);
            return r;
        }

        public DMChannelObject CreateDM(CreateDMParams pars)
        {
            var re = _request.PostRequestAsync("/users/@me/channels", pars).Result;
            if (re.Code != 200)
                return null; // handle these errors ?
            var r = JsonConvert.DeserializeObject<DMChannelObject>(re.Contents);
            return r;
        }

        public DMChannelObject CreateGroupDM(CreateGroupDMParams pars)
        {
            var re = _request.PostRequestAsync("/users/@me/channels", pars).Result;
            if (re.Code != 200)
                return null; // handle these errors ?
            var r = JsonConvert.DeserializeObject<DMChannelObject>(re.Contents);
            return r;
        }

        public ConnectionObject[] GetUsersConnections()
        {
            var re = _request.GetRequestAsync("/users/@me/connections").Result;
            if (re.Code != 200)
                return null; // handle these errors ?
            var r = JsonConvert.DeserializeObject<ConnectionObject[]>(re.Contents);
            return r;
        }

        public User(ResourceRequest request) : base(request)
        {
        }
    }
}