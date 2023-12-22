using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AdventOfCode23
{
    internal class Day20Puzzle : PuzzleBase
    {
        internal static Dictionary<string, Module> modules = new Dictionary<string, Module>();
        internal static List<(string source, string destination, char highOrLow)> unhandledPulses = new List<(string source, string destination, char highOrLow)>();
        internal static int _highs, _lows;
        internal static void Do(bool example)
        {
            var lines = ReadLines(20, example);


            foreach (var line in lines)
            {
                if (line.StartsWith("%"))
                {
                    var name = line.Substring(1, line.IndexOf(' ')).Trim();
                    var list = line.Substring(line.IndexOf("->") + 2);
                    var targets = list.Split(',').Select(x => x.Trim()).ToList();
                    modules.Add(name, new FlipFlop(name, targets));
                }
                if (line.StartsWith("&"))
                {
                    var name = line.Substring(1, line.IndexOf(' ')).Trim();
                    var list = line.Substring(line.IndexOf("->") + 2);
                    var targets = list.Split(',').Select(x => x.Trim()).ToList();
                    modules.Add(name, new Conjunction(name, targets));
                }
                if (line.StartsWith("broadcaster"))
                {
                    var name = line.Substring(0, line.IndexOf(' ')).Trim();
                    var list = line.Substring(line.IndexOf("->") + 2);
                    var targets = list.Split(',').Select(x => x.Trim()).ToList();
                    modules.Add(name, new Broadcaster(name, targets));
                }
            }

            List<string> outputs = new List<string>();
            // Wire up the conjunctions
            foreach (var m in modules.Keys)
            {
                foreach (var name in modules[m]._outputs)
                {
                    if (!modules.ContainsKey(name))
                        // Doesn't appear as any input - "output" in the example but "rx" in real data
                        outputs.Add(name);
                    else if (modules[name] is Conjunction conjunction)
                        conjunction.AddInput(m);
                }
            }

            foreach (var name in outputs)
                modules.Add(name, new Output());

            for (int i = 1; i <= 1000; i++)
            {
                PushButton();
            }

            Console.WriteLine($"After 1000: Highs: {_highs} Lows: {_lows} Product: {_highs * _lows}");

            // Naive part 2 - it looks easy if done as below, but it will take a ludicrously
            // long time (didn't finish in hours). However, we can't look for cycles in the
            // states of the modules as in other puzzles, as we want the *first* occurrence. 
            var output = modules.Values.First(x => x is Output) as Output;
            int i1 = 1000;
            while (!output.HasReceivedL && i1 <= 4000)
            {
                PushButton();
                Console.WriteLine(i1);
                WriteRegisters(modules);
                i1++;
            }
            Console.WriteLine($"Got first L after {i1} pushes");

            /*
At this stage it was obvious that we're not going to calculate an answer in a sensible time.
So, I resorted to mapping out the modules, and it is most definitely NOT an accident:

broadcast -> %sj -> %kj -> %fk -> %xh -> %zs -> %ct -> %rt -> %hq -> %bb -> %kf -> %ph -> hx -> &rb
				 ,   rb                  rb            rb                    rb    rb    rb
				 
             %sr -> %vs -> %tq -> %jm -> kp -> vk -> tk -> sh -> zk > ps -> qz -> kh -> &ml
			         ml     ml           ml                           ml    ml    ml
					 
			 tp ->  %lx -> ff -> df -> nv -> xm -> cq -> mq -> pr -> pf _> nt -> gs -> gp
					gp     gp    gp                gp    gp          gp    gp    gp
					
			 %nk -> nn -> xf -> qr -> zt -> pb -> kq -> tl -> bn -> sv -> dx -> tz -> bt
   			        bt    bt          bt    bt          bt          bt    bt    bt
					
			bt -> %vg -> lg ->
			ml -> %nb -> lg -> %rx
			rb -> %vc -> lg ->
			gp -> %ls -> lg ->

I confirmed my suspicion that the sequences SJ-KJ-FK-XH-ZS-CT-RT-HQ-BB-KF-PH-HX etc act like
four counters here, with the four flip-flops next to "broadcaster" flipping every time,
the next ones in the chain flipping on alternate pushes, and so on. RB fires when the SJ
counter reaches 111100101001 = 3881, and confirmed its consequences are (i) to reset SJ etc
back to 0 and (ii) flip VG. The other three do similar at 3851, 3943, 3931. Which are all 
coprime. So the four inputs to LG all fire off together every 3881*3851*3943*3931 button
presses = 231,657,829,136,023
            */
        }

        private static void WriteRegisters(Dictionary<string, Module> modules)
        {
            var sj = ((modules["sj"] as FlipFlop).IsOn ? 1 : 0)
                + ((modules["kj"] as FlipFlop).IsOn ? 2 : 0)
                + ((modules["fk"] as FlipFlop).IsOn ? 4 : 0)
                + ((modules["xh"] as FlipFlop).IsOn ? 8 : 0)
                + ((modules["zs"] as FlipFlop).IsOn ? 16 : 0)
                + ((modules["ct"] as FlipFlop).IsOn ? 32 : 0)
                + ((modules["rt"] as FlipFlop).IsOn ? 64 : 0)
                + ((modules["hq"] as FlipFlop).IsOn ? 128 : 0)
                + ((modules["bb"] as FlipFlop).IsOn ? 256 : 0)
                + ((modules["kf"] as FlipFlop).IsOn ? 512 : 0)
                + ((modules["ph"] as FlipFlop).IsOn ? 1024 : 0)
                + ((modules["hx"] as FlipFlop).IsOn ? 2048 : 0);
            Console.WriteLine("SJ register: " + sj);
        }

        private static void PushButton()
        {
            // Fire a pulse at the broadcaster
            var broadcaster = modules["broadcaster"];
            _lows++;
            unhandledPulses.AddRange(broadcaster.HandleInput("me", 'L'));
            while (unhandledPulses.Count > 0)
            {
                var first = unhandledPulses.First();
                if (first.highOrLow == 'H') _highs++;
                else _lows++;
                unhandledPulses.AddRange(modules[first.destination].HandleInput(first.source, first.highOrLow));
                unhandledPulses.RemoveAt(0);
            }
        }
    }

    internal abstract class Module
    {
        internal string _name;
        internal int? _depth;
        internal List<string> _outputs = new List<string>();
 
        internal abstract IEnumerable<(string, string, char)> HandleInput(string sender, char highOrLow);
    }

    internal class Broadcaster : Module
    {
        public Broadcaster(string name, List<string> targets)
        {
            _name = name;
            _outputs = targets;
        }

        internal override IEnumerable<(string, string, char)> HandleInput(string sender, char highOrLow)
        {
            // Sends the same to all its outputs
            foreach (string name in _outputs)
                yield return (_name, name, highOrLow);
        }
    }

    internal class FlipFlop : Module
    {
        private bool _isOn = false;
        public FlipFlop(string name, List<string> targets)
        {
            _name = name;
            _outputs = targets;
        }
        internal override IEnumerable<(string, string, char)> HandleInput(string sender, char highOrLow)
        {
            // Receives high - does nothing. Receives low - flips state and sends
            if (highOrLow == 'L')
            {
                _isOn = !_isOn;
                foreach (string name in _outputs)
                    yield return (_name, name, (_isOn ? 'H' : 'L'));
            }
        }

        public bool IsOn => _isOn;
    }

    internal class Conjunction : Module
    {
        private Dictionary<string, char> _inputs = new Dictionary<string, char>();
        public Conjunction(string name, List<string> targets)
        {
            _name = name;
            _outputs = targets;
        }

        public void AddInput(string name)
        {
            _inputs.Add(name, 'L');
        }

        internal override IEnumerable<(string, string, char)> HandleInput(string sender, char highOrLow)
        {
            // Receives anything - updates dictionary, then sends L if all H's, or H otherwise
            _inputs[sender] = highOrLow;
            var valueToSend = (_inputs.Values.Any(x => x == 'L')) ? 'H' : 'L';

            foreach (string name in _outputs)
                    yield return (_name, name, valueToSend);
        }
    }

    internal class Output : Module
    {
        private bool _hasReceivedL = false;
        internal override IEnumerable<(string, string, char)> HandleInput(string sender, char highOrLow)
        {
            if (highOrLow == 'L')
                _hasReceivedL = true;

            return Enumerable.Empty<(string, string, char)>();
        }

        public bool HasReceivedL => _hasReceivedL;
    }
}
