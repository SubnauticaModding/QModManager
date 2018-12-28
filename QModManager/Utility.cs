using System;
using System.Reflection;

namespace QModManager.Utility
{
    public static class ExceptionUtils
    {
        public static void OutputInnerExceptionRecursively(Exception e)
        {
            if (e.InnerException != null)
            {
                Console.WriteLine("Inner exception:");
                Console.WriteLine(e.InnerException.Message);
                Console.WriteLine(e.InnerException.StackTrace);
                OutputInnerExceptionRecursively(e.InnerException);
            }
        }

        public static void ParseException(Exception e, bool exit = true)
        {
            Console.WriteLine();
            Console.WriteLine("Uh-oh. An exception has occurred. This is not a good thing.");
            Console.WriteLine("Please take a screenshot and open a bug report on nexus or an issue on github");
            Console.WriteLine();
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            OutputInnerExceptionRecursively(e);
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            if (exit) Environment.Exit(0);
        }
    }
}