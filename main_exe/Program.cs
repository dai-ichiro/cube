using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Diagnostics;

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

            using (FileStream fs = new FileStream("./data/all_states_from_one_to_five_with_solution.data", FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                Global.all_states_with_solution= (Dictionary<(int, int, int, int), string[]>)bf.Deserialize(fs);
            }

            using (FileStream fs = new FileStream("./data/all_states_after_five_moves.data", FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                Global.all_states_after_five_moves = (HashSet<(int, int, int, int)>)bf.Deserialize(fs);
            }

            using (FileStream fs = new FileStream("./data/cp_co_eo_after_five_moves.data", FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                Global.cp_co_eo_after_five_moves = (HashSet<(int, int, int)>)bf.Deserialize(fs);
            }

            using (FileStream fs = new FileStream("./data/cp_after_five_moves.data", FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                Global.cp_after_five_moves = (HashSet<int>)bf.Deserialize(fs);
            }

            using (FileStream fs = new FileStream("./data/ep_after_7_moves.data", FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                Global.ep_after_7_moves = (HashSet<int>)bf.Deserialize(fs);
            }

            string[] move_names = { "U", "U2", "U'", "D", "D2", "D'", "L", "L2", "L'", "R", "R2", "R'", "F", "F2", "F'", "B", "B2", "B'" };

            bool is_move_available(int pre, int now)
            {
                if (pre == -1) return true;
                if (pre / 3 == now / 3) return false;
                if (pre / 3 == 0 && now / 3 == 1) return false; //U→Dはダメ
                if (pre / 3 == 3 && now / 3 == 2) return false; //R→Lはダメ
                if (pre / 3 == 4 && now / 3 == 5) return false; //F→Bはダメ
                return true;
            }

            int ep_to_index(int[] ep)
            {
                int index = 0;
                for (int i = 0; i < 12; i++)
                {
                    index *= 12 - i;
                    for (int j = i + 1; j < 12; j++)
                    {
                        if (ep[i] > ep[j]) index += 1;
                    }
                }
                return index;
            }

            bool is_solved(State q_state)
            {
                if (!(Global.cp_after_five_moves.Contains(q_state.cp))) return false;
                if (!(Global.cp_co_eo_after_five_moves.Contains((q_state.cp, q_state.co, q_state.eo)))) return false;

                return Global.all_states_after_five_moves.Contains((q_state.cp, q_state.co, ep_to_index(q_state.ep), q_state.eo));
            }

            bool prune(int depth, State q_state)
            {
                if (depth < Global.cp_co_prune_table[q_state.cp, q_state.co] - 5) return true;
                if (depth < Global.cp_eo_prune_table[q_state.cp, q_state.eo] - 5) return true;
                if (depth < Global.co_eo_prune_table[q_state.co, q_state.eo] - 5) return true;
                if (depth == 2 && !(Global.ep_after_7_moves.Contains(ep_to_index(q_state.ep)))) return true;
                return false;
            }

            List<int> current_solution = new List<int> { };
            string[] last_5_solution = new string[] { };

            bool depth_limited_search(State q_state, int depth)
            {
                if (depth == 0 && is_solved(q_state))
                {
                    last_5_solution = Global.all_states_with_solution[(q_state.cp, q_state.co, ep_to_index(q_state.ep), q_state.eo)];
                    return true;
                }
                if (depth == 0)
                {
                    return false;
                }

                if (depth < 6 && prune(depth, q_state))
                {
                    return false;
                }

                int prev_move = current_solution.Count == 0 ? -1 : current_solution.Last();
                for (int move_num = 0; move_num < 18; move_num++)
                {
                    if (!(is_move_available(prev_move, move_num))) continue;
                    current_solution.Add(move_num);
                    if (depth_limited_search(q_state.apply_move(move_num), depth - 1)) return true;
                    current_solution.RemoveAt(current_solution.Count - 1);
                }
                return false;
            }

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
                int[] moves = scramble.Split(" ").Select(x => Array.IndexOf(move_names, x)).ToArray();
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

            if (Global.all_states_with_solution.ContainsKey((scrambled_state.cp, scrambled_state.co, ep_to_index(scrambled_state.ep), scrambled_state.eo)))
            {
                string[] short_solution = Global.all_states_with_solution[(scrambled_state.cp, scrambled_state.co, ep_to_index(scrambled_state.ep), scrambled_state.eo)];
                for (int i = 1; i < short_solution.Count() + 1; i++)
                {
                    Console.WriteLine("Start searching lenght {0}", i);
                }
                Console.WriteLine(string.Join(" ", short_solution));
            }

            else
            {
                for (int i = 1; i < 6; i++)
                {
                    Console.WriteLine("Start searching lenght {0}", i);
                }

                for (int depth = 1; depth < 16; depth++)
                {
                    Console.WriteLine("Start searching lenght {0}", depth + 5);
                    if (depth_limited_search(scrambled_state, depth)) break;
                }
                Console.WriteLine(string.Join(" ", current_solution.Select(x => move_names[x])) + " " + string.Join(" ", last_5_solution));
            }

            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            Console.WriteLine("Finished!({0})", ts);

            Console.ReadKey();
        }
    }
}
