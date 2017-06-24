using Newtonsoft.Json;
using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Resources
{
    public class Invite: Resource
    {
        public InviteObject GetInvite(string inviteCode)
        {
            var response = _request.GetRequest("/invites/" + inviteCode);
            if (response.Code != 200)
                return null; // handle these errors ?
            var inviteObject = JsonConvert.DeserializeObject<InviteObject>(response.Contents);
            return inviteObject;
        }

        public InviteObject DeleteInvite(string inviteCode)
        {
            var response = _request.DeleteRequest("/invites/" + inviteCode);
            if (response.Code != 200)
                return null; // handle these errors ?
            var inviteObject = JsonConvert.DeserializeObject<InviteObject>(response.Contents);
            return inviteObject;
        }

        public InviteObject AcceptInvite(string inviteCode)
        {
            var response = _request.PostRequest("/invites/" + inviteCode);
            if (response.Code != 200)
                return null; // handle these errors ?
            var inviteObject = JsonConvert.DeserializeObject<InviteObject>(response.Contents);
            return inviteObject;
        }

        public Invite(ResourceRequest request) : base(request)
        {
        }
    }
}