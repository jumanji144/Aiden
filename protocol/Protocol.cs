using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;

namespace Aiden
{
    public abstract class Protocol
    {

        public Protocol(string ident)
        {
            this.ident = ident;
        }

        public string ident { get; set; }

        public abstract void execute(Assistant aiden, string[] args);

    }
}
