using ZurvanBot.Discord.Resources;
using ZurvanBot.Plugins;

namespace ZurvanBot.Discord {
    /// <summary>
    /// Main API class for the discord API.
    /// </summary>
    public class API {
        private ResourceRequest _requester;

        /// <summary>
        /// REST for channel management.
        /// </summary>
        public Channel Channel;

        /// <summary>
        /// REST for guild management.
        /// </summary>
        public Guild Guild;

        /// <summary>
        /// REST for invite management.
        /// </summary>
        public Invite Invite;

        /// <summary>
        /// REST for user management.
        /// </summary>
        public User User;

        /// <summary>
        /// Rest for voice management.
        /// </summary>
        public Resources.Voice Voice;

        public API(Authentication auth) {
            _requester = new ResourceRequest(auth);

            Channel = new Channel(_requester);
            Guild = new Guild(_requester);
            Invite = new Invite(_requester);
            User = new User(_requester);
            Voice = new Resources.Voice(_requester);
        }
    }
}