using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Web;

namespace Nexus.Github.Client;
public class OAuth
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    private readonly string requestUrl;
    private const string htmlClose = "<!DOCTYPE html><html><head></head><body onload=\"window.close()\">You can now close this page!</body></html>";
    public string UserCode { get; set; }
    public OAuth(string clientId, string clientSecret)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;

        requestUrl = $"https://github.com/login/oauth/authorize?client_id={ClientId}";
    }

    public async Task RequestLoginAsync(TimeSpan maxAwait)
    {
        Task awaitResponse = Task.Run(() =>
        {
            AwaitResponse("http://localhost:1337/", htmlClose);
        });

        ProcessStartInfo info = new(requestUrl);
        info.UseShellExecute = true;

        Process.Start(info);
        await awaitResponse.WaitAsync(maxAwait);
    }

    private void AwaitResponse(string prefix, string serverResponse)
    {
        bool authenticated = false;
        HttpListener listener = new();
        listener.Prefixes.Add(prefix);
        listener.Start();

        while (!authenticated)
        {
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;


            //
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(serverResponse);
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();

            Uri? uri = request.Url;

            if (uri == null)
                break;

            if (uri.LocalPath != "/Authentication/Github/Callback")
                break;

            NameValueCollection queryCollection = HttpUtility.ParseQueryString(uri.Query);

            if (queryCollection == null)
                break;

            string code = queryCollection["code"];

            if (code == null)
                break;

            UserCode = code;
            authenticated = true;
        }

        listener.Stop();
    }
}
