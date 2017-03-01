using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace PhotgraphyMVC.Models
{
    public class Communication
    {
        private const string BaseApiUrl = "http://localhost:57669/";

        public static string GetRequest(string api, string username)
        {
            string returnValue = string.Empty;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseApiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("username", username);
                var response = client.GetAsync(api).Result;

                if (response.IsSuccessStatusCode)
                {
                    returnValue = response.Content.ReadAsStringAsync().Result;
                }
            }

            return returnValue;
        }

        public static string PostRequest(string api, string username, string content)
        {
            string returnValue = string.Empty;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseApiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("username", username);
                var response = client.PostAsync(api, new StringContent(content, System.Text.Encoding.UTF8, "application/json")).Result;

                //if (response.IsSuccessStatusCode)
                //{
                    returnValue = response.Content.ReadAsStringAsync().Result;
                //}
            }

            return returnValue;
        }

        public static string PutRequest(string api, string username, string content)
        {
            string returnValue = string.Empty;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseApiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("username", username);
                var response = client.PutAsync(api, new StringContent(content, System.Text.Encoding.UTF8, "application/json")).Result;

                if (response.IsSuccessStatusCode)
                {
                    returnValue = response.Content.ReadAsStringAsync().Result;
                }
            }

            return returnValue;
        }

        public static string DeleteRequest(string api, string username)
        {
            string returnValue = string.Empty;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseApiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("username", username);
                var response = client.DeleteAsync(api).Result;

                if (response.IsSuccessStatusCode)
                {
                    returnValue = response.Content.ReadAsStringAsync().Result;
                }
            }

            return returnValue;
        }
    }
}