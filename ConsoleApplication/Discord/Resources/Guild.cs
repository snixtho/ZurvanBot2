using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using ZurvanBot.Discord.Resources.Objects;
using ZurvanBot.Discord.Resources.Params;

namespace ZurvanBot.Discord.Resources
{
    public class Guild : Resource
    {
        public GuildObject CreateGuild(CreateGuildParams pars)
        {
            var response = _request.PostRequest("/guilds", pars);
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<GuildObject>(response.Contents);
            return guildObj;
        }

        public GuildObject GetGuild(ulong guildId)
        {
            var response = _request.GetRequest("/guilds/" + guildId);
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<GuildObject>(response.Contents);
            return guildObj;
        }

        public GuildObject ModifyGuild(ulong guildId, ModifyGuildObject pars)
        {
            var response = _request.PatchRequest("/guilds/" + guildId, pars);
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<GuildObject>(response.Contents);
            return guildObj;
        }

        public GuildObject DeleteGuild(ulong guildId)
        {
            var response = _request.DeleteRequest("/guilds/" + guildId);
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<GuildObject>(response.Contents);
            return guildObj;
        }

        public GuildChannelObject[] GetGuildChannel(ulong guildId)
        {
            var response = _request.GetRequest("/guilds/" + guildId + "/channels");
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<GuildChannelObject[]>(response.Contents);
            return guildObj;
        }

        public GuildChannelObject CreateGuildChannel(ulong guildId, CreateGuildChannelParams pars)
        {
            var response = _request.PostRequest("/guilds/" + guildId + "/channels", pars);
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<GuildChannelObject>(response.Contents);
            return guildObj;
        }

        public GuildChannelObject[] ModifyGuildChannelPositions(ulong guildId, ModifyGuildChannelPositionsObject pars)
        {
            var response = _request.PatchRequest("/guilds/" + guildId + "/channels", pars);
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<GuildChannelObject[]>(response.Contents);
            return guildObj;
        }

        public GuildMemberObject GetGuildMember(ulong guildId, ulong userId)
        {
            var response = _request.GetRequest("/guilds/" + guildId + "/members/" + userId);
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<GuildMemberObject>(response.Contents);
            return guildObj;
        }

        public GuildMemberObject[] ListGuildMembers(ulong guildId, ListGuildMembersParams pars)
        {
            var query = "";
            if (pars.after != null || pars.limit != null)
            {
                query += "?";
                if (pars.after != null)
                    query += "after=" + pars.after;
                if (pars.limit != null)
                    query += "limit=" + pars.limit;
            }
            
            var response = _request.GetRequest("/guilds/" + guildId + "/members" + query);
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<GuildMemberObject[]>(response.Contents);
            return guildObj;
        }

        public GuildMemberObject AddGuildMember(ulong guildId, ulong userId, AddGuildMemberParams pars)
        {
            var response = _request.PutRequest("/guilds/" + guildId + "/members/" + userId, pars);
            if (response.Code != 201)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<GuildMemberObject>(response.Contents);
            return guildObj;
        }

        public bool ModifyGuildMember(ulong guildId, ulong userId, ModifyGuildMemberParams pars)
        {
            var response = _request.PatchRequest("/guilds/" + guildId + "/members/" + userId, pars);
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public string ModifyCurrentUserNick(ulong guildId, ModifyCurrentUserNickParams pars)
        {
            var response = _request.PatchRequest("/guilds/" + guildId + "/members/@me/nick", pars);
            if (response.Code != 200)
                return null; // handle these errors ?
            return response.Contents;
        }

        public bool AddGuildMemberRole(ulong guildId, ulong userId, ulong roleId)
        {
            var response = _request.PutRequest("/guilds/" + guildId + "/members/" + userId + "/roles/" + roleId);
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public bool RemoveGuildMemberRole(ulong guildId, ulong userId, ulong roleId)
        {
            var response = _request.DeleteRequest("/guilds/" + guildId + "/members/" + userId + "/roles/" + roleId);
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public bool RemoveGuildMember(ulong guildId, ulong userId)
        {
            var response = _request.DeleteRequest("/guilds/" + guildId + "/members/" + userId);
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public UserObject[] GetGuildBans(ulong guildId)
        {
            var response = _request.GetRequest("/guilds/" + guildId + "/bans");
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<UserObject[]>(response.Contents);
            return guildObj;
        }

        public bool CreateGuildBan(ulong guildId, ulong userId)
        {
            var response = _request.PutRequest("/guilds/" + guildId + "/bans/" + userId);
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public bool RemoveGuildBan(ulong guildId, ulong userId)
        {
            var response = _request.DeleteRequest("/guilds/" + guildId + "/bans/" + userId);
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public RoleObject[] GetGuildRoles(ulong guildId)
        {
            var response = _request.GetRequest("/guilds/" + guildId + "/roles");
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<RoleObject[]>(response.Contents);
            return guildObj;
        }

        public RoleObject CreateGuildRole(ulong guildId, CreateGuildRoleParams pars)
        {
            var response = _request.PostRequest("/guilds/" + guildId + "/roles", pars);
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<RoleObject>(response.Contents);
            return guildObj;
        }

        public RoleObject[] ModifyGuildRolePositions(ulong guildId, ModifyGuildRolePositionsParams pars)
        {
            var response = _request.PatchRequest("/guilds/" + guildId + "/roles", pars);
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<RoleObject[]>(response.Contents);
            return guildObj;
        }

        public RoleObject ModifyGuildRole(ulong guildId, ulong roleId, ModifyGuildRoleParams pars)
        {
            var response = _request.PatchRequest("/guilds/" + guildId + "/roles/" + roleId, pars);
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<RoleObject>(response.Contents);
            return guildObj;
        }

        public bool DeleteGuildRole(ulong guildId, ulong roleId)
        {
            var response = _request.DeleteRequest("/guilds/" + guildId + "/roles/" + roleId);
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public PruneCountObject GetGuildPruneCount(ulong guildId, GetGuildPruneCountParams pars)
        {
            var query = "";
            if (pars.days != null)
                query = "?days" + pars.days;
            
            var response = _request.GetRequest("/guilds/" + guildId + "/prune/" + query);
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<PruneCountObject>(response.Contents);
            return guildObj;
        }

        public PruneCountObject BeginGuildPrune(ulong guildId, BeginGuildPruneParams pars)
        {
            var query = "";
            if (pars.days != null)
                query = "?days" + pars.days;
            
            var response = _request.PostRequest("/guilds/" + guildId + "/prune/" + query);
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<PruneCountObject>(response.Contents);
            return guildObj;
        }

        public VoiceRegionObject[] GetGuildVoiceRegions(ulong guildId)
        {
            var response = _request.GetRequest("/guilds/" + guildId + "/regions");
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<VoiceRegionObject[]>(response.Contents);
            return guildObj;
        }

        public InviteWithMetadataObject[] GetGuildInvites(ulong guildId)
        {
            var response = _request.GetRequest("/guilds/" + guildId + "/invites");
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<InviteWithMetadataObject[]>(response.Contents);
            return guildObj;
        }

        public IntegrationObject[] GetGuildIntegrations(ulong guildId)
        {
            var response = _request.GetRequest("/guilds/" + guildId + "/integrations");
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<IntegrationObject[]>(response.Contents);
            return guildObj;
        }

        public bool CreateGuildIntegration(ulong guildId, CreateGuildIntegrationParams pars)
        {
            var response = _request.PostRequest("/guilds/" + guildId + "/integrations", pars);
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public bool ModifyGuildIntegration(ulong guildId, ulong integrationId, ModifyGuildIntegrationParams pars)
        {
            var response = _request.PatchRequest("/guilds/" + guildId + "/integrations/" + integrationId, pars);
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public bool DeleteGuildIntegration(ulong guildId, ulong integrationId)
        {
            var response = _request.DeleteRequest("/guilds/" + guildId + "/integrations/" + integrationId);
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public bool SyncGuildIntegration(ulong guildId, ulong integrationId)
        {
            var response = _request.PostRequest("/guilds/" + guildId + "/integrations/" + integrationId + "/sync");
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public GuildEmbedObject GetGuildEmbet(ulong guildId)
        {
            var response = _request.GetRequest("/guilds/" + guildId + "/embed");
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<GuildEmbedObject>(response.Contents);
            return guildObj;
        }

        public GuildEmbedObject ModifyGuildEmbet(ulong guildId, ModifyGuildEmbedParams pars)
        {
            var response = _request.PatchRequest("/guilds/" + guildId + "/embed", pars);
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<GuildEmbedObject>(response.Contents);
            return guildObj;
        }

        public Guild(ResourceRequest request) : base(request)
        {
        }
    }
}
