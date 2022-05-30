namespace Field
{
    using Utils;
    using System.Text.RegularExpressions;
    using System;
    using System.Linq;

    public class Field
    {
        private int width, height, win_len;
        /*
         * width ->        h
         * [0, 1, 2, ...]   e
         * [            ]   i
         * [            ]   g
         * [            ]   h
         *                  t
         *                  
         *                  |
         *                  v
         * s := [0, 1, 2]
         *      [3, 4, 5]
         *      
         *      width: 3
         *      height: 2
         *  =
         * s := [0, 1, 2 ; 3, 4, 5]
         * 
         * s[x][y] = s[x + y * width]
         * 
         * example:
         *  s[0][1] =>                3
         *  s[0][1] = s[0 + 1 * 3] => 3
         *  
         *  s[2][1] =>                 5
         *  s[2][1] => s[2 + 1 * 3] => 5
         *  
         *  Strings are perfect for later on used regex operations.
         *  They spare all the messy loops.
         */
        private string self;

        /*
         * For each player there are own regex pattern. It spares comparison time.
         *Each winning condition also needs an own pattern.
         */
        private Regex[] horizontal = new Regex[2],
                       vertical = new Regex[2],
                       diagonal_to_top = new Regex[2],
                       diagonal_to_bottom = new Regex[2];

        //for programming outside the class
        public const char Player1 = '1';
        public const char Player2 = '2';

        //for loop programming
        private char[] player = new char[] { Player1, Player2 };

        /// <summary>
        /// Creates an instance of the field class.
        /// </summary>
        /// <param name="width">The width of the specified Field.</param>
        /// <param name="height">The height of the specified Field.</param>
        /// <param name="winning_length">The continous length of the "player match".</param>
        public Field(int width, int height, int winning_length)
        {
            //Save data for later calculations.
            this.width = width;
            this.height = height;
            this.win_len = winning_length;

            this.self = new(' ', width * height);

            //Information about the complex regex syntax can be found in:
            //https://regexr.com/
            //https://en.wikipedia.org/wiki/Regular_expression

            /*
             * The player enum is used since it is more explicit and
             *it is going to be an unroled loop (if used) anyway.
             */
            //our match look like this (p = player): ppp... (length is determined by winning_length)
            horizontal[(int)Player.player1] = new Regex(new string(player[(int)Player.player1], winning_length), RegexOptions.Compiled);
            horizontal[(int)Player.player2] = new Regex(new string(player[(int)Player.player2], winning_length), RegexOptions.Compiled);

            /*
             * Vertical 2D representation:
             * [ 0,  1,    2 ]
             * [ 3,  4, ( 5) ]
             * [ 6,  7, ( 8) ]
             * [ 9, 10, (11) ]
             * 
             * Idecies ([x][y]): [2][1], [2][2], [2][3]
             * 
             * => delta x = 0
             * => delta y = 1
             * -> 1D delta index = (delta )x + width * (delta )y = 3
             * 
             * Vertical 1D representation:
             * [ 0, 1, 2, 3, 4, (5), 6, 7, (8), 9, 10, (11) ]
             * Indecies ([i]): [5], [8], [11]
             * => delta i = 3
             *  We conclude that approach is working
             * 
             * The Regex string is (p = player):
             *  "p...(width times - 1)p...(width times - 1)"winning length times
             * Trim it at the end to make it fit in all columns!
             */
            vertical[(int)Player.player1] = new Regex(Util.MulStrInt(player[(int)Player.player1] + new string('.', width - 1), win_len).Trim('.'), RegexOptions.Compiled);
            vertical[(int)Player.player2] = new Regex(Util.MulStrInt(player[(int)Player.player2] + new string('.', width - 1), win_len).Trim('.'), RegexOptions.Compiled);

            /*
             * Diagonal "to bottom" 2D representation:
             * [ 0  ,     1,   2  ]
             * [  3 ,     4,  (5) ]
             * [ 6  ,  (7) ,   8  ]
             * [ (9),  10  ,  11  ]
             * 
             * Idecies ([x][y]): [0][3], [1][2], [2][1]
             * 
             * => delta x = 1
             * => delta y = -1
             * -> 1D delta index = (delta )x + width * (delta )y = 2
             * 
             * Vertical 1D representation:
             * [ 0, 1, 2, 3, 4, 5, 6, (7), 8, (9), 10, (11) ]
             * Indecies ([i]): [9], [7], [11]
             * => delta i = 2
             *  We conclude that approach is working
             * 
             * The Regex string is (p = player):
             *  "p...(width times -2)p...(width times-2)"winning length times
             * Trim it at the end to make it fit in all columns!
             */
            diagonal_to_bottom[(int)Player.player1] = new Regex(Util.MulStrInt(player[(int)Player.player1] + new string('.', width), win_len).Trim('.'), RegexOptions.Compiled);
            diagonal_to_bottom[(int)Player.player2] = new Regex(Util.MulStrInt(player[(int)Player.player2] + new string('.', width), win_len).Trim('.'), RegexOptions.Compiled);
            
            /*
             * Diagonal "to top" 2D representation:
             * [ 0  ,     1,    2 ]
             * [ (3),     4,   5  ]
             * [ 6  ,  (7) ,   8  ]
             * [ 9  ,  10  , (11) ]
             * 
             * Idecies ([x][y]): [0][1], [1][2], [2][3]
             * 
             * => delta x = 1
             * => delta y = 1
             * -> 1D delta index = (delta )x + width * (delta )y = 4
             * 
             * Vertical 1D representation:
             * [ 0, 1, 2, (3), 4, 5, 6, (7), 8, 9, 10, (11) ]
             * Indecies ([i]): [3], [7], [11]
             * => delta i = 4
             *  We conclude that approach is working
             * 
             * The Regex string is (p = player):
             *  "p...(width times + 1)p...(width times + 1)"winning length times
             * Trim it at the end to make it fit in all columns!
             */
            diagonal_to_top[(int)Player.player1] = new Regex(Util.MulStrInt(player[(int)Player.player1] + new string('.', width - 2), win_len).Trim('.'), RegexOptions.Compiled);
            diagonal_to_top[(int)Player.player2] = new Regex(Util.MulStrInt(player[(int)Player.player2] + new string('.', width - 2), win_len).Trim('.'), RegexOptions.Compiled);
        }

        /// <summary>
        /// Places piece on the desired column.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="player"></param>
        /// <returns>Returns true, if successfull. If false is returned, then something was off.</returns>
        public bool Push(int column, Player player)
        {
            // If it's out of bounds it's probably an error. Debug it,
            // only in release it's forbidden to crash.
#if !DEBUG
            if(row >= width)
            {
                return false;
            }
#endif
            for (int i = height - 1; i >= 0; i--)
            {
                int index = i * width + column;
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

        /// <summary>
        /// Returns winner as char.
        /// </summary>
        /// <returns>Gives player corresponding to the defined constants with constants, ' ' means no one won yet.</returns>
        public char CheckWinner()
        {
            //This gets returned if nothing matches.
            char winner = ' ';

            //Loop makes it redundant to write it a second time.
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

        //Does what it says.
        //Usefull in debug
        public string[,] ToString2D()
        {
            string[,] tmp = new string[1, height];

            for (int i = 0; i < height; i++)
            {
                tmp[0, i] = self.Substring(i * width, width);
            }
            return tmp;
        }

        //Does what it says.
        public string ToString1D() => self;

        public (int, int) Dimensions { get => (width, height); }

#if DEBUG
        //Better do not touch!
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
