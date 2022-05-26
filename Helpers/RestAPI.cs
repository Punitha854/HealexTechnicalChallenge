
 using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FirelyClient.Helpers
{
    public class RestAPI
    {
        private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new DefaultContractResolver(),
            DefaultValueHandling = DefaultValueHandling.Include,
            TypeNameHandling = TypeNameHandling.All
            
        };
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<T> Post<T>(string url, string body)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "POST";
            try
            {
                Stream requestStream = request.GetRequestStream();
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(body);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                WebResponse response = request.GetResponse();
                using (var responseStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(
                        responseStream ?? throw new InvalidOperationException(),
                        System.Text.Encoding.UTF8
                    );
                    var jsonString = reader.ReadToEnd();

                    return JsonConvert.DeserializeObject<T>(jsonString);
                }
            }
            catch (WebException ex)
            {
                var errorResponse = ex.Response;
                var errText = "";

                using (var responseStream = errorResponse.GetResponseStream())
                {
                    var reader = new StreamReader(
                        responseStream ?? throw new InvalidOperationException(),
                        System.Text.Encoding.GetEncoding("utf-8")
                    );
                    errText = reader.ReadToEnd();
                    // log errorText
                }
                throw new Exception(errText);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<T> PostAsync<T>(string url, string body)
        {

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Timeout = int.MaxValue;
            try
            {
                Stream requestStream = request.GetRequestStream();

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(body);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                WebResponse response = request.GetResponse();
                using (var responseStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(
                        responseStream ?? throw new InvalidOperationException(),
                        System.Text.Encoding.UTF8
                    );
                    var jsonString = reader.ReadToEnd();

                    return JsonConvert.DeserializeObject<T>(jsonString);
                }
            }
            catch (WebException ex)
            {
                var errorResponse = ex.Response;
                var errText = "";

                using (var responseStream = errorResponse?.GetResponseStream())
                {
                    var reader = new StreamReader(
                        responseStream ?? throw new InvalidOperationException(),
                        System.Text.Encoding.GetEncoding("utf-8")
                    );
                    errText = reader.ReadToEnd();
                    // log errorText
                }
                throw new Exception(errText);
            }
        }

        /// <summary>
        /// Puts the.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <param name="token">The token.</param>
        /// <returns>A Task.</returns>
        public async Task<T> Put<T>(string url, Dictionary<string, string> keyValuePairs, string token = "")
        {
            string responseBody = string.Empty;
            try
            {

                var client = new HttpClient();

                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", token));

                var req = new HttpRequestMessage(HttpMethod.Put, url) { Content = new FormUrlEncodedContent(keyValuePairs) };

                var res = await client.SendAsync(req);

                responseBody = await res.Content.ReadAsStringAsync();
                res.EnsureSuccessStatusCode();

                return JsonConvert.DeserializeObject<T>(responseBody);
            }
            catch (HttpRequestException ex)
            {
                throw new JsonWebApiException(responseBody, ex.Message, ex);
            }
        }

        public async Task<T> Put<T>(string url, string body, string token = "")
        {
            string responseBody = string.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(token))
                        client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", token));

                    using (var content = new StringContent(body, Encoding.UTF8, "application/json"))
                    {
                        var req = new HttpRequestMessage(HttpMethod.Put, url) { Content = content };
                        var res = await client.SendAsync(req);
                        responseBody = await res.Content.ReadAsStringAsync();
                        res.EnsureSuccessStatusCode();
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                throw new JsonWebApiException(responseBody, ex.Message, ex);
            }
            return JsonConvert.DeserializeObject<T>(responseBody);
        }


        /// <summary>
        /// Puts the.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <param name="body">The body.</param>
        /// <param name="token">The token.</param>
        /// <param name="isCerner">If true, is cerner.</param>
        /// <returns>A Task.</returns>
        public async Task<T> Put<T>(string url, string body)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "PUT";
            try
            {
                Stream requestStream = request.GetRequestStream();

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(body);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                WebResponse response = request.GetResponse();
                using (var responseStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(
                        responseStream ?? throw new InvalidOperationException(),
                        System.Text.Encoding.UTF8
                    );
                    var jsonString = reader.ReadToEnd();

                    return JsonConvert.DeserializeObject<T>(jsonString);
                }
            }
            catch (WebException ex)
            {
                var errorResponse = ex.Response;
                string err = "";
                using (var responseStream = errorResponse.GetResponseStream())
                {
                    var reader = new StreamReader(
                        responseStream ?? throw new InvalidOperationException(),
                        System.Text.Encoding.GetEncoding("utf-8")
                    );
                    err = reader.ReadToEnd();
                    // log errorText
                }
                throw new WebException(err, ex);
            }
        }
        /// <summary>
        /// Patches the.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <param name="body">The body.</param>
        /// <param name="token">The token.</param>
        /// <param name="isCerner">If true, is cerner.</param>
        /// <returns>A Task.</returns>
        public async Task<T> Patch<T>(string url, string[] body)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "PATCH";

            try
            {

                Stream requestStream = request.GetRequestStream();

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(body[0]);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                WebResponse response = request.GetResponse();
                using (var responseStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(
                        responseStream ?? throw new InvalidOperationException(),
                        System.Text.Encoding.UTF8
                    );
                    var jsonString = reader.ReadToEnd();

                    return JsonConvert.DeserializeObject<T>(jsonString);
                }
            }
            catch (WebException ex)
            {
                var errorResponse = ex.Response;
                string err = "";
                using (var responseStream = errorResponse.GetResponseStream())
                {
                    var reader = new StreamReader(
                        responseStream ?? throw new InvalidOperationException(),
                        System.Text.Encoding.GetEncoding("utf-8")
                    );
                    err = reader.ReadToEnd();
                    // log errorText
                }
                throw new WebException(err, ex);
            }


        }
        /// <summary>
        /// Gets the.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <param name="body">The body.</param>
        /// <param name="token">The token.</param>       
        /// <returns>A Task.</returns>
        public async Task<T> Get<T>(string url, string body, string token = "")
        {
            //Logger.Instance(LogLevel.Info).Info("Started collecting data from " +url);

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/fhir+json";
            request.Accept = "application/fhir+json";
            request.Method = "GET";
            request.Timeout = int.MaxValue;

            if (!string.IsNullOrEmpty(token))
                request.Headers.Add("Authorization", string.Format("Basic {0}", token));

            try
            {
                var response = await request.GetResponseAsync();
                using (var responseStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(
                        responseStream ?? throw new InvalidOperationException(),
                        System.Text.Encoding.UTF8
                    );
                    var jsonString = reader.ReadToEnd();
                    var results = JsonConvert.DeserializeObject<T>(jsonString, serializerSettings);
                    return results;
                }
            }
            catch (WebException ex)
            {
                var errorResponse = ex.Response;
                if (errorResponse != null)
                {
                    using (var responseStream = errorResponse?.GetResponseStream())
                    {
                        var reader = new StreamReader(
                            responseStream ?? throw new InvalidOperationException(),
                            System.Text.Encoding.GetEncoding("utf-8")
                        );
                        string status = reader.ReadToEnd();

                        throw new JsonWebApiException(status, ex.Message, ex);

                    }
                }
                else throw new Exception(ex.Message);
            }
        }

        public async Task<string> getAPIString(string url, string body = "")
        {
            string result = string.Empty;
            //Logger.Instance(LogLevel.Info).Info("Started collecting data from " + url);
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/fhir+json";
                request.Accept = "application/fhir+json";
                request.Method = "GET";
                request.Timeout = int.MaxValue;

                var response = request.GetResponseAsync().Result;
                using (var responseStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(
                        responseStream ?? throw new InvalidOperationException(),
                        System.Text.Encoding.UTF8
                    );
                    result = reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                var errorResponse = ex.Response;
                if (errorResponse != null)
                {
                    using (var responseStream = errorResponse.GetResponseStream())
                    {
                        var reader = new StreamReader(
                            responseStream ?? throw new InvalidOperationException(),
                            System.Text.Encoding.GetEncoding("utf-8")
                        );
                        string status = reader.ReadToEnd();
                        throw new JsonWebApiException(status, ex.Message, ex);
                    }
                }
                else throw new Exception(ex.Message);

            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// Dels the.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <param name="token">The token.</param>
        /// <returns>A Task.</returns>
        public async Task<T> Del<T>(string url, string token = "")
        {
            string responseBody = string.Empty;
            try
            {

                var client = new HttpClient();

                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", token));

                var req = new HttpRequestMessage(HttpMethod.Delete, url);// { Content = new FormUrlEncodedContent(keyValuePairs) };

                var res = await client.SendAsync(req);

                responseBody = await res.Content.ReadAsStringAsync();
                res.EnsureSuccessStatusCode();

                return JsonConvert.DeserializeObject<T>(responseBody);
            }
            catch (HttpRequestException ex)
            {
                throw new JsonWebApiException(responseBody, ex.Message, ex);
            }
        }
        /// <summary>
        /// PutAsync
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="token"></param>
        /// <param name="platform"></param>
        /// <param name="ClientID"></param>
        /// <returns></returns>
        public async Task<T> PutAsync<T>(string url, string body)
        {

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "PUT";
            try
            {

                Stream requestStream = request.GetRequestStream();
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(body);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                WebResponse response = request.GetResponse();
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)response;
                if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    string str = "{'statusCode': 200,'errors': [],'messageId': 'Done'}";
                    return JsonConvert.DeserializeObject<T>(str);
                }
                using (var responseStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(
                        responseStream ?? throw new InvalidOperationException(),
                        System.Text.Encoding.UTF8
                    );
                    var jsonString = reader.ReadToEnd();

                    return JsonConvert.DeserializeObject<T>(jsonString);
                }
            }
            catch (WebException ex)
            {
                var errorResponse = ex.Response;
                string err = "";
                using (var responseStream = errorResponse.GetResponseStream())
                {
                    var reader = new StreamReader(
                        responseStream ?? throw new InvalidOperationException(),
                        System.Text.Encoding.GetEncoding("utf-8")
                    );
                    err = reader.ReadToEnd();
                }
                throw new WebException(err, ex);
            }
        }
    }


}






