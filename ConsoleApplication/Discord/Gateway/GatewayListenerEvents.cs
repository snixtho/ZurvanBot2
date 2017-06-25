using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ZurvanBot.Discord.Gateway.Events;
using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway
{
    public partial class GatewayListener
    {
        public delegate void ReadyEvent(ReadyEventArgs e);
        public delegate void ChannelCreateEvent(ChannelCreateEventArgs e);
        public delegate void ChannelDeleteEvent(ChannelDeleteEventArgs e);
        public delegate void ChannelUpdateEvent(ChannelUpdateEventArgs e);
        public delegate void GuildBanAddEvent(GuildBanAddEventArgs e);
        public delegate void GuildBanRemoveEvent(GuildBanRemoveEventArgs e);
        public delegate void GuildCreateEvent(GuildCreateEventArgs e);
        public delegate void GuildDeleteEvent(GuildDeleteEventArgs e);
        public delegate void GuildEmojisUpdateEvent(GuildEmojisUpdateEventArgs e);
        public delegate void GuildIntegrationsUpdateEvent(GuildIntegrationsUpdateEventArgs e);
        public delegate void GuildMemberAddEvent(GuildMemberAddEventArgs e);
        public delegate void GuildMemberRemoveEvent(GuildMemberRemoveEventArgs e);
        public delegate void GuildMembersChunkEvent(GuildMembersChunkEventArgs e);
        public delegate void GuildMemberUpdateEvent(GuildMemberUpdateEventArgs e);
        public delegate void GuildRoleCreateEvent(GuildRoleCreateEventArgs e);
        public delegate void GuildRoleDeleteEvent(GuildRoleDeleteEventArgs e);
        public delegate void GuildRoleUpdateEvent(GuildRoleUpdateEventArgs e);
        public delegate void GuildUpdateEvent(GuildUpdateEventArgs e);
        public delegate void MessageCreateEvent(MessageCreateEventArgs e);
        public delegate void MessageDeleteBulkEvent(MessageDeleteBulkEventArgs e);
        public delegate void MessageDeleteEvent(MessageDeleteEventArgs e);
        public delegate void MessageReactionAddEvent(MessageReactionAddEventArgs e);
        public delegate void MessageReactionRemoveAllEvent(MessageReactionRemoveAllEventArgs e);
        public delegate void MessageReactionRemoveEvent(MessageReactionRemoveEventArgs e);
        public delegate void PresenceUpdateEvent(PresenceUpdateEventArgs e);
        public delegate void ResumedEvent(ResumedEventArgs e);
        public delegate void TypingStartEvent(TypingStartEventArgs e);
        public delegate void UserUpdateEvent(UserUpdateEventArgs e);
        public delegate void VoiceServerUpdateEvent(VoiceServerUpdateEventArgs e);
        public delegate void VoiceStateUpdateEvent(VoiceStateUpdateEventArgs e);
        
        public event ReadyEvent OnReadyEvent;
        public event ChannelCreateEvent OnChannelCreateEvent;
        public event ChannelDeleteEvent OnChannelDeleteEvent;
        public event ChannelUpdateEvent OnChannelUpdateEvent;
        public event GuildBanAddEvent OnGuildBanAddEvent;
        public event GuildBanRemoveEvent OnGuildBanRemoveEvent;
        public event GuildCreateEvent OnGuildCreateEvent;
        public event GuildDeleteEvent OnGuildDeleteEvent;
        public event GuildEmojisUpdateEvent OnGuildEmojisUpdateEvent;
        public event GuildIntegrationsUpdateEvent OnGuildIntegrationsUpdateEvent;
        public event GuildMemberAddEvent OnGuildMemberAddEvent;
        public event GuildMemberRemoveEvent OnGuildMemberRemoveEvent;
        public event GuildMembersChunkEvent OnGuildMembersChunkEvent;
        public event GuildMemberUpdateEvent OnGuildMemberUpdateEvent;
        public event GuildRoleCreateEvent OnGuildRoleCreateEvent;
        public event GuildRoleDeleteEvent OnGuildRoleDeleteEvent;
        public event GuildRoleUpdateEvent OnGuildRoleUpdateEvent;
        public event GuildUpdateEvent OnGuildUpdateEvent;
        public event MessageCreateEvent OnMessageCreateEvent;
        public event MessageDeleteBulkEvent OnMessageDeleteBulkEvent;
        public event MessageDeleteEvent OnMessageDeleteEvent;
        public event MessageReactionAddEvent OnMessageReactionAddEvent;
        public event MessageReactionRemoveAllEvent OnMessageReactionRemoveAllEvent;
        public event MessageReactionRemoveEvent OnMessageReactionRemoveEvent;
        public event PresenceUpdateEvent OnPresenceUpdateEvent;
        public event ResumedEvent OnResumedEvent;
        public event TypingStartEvent OnTypingStartEvent;
        public event UserUpdateEvent OnUserUpdateEvent;
        public event VoiceServerUpdateEvent OnVoiceServerUpdateEvent;
        public event VoiceStateUpdateEvent OnVoiceStateUpdateEvent;
        
        private void DispatchEvent(string eventStr, JObject eventData)
        {
            if (eventStr.Equals("READY"))
            {
                if (eventStr.ToUpper().Equals("READY")) {
                    _heart.StartAsync();
                }

                var args = new ReadyEventArgs
                {
                    V = (int) eventData["d"]["v"],
                    User = eventData["d"]["user"].ToObject<UserObject>(),
                    PrivateChannels = eventData["d"]["private_channels"].ToObject<DMChannelObject[]>(),
                    Guilds = eventData["d"]["guilds"].ToObject<UnavailableGuildObject[]>(),
                    SessionID = (string) eventData["d"]["session_id"],
                    _trace = eventData["d"]["_trace"].ToObject<string[]>()
                };
                var et = new Task(() => OnReadyEvent?.Invoke(args));et.Start();if (AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("RESUMED"))
            {
                var args = new ResumedEventArgs { _trace = eventData["d"]["_trace"].ToObject<string[]>() };
                var et = new Task(() => OnResumedEvent?.Invoke(args));et.Start();if (AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("CHANNEL_CREATE"))
            {
                var isDM = (bool) eventData["d"]["is_private"]; // is_private is only true when DM and only false when Guild
                var args = new ChannelCreateEventArgs{ Type = isDM ? ChannelType.DM : ChannelType.Guild };
                if (isDM) args.DMChannel = eventData["d"].ToObject<DMChannelObject>();
                else args.GuildChannel = eventData["d"].ToObject<GuildChannelObject>();
                var et = new Task(() => OnChannelCreateEvent?.Invoke(args));et.Start();if (AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("CHANNEL_UPDATE"))
            {
                var args = new ChannelUpdateEventArgs { Guild = eventData["d"].ToObject<GuildChannelObject>() };
                var et = new Task(() => OnChannelUpdateEvent?.Invoke(args));et.Start();if (AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("CHANNEL_DELETE"))
            {
                var isDM = (bool) eventData["d"]["is_private"];
                var args = new ChannelDeleteEventArgs{ Type = isDM ? ChannelType.DM : ChannelType.Guild };
                if (isDM) args.DMChannel = eventData["d"].ToObject<DMChannelObject>();
                else args.GuildChannel = eventData["d"].ToObject<GuildChannelObject>();
                var et = new Task(() => OnChannelDeleteEvent?.Invoke(args));et.Start();if (AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_CREATE"))
            {
                var args = new GuildCreateEventArgs { Guild = eventData["d"].ToObject<GuildObject>() };
                var et = new Task(() => OnGuildCreateEvent?.Invoke(args));et.Start();if (AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_UPDATE"))
            {
                var args = new GuildUpdateEventArgs { Guild = eventData["d"].ToObject<GuildObject>() };
                var et = new Task(() => OnGuildUpdateEvent?.Invoke(args));et.Start();if (AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_DELETE"))
            {
                var args = new GuildDeleteEventArgs
                {
                    Id = (ulong)eventData["d"]["id"],
                    Unavailable = (bool)eventData["d"]["id"]
                };
                var et = new Task(() => OnGuildDeleteEvent?.Invoke(args));et.Start();if (AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_BAN_ADD"))
            {
                var args = new GuildBanAddEventArgs
                {
                    User = eventData["d"].ToObject<UserObject>(),
                    GuildId = (ulong)eventData["d"]["guild_id"]
                };
                var et = new Task(() => OnGuildBanAddEvent?.Invoke(args));et.Start();if (AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_BAN_REMOVE"))
            {
                var args = new GuildBanRemoveEventArgs
                {
                    User = eventData["d"].ToObject<UserObject>(),
                    GuildId = (ulong)eventData["d"]["guild_id"]
                };
                var et = new Task(() => OnGuildBanRemoveEvent?.Invoke(args));et.Start();if (AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_EMOJIS_UPDATE"))
            {
                var args = new GuildEmojisUpdateEventArgs
                {
                    GuildId = (ulong)eventData["d"]["guild_id"],
                    Emojis = eventData["d"]["emojis"].ToObject<EmojiObject[]>()
                };
                var et = new Task(() => OnGuildEmojisUpdateEvent?.Invoke(args));et.Start();if (AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_INTEGRATIONS_UPDATE"))
            {
                var args = new GuildIntegrationsUpdateEventArgs{ GuildId = (ulong)eventData["d"]["guild_id"] };
                var et = new Task(() => OnGuildIntegrationsUpdateEvent?.Invoke(args));et.Start();if (AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_MEMBER_ADD"))
            {
                var args = new GuildMemberAddEventArgs{ GuildId = (ulong)eventData["d"]["guild_id"] };
                var et = new Task(() => OnGuildMemberAddEvent?.Invoke(args));et.Start();if (AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_MEMBER_REMOVE"))
            {
                var args = new GuildMemberRemoveEventArgs
                {
                    GuildId = (ulong)eventData["d"]["guild_id"],
                    User = eventData["d"]["user"].ToObject<UserObject>()
                };
                var et = new Task(() => OnGuildMemberRemoveEvent?.Invoke(args));et.Start();if (AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_MEMBER_UPDATE"))
            {
                var args = new GuildMemberUpdateEventArgs
                {
                    GuildId = (ulong) eventData["d"]["guild_id"],
                    User = eventData["d"]["user"].ToObject<UserObject>(),
                    Roles = eventData["d"]["roles"].ToObject<ulong[]>(),
                    Nick = (string) eventData["d"]["nick"]
                };
                var et = new Task(() => OnGuildMemberUpdateEvent?.Invoke(args));et.Start();if (AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_MEMBERS_CHUNK"))
            {
                
            }
            else if (eventStr.Equals("GUILD_ROLE_CREATE")) {}
            else if (eventStr.Equals("GUILD_ROLE_UPDATE")) {}
            else if (eventStr.Equals("GUILD_ROLE_DELETE")) {}
            else if (eventStr.Equals("MESSAGE_CREATE")) {}
            else if (eventStr.Equals("MESSAGE_UPDATE")) {}
            else if (eventStr.Equals("MESSAGE_DELETE")) {}
            else if (eventStr.Equals("MESSAGE_DELETE_BULK")) {}
            else if (eventStr.Equals("MESSAGE_REACTION_ADD")) {}
            else if (eventStr.Equals("MESSAGE_REACTION_REMOVE")) {}
            else if (eventStr.Equals("MESSAGE_REACTION_REMOVE_ALL")) {}
            else if (eventStr.Equals("PRESENCE_UPDATE")) {}
            else if (eventStr.Equals("TYPING_START")) {}
            else if (eventStr.Equals("USER_UPDATE")) {}
            else if (eventStr.Equals("VOICE_STATE_UPDATE")) {}
            else if (eventStr.Equals("VOICE_SERVER_UPDATE")) {}
        }
    }
}