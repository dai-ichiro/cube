using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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
            //scramble = "R' U' F R' B' F2 L2 D' U' L2 F2 D' L2";
            //scramble = "R' U' F R' B' F2 L2 D' U' L2 F2 D'";
            scramble = "R' U' F R' B' F2 L2 D' U'";
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

            Console.WriteLine($"start time: {DateTime.Now}");

            Console.WriteLine("Start searching...");

            void multi_task(State scrambled_state, int first_move)
            {
                State state_after_move = scrambled_state.apply_move(first_move);
                Search search = new Search(scrambled_state);
                string result_string = search.start_search();

                if (!(result_string == null))
                {
                    Console.WriteLine(Global.move_names[first_move] + result_string);
                    Console.WriteLine($"finish_time: {DateTime.Now}");
                }  
            }

            State state_after_one_move;

            state_after_one_move = scrambled_state.apply_move(0);
            Task.Run(() => multi_task(state_after_one_move, 0));

            state_after_one_move = scrambled_state.apply_move(1);
            Task.Run(() => multi_task(state_after_one_move, 1));

            state_after_one_move = scrambled_state.apply_move(2);
            Task.Run(() => multi_task(state_after_one_move, 2));

            state_after_one_move = scrambled_state.apply_move(3);
            Task.Run(() => multi_task(state_after_one_move, 3));


            /*
            for (int i = 0; i < 18; i++)
            {
                Task.Run(() => multi_task(scrambled_state, i));
            }
            */
            Console.ReadKey();
        }
    }
}
