using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        bool secure;

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
            ///////////////
            Console.WriteLine("\n::::::::::::::::::::::::");
            Console.WriteLine(requestString);
            Console.WriteLine("::::::::::::::::::::::::\n");
            ///////////////

            //TODO: parse the receivedRequest using the \r\n delimeter
            string[] delimeter = { "\r\n" };
            requestLines = requestString.Split(delimeter, StringSplitOptions.None);

            ///////////////
            int iii = 0;
            foreach (string line in requestLines)
            {
                ++iii;
                Console.WriteLine("line: " + iii.ToString() + " > " + line);
            }
            Console.WriteLine();
            ///////////////

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

            if (secure)
            {
                relativeURI = "https://" + headerLines["Host"] + relativeURI;
            }
            else
            {
                relativeURI = "http://" + headerLines["Host"] + relativeURI;
            }
            Console.WriteLine(relativeURI);
            if (!ValidateIsURI(relativeURI))
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
            ///////////////
            Console.WriteLine("Request line content");
            foreach (string element in requestline)
            {
                Console.WriteLine("> " + element);
            }
            Console.WriteLine("end Request line");
            ///////////////
            if (requestline[0].Equals("GET"))
            {
                relativeURI = requestline[1];
                if (requestline[2].Equals("HTTP/1.1"))
                {
                    secure = false;
                    return true;
                }
                if (requestline[2].Equals("HTTP/2"))
                {
                    secure = true;
                    return true;
                }
            }

            Console.WriteLine("error while parsing Request Line");
            return false;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
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
                    string[] headline = requestLines[i].Split(del, 2, StringSplitOptions.None);
                    ////////////////////
                    Console.WriteLine("Dictionary Key:    " + headline[0] + "\nDictionary Value:  " + headline[1] + "\n");
                    ////////////////////
                    headerLines.Add(headline[0], headline[1]);
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
