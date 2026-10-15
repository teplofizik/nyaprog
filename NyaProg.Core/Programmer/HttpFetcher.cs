using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

using Programmer.Platform;

namespace Programmer
{
    /// <summary>
    /// Реализация IHttpFetcher поверх System.Net.Http.
    /// </summary>
    public sealed class HttpFetcher : IHttpFetcher
    {
        private static readonly HttpClient Client = new HttpClient();

        public string GetText(string Url, string Method = "GET", IReadOnlyDictionary<string, string> Headers = null)
        {
            var Bytes = GetBytes(Url, Method, Headers);
            return (Bytes != null) ? Encoding.UTF8.GetString(Bytes) : null;
        }

        public byte[] GetBytes(string Url, string Method = "GET", IReadOnlyDictionary<string, string> Headers = null)
        {
            try
            {
                var M = string.IsNullOrEmpty(Method) ? "GET" : Method;

                using (var Req = new HttpRequestMessage(new HttpMethod(M), Url))
                {
                    if (Headers != null)
                    {
                        foreach (var H in Headers)
                            Req.Headers.TryAddWithoutValidation(H.Key, H.Value);
                    }

                    using (var Resp = Client.SendAsync(Req).GetAwaiter().GetResult())
                    {
                        if (!Resp.IsSuccessStatusCode)
                        {
                            Log.WriteLine($"HTTP {(int)Resp.StatusCode} {Url}");
                            return null;
                        }

                        return Resp.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                    }
                }
            }
            catch (Exception E)
            {
                Log.WriteLine(E.Message);
                return null;
            }
        }
    }
}
