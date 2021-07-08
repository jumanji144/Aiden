using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.IO;
using System.Speech.AudioFormat;
using System.Diagnostics;
using System.Threading;

namespace Aiden
{
    public partial class Assistant : Form
    {

        private async void FadeIn(Form o, int interval = 80)
        {
            //Object is not fully invisible. Fade it in
            while (o.Opacity < 1.0)
            {
                await Task.Delay(interval);
                o.Opacity += 0.05;
            }
            o.Opacity = 1; //make fully visible       
        }

        private async void FadeOut(Form o, int interval = 80)
        {
            //Object is fully visible. Fade it out
            while (o.Opacity > 0.0)
            {
                await Task.Delay(interval);
                o.Opacity -= 0.05;
            }
            o.Opacity = 0; //make fully invisible       
        }

        SpeechRecognitionEngine _engine = new SpeechRecognitionEngine();
        SpeechSynthesizer aiden = new SpeechSynthesizer();
        SpeechRecognitionEngine start = new SpeechRecognitionEngine();
        Random rnd = new Random();
        List<Protocol> protocols = new List<Protocol>();
        int fadeTime = 10;

        public Assistant()
        {
            InitializeComponent();
        }

        private delegate void SafeCallDelegate();

        private void DisableGIF()
        {
            FadeOut(this, fadeTime);
        }

        string[] appNames = { "discord", "firefox", "chrome", "settings" };


        private void Form1_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.Opacity = 0;
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            this.Left = workingArea.Left + workingArea.Width - this.Size.Width;
            this.Top = workingArea.Top + workingArea.Height - this.Size.Height;
            protocols.Add(new Proto1());
            protocols.Add(new Proto2());
            protocols.Add(new Proto56());
            protocols.Add(new Proto15());
            protocols.Add(new ProtoChill());
            protocols.Add(new ProtoMarvin());

            // Initialize a new instance of the SpeechSynthesizer.  
            aiden.SelectVoice("Microsoft David Desktop");

                Grammar g;

            Choices commandtype = new Choices();
            commandtype.Add("fuck off");
            commandtype.Add("shut up");
            commandtype.Add("search");
            commandtype.Add("stop");
            commandtype.Add("what is adam");
            commandtype.Add("status");
            foreach(Protocol proto in protocols)
            {
                commandtype.Add("execute protocol " + proto.ident);
            }
            foreach(string app in appNames)
            {
                commandtype.Add("open " + app);
            }

            SemanticResultKey srkComtype = new SemanticResultKey("comtype", commandtype.ToGrammarBuilder());


            GrammarBuilder builder = new GrammarBuilder();
            builder.Culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");
            builder.Append(srkComtype);
            builder.AppendDictation();


            g = new Grammar(builder);

            _engine.SetInputToDefaultAudioDevice();
            _engine.LoadGrammarAsync(g);
            _engine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(onSpeechRec);
            _engine.SpeechDetected += new EventHandler<SpeechDetectedEventArgs>(onSpeechDetect);

            start.SetInputToDefaultAudioDevice();
            start.LoadGrammarAsync(new Grammar(new GrammarBuilder(new Choices("wake up aidan", "wake up assistant"))));
            start.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(onStartSpeechRec);
            start.RecognizeAsync(RecognizeMode.Multiple);


        }

        private void onSpeechRec(object sender, SpeechRecognizedEventArgs e)
        {
            string speech = e.Result.Text;

            Console.WriteLine(speech);


            if (speech == "status")
            {
                getStatus();
            }

            bool already = false;

            string[] split = speech.Split(' ');
            switch(split[0])
            {
                case "execute":
                    {
                        switch(split[1])
                        {
                            case "protocol":
                                {

                                    string name = split[2];

                                    foreach(Protocol proto in protocols)
                                    {
                                        if(proto.ident == name)
                                        {
                                            new Thread(() =>
                                            {
                                                proto.execute(aiden, split.Skip(3).ToArray());
                                                this.Invoke(new SafeCallDelegate(DisableGIF), new object[] { });
                                                already = true;
                                            }).Start();
                                            break;
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case "search":
                    {
                        Process.Start("firefox.exe", "duckduckgo.com/?q=" + speech.Substring("search ".Length).Replace(" ", "+"));
                        break;
                    }
                case "fuck":
                case "shut":
                case "stop":
                    {
                        break;
                    }

                case "open":
                    {

                        break;
                    }
                    

            }

            if (split[0] == "what" && split[1] == "is" && split[2] == "adam")
            {
                aiden.Speak("a retard");
            }

            if (!already)
            FadeOut(this, fadeTime);
            _engine.RecognizeAsyncCancel();
            start.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void getStatus()
        {

            aiden.SpeakAsync("Status: Normal");

        }

        private void onSpeechDetect(object sender, SpeechDetectedEventArgs e)
        {
        }

        private void onStartSpeechRec(object sender, SpeechRecognizedEventArgs e)
        {

            Console.WriteLine(e.Result.Text);

            if(e.Result.Text == "wake up aidan" || e.Result.Text == "wake up assistant")
            {

                start.RecognizeAsyncCancel();
                //search foraiden.SpeakAsync("I am here");
                _engine.RecognizeAsync(RecognizeMode.Multiple);
                FadeIn(this, fadeTime);

            }

        }

        
    }
}
