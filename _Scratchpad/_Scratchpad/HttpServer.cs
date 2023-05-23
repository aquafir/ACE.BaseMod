using System;
using System.Net;
using System.IO;
using ACE.Server.Network.GameAction.Actions;

namespace _Scratchpad;

/// <summary>
/// Requires running as admin 
/// </summary>
public class HttpServer
{
    public int Port = 5050;

    private HttpListener _listener;

    public void Start()
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add("http://*:" + Port.ToString() + "/");
        _listener.Start();
        Receive();
    }

    public void Stop()
    {
        try
        {
            _listener?.Stop();
        }
        catch (Exception ex) { } //todo: fix
    }

    private void Receive()
    {
        _listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);
    }

    private void ListenerCallback(IAsyncResult result)
    {
        if (_listener.IsListening)
        {
            var context = _listener.EndGetContext(result);
            var request = context.Request;

            // do something with the request
            Console.WriteLine($"{request.Url}");
            if (request.HasEntityBody)
            {
                var body = request.InputStream;
                var encoding = request.ContentEncoding;
                var reader = new StreamReader(body, encoding);
                if (request.ContentType != null)
                {
                    Console.WriteLine("Client data content type {0}", request.ContentType);
                }
                Console.WriteLine("Client data content length {0}", request.ContentLength64);

                Console.WriteLine("Start of data:");
                string s = reader.ReadToEnd();
                Console.WriteLine(s);
                Console.WriteLine("End of data:");
                reader.Close();
                body.Close();
            }

            //var response = context.Response;
            //response.StatusCode = (int)HttpStatusCode.OK;
            //response.ContentType = "text/plain";
            //response.OutputStream.Write(new byte[] { }, 0, 0);
            //response.OutputStream.Close();


            // Check if the request is for a specific file
            Debugger.Break();
            if (request.Url.AbsolutePath == "/myfile.txt")
            {
                // Read the file contents
                var path = Path.Combine(Mod.ModPath, "myfile.txt");

                byte[] fileBytes;
                if (File.Exists(path))
                    fileBytes = File.ReadAllBytes(path);
                else
                    fileBytes = new byte[0];

                // Set the response headers
                HttpListenerResponse response = context.Response;
                response.ContentType = "text/plain";
                response.ContentLength64 = fileBytes.Length;
                response.AddHeader("Content-Disposition", "attachment; filename=myfile.txt");

                // Write the file to the response stream
                using (Stream outputStream = response.OutputStream)
                {
                    outputStream.Write(fileBytes, 0, fileBytes.Length);
                }
            }
            else
            {
                // Handle other requests or return an error response
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            context.Response.Close();

            Receive();
        }
    }
}