using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            this.LoadRedirectionRules(redirectionMatrixPath);
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint hostEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket.Bind(hostEndPoint);

            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            //TODO: initialize this.serverSocket
        }

        public void StartServer()
        {
            serverSocket.Listen(100);

            while (true)
            {
                Socket clientSocket = this.serverSocket.Accept();
                Console.WriteLine("Connected...");
                Console.WriteLine("New client accepted: {0}", clientSocket.RemoteEndPoint);
                //HandleConnection(clientSocket);
                ThreadPool.QueueUserWorkItem(new WaitCallback(HandleConnection), clientSocket);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period

            // TODO: receive requests in while true until remote client closes the socket.
            Socket clientSock = (Socket)obj;
            clientSock.ReceiveTimeout = 0;
            byte[] data = new byte[1024];
            int receivedLength;

            while (true)
            {
                try
                {
                    data = new byte[1024];
                    receivedLength = clientSock.Receive(data);
                    string message = Encoding.ASCII.GetString(data, 0, receivedLength);

                    if (receivedLength == 0)
                    {
                        Console.WriteLine("Client: {0} ended the connection", clientSock.RemoteEndPoint);
                        break;
                    }
                    Console.WriteLine("Received: {0} from Client: {1}", Encoding.ASCII.GetString(data, 0, receivedLength), clientSock.RemoteEndPoint);

                    //clientSock.Send(data, 0, receivedLength, SocketFlags.None);

                    // TODO: Receive request

                    // TODO: break the while loop if receivedLen==0
                    Request req = new Request(message);
                    // TODO: Create a Request object using received request string
                    Response response = HandleRequest(req);
                    // TODO: Call HandleRequest Method that returns the response
                    byte[] finalres = Encoding.ASCII.GetBytes(response.ResponseString);
                    clientSock.Send(finalres);
                    // TODO: Send Response back to client

                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
            clientSock.Close();
            // TODO: close client socket
        }

        Response HandleRequest(Request request)
        {
            string content;
            string pathToFiles;
            Response r = null;
            try
            {
                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    pathToFiles = Path.Combine(Configuration.RootPath, Configuration.BadRequestDefaultPageName);
                    content = LoadDefaultPage(pathToFiles);
                    r = new Response(StatusCode.BadRequest, "text/html; charset=UTF-8", content, pathToFiles);
                    return r;
                }
                else
                {
                    //TODO: map the relativeURI in request to get the physical path of the resource.
                    string filename = request.relativeURI;
                    string[] sub2 = filename.Split(new char[] { '/' });
                    sub2[1] = sub2[1].Trim();
                    filename = sub2[1];
                    pathToFiles = Path.Combine(Configuration.RootPath, filename);
                    //TODO: check for redirect
                    if (filename.Equals(""))
                    {
                        content = LoadDefaultPage("default.html");
                        r = new Response(StatusCode.OK, "text/html; charset=UTF-8", content, String.Empty);
                        return r;
                    }
                    if (GetRedirectionPagePathIFExist(filename) != String.Empty)
                    {
                        filename = GetRedirectionPagePathIFExist(filename);
                        pathToFiles = Path.Combine(Configuration.RootPath, filename);
                        content = File.ReadAllText(pathToFiles);
                        r = new Response(StatusCode.Redirect, "text/html; charset=UTF-8", content, pathToFiles);
                    }
                    //bool test=File.Exists(pathToFiles);
                    if (!File.Exists(pathToFiles))
                    {
                        pathToFiles = Path.Combine(Configuration.RootPath, Configuration.NotFoundDefaultPageName);
                        content = File.ReadAllText(pathToFiles);
                        r = new Response(StatusCode.NotFound, "text/html; charset=UTF-8", content, pathToFiles);
                    }
                    else
                    {
                        pathToFiles = Path.Combine(Configuration.RootPath, filename);
                        content = File.ReadAllText(pathToFiles);
                        r = new Response(StatusCode.OK, "text/html; charset=UTF-8", content, pathToFiles);
                    }
                    //TODO: check file exists

                    //TODO: read the physical file

                    // Create OK response
                    if (r == null)
                    {
                        pathToFiles = Path.Combine(Configuration.RootPath, Configuration.InternalErrorDefaultPageName);
                        content = File.ReadAllText(Configuration.RootPath + "\\InternalError.html");
                        r = new Response(StatusCode.InternalServerError, "text/html; charset=UTF-8", content, pathToFiles);
                    }
                }
            }
            catch (Exception ex)
            {
                pathToFiles = Path.Combine(Configuration.RootPath, Configuration.InternalErrorDefaultPageName);
                content = File.ReadAllText(Configuration.RootPath + "\\InternalError.html");
                r = new Response(StatusCode.InternalServerError, "text/html; charset=UTF-8", content, pathToFiles);
                Logger.LogException(ex);

                // TODO: log exception using Logger class
                // TODO: in case of exception, return Internal Server Error. 
            }
            return r;
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            foreach (var redirect in Configuration.RedirectionRules)
            {
                if (relativePath == redirect.Key)
                {
                    return redirect.Value;
                }
            }
            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            String content;
            if (!File.Exists(filePath))
            {
                FileNotFoundException ex = new FileNotFoundException();
                Logger.LogException(ex);
                return String.Empty;
            }
            else
            {
                content = File.ReadAllText(filePath);
            }
            // else read file and return its content
            return content;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                String[] rules = File.ReadAllLines("redirectionRules.txt");
                Configuration.RedirectionRules = new Dictionary<string, string>();
                foreach (String rule in rules)
                {
                    string[] sub = rule.Split(',');
                    sub[1] = sub[1].Trim();
                    Configuration.RedirectionRules.Add(sub[0], sub[1]);
                }
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary 
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);

                // TODO: log exception using Logger class
                Environment.Exit(1);
            }
        }
    }
}
