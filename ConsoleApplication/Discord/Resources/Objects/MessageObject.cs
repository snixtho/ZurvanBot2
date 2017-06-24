using System;

namespace ZurvanBot.Discord.Resources.Objects
{
    public class MessageObject : ResObject
    {
        public UInt64 channel_id;
        public UserObject author;
        public string content;
        public string timestamp;
        public string edited_timestamp;
        public bool tts;
        public bool mention_everyone;
        public UserObject[] mentions;
        public RoleObject[] mention_roles;
        public AttachmentObject[] AttachmentsObject;
        public EmbedObject[] embets;
        public ReactionObject[] reactions;
        public UInt64 nonce;
        public bool pinned;
        public string webhook_id;
    }
}