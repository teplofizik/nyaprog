using System;
using System.Diagnostics;

using Programmer.Platform;

namespace Programmer
{
    /// <summary>
    /// Реализация IProcessRunner поверх System.Diagnostics.Process.
    /// </summary>
    public sealed class SystemProcessRunner : IProcessRunner
    {
        public ProcessResult Run(string Program, string Arguments, string WorkingDir)
        {
            Log.WriteLine($"{Program} {Arguments} Dir: {WorkingDir}");

            var Info = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = Program,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                Arguments = Arguments,
                WorkingDirectory = WorkingDir
            };

            try
            {
                using (var P = Process.Start(Info))
                {
                    var Output = P.StandardOutput.ReadToEnd();
                    var Error = P.StandardError.ReadToEnd();
                    P.WaitForExit();

                    if (Output != null) Log.WriteLine(Output);
                    if (Error != null) Log.WriteLine(Error);

                    return new ProcessResult(P.ExitCode, $"{Output}\n{Error}");
                }
            }
            catch (Exception E)
            {
                Log.WriteLine(E.Message);
                return null;
            }
        }
    }
}
