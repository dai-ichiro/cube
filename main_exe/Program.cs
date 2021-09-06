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

            State scramble2state(List<int> moves)
            {
                State temp_state = new State(0, 0, Enumerable.Range(0, 12).ToArray(), 0);

                foreach (int each_move in moves)
                {
                    temp_state = temp_state.apply_move(each_move);
                }
                return temp_state;
            }

            var moves = new List<int>();        //初期値
            var result = new List<int>();     //結果を記録するリスト

            State state_after_moves;

            void try_move(int depth)
            {
                if (depth == 0)
                {
                    state_after_moves = scramble2state(moves);
                    if(state_after_moves.cp==0 && state_after_moves.co==0 && state_after_moves.eo == 0)
                    {
                        result.Add(ep_to_index(state_after_moves.ep));
                    }
                    return;
                }
                int prev_move = moves.Count == 0 ? -1 : moves.Last();
                foreach (int i in Enumerable.Range(0, 18))
                {
                    if (is_move_available(prev_move, i))
                    {
                        moves.Add(i);
                        try_move(depth - 1);
                        moves.RemoveAt(moves.Count - 1);
                    }
                }
            }

            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("Start searching...");
            
            int move_count = 8;
            try_move(move_count);

            HashSet<int> final_result = new HashSet<int>(result);

            Console.WriteLine($"list_count: {result.Count}");
            Console.WriteLine($"hash_count: {final_result.Count}");

            string file_name = $"ep_index_{move_count}.data";
            using (FileStream fs = new FileStream(file_name, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, final_result);
            }

            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            Console.WriteLine("Finished!({0})", ts);

            Console.ReadKey();
        }
    }
}
