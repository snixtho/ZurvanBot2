using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ZurvanBot.Discord.Gateway.Events;
using ZurvanBot.Discord.Resources.Objects;
using ZurvanBot.Util;

namespace ZurvanBot.Discord.Gateway {
    public partial class GatewayListener {
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
        private async void ProcessEvent(string eventStr, JObject eventData) {
            var handlerTask = new GatewayEvent(eventStr, eventData)
                .OnEvent("READY", token => {
                    await _heart.StartAsync();

                    Monitor.Enter(_readyWait);
                    try {
                        _isReady = true;
                        Monitor.PulseAll(_readyWait);
                    }
                    finally {
                        Monitor.Exit(_readyWait);
                    }

                    var args = new ReadyEventArgs {
                        V = (int) eventData["v"],
                        User = eventData["user"].ToObject<UserObject>(),
                        PrivateChannels = eventData["private_channels"].ToObject<DMChannelObject[]>(),
                        Guilds = eventData["guilds"].ToObject<UnavailableGuildObject[]>(),
                        SessionID = (string) eventData["session_id"],
                        _trace = eventData["_trace"].ToObject<string[]>()
                    };

                    OnReady?.Invoke(args);
                })
                .OnEvent("RESUMED", token => {
                    _isReady = true;
                    var args = new ResumedEventArgs {_trace = eventData["_trace"].ToObject<string[]>()};
                    OnResumed?.Invoke(args);
                })
                .OnEvent("CHANNEL_CREATE", token => {
                    var isDM = (bool) eventData[
                        "is_private"]; // is_private is only true when DM and only false when Guild
                    var args = new ChannelCreateEventArgs {Type = isDM ? ChannelType.DM : ChannelType.Guild};
                    if (isDM) args.DMChannel = eventData.ToObject<DMChannelObject>();
                    else args.GuildChannel = eventData.ToObject<GuildChannelObject>();
                    OnChannelCreate?.Invoke(args);
                })
                .OnEvent("CHANNEL_UPDATE", token => {
                    var args = new ChannelUpdateEventArgs {Guild = eventData.ToObject<GuildChannelObject>()};
                    OnChannelUpdate?.Invoke(args);
                })
                .OnEvent("CHANNEL_DELETE", token => {
                    var isDM = (bool) eventData["is_private"];
                    var args = new ChannelDeleteEventArgs {Type = isDM ? ChannelType.DM : ChannelType.Guild};
                    if (isDM) args.DMChannel = eventData.ToObject<DMChannelObject>();
                    else args.GuildChannel = eventData.ToObject<GuildChannelObject>();
                    OnChannelDelete?.Invoke(args);
                })
                .OnEvent("GUILD_CREATE", token => {
                    var args = new GuildCreateEventArgs {Guild = eventData.ToObject<GuildObject>()};
                    OnGuildCreate?.Invoke(args);
                })
                .OnEvent("GUILD_UPDATE", token => {
                    var args = new GuildUpdateEventArgs {Guild = eventData.ToObject<GuildObject>()};
                    OnGuildUpdate?.Invoke(args);
                })
                .OnEvent("GUILD_DELETE", token => {
                    var args = new GuildDeleteEventArgs {
                        Id = (ulong) eventData["id"],
                        Unavailable = (bool) eventData["id"]
                    };
                    OnGuildDelete?.Invoke(args);
                })
                .OnEvent("GUILD_BAN_ADD", token => {
                    var args = new GuildBanAddEventArgs {
                        User = eventData.ToObject<UserObject>(),
                        GuildId = (ulong) eventData["guild_id"]
                    };
                    OnGuildBanAdd?.Invoke(args);
                })
                .OnEvent("GUILD_BAN_REMOVE", token => {
                    var args = new GuildBanRemoveEventArgs {
                        User = eventData.ToObject<UserObject>(),
                        GuildId = (ulong) eventData["guild_id"]
                    };
                    OnGuildBanRemove?.Invoke(args);
                })
                .OnEvent("GUILD_EMOJIS_UPDATE", token => {
                    var args = new GuildEmojisUpdateEventArgs {
                        GuildId = (ulong) eventData["guild_id"],
                        Emojis = eventData["emojis"].ToObject<EmojiObject[]>()
                    };
                    OnGuildEmojisUpdate?.Invoke(args);
                })
                .OnEvent("GUILD_INTEGRATIONS_UPDATE", token => {
                    var args = new GuildIntegrationsUpdateEventArgs {GuildId = (ulong) eventData["guild_id"]};
                    OnGuildIntegrationsUpdate?.Invoke(args);
                })
                .OnEvent("GUILD_MEMBER_ADD", token => {
                    var args = new GuildMemberAddEventArgs {
                        GuildMember = eventData.ToObject<GuildMemberObject>(),
                        GuildId = (ulong) eventData["guild_id"]
                    };
                    OnGuildMemberAdd?.Invoke(args);
                })
                .OnEvent("GUILD_MEMBER_REMOVE", token => {
                    var args = new GuildMemberRemoveEventArgs {
                        GuildId = (ulong) eventData["guild_id"],
                        User = eventData["user"].ToObject<UserObject>()
                    };
                    OnGuildMemberRemove?.Invoke(args);
                })
                .OnEvent("GUILD_MEMBER_UPDATE", token => {
                    var args = new GuildMemberUpdateEventArgs {
                        GuildId = (ulong) eventData["guild_id"],
                        User = eventData["user"].ToObject<UserObject>(),
                        Roles = eventData["roles"].ToObject<ulong[]>(),
                        Nick = (string) eventData["nick"]
                    };
                    OnGuildMemberUpdate?.Invoke(args);
                })
                .OnEvent("GUILD_MEMBERS_CHUNK", token => {
                    var args = new GuildMembersChunkEventArgs {
                        GuildId = (ulong) eventData["guild_id"],
                        Members = eventData["members"].ToObject<GuildMemberObject[]>()
                    };
                    OnGuildMembersChunk?.Invoke(args);
                })
                .OnEvent("GUILD_ROLE_CREATE", token => {
                    var args = new GuildRoleCreateEventArgs {
                        GuildId = (ulong) eventData["guild_id"],
                        Role = eventData["role"].ToObject<RoleObject>()
                    };
                    OnGuildRoleCreate?.Invoke(args);
                })
                .OnEvent("GUILD_ROLE_UPDATE", token => {
                    var args = new GuildRoleUpdateEventArgs {
                        GuildId = (ulong) eventData["guild_id"],
                        Role = eventData["role"].ToObject<RoleObject>()
                    };
                    OnGuildRoleUpdate?.Invoke(args);
                })
                .OnEvent("GUILD_ROLE_DELETE", token => {
                    var args = new GuildRoleDeleteEventArgs {
                        GuildId = (ulong) eventData["guild_id"],
                        RoleId = (ulong) eventData["role_id"]
                    };
                    OnGuildRoleDelete?.Invoke(args);
                })
                .OnEvent("MESSAGE_CREATE", token => {
                    var args = new MessageCreateEventArgs {Message = eventData.ToObject<MessageObject>()};
                    OnMessageCreate?.Invoke(args);
                })
                .OnEvent("MESSAGE_UPDATE", token => {
                    var args = new MessageUpdateEventArgs {Message = eventData.ToObject<MessageObject>()};
                    OnMessageUpdate?.Invoke(args);
                })
                .OnEvent("MESSAGE_DELETE", token => {
                    var args = new MessageDeleteEventArgs {
                        Id = (ulong) eventData["id"],
                        ChannelId = (ulong) eventData["channel_id"]
                    };
                    OnMessageDelete?.Invoke(args);
                })
                .OnEvent("MESSAGE_DELETE_BULK", token => {
                    var args = new MessageDeleteBulkEventArgs {
                        Ids = eventData["ids"].ToObject<ulong[]>(),
                        ChannelId = (ulong) eventData["channel_id"]
                    };
                    OnMessageDeleteBulk?.Invoke(args);
                })
                .OnEvent("MESSAGE_REACTION_ADD", token => {
                    var args = new MessageReactionAddEventArgs {
                        UserId = (ulong) eventData["user_id"],
                        ChannelId = (ulong) eventData["channel_id"],
                        MessageId = (ulong) eventData["message_id"],
                        Emoji = eventData["emoji"].ToObject<EmojiObject>()
                    };
                    OnMessageReactionAdd?.Invoke(args);
                })
                .OnEvent("MESSAGE_REACTION_REMOVE", token => {
                    var args = new MessageReactionRemoveEventArgs {
                        UserId = (ulong) eventData["user_id"],
                        ChannelId = (ulong) eventData["channel_id"],
                        MessageId = (ulong) eventData["message_id"],
                        Emoji = eventData["emoji"].ToObject<EmojiObject>()
                    };
                    OnMessageReactionRemove?.Invoke(args);
                })
                .OnEvent("MESSAGE_REACTION_REMOVE_ALL", token => {
                    var args = new MessageReactionRemoveAllEventArgs {
                        ChannelId = (ulong) eventData["channel_id"],
                        MessageId = (ulong) eventData["message_id"]
                    };
                    OnMessageReactionRemoveAll?.Invoke(args);
                })
                .OnEvent("PRESENCE_UPDATE", token => {
                    var args = new PresenceUpdateEventArgs {
                        User = eventData["user"].ToObject<UserObject>(),
                        Roles = eventData["roles"].ToObject<ulong[]>(),
                        Game = eventData["game"].ToObject<GameObject>(),
                        GuildId = (ulong) eventData["guild_id"],
                        Status = (string) eventData["status"]
                    };
                    OnPresenceUpdate?.Invoke(args);
                })
                .OnEvent("TYPING_START", token => {
                    var args = new TypingStartEventArgs {
                        ChannelId = (ulong) eventData["channel_id"],
                        UserId = (ulong) eventData["user_id"],
                        Timestamp = (int) eventData["timestamp"]
                    };
                    OnTypingStart?.Invoke(args);
                })
                .OnEvent("USER_UPDATE", token => {
                    var args = new UserUpdateEventArgs {User = eventData["d"].ToObject<UserObject>()};
                    OnUserUpdate?.Invoke(args);
                })
                .OnEvent("VOICE_STATE_UPDATE", token => {
                    var args = new VoiceStateUpdateEventArgs {VoiceState = eventData.ToObject<VoiceStateObject>()};
                    OnVoiceStateUpdate?.Invoke(args);
                })
                .OnEvent("VOICE_SERVER_UPDATE", token => {
                    var args = new VoiceServerUpdateEventArgs {
                        Token = (string) eventData["token"],
                        GuildId = (ulong) eventData["guild_id"],
                        Endpoint = (string) eventData["endpoint"]
                    };
                    OnVoiceServerUpdate?.Invoke(args);
                }).Handle();

            if (!AsyncEvents)
                handlerTask.Wait();
        }
    }
}