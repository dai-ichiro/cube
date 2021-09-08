using System;
using System.Diagnostics;
using System.Linq;

namespace main_exe
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Loading pretrained data...");

            Global.cp_move_table = File_Operation.read_array("./data/cp_move_table.data");
            Global.co_move_table = File_Operation.read_array("./data/co_move_table.data");
            Global.eo_move_table = File_Operation.read_array("./data/eo_move_table.data");

            Global.cp_co_prune_table = File_Operation.read_array("./data/cp_co_prune_table.data");
            Global.cp_eo_prune_table = File_Operation.read_array("./data/cp_eo_prune_table.data");
            Global.co_eo_prune_table = File_Operation.read_array("./data/co_eo_prune_table.data");

            Global.ep_dict = File_Operation.read_dict("./data/ep_dict.data");

            string scramble;
            //scramble = "R' U' F R' B' F2 L2 D' U' L2 F2 D' L2 D' R B D2 L D2 F2 U2 L R' U' F";
            //scramble = "R' U' F R' B' F2 L2 D' U' L2 F2 D' L2 D' R B";
            //scramble = "R' U' F R' B' F2 L2 D' U' L2 F2 D' L2 D' R";
            scramble = "R' U' F R' B' F2 L2 D' U' L2 F2 D' L2";
            //scramble = "R' U' F R' B' F2 L2 D' U' L2 F2 D'";
            //scramble = "R' U' F R' B' F2 L2 D' U'";
            //scramble = "R' U' F R' B' F2 L2";
            //scramble = "R' U' F R' B'";
            //scramble = "R' U' F R'";

            State scramble2state(string scramble)
            {
                State start_state = new State(0, 0, Enumerable.Range(0, 12).ToArray(), 0);
                int[] moves = scramble.Split(" ").Select(x => Array.IndexOf(Global.move_names, x)).ToArray();
                foreach (int each_move in moves)
                {
                    start_state = start_state.apply_move(each_move);
                }
                return start_state;
            }

            State scrambled_state = scramble2state(scramble);

            var sw = new Stopwatch();
            sw.Start();

            Console.WriteLine("Start searching...");

            Search search = new Search(scrambled_state);

            string result_string = search.start_search(9);
            
            Console.WriteLine(result_string);
            
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            Console.WriteLine("Finished!({0})", ts);

            Console.ReadKey();
        }
    }
}
