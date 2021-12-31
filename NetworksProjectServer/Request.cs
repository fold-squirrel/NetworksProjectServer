using System;
using System.Collections.Generic;
using System.Linq;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        string[] requestline;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;
        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            Console.WriteLine("parsing");

            //TODO: parse the receivedRequest using the \r\n delimeter
            string[] delimeter = { "\r\n" };
            requestLines = requestString.Split(delimeter, StringSplitOptions.None);

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)

            // Parse Request line
            if (!ParseRequestLine())
            {
                return false;
            }
            Console.WriteLine("parsed Request Line, no errors occured");
            Console.WriteLine();
            // Validate blank line exists

            if (!ValidateBlankLine())
            {
                return false;
            }
            Console.WriteLine("blank line exist");
            Console.WriteLine();

            if (!LoadHeaderLines())
            {
                return false;
            }
            Console.WriteLine("header lines loaded into Dictionary");
            Console.WriteLine();



            string absoluteURI = "http://" + headerLines["Host"] + relativeURI;
          
            Console.WriteLine(relativeURI);
            if (!ValidateIsURI(absoluteURI))
            {
                return false;
            }
            Console.WriteLine("URI is valid");
            Console.WriteLine();

            return true;
        }

        private bool ParseRequestLine()
        {
            string[] delimeter = { " " };
            requestline = requestLines[0].Split(delimeter, StringSplitOptions.None);

            if (requestline[0].Equals("GET"))
            {
                relativeURI = requestline[1];
                if (requestline[2].Equals("HTTP/1.1"))
                {
                    return true;
                }
            }

            Console.WriteLine("error while parsing Request Line");
            return false;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.Absolute);
        }

        private bool LoadHeaderLines()
        {
            //throw new NotImplementedException();
            // Load header lines into HeaderLines dictionary

            headerLines = new Dictionary<string, string>();

            try

            {
                for (int i = 1; i < requestLines.Length; i++)
                {
                    string[] del = { ": " };
                    contentLines = requestLines[i].Split(del, 2, StringSplitOptions.None);
                    headerLines.Add(contentLines[0], contentLines[1]);
                    if (requestLines[i + 1].Equals(""))
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                Console.WriteLine("Exption occured while loading header lines into dictionary\nExption logged");
                return false;
            }

            return true;
        }

        private bool ValidateBlankLine()
        {
            //throw new NotImplementedException();

            if (requestLines.Last() == "")
            {
                return true;
            }

            Console.WriteLine("blank line doesn't exists");
            return false;
        }
    }
}
