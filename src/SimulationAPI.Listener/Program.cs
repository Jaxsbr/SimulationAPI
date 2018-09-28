using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace FakeApi.Listener
{
    class Program
    {
        static bool _listening = true;
        static HttpListener _httpListener;
        static string _url = "http://localhost:5001/";

        static void Main(string[] args)
        {
            Console.WriteLine($"Listening On {_url}");

            Task.Run(() => Listen());

            Console.Read();

            _httpListener.Stop();
        }

        private static void Listen()
        {
            _httpListener = new HttpListener();

            _httpListener.Prefixes.Add("http://localhost:5001/");

            _httpListener.Start();

            HandleGetContext();            
        }

        private static void HandleGetContext()
        {
            var result = _httpListener.BeginGetContext(new AsyncCallback(ListenerCallback), _httpListener);

            result.AsyncWaitHandle.WaitOne();

            if (!_listening)
            { 
                _httpListener.Stop();
                return;
            }

            HandleGetContext();
        }

        private static void ListenerCallback(IAsyncResult asyncResult)
        {
            Console.WriteLine("Packet Received");

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
