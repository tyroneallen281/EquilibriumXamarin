using Acr.UserDialogs;
using EquilibriumApp.Mobile.Helpers;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EquilibriumApp.Services
{
    public static class HttpService
    {
        /// <summary>
        /// Get Async with Authorization.
        ///
        /// </summary>
        /// <param name="baseUrl">BaseURL is the baseUrl of your API. For example, http://something.com/</param>
        /// <param name="endpoint">for example, api/GetUsers</param>
        /// <param name="parameters">Parameters for a specific endpoint, set as null if  they are no parameters</param>
        /// <param name="token">token for auth. No need to pass Bearer Token, just pass token only.</param>
        /// <returns></returns>
        public static async Task<string> GetAsync(string baseUrl, string endpoint, Dictionary<string, string> parameters = null, string token = null)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                return null;
            }
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };
            try
            {
                using (var client = new HttpClient(handler))
                {
                    //

                    client.Timeout = TimeSpan.FromSeconds(120);

                    //client.DefaultRequestHeaders.Host = baseUrl;
                    if (token != null)
                    {
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    }
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    string parameter = string.Empty;
                    client.BaseAddress = new Uri(baseUrl);
                    if (parameters != null)
                    {
                        foreach (var keyvalue in parameters)
                        {
                            if (!string.IsNullOrEmpty(keyvalue.Value))
                            {
                                parameter += $"{keyvalue.Key}={keyvalue.Value}&";
                            }
                        }
                    }
                    string uriLink = $"{client.BaseAddress.AbsoluteUri}{endpoint}?{parameter}";
                    if (uriLink.EndsWith("&"))
                    {
                        uriLink = uriLink.Substring(0, uriLink.Length - 1);
                    }
                    var s = await client.GetAsync(uriLink);

                    if (s.IsSuccessStatusCode)
                    {
                        var response = await s.Content.ReadAsStringAsync();
                        return response;
                    }
                    return null;
                }
            }
            catch (Exception exception)
            {
                return null;
            }
        }
        public class ReturnModel
        {
            public string Data { get; set; }
            public string ErrorMessage { get; set; }
            public bool IsSuccess { get; set; }
        }

        public static async Task<bool> IsServerReachableAsync()
        {
            if (CrossConnectivity.Current == null)
            {
                return true;
            }
            if (CrossConnectivity.Current.IsConnected)
            {
                return await CrossConnectivity.Current.IsRemoteReachable("baobabtech.co.za");
            }
            return false;
        }
        public static async Task<ReturnModel> PostAsync<T>(string baseUrl, string endpoint, T content = default(T), Dictionary<string, string> formUrlEncoded = null, Dictionary<string, string> parameters = null, string token = null)
        {
            try
            {
                if (!(await IsServerReachableAsync()))
                {
                    UserDialogs.Instance.Toast("No internet connection - Please try again.", TimeSpan.FromSeconds(5));
                    return new ReturnModel() { ErrorMessage = "No internet connection - Please try again.", IsSuccess = false };
                }

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(60);
                    if (token != null)
                    {
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    }
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    string parameter = string.Empty;
                    string formencoded = string.Empty;
                    string serialized = string.Empty;
                    client.BaseAddress = new Uri(baseUrl);
                    if (parameters != null)
                    {
                        parameter = parameters.Aggregate(parameter, (current, keyvalue) => current + $"{keyvalue.Key}={keyvalue.Value}&");
                    }
                    if (formUrlEncoded != null)
                    {
                        formencoded = formUrlEncoded.Aggregate(formencoded, (current, keyvalue) => current + $"{keyvalue.Key}={keyvalue.Value}&");
                        serialized = formencoded;
                    }

                    string uriLink = $"{client.BaseAddress.AbsoluteUri}{endpoint}";
                    if (!string.IsNullOrEmpty(parameter))
                    {
                        uriLink = $"{uriLink}?{parameter}";
                    }
                    if (content != null && content as string != string.Empty)
                    {
                        serialized = content.Serialize();
                    }
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                    HttpContent httpContent = new StringContent(serialized, Encoding.UTF8, string.IsNullOrEmpty(formencoded) ? "application/json" : "application/x-www-form-urlencoded");
                    var s = await client.PostAsync(uriLink, httpContent);
                    if (s.IsSuccessStatusCode)
                    {
                        var response = await s.Content.ReadAsStringAsync();
                        return new ReturnModel() { ErrorMessage = null, IsSuccess = true, Data = response };
                    }
                    else
                    {
                        var errorMessage = string.Empty;
                        var response = await s.Content.ReadAsStringAsync();
                        try
                        {
                            errorMessage = response.Deserialize<string>();

                        }
                        catch (Exception)
                        {


                        }

                        return new ReturnModel() { ErrorMessage = errorMessage, IsSuccess = false };
                    }
                }
            }
            catch (Exception exception)
            {
                return new ReturnModel() { ErrorMessage = "Error Occured - Please try again.", IsSuccess = false };
            }
        }

        public static async Task<byte[]> DownloadImage(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync(url))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return await response.Content.ReadAsByteArrayAsync();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T">the type of the Model e.g. UserModel</typeparam>
        /// <param name="baseUrl">BaseURL is the baseUrl of your API. For example, http://something.com/</param>
        /// <param name="endpoint">for example, api/PUTUsers</param>
        /// <param name="content">the model you are sending through to the API</param>
        /// <param name="parameters">Parameters for a specific endpoint, set as null if  they are no parameters<</param>
        /// <param name="token">token for auth. No need to pass Bearer Token, just pass token only.</param>
        /// <returns>the API json string</returns>
        public static async Task<string> PutAsync<T>(string baseUrl, string endpoint, T content = default(T), Dictionary<string, string> parameters = null, string token = null)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                return null;
            }
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(60);
                    if (token != null)
                    {
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    }
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    // client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                    string parameter = string.Empty;

                    string serialized = string.Empty;
                    client.BaseAddress = new Uri(baseUrl);
                    if (parameters != null)
                    {
                        parameter = parameters.Aggregate(parameter, (current, keyvalue) => current + $"{keyvalue.Key}={keyvalue.Value}&");
                    }

                    string uriLink = $"{client.BaseAddress.AbsoluteUri}{endpoint}?{parameter}";
                    if (content != null && content as string != string.Empty)
                    {
                        serialized = content.Serialize();
                    }

                    HttpContent httpContent = new StringContent(serialized, Encoding.UTF8, "application/json");
                    var s = await client.PutAsync(uriLink.Replace(',', '.'), httpContent);
                    if (s.IsSuccessStatusCode)
                    {
                        var response = await s.Content.ReadAsStringAsync();
                        return response;
                    }
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}