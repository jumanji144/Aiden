using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Aiden
{
    public class SysUtils
    {

        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        private const int WM_APPCOMMAND = 0x319;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg,
            IntPtr wParam, IntPtr lParam);

        public static void Mute(Form form)
        {
            SendMessageW(form.Handle, WM_APPCOMMAND, form.Handle,
                (IntPtr)APPCOMMAND_VOLUME_MUTE);
        }

        public static void VolDown(Form form)
        {
            SendMessageW(form.Handle, WM_APPCOMMAND, form.Handle,
                (IntPtr)APPCOMMAND_VOLUME_DOWN);
        }

        public static void VolUp(Form form)
        {
            SendMessageW(form.Handle, WM_APPCOMMAND, form.Handle,
                (IntPtr)APPCOMMAND_VOLUME_UP);
        }

        public enum VolumeUnit
        {
            //Perform volume action in decibels</param>
            Decibel,
            //Perform volume action in scalar
            Scalar
        }

        /// <summary>
        /// Gets the current volume
        /// </summary>
        /// <param name="vUnit">The unit to report the current volume in</param>
        [DllImport("SysVolUtil.dll")]
        public static extern float GetSystemVolume(VolumeUnit vUnit);
        /// <summary>
        /// sets the current volume
        /// </summary>
        /// <param name="newVolume">The new volume to set</param>
        /// <param name="vUnit">The unit to set the current volume in</param>
        [DllImport("SysVolUtil.dll")]
        public static extern void SetSystemVolume(double newVolume, VolumeUnit vUnit);


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

        public static IEnumerable<T> CreateAllInstancesOf<T>()
        {
            return typeof(SysUtils).Assembly.GetTypes()
                .Where(t => typeof(T).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract && t.IsClass)
                .Select(t => (T)Activator.CreateInstance(t));
        }

    }
}
