using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace FakeApi.Listener
{
    class Program
    {
        static bool _listening = true;

        static void Main(string[] args)
        {
            Task.Run(() => Listen());

            Console.Read();
        }

        private static void Listen()
        {
            var httpListener = new HttpListener();

            httpListener.Prefixes.Add("http://localhost:8080/");

            httpListener.Start();

            while (_listening)
            {
                httpListener.BeginGetContext(new AsyncCallback(ListenerCallback), httpListener);
            }

            httpListener.Stop();
        }

        private static void ListenerCallback(IAsyncResult asyncResult)
        {
            var httpListener = (HttpListener)asyncResult.AsyncState;

            var context = httpListener.EndGetContext(asyncResult);

            var request = context.Request;

            var response = context.Response;

            var headerValue = context.Request.Headers.GetValues("IsValid");

            var isValid = context.Request.Headers["IsValid"];

            if (string.IsNullOrWhiteSpace(isValid))
            {
                response.StatusCode = 400; // Bad Request

                SendResponse(response, GetResponseString(ResponseTypes.InvalidHeader));

                return;
            }

            if (isValid.ToLower() == "y")
            {
                SendResponse(response, GetResponseString(ResponseTypes.ValidToken));
            }
            else
            {
                SendResponse(response, GetResponseString(ResponseTypes.InvalidToken));
            }            
        }

        private static string GetResponseString(ResponseTypes responseType)
        {
            switch (responseType)
            {
                case ResponseTypes.InvalidHeader:
                    return ObjectToJsonString(new InvalidRequest());
                case ResponseTypes.ValidToken:
                    return ObjectToJsonString(new Token(true));
                case ResponseTypes.InvalidToken:
                    return ObjectToJsonString(new Token(false));                
            }

            throw new InvalidOperationException("Invalid response type used");
        }

        private static string ObjectToJsonString<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        private static void SendResponse(HttpListenerResponse response, string responseString)
        {
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

            response.ContentLength64 = buffer.Length;

            var output = response.OutputStream;

            output.Write(buffer, 0, buffer.Length);

            output.Close();
        }
    }
}
