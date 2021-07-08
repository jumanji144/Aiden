using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace Aiden
{
    public class ProtoMarvin : Protocol
    {

        public ProtoMarvin() : base("marvin") { }

        public override void execute(SpeechSynthesizer aiden, string[] args)
        {
            Process.Start("firefox.exe", "https://www.youtube.com/watch?v=NkdpBWzb2hw");
        }
    }
}