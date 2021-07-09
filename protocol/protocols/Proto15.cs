using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Diagnostics;

namespace Aiden
{
    public class Proto15 : Protocol
    {

        public Proto15() : base("15") { }

        public override void execute(Assistant aiden, string[] args)
        {
            Process.Start(@"cmd.exe", @"/c shutdown /s /t 15 /c ""The final net""");
            aiden.Speak("15");
            aiden.Speak("14");
            aiden.Speak("13");
            aiden.Speak("12");
            aiden.Speak("11");
            aiden.Speak("10");
            aiden.Speak("9");
            aiden.Speak("8");
            aiden.Speak("7");
            aiden.Speak("6");
            aiden.Speak("5");
            aiden.Speak("4");
            aiden.Speak("3");
            aiden.Speak("2");
            aiden.Speak("1");
            aiden.Disable();
        }

    }
}

