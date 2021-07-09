using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech;
using System.Speech.Synthesis;
using System.Threading;
using static Aiden.Assistant;

namespace Aiden
{
    class Proto1 : Protocol
    {

        public Proto1()  : base("1") { }

        public override void execute(Assistant aiden, string[] args)
        {
            aiden.Speak("A password is required for protocol 1");
            aiden.AwaitSpeechResponse((res) =>
            {
                if (res == Properties.FileRef.proto1pass)
                {

                    aiden.Speak(res + " accepted ... Self destruct sequence initialized");
                    aiden.Invoke(new SafeCallDelegate(System.Windows.Forms.Application.ExitThread));
                    return;

                }
                else
                {
                    aiden.Speak("Wrong password... Aborting protocol 1");
                }
                aiden.Disable();
            });
        }
    }
}
