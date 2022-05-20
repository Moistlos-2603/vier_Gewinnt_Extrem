namespace Field
{
    using Utils;
    using System.Text.RegularExpressions;

    public class Field
    {
        /*
         * 2D:
         * [ , , , , , , ]
         * [ , , , , , , ]
         * [ , , , , , , ]
         * [ , , , , , , ]
         * 
         * height: 4 width: 7
         * 
         * 1D
         * [ , , , , , , , , , , , , , , , , , , , , , , , , , , , ]
         * length 28 = 4 * 7 = height * width
         */
        private string self;
        private int height, width, win_len;

        private Regex horizontal;
        private Regex vertical;
        private Regex diagonal_left_to_bottom;
        private Regex diagonal_left_to_top;

        public Field(int width, int height, int win_len)
        {
            //Field init
            this.self = new(' ', width * height);
            this.height = height;
            this.width = width;
            this.win_len = win_len;

            //winning conditions init
            //player 1
            /*
             * 2D:
             * [ ,i0, , , , , ]
             * [ ,i1, , , , , ]
             * [ ,i2, , , , , ]
             * [ ,  , , , , , ]
             * 
             * i0 position (1, 0)
             * i1 position (1, 1)
             * i2 position (1, 2)
             * 
             * 1D
             * [ ,i0, , , , , , ,i1, , , , , , ,i2, , , , , , , , , , , , ]
             * i0 index 1
             *      i0 index = 0 * 7 + 1 = y * width + x
             *      
             * i1 index 8
             *      i1 index = 1 * 7 + 1 = y * width + x
             * 
             * i2 index 15
             *      i2 index = 2 * 7 + 1 = y * width + x
             * 
             * delta index = 7
             */
            this.horizontal = new Regex(Util.MulStrInt(@"\S", win_len), RegexOptions.Compiled);

            /*
             * 2D:
             * [ ,i0,  ,  , , , ]
             * [ ,  ,i1,  , , , ]
             * [ ,  ,  ,i2, , , ]
             * [ ,  ,  ,  , , , ]
             * 
             * i0 position (1, 0)
             * i1 position (2, 1)
             * i2 position (3, 2)
             * 
             * 1D
             * [ ,i0, , , , , , , , ,i1, , , , , , , , ,i2, , , , , , , , ]
             * 
             * i0 index 1
             *      i0 index = 0 * 7 + 1 = y * width + x + y
             *      
             * i1 index 9
             *      i1 index = 1 * 7 + 1 = y * width + x + y
             * 
             * i2 index 17
             *      i2 index = 2 * 7 + 1 = y * width + x + y
             * 
             * delta index = 8
             */
            this.diagonal_left_to_bottom = new Regex(
                Util.MulStrInt(@"\S" + new string('.', width), win_len),
                RegexOptions.Compiled);

            /*
             * 2D:
             * [ ,i0,i1,i2, , , ]
             * [ ,  ,  ,  , , , ]
             * [ ,  ,  ,  , , , ]
             * [ ,  ,  ,  , , , ]
             * 
             * i0 position (0, 1)
             * i1 position (0, 2)
             * i2 position (0, 3)
             * 
             * 1D
             * [ ,i0, , , , , , ,i1, , , , , , ,i2, , , , , , , , , , , , ]
             * 
             * i0 index 1
             *      i0 index = 7 * 0 + 1 = width * y + x
             *      
             * i1 index 2
             *      i1 index = 7 * 0 + 2 = width * y + x
             * 
             * i2 index 3
             *      i2 index = 7 * 0 + 3 = width * y + x
             * 
             * delta index = 1
             */
            this.vertical = new Regex(
                Util.MulStrInt(@"\S" + new string('.', width - 1), win_len),
                RegexOptions.Compiled);

            /*
             * 2D:
             * [ ,  ,  ,i0, , , ]
             * [ ,  ,i1,  , , , ]
             * [ ,i2,  ,  , , , ]
             * [ ,  ,  ,  , , , ]
             * 
             * i0 position (0, 3)
             * i1 position (1, 2)
             * i2 position (2, 1)
             * 
             * 1D
             * [ ,i0, , , , , , , , ,i1, , , , , , , , ,i2, , , , , , , , ]
             * 
             * i0 index 1
             *      i0 index = 0 * 7 + 1 = y * width + x + y
             *      
             * i1 index 9
             *      i1 index = 1 * 7 + 1 = y * width + x + y
             * 
             * i2 index 17
             *      i2 index = 2 * 7 + 1 = y * width + x + y
             * 
             * delta index = 8
             */
            this.diagonal_left_to_top = new Regex(
                Util.MulStrInt(@"\S" + new string('.', width - 2), win_len),
                RegexOptions.Compiled);
        }

        /// <summary>
        /// Player character gets pushed on the stack like row. Returns false, if failed.
        /// Failure can be result of a filled row.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool Push(int row, char player)
        {
            // test if row index is in bounds. Tests all
            // indecies from of the row from the start to end
            // if it's empty, if true it's set to a player value.

            char[] self_array = this.self.ToArray();

            if (row >= this.width)
            {
                throw new Exception($"row {row} to large, highest index is {this.width - 1}.");
            }

            for (int i = 0; i < this.width; i++)
            {
                int index = this.height * row + i;
                if (self_array[index] == ' ')
                {
                    self_array[index] = player;
                    this.self = new(self_array);
                    return true;
                }
            }

            // if no index is filled, there is now space. An error occured!
            return false;
        }

        /// <summary>
        /// Checks wether someone won. Returns the winner character, if there is now winner whitespace (' ') is returned.
        /// </summary>
        /// <returns>char</returns>
        public char CheckWinner()
        {
            //Checks with regex, if an winning condition is fullfilled. Returns if true the 'value of the winner'.
            char winner = ' ';

            Match match = this.horizontal.Match(this.self);

            if (match.Success && (match.Index % width) == 0)
            {
                return match.Value[0];
            }

            match = this.vertical.Match(this.self);
            if (match.Success)
            {
                return match.Value[0];
            }
            /*
             * [ , , , , , , ]
             * [ , , ,p, , , ]
             * [ , , , ,p, , ]
             * [ , , , , ,p, ]
             * 
             * matches on (m = match):
             * [ , , , , , , ]
             * [ , , ,m, , , ]
             * [ , , , ,m, , ]
             * [ , , , , ,m, ]
             * 
             * with modulo get the corresponging index on the first line:
             * [ , , ,  ,  ,  , ]
             * [ , , ,m0,  ,  , ]
             * [ , , ,  ,m1,  , ]
             * [ , , ,  ,  ,m2, ]
             * m0(3, 1)
             * 
             * 2D:
             * [ , , ,  ,  ,  , ]
             * [ , , ,m0,  ,  , ]
             * [ , , ,  ,m1,  , ]
             * [ , , ,  ,  ,m2, ]
             * m0(3, 1)
             * 
             * 1D:
             * [ , , , , , , , , , ,m0, , , , , , , ,m1, , , , , , , ,m2, ]
             * m0 index = 10
             * index % width = 10 % 7 = 3
             * 
             * we found x ( x(0, 3):
             * 2D:
             * [ , , ,x ,  ,  , ]
             * [ , , ,m0,  ,  , ]
             * [ , , ,  ,m1,  , ]
             * [ , , ,  ,  ,m2, ]
             * 
             * Add the length of the diagonal to get the highest index on the first line (marked with h):
             * 2D:
             * [ , , ,x ,  ,h , ]
             * [ , , ,m0,  ,  , ]
             * [ , , ,  ,m1,  , ]
             * [ , , ,  ,  ,m2, ]
             * 1D:
             * [ , , ,x , ,h , , , , ,m0, , , , , , , ,m1, , , , , , , ,m2, ]
             * 
             * h < width so it' valid! Edge cases don't need additional ifs!
             */
            match = this.diagonal_left_to_bottom.Match(this.self);
            if (match.Success && (match.Index % width) + win_len - 1 < width)
            {
                return match.Value[0];
            }

            /*
             * p = player
             * 
             * 2D:
             * [ , , , , , , ]
             * [ , , , , ,p, ]
             * [ , , , ,p, , ]
             * [ , , ,p, , , ]
             * 
             * matches on (m = match):
             * [ , , , , , , ]
             * [ , , , , ,m, ]
             * [ , , , ,m, , ]
             * [ , , ,m, , , ]
             * 
             * Get add length to the start position to get the matching character on the highest end (x = "highest index"):
             * [ , , , , ,x, ]
             * [ , , , , ,m, ]
             * [ , , , ,m, , ]
             * [ , , ,m, , , ]
             * 
             * If index is the maximal index, it's a valid match. (For more information look at diagonal left to bottom match) 
             */
            match = this.diagonal_left_to_top.Match(this.self);
            if (match.Success && (match.Index + match.Length - 1) % width + win_len - 1 < width)
            {
                return match.Value[0];
            }
            return winner;
        }

        /// <summary>
        /// Returns the field as string with new lines.
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            string temp = string.Empty;
            for (int i = 0; i < height; i++)
            {
                temp += this.self.Substring(i * width, width) + "|\n";
            }
            return temp;
        }

#if DEBUG
        public string DEBUG_CHANGE
        {
            set
            {
                if (value.Length == this.self.Length)
                {
                    this.self = value;
                    return;
                }
                throw new Exception("string length difference; width, hight, etc. are not applying anymore.");
            }

        }
#endif
    }
}
