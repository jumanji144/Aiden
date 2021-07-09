using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aiden
{
    public class SysUtils
    {

        public static void ExecuteCommandAsync(object command)
        {
            new Thread(() =>
            {
                ExecuteCommandSync(command);
            }).Start();
        }
        public static string ExecuteCommandSync(object command)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                return result;
            }
            catch (Exception objException)
            {
                // Log the exception
            }
            return "";
        }

        public static string FindAppPath(string basepath, string appname)
        {

            string res = SysUtils.ExecuteCommandSync("where /R " + basepath + " " + appname);

            return res.Split('\n')[0];

        }

    }
}
