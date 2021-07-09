using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace Aiden
{
    public class Proto56 : Protocol
    {

        public Proto56() : base("56") { }

        public override void execute(Assistant aiden, string[] args)
        {
            Process.Start(@"C:\Program Files (x86)\Ubisoft\Ubisoft Game Launcher\upc.exe");
            aiden.Disable();
        }
    }
}
