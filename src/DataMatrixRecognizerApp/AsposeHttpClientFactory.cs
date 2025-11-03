using System.Net;
using System.Net.Http.Headers;

namespace DataMatrixRecognizerApp;

public static class AsposeHttpClientFactory
{
    public static HttpClient Create()
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };

        var client = new HttpClient(handler, disposeHandler: true);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.Timeout = TimeSpan.FromSeconds(100);
        return client;
    }
}
