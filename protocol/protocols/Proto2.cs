using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Diagnostics;

namespace Aiden
{
    class Proto2 : Protocol
    {

        public Proto2() : base("2") { }

        public override void execute(SpeechSynthesizer aiden, string[] args)
        {
            Process.Start("firefox.exe", "https://www.youtube.com/watch?v=zkU5JYX-bIc&list=PLX-KY8nhnd1Lbh76AnE3c3tX36rieoxE1&index=1");
        }
    }
}
