using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace ZurvanBot.Util.HTTP {
    public class HTTPSRequest {
        private SslStream _sslStream;
        private TcpClient _tcpClient;
        private string _method;
        private MemoryStream _bodyStream;

        /// <summary>
        /// The url to connect to.
        /// </summary>
        public Uri URL { get; }

        /// <summary>
        /// The set of header fields for the http request.
        /// </summary>
        public WebHeaderCollection Headers { get; set; }

        /// <summary>
        /// Number of retries after a connection failure. Default is 1.
        /// </summary>
        public uint ConnectionRetries { get; set; }

        /// <summary>
        /// The port to connect to the https server. 443 by default.
        /// </summary>
        public int ConnectionPort { get; set; }

        /// <summary>
        /// The method is GET by default.
        /// </summary>
        public string Method {
            get { return _method.ToUpper(); }
            set { _method = value; }
        }

        public HTTPSRequest(Uri url) {
            URL = url;
            Headers = new WebHeaderCollection();
            ConnectionRetries = 1;
            ConnectionPort = 443;
            Method = "GET";

            SetDefaultHeaders();
        }

        /// <summary>
        /// Sets default headers for the request.
        /// </summary>
        private void SetDefaultHeaders() {
            Headers.Set("Host", URL.Host);
        }

        /// <summary>
        /// Attempts to connect to the server and setup a stream
        /// for reading and writing.
        /// </summary>
        /// <returns>True if the connection and stream was setup correctly, false if not.</returns>
        private bool SetupNewStream() {
            if (_tcpClient != null && _tcpClient.Connected)
                _tcpClient.Close();
            _sslStream?.Close();
            _tcpClient = new TcpClient();

            var retries = 0;
            do _tcpClient.Connect(URL.Host, ConnectionPort); while (!_tcpClient.Connected &&
                                                                    ++retries < ConnectionRetries);

            if (!_tcpClient.Connected)
                return false; // connection failed.

            // ServicePointManager.ServerCertificateValidationCallback = (a,b,c,d) => true;
            ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;

            Console.WriteLine(_tcpClient.Connected);

            _sslStream = new SslStream(_tcpClient.GetStream());
            _sslStream.AuthenticateAsClient(URL.Host);
            return _sslStream.IsAuthenticated;
        }

        /// <summary>
        /// Creates a http header for the request based on the provided
        /// headers in the Headers variable.
        /// </summary>
        /// <returns>The created header.</returns>
        private string CreateHttpHeader() {
            var headerStr = "";

            headerStr += "GET " + URL.PathAndQuery + " HTTP/1.1\r\n";

            for (var i = 0; i < Headers.Count; i++) {
                var header = Headers.GetKey(i);
                foreach (var value in Headers.GetValues(i))
                    headerStr += header + ": " + value + "\r\n";
            }

            headerStr += "\r\n";

            return headerStr;
        }

        public HTTPSResponse GetResponse() {
            if (!SetupNewStream())
                throw new WebException("Connection to the web server failed. Ethier timeout or SSL auth error.");

            var headerstr = CreateHttpHeader();
            var headerBuf = Encoding.ASCII.GetBytes(headerstr);

            _sslStream.Write(headerBuf);

            const int chunkSize = 1024;
            long read = 0;
            while (read < _bodyStream.Length) {
                var chunk = new byte[chunkSize];
                var newRead = _bodyStream.Read(chunk, (int) read, chunkSize);
                if (newRead < chunkSize)
                    _sslStream.Write(chunk, 0, newRead);
                else
                    _sslStream.Write(chunk);

                read += chunkSize;
            }

            _sslStream.Flush();

            // get response
            var responseData = new byte[0];
            long bytesRead = 0;
            int currRead;
            do {
                currRead = _sslStream.Read(responseData, (int) bytesRead, chunkSize);
                bytesRead += currRead;
            } while (currRead == chunkSize);

            _sslStream.Close();
            _tcpClient.Close();

            var response = new HTTPSResponse(responseData);
            return response;
        }

        /// <summary>
        /// Get the stream for writing data to the body of the request.
        /// </summary>
        /// <returns>The body stream created.</returns>
        public Stream NewBodyStream() {
            _bodyStream?.Close();
            _bodyStream = new MemoryStream();
            return _bodyStream;
        }
    }
}