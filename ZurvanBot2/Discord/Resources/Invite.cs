using Newtonsoft.Json;
using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Resources {
    public class Invite : Resource {
        public InviteObject GetInvite(string inviteCode) {
            var response = _request.GetRequestAsync("/invites/" + inviteCode).Result;
            if (response.Code != 200)
                return null; // handle these errors ?
            var inviteObject = JsonConvert.DeserializeObject<InviteObject>(response.Contents);
            return inviteObject;
        }

        public InviteObject DeleteInvite(string inviteCode) {
            var response = _request.DeleteRequestAsync("/invites/" + inviteCode).Result;
            if (response.Code != 200)
                return null; // handle these errors ?
            var inviteObject = JsonConvert.DeserializeObject<InviteObject>(response.Contents);
            return inviteObject;
        }

        public InviteObject AcceptInvite(string inviteCode) {
            var response = _request.PostRequestAsync("/invites/" + inviteCode).Result;
            if (response.Code != 200)
                return null; // handle these errors ?
            var inviteObject = JsonConvert.DeserializeObject<InviteObject>(response.Contents);
            return inviteObject;
        }

        public Invite(ResourceRequest request) : base(request) {
        }
    }
}