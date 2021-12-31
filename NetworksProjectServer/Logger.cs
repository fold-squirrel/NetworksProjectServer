using System;
using System.IO;

namespace HTTPServer
{

    class Logger

    {
        static StreamWriter sr = new StreamWriter("log.txt", true);
        public static void LogException(Exception ex)
        {

            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime 

            string Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            sr.WriteLine("DateTime: " + Date);

            string ExceptionMessage = ex.Message;

            sr.WriteLine("Message: " + ExceptionMessage + " \n");

            sr.Flush();

            Console.WriteLine("Logger Class : Exception recorded and saved in file log.txt\n");
        }
    }
}
