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
                    return true;
                }

                if (depth == 0)
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

            Mini_State start_state = new Mini_State(0, 0, 0);

            var sw = new Stopwatch();
            sw.Start();

            Console.WriteLine("Start searching...");

            for (int depth = 1; depth < 10; depth++)
            {
                Console.WriteLine("Start searching lenght {0}", depth);
                if (depth_limited_search(start_state, depth)) break;
            }
            
            Console.WriteLine(string.Join(" ", current_solution.Select(x => move_names[x])));


            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            Console.WriteLine("Finished!({0})", ts);

            Console.ReadKey();
        }
    }
}
