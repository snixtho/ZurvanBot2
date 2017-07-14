using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZurvanBot.Discord {
    public class ResourceRequest {
        public static string APIBase = "https://discordapp.com/api";

        private readonly Authentication _auth;
        private RateLimiter _rateLimit = new RateLimiter(5);

        public ResourceRequest(Authentication auth) {
            // invalid certificate fix for linux machines
            ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
            ServicePointManager.DefaultConnectionLimit = 10000;

            _auth = auth;
        }

        /// <summary>
        /// Creates a request for use with the discord api.
        /// </summary>
        /// <param name="path">The request path relative to the main api base path.</param>
        /// <returns>The created request object.</returns>
        public HttpWebRequest CreateRequest(string path) {
            _rateLimit.WaitOrContinue();

            var request = (HttpWebRequest) WebRequest.Create(APIBase + path);

            request.Headers.Set("Authorization", _auth.AuthTypeStr + " " + _auth.AuthString);
            request.UserAgent = "ZurvanBot (https://www.zurvan-labs.net, " + ZurvanBot.Version + ")";

            return request;
        }

        /// <summary>
        /// Executes a http request and get its response.
        /// </summary>
        /// <param name="request">The request to execute.</param>
        /// <returns>Basic response of the request.</returns>
        public BasicRequestResponse RunRequest(HttpWebRequest request) {
            var returnResponse = new BasicRequestResponse {
                Code = 0,
                Contents = "",
                Headers = new WebHeaderCollection()
            };

            try {
                var response = (HttpWebResponse) request.GetResponse();
                returnResponse.Code = (int) response.StatusCode;
                returnResponse.Headers = response.Headers;
                // returnResponse.Contents = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (WebException e) {
                Console.WriteLine(e.Message);
                var response = (HttpWebResponse) e.Response;
                returnResponse.Code = (int) response.StatusCode;
                returnResponse.Headers = response.Headers;
            }

            return returnResponse;
        }

        /// <summary>
        /// Does a GET request against the api.
        /// </summary>
        /// <param name="path">The path to where to do the get request.</param>
        /// <returns>The respones of the request.</returns>
        public BasicRequestResponse GetRequest(string path) {
            var request = CreateRequest(path);
            request.Method = "GET";

            var response = RunRequest(request);
            return response;
        }

        /// <summary>
        /// Does a GET request against the api.
        /// </summary>
        /// <param name="path">The path to where to do the get request.</param>
        /// <returns>The respones of the request.</returns>
        public Task<BasicRequestResponse> GetRequestAsync(string path) {
            return Task<BasicRequestResponse>.Factory.StartNew(() => GetRequest(path));
        }

        /// <summary>
        /// Do a put request against the api.
        /// </summary>
        /// <param name="path">The put to do the put request.</param>
        /// <returns>The request response.</returns>
        public BasicRequestResponse PutRequest(string path) {
            var request = CreateRequest(path);
            request.Method = "PUT";

            var response = RunRequest(request);
            return response;
        }

        /// <summary>
        /// Do a put request against the api.
        /// </summary>
        /// <param name="path">The put to do the put request.</param>
        /// <returns>The request response.</returns>
        public Task<BasicRequestResponse> PutRequestAsync(string path) {
            return Task<BasicRequestResponse>.Factory.StartNew(() => PutRequest(path));
        }

        /// <summary>
        /// Do a put request against the api.
        /// </summary>
        /// <param name="path">The put to do the put request.</param>
        /// <param name="param">The parameters for the put request.</param>
        /// <typeparam name="T">Should be one of the instances of the classes
        /// in ZurvanBot.Discord.Resources.Params if it matches the api docs.</typeparam>
        /// <returns>The request response.</returns>
        public BasicRequestResponse PutRequest<T>(string path, T param) {
            var request = CreateRequest(path);
            var paramsStr = JsonConvert.SerializeObject(param, new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore
            });

            Console.WriteLine(paramsStr);

            request.Method = "PUT";
            request.ContentLength = paramsStr.Length;
            request.ContentType = "application/json";

            using (var stream = request.GetRequestStream()) {
                var data = Encoding.ASCII.GetBytes(paramsStr);
                stream.Write(data, 0, data.Length);
            }

            var response = RunRequest(request);
            return response;
        }

        /// <summary>
        /// Do a put request against the api.
        /// </summary>
        /// <param name="path">The put to do the put request.</param>
        /// <param name="param">The parameters for the put request.</param>
        /// <typeparam name="T">Should be one of the instances of the classes
        /// in ZurvanBot.Discord.Resources.Params if it matches the api docs.</typeparam>
        /// <returns>The request response.</returns>
        public Task<BasicRequestResponse> PutRequestAsync<T>(string path, T param) {
            return Task<BasicRequestResponse>.Factory.StartNew(() => PutRequest(path, param));
        }

        /// <summary>
        /// Do a patch request against the api.
        /// </summary>
        /// <param name="path">The path to do the patch request.</param>
        /// <returns>The request response.</returns
        public BasicRequestResponse PatchRequest(string path) {
            var request = CreateRequest(path);
            request.Method = "PATCH";

            var response = RunRequest(request);
            return response;
        }

        /// <summary>
        /// Do a patch request against the api.
        /// </summary>
        /// <param name="path">The path to do the patch request.</param>
        /// <returns>The request response.</returns
        public Task<BasicRequestResponse> PatchRequestAsync(string path) {
            return Task<BasicRequestResponse>.Factory.StartNew(() => PatchRequest(path));
        }

        /// <summary>
        /// Do a patch request against the api.
        /// </summary>
        /// <param name="path">The path to do the patch request.</param>
        /// <param name="param">The parameters for the patch request.</param>
        /// <typeparam name="T">Should be one of the instances of the classes
        /// in ZurvanBot.Discord.Resources.Params if it matches the api docs.</typeparam>
        /// <returns>The request response.</returns
        public BasicRequestResponse PatchRequest<T>(string path, T param) {
            var request = CreateRequest(path);
            var paramsStr = JsonConvert.SerializeObject(param, new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore
            });

            Console.WriteLine(paramsStr);

            request.Method = "PATCH";
            request.ContentLength = paramsStr.Length;
            request.ContentType = "application/json";

            using (var stream = request.GetRequestStream()) {
                var data = Encoding.ASCII.GetBytes(paramsStr);
                stream.Write(data, 0, data.Length);
            }

            var response = RunRequest(request);
            return response;
        }

        /// <summary>
        /// Do a patch request against the api.
        /// </summary>
        /// <param name="path">The path to do the patch request.</param>
        /// <param name="param">The parameters for the patch request.</param>
        /// <typeparam name="T">Should be one of the instances of the classes
        /// in ZurvanBot.Discord.Resources.Params if it matches the api docs.</typeparam>
        /// <returns>The request response.</returns
        public Task<BasicRequestResponse> PatchRequestAsync<T>(string path, T param) {
            return Task<BasicRequestResponse>.Factory.StartNew(() => PatchRequest(path, param));
        }

        /// <summary>
        /// Sends a delete request to the api server.
        /// </summary>
        /// <param name="path">The request path to send a delete request to.</param>
        /// <returns>The response of the request.</returns>
        public BasicRequestResponse DeleteRequest(string path) {
            var request = CreateRequest(path);
            request.Method = "DELETE";

            var response = RunRequest(request);
            return response;
        }

        /// <summary>
        /// Sends a delete request to the api server.
        /// </summary>
        /// <param name="path">The request path to send a delete request to.</param>
        /// <returns>The response of the request.</returns>
        public Task<BasicRequestResponse> DeleteRequestAsync(string path) {
            return Task<BasicRequestResponse>.Factory.StartNew(() => DeleteRequest(path));
        }

        /// <summary>
        /// Sends a delete request to the api server.
        /// </summary>
        /// <param name="path">The request path to send a delete request to.</param>
        /// <returns>The response of the request.</returns>
        public BasicRequestResponse DeleteRequest<T>(string path, T param) {
            var request = CreateRequest(path);
            var paramsStr = JsonConvert.SerializeObject(param, new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore
            });

            Console.WriteLine(paramsStr);

            request.Method = "DELETE";
            request.ContentLength = paramsStr.Length;
            request.ContentType = "application/json";

            using (var stream = request.GetRequestStream()) {
                var data = Encoding.ASCII.GetBytes(paramsStr);
                stream.Write(data, 0, data.Length);
            }

            var response = RunRequest(request);
            return response;
        }

        /// <summary>
        /// Sends a delete request to the api server.
        /// </summary>
        /// <param name="path">The request path to send a delete request to.</param>
        /// <returns>The response of the request.</returns>
        public Task<BasicRequestResponse> DeleteRequestAsync<T>(string path, T param) {
            return Task<BasicRequestResponse>.Factory.StartNew(() => DeleteRequest(path, param));
        }

        /// <summary>
        /// Do a post request against the api.
        /// </summary>
        /// <param name="path">The path to do the post request.</param>
        /// <param name="param">The parameters for the post request.</param>
        /// <typeparam name="T">Should be one of the instances of the classes
        /// in ZurvanBot.Discord.Resources.Params if it matches the api docs.</typeparam>
        /// <returns>The request response.</returns
        public BasicRequestResponse PostRequest<T>(string path, T param) {
            var request = CreateRequest(path);
            var paramsStr = JsonConvert.SerializeObject(param, new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore
            });

            request.Method = "POST";
            request.ContentLength = paramsStr.Length;
            request.ContentType = "application/json";

            using (var stream = request.GetRequestStream()) {
                var data = Encoding.ASCII.GetBytes(paramsStr);
                stream.Write(data, 0, data.Length);
            }

            var response = RunRequest(request);
            return response;
        }

        /// <summary>
        /// Do a post request against the api.
        /// </summary>
        /// <param name="path">The path to do the post request.</param>
        /// <param name="param">The parameters for the post request.</param>
        /// <typeparam name="T">Should be one of the instances of the classes
        /// in ZurvanBot.Discord.Resources.Params if it matches the api docs.</typeparam>
        /// <returns>The request response.</returns
        public Task<BasicRequestResponse> PostRequestAsync<T>(string path, T param) {
            return Task<BasicRequestResponse>.Factory.StartNew(() => PostRequest(path, param));
        }

        /// <summary>
        /// Do a post request against the api.
        /// </summary>
        /// <param name="path">The path to do the post request.</param>
        /// <returns>The request response.</returns
        public BasicRequestResponse PostRequest(string path) {
            var request = CreateRequest(path);
            request.Method = "POST";

            var response = RunRequest(request);
            return response;
        }

        /// <summary>
        /// Do a post request against the api.
        /// </summary>
        /// <param name="path">The path to do the post request.</param>
        /// <returns>The request response.</returns
        public Task<BasicRequestResponse> PostRequestAsync(string path) {
            return Task<BasicRequestResponse>.Factory.StartNew(() => PostRequest(path));
        }
    }
}