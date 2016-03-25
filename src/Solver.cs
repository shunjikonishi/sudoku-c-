using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuChallengeMedium.src
{
    public static class Solver
    {
        public static byte[,] Solve(byte[,] sudoku)
        {
        }

        /// <summary>
        /// Checks if a certain value in allowed within a row
        /// </summary>
        /// <param name="sudoku">Current sudoku field</param>
        /// <param name="x">X position to check</param>
        /// <param name="y">Y position to check</param>
        /// <param name="val">Value to check</param>
        /// <returns>If <paramref name="val"/> is posible</returns>
        private static bool CheckRow(byte[,] sudoku, int x, int y, byte val)
        {
            for (int ix = 0; ix < 9; ix++)
            {
                if (x == ix) continue;
                if (sudoku[ix, y] == val) return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if a certain value in allowed within a column
        /// </summary>
        /// <param name="sudoku">Current sudoku field</param>
        /// <param name="x">X position to check</param>
        /// <param name="y">Y position to check</param>
        /// <param name="val">Value to check</param>
        /// <returns>If <paramref name="val"/> is posible</returns>
        private static bool CheckColumn(byte[,] sudoku, int x, int y, byte val)
        {
            for (int iy = 0; iy < 9; iy++)
            {
                if (y == iy) continue;
                if (sudoku[x, iy] == val) return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if a certain value in allowed within a cell
        /// </summary>
        /// <param name="sudoku">Current sudoku field</param>
        /// <param name="x">X position to check</param>
        /// <param name="y">Y position to check</param>
        /// <param name="val">Value to check</param>
        /// <returns>If <paramref name="val"/> is posible</returns>
        private static bool CheckCell(byte[,] sudoku, int x, int y, byte val)
        {
            int startx = x - (x % 3);
            int starty = y - (y % 3);

            for (int ix = startx; ix < startx + 3; ix++)
            {
                for (int iy = starty; iy < starty + 3; iy++)
                {
                    if (y == iy && x == ix) continue;
                    if (sudoku[ix, iy] == val) return false;
                }
            }
            return true;
        }

    }
}
