using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ZurvanBot.Discord.Gateway.Events;
using ZurvanBot.Discord.Resources.Objects;
using ZurvanBot.Util;

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
        public delegate void MessageUpdateEvent(MessageUpdateEventArgs e);
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
        
        public event ReadyEvent OnReady;
        public event ChannelCreateEvent OnChannelCreate;
        public event ChannelDeleteEvent OnChannelDelete;
        public event ChannelUpdateEvent OnChannelUpdate;
        public event GuildBanAddEvent OnGuildBanAdd;
        public event GuildBanRemoveEvent OnGuildBanRemove;
        public event GuildCreateEvent OnGuildCreate;
        public event GuildDeleteEvent OnGuildDelete;
        public event GuildEmojisUpdateEvent OnGuildEmojisUpdate;
        public event GuildIntegrationsUpdateEvent OnGuildIntegrationsUpdate;
        public event GuildMemberAddEvent OnGuildMemberAdd;
        public event GuildMemberRemoveEvent OnGuildMemberRemove;
        public event GuildMembersChunkEvent OnGuildMembersChunk;
        public event GuildMemberUpdateEvent OnGuildMemberUpdate;
        public event GuildRoleCreateEvent OnGuildRoleCreate;
        public event GuildRoleDeleteEvent OnGuildRoleDelete;
        public event GuildRoleUpdateEvent OnGuildRoleUpdate;
        public event GuildUpdateEvent OnGuildUpdate;
        public event MessageCreateEvent OnMessageCreate;
        public event MessageUpdateEvent OnMessageUpdate;
        public event MessageDeleteBulkEvent OnMessageDeleteBulk;
        public event MessageDeleteEvent OnMessageDelete;
        public event MessageReactionAddEvent OnMessageReactionAdd;
        public event MessageReactionRemoveAllEvent OnMessageReactionRemoveAll;
        public event MessageReactionRemoveEvent OnMessageReactionRemove;
        public event PresenceUpdateEvent OnPresenceUpdate;
        public event ResumedEvent OnResumed;
        public event TypingStartEvent OnTypingStart;
        public event UserUpdateEvent OnUserUpdate;
        public event VoiceServerUpdateEvent OnVoiceServerUpdate;
        public event VoiceStateUpdateEvent OnVoiceStateUpdate;
        
        /// <summary>
        /// Processes an event and dispatches it to all listeners.
        /// </summary>
        /// <param name="eventStr">The even identifier string.</param>
        /// <param name="eventData">The event's data.</param>
        private async void ProcessEvent(string eventStr, JObject eventData)
        {
            if (eventStr.Equals("READY"))
            {
                await _heart.StartAsync();
                
                Monitor.Enter(_readyWait);
                try
                {
                    _isReady = true;
                    Monitor.PulseAll(_readyWait);
                }
                finally
                {
                    Monitor.Exit(_readyWait);
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
                var et = new Task(() => OnReady?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("RESUMED"))
            {
                _isReady = true;
                var args = new ResumedEventArgs { _trace = eventData["d"]["_trace"].ToObject<string[]>() };
                var et = new Task(() => OnResumed?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("CHANNEL_CREATE"))
            {
                var isDM = (bool) eventData["d"]["is_private"]; // is_private is only true when DM and only false when Guild
                var args = new ChannelCreateEventArgs{ Type = isDM ? ChannelType.DM : ChannelType.Guild };
                if (isDM) args.DMChannel = eventData["d"].ToObject<DMChannelObject>();
                else args.GuildChannel = eventData["d"].ToObject<GuildChannelObject>();
                var et = new Task(() => OnChannelCreate?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("CHANNEL_UPDATE"))
            {
                var args = new ChannelUpdateEventArgs { Guild = eventData["d"].ToObject<GuildChannelObject>() };
                var et = new Task(() => OnChannelUpdate?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("CHANNEL_DELETE"))
            {
                var isDM = (bool) eventData["d"]["is_private"];
                var args = new ChannelDeleteEventArgs{ Type = isDM ? ChannelType.DM : ChannelType.Guild };
                if (isDM) args.DMChannel = eventData["d"].ToObject<DMChannelObject>();
                else args.GuildChannel = eventData["d"].ToObject<GuildChannelObject>();
                var et = new Task(() => OnChannelDelete?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_CREATE"))
            {
                var args = new GuildCreateEventArgs { Guild = eventData["d"].ToObject<GuildObject>() };
                var et = new Task(() => OnGuildCreate?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_UPDATE"))
            {
                var args = new GuildUpdateEventArgs { Guild = eventData["d"].ToObject<GuildObject>() };
                var et = new Task(() => OnGuildUpdate?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_DELETE"))
            {
                var args = new GuildDeleteEventArgs
                {
                    Id = (ulong)eventData["d"]["id"],
                    Unavailable = (bool)eventData["d"]["id"]
                };
                var et = new Task(() => OnGuildDelete?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_BAN_ADD"))
            {
                var args = new GuildBanAddEventArgs
                {
                    User = eventData["d"].ToObject<UserObject>(),
                    GuildId = (ulong)eventData["d"]["guild_id"]
                };
                var et = new Task(() => OnGuildBanAdd?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_BAN_REMOVE"))
            {
                var args = new GuildBanRemoveEventArgs
                {
                    User = eventData["d"].ToObject<UserObject>(),
                    GuildId = (ulong)eventData["d"]["guild_id"]
                };
                var et = new Task(() => OnGuildBanRemove?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_EMOJIS_UPDATE"))
            {
                var args = new GuildEmojisUpdateEventArgs
                {
                    GuildId = (ulong)eventData["d"]["guild_id"],
                    Emojis = eventData["d"]["emojis"].ToObject<EmojiObject[]>()
                };
                var et = new Task(() => OnGuildEmojisUpdate?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_INTEGRATIONS_UPDATE"))
            {
                var args = new GuildIntegrationsUpdateEventArgs{ GuildId = (ulong)eventData["d"]["guild_id"] };
                var et = new Task(() => OnGuildIntegrationsUpdate?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_MEMBER_ADD"))
            {
                var args = new GuildMemberAddEventArgs
                {
                    GuildMember = eventData["d"].ToObject<GuildMemberObject>(),
                    GuildId = (ulong)eventData["d"]["guild_id"]
                };
                var et = new Task(() => OnGuildMemberAdd?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_MEMBER_REMOVE"))
            {
                var args = new GuildMemberRemoveEventArgs
                {
                    GuildId = (ulong)eventData["d"]["guild_id"],
                    User = eventData["d"]["user"].ToObject<UserObject>()
                };
                var et = new Task(() => OnGuildMemberRemove?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
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
                var et = new Task(() => OnGuildMemberUpdate?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_MEMBERS_CHUNK"))
            {
                var args = new GuildMembersChunkEventArgs
                {
                    GuildId = (ulong) eventData["d"]["guild_id"],
                    Members = eventData["d"]["members"].ToObject<GuildMemberObject[]>()
                };
                var et = new Task(() => OnGuildMembersChunk?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_ROLE_CREATE"))
            {
                var args = new GuildRoleCreateEventArgs
                {
                    GuildId = (ulong) eventData["d"]["guild_id"],
                    Role = eventData["d"]["role"].ToObject<RoleObject>()
                };
                var et = new Task(() => OnGuildRoleCreate?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_ROLE_UPDATE"))
            {
                var args = new GuildRoleUpdateEventArgs
                {
                    GuildId = (ulong) eventData["d"]["guild_id"],
                    Role = eventData["d"]["role"].ToObject<RoleObject>()
                };
                var et = new Task(() => OnGuildRoleUpdate?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("GUILD_ROLE_DELETE"))
            {
                var args = new GuildRoleDeleteEventArgs
                {
                    GuildId = (ulong) eventData["d"]["guild_id"],
                    RoleId = (ulong) eventData["d"]["role_id"],
                };
                var et = new Task(() => OnGuildRoleDelete?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("MESSAGE_CREATE"))
            {
                var args = new MessageCreateEventArgs { Message = eventData["d"].ToObject<MessageObject>() };
                var et = new Task(() => OnMessageCreate?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("MESSAGE_UPDATE"))
            {
                var args = new MessageUpdateEventArgs { Message = eventData["d"].ToObject<MessageObject>() };
                var et = new Task(() => OnMessageUpdate?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("MESSAGE_DELETE"))
            {
                var args = new MessageDeleteEventArgs
                {
                    Id = (ulong)eventData["d"]["id"],
                    ChannelId = (ulong)eventData["d"]["channel_id"]
                };
                var et = new Task(() => OnMessageDelete?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("MESSAGE_DELETE_BULK"))
            {
                var args = new MessageDeleteBulkEventArgs
                {
                    Ids = eventData["d"]["ids"].ToObject<ulong[]>(),
                    ChannelId = (ulong)eventData["d"]["channel_id"]
                };
                var et = new Task(() => OnMessageDeleteBulk?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("MESSAGE_REACTION_ADD"))
            {
                var args = new MessageReactionAddEventArgs
                {
                    UserId = (ulong)eventData["d"]["user_id"],
                    ChannelId = (ulong)eventData["d"]["channel_id"],
                    MessageId = (ulong)eventData["d"]["message_id"],
                    Emoji = eventData["d"]["emoji"].ToObject<EmojiObject>()
                };
                var et = new Task(() => OnMessageReactionAdd?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("MESSAGE_REACTION_REMOVE"))
            {
                var args = new MessageReactionRemoveEventArgs
                {
                    UserId = (ulong)eventData["d"]["user_id"],
                    ChannelId = (ulong)eventData["d"]["channel_id"],
                    MessageId = (ulong)eventData["d"]["message_id"],
                    Emoji = eventData["d"]["emoji"].ToObject<EmojiObject>()
                };
                var et = new Task(() => OnMessageReactionRemove?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("MESSAGE_REACTION_REMOVE_ALL"))
            {
                var args = new MessageReactionRemoveAllEventArgs
                {
                    ChannelId = (ulong)eventData["d"]["channel_id"],
                    MessageId = (ulong)eventData["d"]["message_id"]
                };
                var et = new Task(() => OnMessageReactionRemoveAll?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("PRESENCE_UPDATE"))
            {
                var args = new PresenceUpdateEventArgs
                {
                    User = eventData["d"]["user"].ToObject<UserObject>(),
                    Roles = eventData["d"]["roles"].ToObject<ulong[]>(),
                    Game = eventData["d"]["game"].ToObject<GameObject>(),
                    GuildId = (ulong)eventData["d"]["guild_id"],
                    Status = (string)eventData["d"]["status"]
                };
                var et = new Task(() => OnPresenceUpdate?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("TYPING_START"))
            {
                var args = new TypingStartEventArgs
                {
                    ChannelId = (ulong)eventData["d"]["channel_id"],
                    UserId = (ulong)eventData["d"]["user_id"],
                    Timestamp = (int)eventData["d"]["timestamp"]
                };
                var et = new Task(() => OnTypingStart?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("USER_UPDATE"))
            {
                var args = new UserUpdateEventArgs { User = eventData["d"].ToObject<UserObject>() };
                var et = new Task(() => OnUserUpdate?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("VOICE_STATE_UPDATE"))
            {
                var args = new VoiceStateUpdateEventArgs { VoiceState = eventData["d"].ToObject<VoiceStateObject>() };
                var et = new Task(() => OnVoiceStateUpdate?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
            else if (eventStr.Equals("VOICE_SERVER_UPDATE"))
            {
                var args = new VoiceServerUpdateEventArgs
                {
                    Token = (string)eventData["d"]["token"],
                    GuildId = (ulong)eventData["d"]["guild_id"],
                    Endpoint = (string)eventData["d"]["endpoint"]
                };
                var et = new Task(() => OnVoiceServerUpdate?.Invoke(args));et.Start();if (!AsyncEvents) et.Wait();
            }
        }
    }
}