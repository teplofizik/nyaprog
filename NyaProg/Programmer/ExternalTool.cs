using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Programmer
{
    class ExternalTool
    {
        static public ExternalToolResult Run(string File, string Args, string Dir)
        {
            Console.WriteLine($"{File} {Args} Dir: {Dir}");
            Log.WriteLine($"{File} {Args} Dir: {Dir}");

            //string ProgramPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //string D = Directory.GetCurrentDirectory();

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = File;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = Args;
            startInfo.WorkingDirectory = Dir;

            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    string output = exeProcess.StandardOutput.ReadToEnd();
                    string error = exeProcess.StandardError.ReadToEnd();
                    exeProcess.WaitForExit();

                    List<string> textout = new List<string>();
                    if (output != null)
                    {
                        textout.Add(output);
                        Log.WriteLine(output);
                    }

                    if (error != null)
                    {
                        textout.Add(error);
                        Log.WriteLine(error);
                    }

                    return new ExternalToolResult(exeProcess.ExitCode, textout.ToArray());
                }
            }
            catch (Exception E)
            {
                Log.WriteLine(E.Message);
            }

            return null;
        }
    }

    public class ExternalToolResult
    {
        public string[] Lines;

        public int ExitCode;

        public ExternalToolResult(int Code, string[] Text)
        {
            ExitCode = Code;
            Lines = Text;
        }

        public string Result => String.Join("\n", Lines);
    }
}
