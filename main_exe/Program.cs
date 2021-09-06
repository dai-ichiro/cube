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
 
            string[] move_names = { "U", "U2", "U'", "D", "D2", "D'", "L", "L2", "L'", "R", "R2", "R'", "F", "F2", "F'", "B", "B2", "B'" };

            int[] new_ep = new int[12];
            int now_ep_index;

            
            
            
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

            int[] index_to_ep(int ep_index)
            {
                int[] ep = new int[12];
                for (int i = 10; i > -1; i--)
                {
                    ep[i] = ep_index % (12 - i);
                    ep_index /= (12 - i);
                    for (int j = i + 1; j< 12; j++)
                    {
                        if(ep[j] >= ep[i])
                        {
                            ep[j] += 1;
                        }
                    }
                }
                return ep;
            }

            bool is_solved(Mini_State m_state)
            {
                return (m_state.cp == 0 && m_state.co == 0 && m_state.eo == 0);
            }

            bool prune(int depth, Mini_State m_state)
            {
                if (depth < Global.cp_co_prune_table[m_state.cp, m_state.co]) return true;
                if (depth < Global.cp_eo_prune_table[m_state.cp, m_state.eo]) return true;
                if (depth < Global.co_eo_prune_table[m_state.co, m_state.eo]) return true;
                return false;
            }

            int ep_move(int[] ep, List<int> moves)
            {
                new_ep = ep;
                foreach (int each_move in moves)
                {
                    new_ep = Global.ep_move_dict[each_move].Select(x => new_ep[x]).ToArray();
                }

                return ep_to_index(new_ep);  
            }

            State scrambled_state = new State(0, 0, index_to_ep(1), 0);
            Mini_State scrambled_mini_state = new Mini_State(0, 0, 0);
            int[] initial_ep = scrambled_state.ep;

            List<int> current_solution = new List<int> { };
            string[] last_5_solution = new string[] { };

            bool depth_limited_search(Mini_State m_state, int depth)
            {
                if (depth == 0 && is_solved(m_state))
                {
                    now_ep_index = ep_move(initial_ep, current_solution);
                    if (now_ep_index == 0)
                    {
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

            
            
            for (int depth = 0; depth < 16; depth++)
            {
                Console.WriteLine("Start searching lenght {0}", depth);
                if (depth_limited_search(scrambled_mini_state, depth)) break;
            }
            Console.WriteLine(string.Join(" ", current_solution.Select(x => move_names[x])));
            

            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            Console.WriteLine("Finished!({0})", ts);

            Console.ReadKey();
        }
    }
}
