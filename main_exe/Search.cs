using System;
using System.Collections.Generic;
using System.Linq;

namespace main_exe
{
    class Search
    {
        int[] new_ep;
        int now_ep_index;
        int back_count;
        int dict_index;

        Mini_State scrambled_mini_state;
        int[] initial_ep;
        
        List<int> current_solution;

        public Search(State scrambled_state)
        {
            scrambled_mini_state = new Mini_State(scrambled_state.cp, scrambled_state.co, scrambled_state.eo);
            initial_ep = scrambled_state.ep;

            new_ep = new int[12];
            current_solution = new List<int> { };
            back_count = 0;
        }

        private int calc_back_count(int index)
        {
            Global.ep_dict.TryGetValue(index, out dict_index);
            return dict_index == 0 ? 6 : dict_index;
        }

        private bool is_move_available(int pre, int now)
        {
            if (pre == -1) return true;
            if (pre / 3 == now / 3) return false;
            if (pre / 3 == 0 && now / 3 == 1) return false; //U→Dはダメ
            if (pre / 3 == 3 && now / 3 == 2) return false; //R→Lはダメ
            if (pre / 3 == 4 && now / 3 == 5) return false; //F→Bはダメ
            return true;
        }

        private bool is_solved(Mini_State m_state)
        {
            return (m_state.cp == 0 && m_state.co == 0 && m_state.eo == 0);
        }

        private bool prune(int depth, Mini_State m_state)
        {
            if (depth < Global.cp_co_prune_table[m_state.cp, m_state.co]) return true;
            if (depth < Global.cp_eo_prune_table[m_state.cp, m_state.eo]) return true;
            if (depth < Global.co_eo_prune_table[m_state.co, m_state.eo]) return true;
            return false;
        }
        private int ep_to_index(int[] ep)
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
        private int ep_move(int[] ep, List<int> moves)
        {
            new_ep = ep;
            foreach (int each_move in moves)
            {
                new_ep = Global.ep_move_dict[each_move].Select(x => new_ep[x]).ToArray();
            }

            return ep_to_index(new_ep);
        }

        public bool depth_limited_search(Mini_State m_state, int depth)
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
                    back_count = calc_back_count(now_ep_index);
                    return false;
                }
            }

            if (depth == 0)
            {
                return false;
            }

            if (prune(depth, m_state))
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
                if (back_count > 0)
                {
                    back_count -= 1;
                    break;
                }
            }
            return false;
        }

        public string start_search(int current_depth)
        {
            for (int depth = current_depth; depth < current_depth+5; depth++)
            {
                Console.WriteLine("Start searching lenght {0}", depth);
                back_count = 0;
                if (depth_limited_search(scrambled_mini_state, depth)) break;
            }

            return string.Join(" ", current_solution.Select(x => Global.move_names[x]));
        } 
    }
}
