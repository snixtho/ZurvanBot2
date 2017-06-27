using System;
using System.Collections.Generic;
using ZurvanBot.Discord.Gateway.Events;
using ZurvanBot.Discord.Resources;
using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.StateTracking
{
    public class StateTracker
    {
        public delegate void UserLoopCallback(PresenceObject user);
        public delegate void GuildLoopCallback(GuildObject guild);
        
        private Dictionary<ulong, GuildObject> _guilds;
        private Dictionary<ulong, Dictionary<ulong, PresenceObject>> _users;
        public object _theLock = new object();
        
        public Dictionary<ulong, GuildObject> Guilds => _guilds;
        public Dictionary<ulong, Dictionary<ulong, PresenceObject>> Users => _users;

        public StateTracker(GatewayListener listener)
        {
            _guilds = new Dictionary<ulong, GuildObject>();
            _users = new Dictionary<ulong, Dictionary<ulong, PresenceObject>>()
            
            listener.OnGuildCreate += ListenerOnOnGuildCreate;
            listener.OnGuildDelete += ListenerOnOnGuildDelete;
            listener.OnPresenceUpdate += ListenerOnOnPresenceUpdate;
        }

        private void updateUser(PresenceObject user)
        {
            if (_users.ContainsKey(user.id))
            {
                if (_users[user.id].ContainsKey(user.guild_id))
                    _users[user.id][user.guild_id] = user; // update presence
                else
                    _users[user.id].Add(user.guild_id, user); // add new user presence from a different guild
            }
            else
            { // add a completely new user
                _users.Add(user.id, new Dictionary<ulong, PresenceObject>());
                _users[user.id].Add(user.guild_id, user);
            }
        }

        private void ListenerOnOnPresenceUpdate(PresenceUpdateEventArgs e)
        {
            lock (_theLock)
            {
                if (_guilds.ContainsKey(e.GuildId))
                {
                    
                }
            }
        }

        private void ListenerOnOnGuildDelete(GuildDeleteEventArgs e)
        {
            lock (_theLock)
            {
                if (_guilds.ContainsKey(e.Id))
                    _guilds.Remove(e.Id);

                foreach (var userId in _users.Keys)
                {
                    var userList = _users[userId];
                    if (userList == null) continue;
                    foreach (var guildId in userList.Keys)
                    {
                        var user = userList[guildId];
                        if (user.guild_id == e.Id)
                            userList.Remove(guildId);
                    }

                    // cleanup if empty
                    if (userList.Count == 0)
                        _users.Remove(userId);
                }
            }
        }

        private void ListenerOnOnGuildCreate(GuildCreateEventArgs e)
        {
            lock (_theLock)
            {
                // Update if it already exists for some reason
                if (_guilds.ContainsKey(e.Guild.id))
                {
                    _guilds[e.Guild.id] = e.Guild;
                }
                else
                {
                    _guilds.Add(e.Guild.id, e.Guild);
                    foreach (var user in e.Guild.presences) 
                        updateUser(user);
                }
            }
        }
        
        /////////////

        /// <summary>
        /// Loop through all and every user from all guilds, including the same users from different guilds.
        /// </summary>
        /// <param name="callback">Callback to gather the current user in the iteration.</param>
        public void ForeachUser(UserLoopCallback callback)
        {
            foreach (var userList in _users.Values)
            foreach (var user in userList.Values)
                callback(user);
        }

        /// <summary>
        /// Loop through all users from one guild.
        /// </summary>
        /// <param name="guildId">The guild's id.</param>
        /// <param name="callback">Callback to gather the current user in the iteration.</param>
        public void ForeachUser(ulong guildId, UserLoopCallback callback)
        {
            foreach (var userList in _users.Values)
            {
                foreach (var user in userList.Values)
                {
                    if (user.guild_id == guildId)
                        callback(user);
                }
            }
        }

        /// <summary>
        /// Loop through all guilds.
        /// </summary>
        /// <param name="callback">Callback to gather the current guild in the iteration.</param>
        public void ForeachGuild(GuildLoopCallback callback)
        {
            foreach (var guild in _guilds.Values)
                callback(guild);
        }
    }
}