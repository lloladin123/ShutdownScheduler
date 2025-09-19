using System;
using System.Diagnostics;

namespace ShutdownScheduler
{
    public class SchedulerService
    {
        public bool TestMode { get; set; } = false; // ✅ Toggle this for dry-run vs real

        public void RunShutdown(string arguments) =>
            RunCommand("shutdown", arguments);

        public void RunSchtasks(string arguments) =>
            RunCommand("schtasks", arguments);

        public string RunSchtasksCapture(string arguments) =>
            RunCommand("schtasks", arguments, captureOutput: true);

        private string RunCommand(string fileName, string arguments, bool captureOutput = false)
        {
            if (TestMode)
            {
                Console.WriteLine($"[TEST MODE] Would run: {fileName} {arguments}");
                return string.Empty;
            }

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardOutput = captureOutput,
                    RedirectStandardError = captureOutput,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = captureOutput ? process.StandardOutput.ReadToEnd() : string.Empty;
            string error = captureOutput ? process.StandardError.ReadToEnd() : string.Empty;
            process.WaitForExit();

            if (!string.IsNullOrWhiteSpace(error))
                Console.WriteLine($"⚠️ {fileName} error: {error.Trim()}");

            return output;
        }
    }
}
