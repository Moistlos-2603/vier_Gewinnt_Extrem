namespace Field
{
    using Utils;
    using System.Text.RegularExpressions;
    using System;
    using System.Linq;

    public class Field
    {
        private int width, height, win_len;
        /*  width ->        h
         * [0, 1, 2, ...]   e
         * [            ]   i
         * [            ]   g
         * [            ]   h
         *                  t
         *                  
         *                  |
         *                  v
         */
        private string self;

        private Regex[] horizontal = new Regex[2],
                       vertical = new Regex[2],
                       diagonal_to_top = new Regex[2],
                       diagonal_to_bottom = new Regex[2];

        //for programming outside the class
        public const char Player1 = '1';
        public const char Player2 = '2';

        //for dynamic programming
        private char[] player = new char[] { Player1, Player2 };

        public Field(int width, int height, int winning_length)
        {
            this.width = width;
            this.height = height;
            this.win_len = winning_length;

            this.self = new(' ', width * height);

            //very explicit
            horizontal[(int)Player.player1] = new Regex(new string(player[(int)Player.player1], winning_length), RegexOptions.Compiled);
            horizontal[(int)Player.player2] = new Regex(new string(player[(int)Player.player2], winning_length), RegexOptions.Compiled);

            vertical[(int)Player.player1] = new Regex(Util.MulStrInt(player[(int)Player.player1] + new string('.', width - 1), win_len).Trim('.'), RegexOptions.Compiled);
            vertical[(int)Player.player2] = new Regex(Util.MulStrInt(player[(int)Player.player2] + new string('.', width - 1), win_len).Trim('.'), RegexOptions.Compiled);

            diagonal_to_bottom[(int)Player.player1] = new Regex(Util.MulStrInt(player[(int)Player.player1] + new string('.', width), win_len).Trim('.'), RegexOptions.Compiled);
            diagonal_to_bottom[(int)Player.player2] = new Regex(Util.MulStrInt(player[(int)Player.player2] + new string('.', width), win_len).Trim('.'), RegexOptions.Compiled);

            diagonal_to_top[(int)Player.player1] = new Regex(Util.MulStrInt(player[(int)Player.player1] + new string('.', width - 2), win_len).Trim('.'), RegexOptions.Compiled);
            diagonal_to_top[(int)Player.player2] = new Regex(Util.MulStrInt(player[(int)Player.player2] + new string('.', width - 2), win_len).Trim('.'), RegexOptions.Compiled);
        }

        public bool Push(int row, Player player)
        {
            for (int i = height - 1; i >= 0; i--)
            {
                int index = i * width + row;
                if (self[index] == ' ')
                {
                    //self to char array
                    char[] tmp_arr = self.ToArray<char>();
                    //set value
                    tmp_arr[index] = this.player[(int)player];
                    //char array to self 
                    self = new(tmp_arr);
                    return true;
                }
            }

            //no placing happend
            return false;
        }

        public char CheckWinner()
        {
            char winner = ' ';

            for (int i = 0; i < 2; i++)
            {
                Match m = horizontal[i].Match(self);
                if (m.Success && m.Index % width <= width - win_len)
                {
                    winner = m.Value[0];
                    break;
                }

                m = vertical[i].Match(self);
                if (m.Success)
                {
                    winner = m.Value[0];
                    break;
                }

                m = diagonal_to_bottom[i].Match(self);
                if (m.Success && m.Index % width <= width - win_len)
                {
                    winner = m.Value[0];
                    break;
                }

                m = diagonal_to_top[i].Match(self);
                if (m.Success && (m.Index + win_len - 1) % width <= width - win_len)
                {
                    winner = m.Value[0];
                    break;
                }
            }
            return winner;
        }

        public string[,] ToString2D()
        {
            string[,] tmp = new string[1, height];

            for (int i = 0; i < height; i++)
            {
                tmp[0, i] = self.Substring(i * width, width);
            }
            return tmp;
        }

        public string ToString1D() => self;

        public (int, int) Dimensions { get => (width, height); }

#if DEBUG
        public string DebugSelf
        {
            set
            {
                if (value.Length == self.Length)
                {
                    self = value;
                }
                else
                {
                    throw new Exception("Intput string was not right size!");
                }
            }
        }
#endif
    }
}
