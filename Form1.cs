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
using System.Speech.AudioFormat;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

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
        SpeechRecognitionEngine _callbackListener = new SpeechRecognitionEngine();
        Action<string> currentCallback;
        SpeechSynthesizer aiden = new SpeechSynthesizer();
        SpeechRecognitionEngine start = new SpeechRecognitionEngine();
        Random rnd = new Random();
        List<Protocol> protocols = new List<Protocol>();
        int fadeTime = 10;

        public Assistant()
        {
            InitializeComponent();
        }

        public delegate void SafeCallDelegate();

        private void DisableGIF()
        {
            Disable();
        }

        private Dictionary<string, string> pathCache = new Dictionary<string, string>();

        private void FindPathAndExecute(string basepath, string appname)
        {
            string path = "";

            if (!pathCache.ContainsKey(appname))
            {
                path = SysUtils.FindAppPath(@"%HOMEPATH%\AppData\Local", "discord");
                pathCache.Add(appname, path);
            }
            else
                pathCache.TryGetValue(appname, out path);
            if (path != "INFO: Could not find files for the given pattern(s).")
            {

                SysUtils.ExecuteCommandAsync(path);
            }else
            {
                aiden.SpeakAsync("Cant find " + appname);
            }
        }

        private void FindPathAndCache(string basepath, string appname)
        {
            new Thread(() =>
            {
                string path = "";
                 if (!pathCache.ContainsKey(appname))
                {
                    path = SysUtils.FindAppPath(@"%HOMEPATH%\AppData\Local", "discord");
                    pathCache.Add(appname, path);
                }
                Console.WriteLine("Finished Cachching path of " + appname);
            }).Start();
        }

        public void CacheAppPaths()
        {
            FindPathAndCache("%HOMEPATH%/AppData/Local", "discord");
        }

        public void Disable()
        {
            this.Invoke(new SafeCallDelegate(DisableSafe));
        }

        public void DisableSafe()
        {
            FadeOut(this, fadeTime);
        }

        string[] appNames = Properties.FileRef.appNames.Split(',');


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

            CacheAppPaths();

            // Initialize a new instance of the SpeechSynthesizer.  
            aiden.SelectVoiceByHints(VoiceGender.Male);

                Grammar g;
            Choices commandtype = new Choices();
            commandtype.Add(Properties.FileRef.commands.Split(','));
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

            GrammarBuilder english = new GrammarBuilder();
            english.Culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");
            english.AppendDictation();


            g = new Grammar(builder);

            _engine.SetInputToDefaultAudioDevice();
            _engine.LoadGrammarAsync(g);
            _engine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(onSpeechRec);

            start.SetInputToDefaultAudioDevice();
            start.LoadGrammarAsync(new Grammar(new GrammarBuilder(new Choices(Properties.FileRef.wakeUp.Split(',')))));
            start.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(onStartSpeechRec);
            start.RecognizeAsync(RecognizeMode.Multiple);

            _callbackListener.SetInputToDefaultAudioDevice();
            _callbackListener.LoadGrammarAsync(new Grammar(english));
            _callbackListener.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(callbackAccept);


        }

        private void callbackAccept(object sender, SpeechRecognizedEventArgs e)
        {
            if(currentCallback != null)
            {
                new Thread(() =>
                {
                    currentCallback.Invoke(e.Result.Text);
                    currentCallback = null;
                }).Start();
            }
            _callbackListener.RecognizeAsyncCancel();
        }

        public void AwaitSpeechResponse(Action<string> callback)
        {
            currentCallback = callback;
            _callbackListener.RecognizeAsync();
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool LockWorkStation();

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
                                            already = true;
                                            new Thread(() =>
                                            {
                                                proto.execute(this, split.Skip(3).ToArray());
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
                        switch(split[1])
                        {

                            case "discord":
                                {

                                    FindPathAndExecute(@"%HOMEPATH%\AppData\Local", "discord");
                                    break;
                                }

                        }
                        break;
                    }
                case "time":
                    {
                        aiden.SpeakAsync(DateTime.Now.ToString("h m tt"));
                        break;
                    }
                case "date":
                    {
                        aiden.SpeakAsync(DateTime.Now.ToString("dddd") + " the " + StringUtil.ordinal_suffix_of(DateTime.Now.Day) + " of " + DateTime.Now.ToString("MMMM"));
                        break;
                    }
                case "system":
                    {
                        switch(split[1])
                        {
                            case "lock":
                                {
                                    bool result = LockWorkStation();

                                    if (result == false)
                                    {
                                        // An error occured
                                       aiden.SpeakAsync("Error code: " + Marshal.GetLastWin32Error().ToString());
                                    }
                                    break;
                                }
                            case "sleep":
                                {
                                    bool retVal = Application.SetSuspendState(PowerState.Suspend, false, false);

                                    if (retVal == false)
                                        aiden.SpeakAsync("Could not suspend the system.");
                                    break;
                                }
                            case "hibernate":
                                {
                                    bool retVal = Application.SetSuspendState(PowerState.Hibernate, false, false);

                                    if (retVal == false)
                                        aiden.SpeakAsync("Could not hibernate the system.");
                                    break;
                                }
                        }
                        break;
                    }

            }

            if (split[0] == "what" && split[1] == "is" && split[2] == "adam")
            {
                aiden.SpeakAsync("a retard");
            }

            if (!already)
                Disable();
            _engine.RecognizeAsyncCancel();
            start.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void getStatus()
        {

            aiden.SpeakAsync("Status: Normal");

        }

        public void Speak(string text)
        {
            aiden.Speak(text);
        }

        public void SpeakAsync(string text)
        {
            aiden.SpeakAsync(text);
        }

        private void onStartSpeechRec(object sender, SpeechRecognizedEventArgs e)
        {

            Console.WriteLine(e.Result.Text);

            if(Properties.FileRef.wakeUp.Split(',').Contains(e.Result.Text))
            {

                start.RecognizeAsyncCancel();
                _engine.RecognizeAsync(RecognizeMode.Multiple);
                FadeIn(this, fadeTime);

            }

        }

        
    }
}
