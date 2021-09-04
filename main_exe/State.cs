using System.Linq;

namespace main_exe
{ 
    class Mini_State
    {
        public int cp;
        public int co;
        public int eo;

        public Mini_State(int cp, int co, int eo)
        {
            this.cp = cp;
            this.co = co;
            this.eo = eo;
        }
        public Mini_State apply_move(int move_num)
        {
            int new_cp = Global.cp_move_table[this.cp, move_num];
            int new_co = Global.co_move_table[this.co, move_num];
            int new_eo = Global.eo_move_table[this.eo, move_num];
            return new Mini_State(new_cp, new_co, new_eo);
        }
    }
}
