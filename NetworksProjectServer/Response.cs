using System;
using System.Collections.Generic;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            // throw new NotImplementedException();

            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])

            this.code = code;
            headerLines.Add(GetStatusLine());
            if (code == StatusCode.Redirect)
            {
                headerLines.Add("Location: " + redirectoinPath);
            }
            headerLines.Add("Date: " + DateTime.Now.ToUniversalTime().ToString("r"));
            headerLines.Add("Content-Type: " + contentType);
            headerLines.Add("Content-Length: " + content.Length.ToString());
            headerLines.Add("Connection: close");
            headerLines.Add("");
            headerLines.Add(content);


            // TODO: Create the request string
            responseString = "";
            for (int i = 0; i < headerLines.Count - 1; i++)
            {
                responseString += headerLines[i] + "\r\n";
            }
            responseString += headerLines[headerLines.Count - 1];
        }

        private string GetStatusLine()
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;
            statusLine += "HTTP/1.1";

            switch (code)
            {
                case StatusCode.OK:
                    statusLine += " 200 OK";
                    break;
                case StatusCode.Redirect:
                    statusLine += " 301 Redirect";
                    break;
                case StatusCode.BadRequest:
                    statusLine += " 400 BadRequest";
                    break;
                case StatusCode.NotFound:
                    statusLine += " 404 Not Found";
                    break;
                case StatusCode.InternalServerError:
                    statusLine += " 500 Internal Server Error";
                    break;
                default:
                    Console.WriteLine("Status Code not supported");
                    break;
            }

            return statusLine;

        }
    }
}

