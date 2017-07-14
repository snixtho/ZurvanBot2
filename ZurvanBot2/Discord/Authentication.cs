using System;

namespace ZurvanBot.Discord {
    /// <summary>
    /// Holds discord authentication information.
    /// </summary>
    public class Authentication {
        public enum AuthType {
            Bot,
            Bearer
        }

        private AuthType _authType;

        /// <summary>
        /// The authentication token/identifier string.
        /// </summary>
        public string AuthString { get; }

        /// <summary>
        /// The type of authentication. Usually Bot for bot accounts.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public string AuthTypeStr {
            get {
                switch (_authType) {
                    case AuthType.Bearer: return "bearer";
                    case AuthType.Bot: return "Bot";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Authentication(AuthType authType, string authString) {
            AuthString = authString;
            _authType = authType;
        }
    }
}