namespace Field
{
    using Utils;
    using System.Text.RegularExpressions;
    using System;
    using System.Linq;

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
        private char[,] self;
        private int height, width, win_len;

        public Field(int width, int height, int win_len)
        {
            //Field init
            this.self = new char[width, height];

            this.height = height;
            this.width = width;
            this.win_len = win_len;

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    self[j, i] = ' ';

        }

        /// <summary>
        /// Player character gets pushed on the stack like row. Returns false, if failed.
        /// Failure can be result of a filled row.
        /// </summary>
        /// <param name="player"></param>
        /// <returns>bool</returns>
        public bool Push(int row, char player)
        {
            // test if row index is in bounds. Tests all
            // indecies from of the row from the start to end
            // if it's empty, if true it's set to a player value.
            for (int i = 0; i < this.height; i++)
            {
                if (self[row, i] == ' ')
                {
                    self[row, i] = player;
                    return true;
                }
            }

            // if no index is filled, there is now space. An error occured!
            return false;
        }

        /// <summary>
        /// Checks wether someone won. Returns the winner character, if there is no winner whitespace (' ') is returned.
        /// </summary>
        /// <returns>char</returns>
        public char CheckWinner()
        {
            string[] sfield = this.ToString2D().Split("|\n")[0..^1];

            //horizontal
            for (int j = 0; this.height > j; j++)
            {
                string tmp = string.Empty;

                for (int i = 0; this.width > i; i++)
                {
                    tmp += sfield[i][j];
                }

                char m = EqualMatch(tmp);
                if (m != ' ')
                    return m;
            }

            //vertical
            for (int i = 0; i < this.width; i++)
            {
                char m = EqualMatch(sfield[i]);
                if (m != ' ')
                    return m;
            }

            //TODO: make it work
            //diagonal left to bottom
            for (int i = 0; i -1 < this.width - this.win_len; i++)
            {
                for (int j = 0; j - 1 < this.height - this.win_len; j++)
                {
                    string tmp = string.Empty;

                    for (int k = 0; k < this.win_len; k++)
                    {
                        tmp += self[i + k, j + k];
                    }

                    if (Util.AllEqual(tmp) && tmp[0] != ' ')
                    {
                        return tmp[0];
                    }
                }
            }

            //diagonal left to top
            Console.WriteLine("ltt");
            for (int i = 0; i < this.width - this.win_len; i++)
            {
                for (int j = this.win_len - 1; j < this.height; j++)
                {
                    string tmp = string.Empty;
                    for (int k = 0; k < this.win_len; k++)
                    {
                        tmp += self[i + k, j - k];
                    }

                    if (Util.AllEqual(tmp) && tmp[0] != ' ')
                    {
                        return tmp[0];
                    }
                }
            }
            return ' ';
        }

        /// <summary>
        /// Returns the field as string with new lines.
        /// </summary>
        /// <returns>string</returns>
        public string ToString2D()
        {
            string tmp = string.Empty;
            for (int i = 0; i < this.width; i++)
            {
                for (int j = 0; j < this.height; j++)
                    tmp += self[i, j];
                tmp += "|\n";
            }
            return tmp;
        }

        public string ToString1D()
        {
            string tmp = string.Empty;
            for (int i = 0; i < this.width; i++)
            {
                for (int j = 0; j < this.height; j++)
                {
                    tmp += self[i, j];
                }
            }
            return tmp;
        }

        public (int, int) Dimensions { get => (width, height); }

        public char[,] GetCharArray() => self;

        private char EqualMatch(string test)
        {

            for (int i = 0; i < test.Length - this.win_len; i++)
            {
                if (Util.AllEqual(test.Substring(i, this.win_len)) && test[i] != ' ')
                {
                    return test[i];
                }
            }
            if (Util.AllEqual(test) && test[0] != ' ')
            {
                return test[0];
            }

            return ' ';
        }

#if DEBUG
        public string DEBUG_CHANGE
        {
            set
            {
                if (value.Length == this.self.Length)
                {
                    for (int i = 0; i < this.width; i++)
                    {
                        for (int j = 0; j < this.height; j++)
                        {
                            this.self[i, j] = value[j * height + i];
                        }
                    }
                    return;
                }
                throw new Exception("string length difference; width, hight, etc. are not applying anymore.");
            }

        }
#endif
    }
}
