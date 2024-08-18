using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Programmer
{
    static class Log
    {
        public static string LogFileName = "log.txt";

        public static void WriteLine(string Text)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(LogFileName))
                {
                    //Debug.WriteLine(Text);
                    sw.WriteLine(Text);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void WriteLines(string[] Lines)
        {
            if (Lines == null) return;

            try
            {
                using (StreamWriter sw = File.AppendText(LogFileName))
                {
                    foreach (string Line in Lines)
                    {
                        //Debug.WriteLine(Line);
                        sw.WriteLine(Line);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
