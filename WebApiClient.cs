using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GuidX.WebApiClient
{
    public class WebApiClient
    {
        #region Fields

        private static string _serviceUrl;

        private static string _accessToken;

        #endregion

        #region Methods

        public static void SetServiceUrl(string url)
        {
            _serviceUrl = url;
        }

        public static void SetAccessToken(string token)
        {
            _accessToken = token;
        }


        public static async Task<WebApiResult<T>> GetJson<T>(string url, bool isAnonymous)
        {
            var result = await Get<T>(url, isAnonymous);
            return result;
        }


        public static async Task<WebApiResult<T>> PostString<T>(string url, string payload, bool httpResponseOnly,
            bool isAnonymous)
        {
            var httpContent = new StringContent(payload, Encoding.UTF8, "application/x-www-form-urlencoded");
            var result = await Post<T>(url, httpContent, isAnonymous);
            return result;
        }


        public static async Task<WebApiResult<bool>> PostJsonForHttpStatusOnly(string url, object payload,
            bool isAnonymous)
        {
            var json = JsonConvert.SerializeObject(payload);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await PostForHttpStatusOnly(url, httpContent, isAnonymous);
            result.Value = result.HttpStatusCode == HttpStatusCode.OK;
            return result;
        }


        public static async Task<WebApiResult<T>> PostJson<T>(string url, object payload, bool isAnonymous)

        {
            var json = JsonConvert.SerializeObject(payload);

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await Post<T>(url, httpContent, isAnonymous);
            return result;
        }

        private static void SetBaseAddress(HttpClient client)
        {
            if (!string.IsNullOrEmpty(_serviceUrl))
                client.BaseAddress = new Uri(_serviceUrl);
        }

        private static async Task<WebApiResult<T>> Get<T>(string url, bool isAnonymous)
        {
            using (var client = new HttpClient())
            {
                SetBaseAddress(client);

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!isAnonymous)
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);
                var result = await client.GetAsync(url);

                if (!result.IsSuccessStatusCode)
                    return new WebApiResult<T>
                    {
                        HttpStatusCode = result.StatusCode
                    };
                var json = result.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<T>(json);
                return new WebApiResult<T>
                {
                    HttpStatusCode = result.StatusCode,
                    Value = data
                };
            }
        }


        private static async Task<WebApiResult<T>> Post<T>(string url, HttpContent httpContent, bool isAnonymous)
        {
            using (var client = new HttpClient())
            {
                SetBaseAddress(client);


                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!isAnonymous)
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);
                var result = await client.PostAsync(url, httpContent);
                if (!result.IsSuccessStatusCode)
                    return new WebApiResult<T>
                    {
                        HttpStatusCode = result.StatusCode
                    };


                var json = result.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<T>(json);
                return new WebApiResult<T>
                {
                    HttpStatusCode = result.StatusCode,
                    Value = data
                };
            }
        }


        private static async Task<WebApiResult<bool>> PostForHttpStatusOnly(string url, HttpContent httpContent,
            bool isAnonymous)
        {
            using (var client = new HttpClient())
            {
                SetBaseAddress(client);


                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!isAnonymous)
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);
                var result = await client.PostAsync(url, httpContent);

                return new WebApiResult<bool>
                {
                    HttpStatusCode = result.StatusCode
                };
            }
        }

        #endregion
    }
}