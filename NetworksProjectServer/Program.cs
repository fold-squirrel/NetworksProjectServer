using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();
            Server s = new Server(1000, "~/");
            s.StartServer();
            //Start server
            // 1) Make server object on port 1000
            // 2) Start Server
        }

        static void CreateRedirectionRulesFile()
        {
            StreamWriter sr = new StreamWriter("redirectionRules.txt");
            sr.WriteLine("aboutus.html,aboutus2.html");
            sr.Close();
            //FileStream sr = File.Create("redirectionRules.txt");
            //sr.Write("aboutus.html,aboutus2.html");
            // TODO: Create file named redirectionRules.txt
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2
        }
         
    }
}
