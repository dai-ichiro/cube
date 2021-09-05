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

            Global.cp_co_prune_table = File_Operation.read_array("./data/cp_co_prune_table_minus_five.data");
            Global.cp_eo_prune_table = File_Operation.read_array("./data/cp_eo_prune_table_minus_five.data");
            Global.co_eo_prune_table = File_Operation.read_array("./data/co_eo_prune_table_minus_five.data");

 

            string[] move_names = { "U", "U2", "U'", "D", "D2", "D'", "L", "L2", "L'", "R", "R2", "R'", "F", "F2", "F'", "B", "B2", "B'" };

            int[] new_ep = new int[12];
            int now_ep_index;

            string scramble;
            //scramble = "R' U' F R' B' F2 L2 D' U' L2 F2 D' L2 D' R B D2 L D2 F2 U2 L R' U' F";
            //scramble = "R' U' F R' B' F2 L2 D' U' L2 F2 D' L2 D' R B";
            scramble = "R' U' F R' B' F2 L2 D' U' L2 F2 D' L2 D' R";
            //scramble = "R' U' F R' B' F2 L2 D' U' L2 F2 D' L2";
            //scramble = "R' U' F R' B' F2 L2 D' U' L2 F2 D'";
            //scramble = "R' U' F R' B' F2 L2 D' U'";
            //scramble = "R' U' F R' B' F2 L2";
            //scramble = "R' U' F R' B'";
            //scramble = "R' U' F R'";

            
            bool is_move_available(int pre, int now)
            {
                if (pre == -1) return true;
                if (pre / 3 == now / 3) return false;
                if (pre / 3 == 0 && now / 3 == 1) return false; //U→Dはダメ
                if (pre / 3 == 3 && now / 3 == 2) return false; //R→Lはダメ
                if (pre / 3 == 4 && now / 3 == 5) return false; //F→Bはダメ
                return true;
            }

            bool is_solved(Mini_State m_state)
            {
                return (m_state.cp == 0 && m_state.co == 0 && m_state.eo == 0);
            }

            List<int> current_solution = new List<int> { };
            string[] last_5_solution = new string[] { };

            bool depth_limited_search(Mini_State m_state, int depth)
            {
                if (depth == 0 && is_solved(m_state))
                {
                    now_ep_index = ep_move(initial_ep, current_solution);
                    if (Global.all_states_after_five_moves.Contains((m_state.cp, m_state.co, now_ep_index, m_state.eo)))
                    {
                        last_5_solution = Global.all_states_with_solution[(m_state.cp, m_state.co, now_ep_index, m_state.eo)];
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (depth == 0)
                {
                    return false;
                }

                if (depth < 6 && prune(depth, m_state))
                {
                    return false;
                }

                int prev_move = current_solution.Count == 0 ? -1 : current_solution.Last();
                for (int move_num = 0; move_num < 18; move_num++)
                {
                    if (!(is_move_available(prev_move, move_num))) continue;
                    current_solution.Add(move_num);
                    if (depth_limited_search(m_state.apply_move(move_num), depth - 1)) return true;
                    current_solution.RemoveAt(current_solution.Count - 1);
                }
                return false;
            }

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
                    if (depth_limited_search(scrambled_mini_state, depth)) break;
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
