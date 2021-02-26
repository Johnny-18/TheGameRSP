using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using RSPGame.UI.Models;

namespace RSPGame.UI
{
    public static class RequestHandler
    {
        public static Response HandleRequest(HttpClient client, RequestOptions requestOptions)
        {
            if (requestOptions == null)
                throw new ArgumentNullException(nameof(requestOptions));
            if (!requestOptions.IsValid)
                throw new ArgumentOutOfRangeException(nameof(requestOptions));

            return GetResponse(client, requestOptions).Result;
        }

        private static async Task<Response> GetResponse(HttpClient client, RequestOptions requestOptions)
        {
            using var requestMessage = new HttpRequestMessage(MapMethods(requestOptions.Method),
                new Uri(requestOptions.Address));

            if (!string.IsNullOrEmpty(requestOptions.Body))
            {
                requestMessage.Content = new StringContent(requestOptions.Body, Encoding.UTF8, "application/json");
            }

            if (!string.IsNullOrEmpty(requestOptions.Token))
            {
                requestMessage.Headers.Authorization = AuthenticationHeaderValue.Parse(requestOptions.Token);
            }
            
            var httpResponseMessage = await client.SendAsync(requestMessage);
            var response = new Response
            {
                StatusCode = (int)httpResponseMessage.StatusCode,
                Content = await httpResponseMessage.Content.ReadAsStringAsync()
            };

            return response;
        }
        
        private static HttpMethod MapMethods(RequestMethod method)
        {
            switch (method)
            {
                case RequestMethod.Get:
                    return HttpMethod.Get;
                case RequestMethod.Delete:
                    return HttpMethod.Delete;
                case RequestMethod.Post:
                    return HttpMethod.Post;
                case RequestMethod.Put:
                    return HttpMethod.Put;
                case RequestMethod.Patch:
                    return HttpMethod.Patch;
                case RequestMethod.Undefined:
                    throw new ArgumentOutOfRangeException(nameof(method));
                default:
                    return null;
            }
        }
    }
}