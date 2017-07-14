using System.Net;

namespace ZurvanBot.Discord {
    /// <summary>
    /// Holds basic response information from a HTTP request.
    /// </summary>
    public class BasicRequestResponse {
        public int Code;
        public string Contents;
        public WebHeaderCollection Headers;
    }
}