using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech;
using System.Speech.Synthesis;
using System.Threading;

namespace Aiden
{
    class Proto1 : Protocol
    {

        public Proto1()  : base("1") { }

        public override void execute(SpeechSynthesizer aiden, string[] args)
        {
            string pass = args[0];
            if(pass == "apple")
            {

                aiden.Speak(pass + " accepted ... Self destruct sequence initialized");
                System.Windows.Forms.Application.ExitThread();

            } else
            {
                aiden.Speak("WARNING: password required to activate Protocol one");
            }
        }
    }
}
