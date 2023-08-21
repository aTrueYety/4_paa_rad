using System;
using System.IO;

namespace HelloWorld
{
    class Board
    {
        public ulong p1_binarytable;
        public ulong p2_binarytable;
        public ulong all_binarytable;
        string winner;

        public Board(ulong _p1_binarytable, ulong _p2_binarytable)
        {
            p1_binarytable = _p1_binarytable;
            p2_binarytable = _p2_binarytable;
            Update_all_binarytable();
        }
        void Update_all_binarytable() {
            all_binarytable = p1_binarytable | p2_binarytable;
        }
        static bool CheckBit(ulong value, byte index) // https://stackoverflow.com/questions/51788932/how-to-set-get-bits-of-a-uint64
        {
            if (index < 0 || index > 63) throw new ArgumentOutOfRangeException("index");
            ulong bitIndexValue = 1UL << index;
            return (value & bitIndexValue) == bitIndexValue;
        }
        public bool Push(Int16 pos, bool side_p1) {
            int i = 0;
            while (i < 8) {
                int target_bit = pos+i*8;
                if (!CheckBit(all_binarytable, Convert.ToByte(target_bit))) { // check if piece empty
                    if (side_p1) { p1_binarytable |= 1UL << (target_bit);} // set piece for p1
                    else { p2_binarytable |= 1UL << (target_bit);} // set piece for p2
                    Update_all_binarytable(); // mby redundant 
                    /* Console.WriteLine("target i: " + Convert.ToString(target_bit));
                    string txt =  Convert.ToString((long) all_binarytable, toBase: 2);
                    int len = txt.Length;
                    for (int k = 0; k < (64-len); k++) {txt = "0" + txt;}
                    Console.WriteLine( txt ); */
                    return true;
                }
                i++;
            }
            return false;
        }    
        public string Get_Winner() {
            if (winner != null) { return winner; }

                // test horizontal
            ulong h_test = Convert.ToUInt64(15); // horizontal mask        make const pls
            for (int i = 0; i < 8; i++) // per row
            {
                for (int j = 0; j < 5; j++) // per tile minus last thre tiles
                {
                    ulong h_test_shifted = h_test << i*8+j; // make const pls
                    if ( (p1_binarytable & h_test_shifted) == h_test_shifted ) { // test from i*8+j to and icluding i*8+j+3
                        winner = "p1";
                        return winner;
                    }
                    if ( (p2_binarytable & h_test_shifted) == h_test_shifted ) { // test from i*8+j to and icluding i*8+j+3
                        winner = "p2";
                        return winner;
                    }
                }
            }
                // test vertical
            ulong v_test = Convert.ToUInt64(16843009); // vertical mask        make const pls
            for (int i = 0; i < 8; i++) // per row
            {
                for (int j = 0; j < 5; j++) // per tile minus last thre tiles
                {
                    ulong v_test_shifted = v_test << i+j*8; // make const pls
                    if ( (p1_binarytable & v_test_shifted) == v_test_shifted ) { // test from i+j*8 to and icluding i+(j+3)*8
                        winner = "p1";
                        return winner;
                    }
                    if ( (p2_binarytable & v_test_shifted) == v_test_shifted ) { // test from i+j*8 to and icluding i+(j+3)*8
                        winner = "p2";
                        return winner;
                    }
                }
            }
                // test skrå opp
            ulong du_test = Convert.ToUInt64(134480385); //diagonal up mask         make const pls
            for (int i = 0; i < 5; i++) // per tile minus last thre tiles
            {
                for (int j = 0; j < 5; j++) // per tile minus last thre tiles
                {
                    ulong du_test_shifted = du_test << i*8+j;
                    if ( (p1_binarytable & du_test_shifted) == du_test_shifted ) { // test from i+j*8 to and icluding i+(j+3)*8
                        winner = "p1";
                        return winner;
                    }
                    if ( (p2_binarytable & du_test_shifted) == du_test_shifted ) { // test from i+j*8 to and icluding i+(j+3)*8
                        winner = "p2";
                        return winner;
                    }
                }
            }
                // test skrå ned
            ulong dd_test = Convert.ToUInt64(16909320); //diagonal up mask         make const pls
            for (int i = 0; i < 5; i++) // per tile minus last thre tiles
            {
                for (int j = 0; j < 5; j++) // per tile minus last thre tiles
                {
                    ulong dd_test_shifted = dd_test << i*8+j;
                    if ( (p1_binarytable & dd_test_shifted) == dd_test_shifted ) { // test from i+j*8 to and icluding i+(j+3)*8
                        winner = "p1";
                        return winner;
                    }
                    if ( (p2_binarytable & dd_test_shifted) == dd_test_shifted ) { // test from i+j*8 to and icluding i+(j+3)*8
                        winner = "p2";
                        return winner;
                    }
                }
            }

            // test draw
            if (all_binarytable == ulong.MaxValue) {
                winner = "draw";
                return winner; }

            // else
            return "None";
        }
        public void Draw() {
            string txt;
            for (int i = 0; i < 8; i++) 
            {
                txt = "-"; 
                for (int j = 0; j < 8; j++) {
                    txt += "----";
                }
                Console.WriteLine(txt);

                txt = "|"; 
                for (int j = 0; j < 8; j++) {
                    char sign = ' ';
                    if (CheckBit(p1_binarytable, Convert.ToByte(j+(7-i)*8))) {sign = 'X';}
                    if (CheckBit(p2_binarytable, Convert.ToByte(j+(7-i)*8))) {sign = 'O';}
                    txt += " " + sign + " |";
                }
                Console.WriteLine(txt);
            }
            txt = "-"; 
            for (int i = 0; i < 8; i++) {
                txt += "----";
            }
            Console.WriteLine(txt);

            Console.WriteLine("  0   1   2   3   4   5   6   7");
        }
    }
    class Game {
        public Board board;
        public bool p1_turn = true;

        public Game() {
            board = new Board(ulong.MinValue, ulong.MinValue);
        }

        public void Start() {
            while (board.Get_Winner() == "None") {
                board.Draw();
                string txt;
                if (p1_turn) { txt = "p1 move: "; } else { txt = "p2 move: "; }
                Console.WriteLine(txt);
                string inn = Console.ReadLine();
                if (inn == "s") {break;}
                board.Push(Convert.ToInt16(inn), p1_turn);
                p1_turn = !p1_turn;
            }
            board.Draw();
            Console.WriteLine( "Winner: " + board.Get_Winner() );
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Game game= new Game();
            game.Start();

            /* cur.Push(0, true);
            cur.Push(0, true);
            cur.Push(0, true);
            cur.Push(0, true);
            cur.Push(0, true);
            cur.Push(0, true);
            cur.Push(0, true);
            cur.Push(0, true); */

            /* cur.Push(0, false);
            cur.Push(1, true);
            cur.Push(2, false);
            cur.Push(3, true);
            cur.Push(4, false);
            cur.Push(5, true);
            cur.Push(6, false);
            cur.Push(7, true);
            cur.Push(0, false);
            cur.Push(1, true);
            cur.Push(2, false);
            cur.Push(3, true);
            cur.Push(4, false);
            cur.Push(5, true);
            cur.Push(6, false);
            cur.Push(7, true);

            cur.Push(0, true);
            cur.Push(1, false);
            cur.Push(2, true);
            cur.Push(3, false);
            cur.Push(4, true);
            cur.Push(5, false);
            cur.Push(6, true);
            cur.Push(7, false);
            cur.Push(0, true);
            cur.Push(1, false);
            cur.Push(2, true);
            cur.Push(3, false);
            cur.Push(4, true);
            cur.Push(5, false);
            cur.Push(6, true);
            cur.Push(7, false);

            cur.Push(0, true);
            cur.Push(1, true);
            cur.Push(2, false);
            cur.Push(3, true);
            cur.Push(4, false);
            cur.Push(5, true);
            cur.Push(6, false);
            cur.Push(7, true);
            cur.Push(0, false);
            cur.Push(1, true);
            cur.Push(2, false);
            cur.Push(3, true);
            cur.Push(4, false);
            cur.Push(5, true);
            cur.Push(6, false);
            cur.Push(7, true);

            cur.Push(0, true);
            cur.Push(1, false);
            cur.Push(2, true);
            cur.Push(3, false);
            cur.Push(4, true);
            cur.Push(5, false);
            cur.Push(6, true);
            cur.Push(7, false);
            cur.Push(0, true);
            cur.Push(1, false);
            cur.Push(2, true);
            cur.Push(3, false);
            cur.Push(4, true);
            cur.Push(5, false);
            cur.Push(6, true);
            cur.Push(7, false); */
            
            //Console.WriteLine( "p1: " + Convert.ToString((long)cur.p1_binarytable, toBase: 2) + "  -  " + "p2: " + Convert.ToString((long)cur.p2_binarytable, toBase: 2));
        }
    }
}