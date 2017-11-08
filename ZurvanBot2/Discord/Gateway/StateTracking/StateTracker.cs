using System;
using System.Collections.Generic;
using ZurvanBot.Discord.Gateway.Events;
using ZurvanBot.Discord.Resources;
using ZurvanBot.Discord.Resources.Objects;
using ZurvanBot.Util;

namespace ZurvanBot.Discord.Gateway.StateTracking {
    public class StateTracker {
        public delegate void UserLoopCallback(GuildMemberObject user, ulong guildId);

        public delegate void GuildLoopCallback(GuildObject guild);

        private Dictionary<ulong, GuildObject> _guilds;
        private Dictionary<ulong, Dictionary<ulong, GuildMemberObject>> _users;
        public object _theLock = new object();

        public Dictionary<ulong, GuildObject> Guilds => _guilds;
        public Dictionary<ulong, Dictionary<ulong, GuildMemberObject>> Users => _users;

        public StateTracker(GatewayListener listener) {
            _guilds = new Dictionary<ulong, GuildObject>();
            _users = new Dictionary<ulong, Dictionary<ulong, GuildMemberObject>>();

            listener.OnGuildCreate += ListenerOnOnGuildCreate;
            listener.OnGuildDelete += ListenerOnOnGuildDelete;
            listener.OnPresenceUpdate += ListenerOnOnPresenceUpdate;
        }

        private void updateUser(ulong guildId, GuildMemberObject user) {
            if (_users.ContainsKey(user.user.id)) {
                if (_users[user.user.id].ContainsKey(guildId)) {
                    _users[user.user.id][guildId] = user; // update presence
                }
                else {
                    _users[user.user.id].Add(guildId, user); // add new user presence from a different guild
                }
            }
            else {
                // add a completely new user
                _users.Add(user.user.id, new Dictionary<ulong, GuildMemberObject>());
                _users[user.user.id].Add(guildId, user);
            }
        }

        private void ListenerOnOnPresenceUpdate(PresenceUpdateEventArgs e) {
            lock (_theLock) {
                // todo: add proper UserState class and implement presence update tracking
            }
        }

        private void ListenerOnOnGuildDelete(GuildDeleteEventArgs e) {
            lock (_theLock) {
                if (_guilds.ContainsKey(e.Id))
                    _guilds.Remove(e.Id);

                foreach (var userId in _users.Keys) {
                    var userList = _users[userId];
                    if (userList == null) continue;
                    foreach (var guildId in userList.Keys) {
                        if (guildId == e.Id)
                            userList.Remove(guildId);
                    }

                    // cleanup if empty
                    if (userList.Count == 0)
                        _users.Remove(userId);
                }
            }
        }

        private void ListenerOnOnGuildCreate(GuildCreateEventArgs e) {
            lock (_theLock) {
                // Update if it already exists for some reason
                if (_guilds.ContainsKey(e.Guild.id)) {
                    _guilds[e.Guild.id] = e.Guild;
                }
                else {
                    _guilds.Add(e.Guild.id, e.Guild);
                    foreach (var user in e.Guild.members)
                        updateUser(e.Guild.id, user);
                }
            }
        }

        /////////////

        /// <summary>
        /// Loop through all and every user from all guilds, including the same users from different guilds.
        /// </summary>
        /// <param name="callback">Callback to gather the current user in the iteration.</param>
        public void ForEachUser(UserLoopCallback callback) {
            foreach (var userList in _users.Values)
            foreach (var userGuild in userList.Keys)
                callback(userList[userGuild], userGuild);
        }

        /// <summary>
        /// Loop through all users from one guild.
        /// </summary>
        /// <param name="guildId">The guild's id.</param>
        /// <param name="callback">Callback to gather the current user in the iteration.</param>
        public void ForEachUser(ulong guildId, UserLoopCallback callback) {
            foreach (var userList in _users.Values) {
                foreach (var userGuild in userList.Keys) {
                    var user = userList[userGuild];
                    if (userGuild == guildId)
                        callback(user, guildId);
                }
            }
        }

        /// <summary>
        /// Loop through all guilds.
        /// </summary>
        /// <param name="callback">Callback to gather the current guild in the iteration.</param>
        public void ForEachGuild(GuildLoopCallback callback) {
            foreach (var guild in _guilds.Values)
                callback(guild);
        }
    }
}