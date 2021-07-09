using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace Aiden
{
    public class ProtoChill : Protocol
    {

        public ProtoChill() : base("chill") { }

        public override void execute(Assistant aiden, string[] args)
        {
            Process.Start("firefox.exe", "https://www.youtube.com/watch?v=P5ByrJVgaeU");
            aiden.Disable();
        }
    }
}
