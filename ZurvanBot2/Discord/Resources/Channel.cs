using System;
using Newtonsoft.Json;
using ZurvanBot.Discord.Resources.Objects;
using ZurvanBot.Discord.Resources.Params;
using ZurvanBot.Util;

namespace ZurvanBot.Discord.Resources {
    public class Channel : Resource {
        public GuildChannelObject GetChannel(UInt64 channelId) {
            var response = _request.GetRequestAsync("/channels/" + channelId).Result;
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<GuildChannelObject>(response.Contents);
            return guildObj;
        }

        public GuildChannelObject ModifyChannel(UInt64 channelId, ModifyChannelParams pars) {
            var response = _request.PatchRequestAsync("/channels/" + channelId, pars).Result;
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<GuildChannelObject>(response.Contents);
            return guildObj;
        }

        public GuildChannelObject DeleteChannel(UInt64 channelId) {
            var response = _request.DeleteRequestAsync("/channels/" + channelId).Result;
            if (response.Code != 200)
                return null; // handle these errors ?
            var guildObj = JsonConvert.DeserializeObject<GuildChannelObject>(response.Contents);
            return guildObj;
        }

        public MessageObject[] GetChannelMessages(UInt64 channelId, GetChannelMessagesParams pars) {
            var query = "";
            if (pars.after != null || pars.around != null || pars.before != null || pars.limit != null) {
                query += "?";
                if (pars.after != null)
                    query += "after=" + pars.after;
                else if (pars.before != null)
                    query += "before=" + pars.before;
                else if (pars.around != null)
                    query += "around=" + pars.around;
                if (pars.limit != null)
                    query += "limit=" + pars.limit;
            }


            var response = _request.GetRequestAsync("/channels/" + channelId + "/messages" + query).Result;
            if (response.Code != 200)
                return null; // handle these errors ?
            var messages = JsonConvert.DeserializeObject<MessageObject[]>(response.Contents);
            return messages;
        }

        public MessageObject GetChannelMessage(UInt64 channelId, UInt64 messageId) {
            var response = _request.GetRequestAsync("/channels/" + channelId + "/messages/" + messageId).Result;
            if (response.Code != 200)
                return null; // handle these errors ?
            var messages = JsonConvert.DeserializeObject<MessageObject>(response.Contents);
            return messages;
        }

        public MessageObject CreateMessage(UInt64 channelId, CreateMessageParams pars) {
            Log.Verbose("Create message: PostRequest");
            var response = _request.PostRequestAsync("/channels/" + channelId + "/messages", pars).Result;
            Log.Verbose("Create message: PostRequest.Done");
            if (response.Code != 200) {
                Log.Verbose("Create message: ErrorCode: " + response.Code);
                return null; // handle these errors ?
            }
            Log.Verbose("Create message: Deserializing");
            var messages = JsonConvert.DeserializeObject<MessageObject>(response.Contents);
            Log.Verbose("Create message: Deserializing.Done");
            return messages;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="messageId"></param>
        /// <param name="emoji"s></param>
        /// <returns>Returns true on success.</returns>
        public bool CreateReaction(UInt64 channelId, UInt64 messageId, string emoji) {
            var response = _request
                .PutRequestAsync("/channels/" + channelId + "/messages/" + messageId + "/reactions/" + emoji + "/@me")
                .Result;
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public bool DeleteOwnReaction(UInt64 channelId, UInt64 messageId, string emoji) {
            var response = _request
                .DeleteRequestAsync(
                    "/channels/" + channelId + "/messages/" + messageId + "/reactions/" + emoji + "/@me").Result;
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public bool DeleteUserReaction(UInt64 channelId, UInt64 messageId, string emoji, UInt64 userId) {
            var response = _request
                .DeleteRequestAsync(
                    "/channels/" + channelId + "/messages/" + messageId + "/reactions/" + emoji + userId).Result;
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public UserObject[] GetReactions(UInt64 channelId, UInt64 messageId, string emoji) {
            var response = _request
                .GetRequestAsync("/channels/" + channelId + "/messages/" + messageId + "/reactions/" + emoji).Result;
            if (response.Code != 200)
                return null; // handle these errors ?
            var messages = JsonConvert.DeserializeObject<UserObject[]>(response.Contents);
            return messages;
        }

        public bool DeleteAllReactions(UInt64 channelId, UInt64 messageId) {
            var response = _request
                .DeleteRequestAsync("/channels/" + channelId + "/messages/" + messageId + "/reactions").Result;
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public MessageObject EditMessage(UInt64 channelId, UInt64 messageId, EditMessageParams pars) {
            var response = _request.PatchRequestAsync("/channels/" + channelId + "/messages/" + messageId, pars).Result;
            if (response.Code != 200)
                return null; // handle these errors ?
            var message = JsonConvert.DeserializeObject<MessageObject>(response.Contents);
            return message;
        }

        public bool DeleteMessage(UInt64 channelId, UInt64 messageId) {
            var response = _request.DeleteRequestAsync("/channels/" + channelId + "/messages/" + messageId).Result;
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public bool BulkDeleteMessages(UInt64 channelId, BulkDeleteMessagesParams pars) {
            var response = _request.PostRequestAsync("/channels/" + channelId + "/messages/bulk-delete", pars).Result;
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public bool EditChannelPermissions(UInt64 channelId, UInt64 overwriteId, EditChannelPermissionsParams pars) {
            var response = _request.PutRequestAsync("/channels/" + channelId + "/permissions/" + overwriteId, pars)
                .Result;
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public InviteObject[] GetChannelInvites(UInt64 channelId) {
            var response = _request.GetRequestAsync("/channels/" + channelId + "/invites").Result;
            if (response.Code != 200)
                return null; // handle these errors ?
            var message = JsonConvert.DeserializeObject<InviteObject[]>(response.Contents);
            return message;
        }

        public InviteObject CreateChannelInvite(UInt64 channelId, CreateChannelInviteParams pars) {
            var response = _request.PostRequestAsync("/channels/" + channelId + "/invites", pars).Result;
            if (response.Code != 200)
                return null; // handle these errors ?
            var message = JsonConvert.DeserializeObject<InviteObject>(response.Contents);
            return message;
        }

        public bool DeleteChannelPermission(UInt64 channelId, UInt64 overwriteId) {
            var response = _request.DeleteRequestAsync("/channels/" + channelId + "/invites/" + overwriteId).Result;
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public bool TriggerTypingIndicator(UInt64 channelId) {
            var response = _request.PostRequestAsync("/channels/" + channelId + "/typing").Result;
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public MessageObject[] GetPinnedMessages(UInt64 channelId) {
            var response = _request.GetRequestAsync("/channels/" + channelId + "/pins").Result;
            if (response.Code != 200)
                return null; // handle these errors ?
            var message = JsonConvert.DeserializeObject<MessageObject[]>(response.Contents);
            return message;
        }

        public bool AddPinnedChannelMessage(UInt64 channelId, UInt64 messageId) {
            var response = _request.PutRequestAsync("/channels/" + channelId + "/pins/" + messageId).Result;
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public bool DeletePinnedChannelMessage(UInt64 channelId, UInt64 messageId) {
            var response = _request.DeleteRequestAsync("/channels/" + channelId + "/pins/" + messageId).Result;
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public bool GroupDMAddRecipient(UInt64 channelId, UInt64 userId) {
            var response = _request.PutRequest("/channels/" + channelId + "/recipients/" + userId);
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public bool GroupDMRemoveRecipient(UInt64 channelId, UInt64 userId) {
            var response = _request.DeleteRequest("/channels/" + channelId + "/recipients/" + userId);
            if (response.Code == 204 || response.Code == 200)
                return true; // handle these errors ?
            return false;
        }

        public Channel(ResourceRequest request) : base(request) {
        }
    }
}