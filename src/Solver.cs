using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuChallengeMedium.src {
    public static class Solver {
        public static byte[,] Solve(byte[,] sudoku) {
            Sudoku solver = new Sudoku(sudoku);
            return solver.Solve();
        }

    }

    public class Sudoku {
        private byte[,] data;

        public Sudoku(byte[,] data) {
            this.data = data;
            WriteData(data);
        }

        public byte[,] GetData() {
            return data;
        }

        public byte[,] deepcopy(byte[,] data) {
            byte[,] newData = new byte[9, 9];
            for (int x=0; x<9; x++) {
                for (int y=0; y<9; y++) {
                    newData[y, x] = data[y, x];
                }
            }
            return newData;
        }

        public byte[] ExtractRow(int n) {
            byte[] ret = new byte[9];
            for (int i=0; i<9; i++) {
                ret[i] = data[n - 1, i];
            }
            return ret;
        }

        public byte[] ExtractCol(int n) {
            byte[] ret = new byte[9];
            for (int i=0; i<9; i++) {
                ret[i] = data[i, n - 1];
            }
            return ret;
        }

        public byte[] ExtractGrid(int n) {
            int fromRow = (n - 1) / 3 * 3;
            int fromCol = (n - 1) % 3 * 3;
            return ExtractGrid(fromCol + 1, fromRow + 1);
        }

        public byte[] ExtractGrid(int x, int y) {
            int fromRow = (y - 1) / 3 * 3;
            int fromCol = (x - 1) / 3 * 3;
            byte[] ret = new byte[9];
            int idx = 0;
            for (int cy=fromRow; cy<fromRow+3; cy++) {
                for (int cx=fromCol; cx<fromCol+3; cx++) {
                    ret[idx++] = data[cy, cx];
                }
            }
            return ret;
        }

        public bool Check(byte[] nums) {
            return Enumerable.All(
                Enumerable.Range(1, 9),
                n => Enumerable.Count(nums.Where(n2 => n == n2)) == 1
            );
        }

        public bool Duplicate(byte[] nums) {
            return Enumerable.Any(
                Enumerable.Range(1, 9),
                n => Enumerable.Count(nums.Where(n2 => n == n2)) > 1
            );
        }

        public bool IsSolved() {
            IEnumerable<byte[]> rows  = Enumerable.Range(1, 9).Select(ExtractRow);
            IEnumerable<byte[]> cols  = Enumerable.Range(1, 9).Select(ExtractCol);
            IEnumerable<byte[]> grids = Enumerable.Range(1, 9).Select(n => ExtractGrid(n));

            return Enumerable.All(rows.Concat(cols).Concat(grids), Check);
        }

        public bool IsValid() {
            for (int x=1; x<10; x++) {
                for (int y=1; y<10; y++) {
                    Cell cell = CalcCell(x, y);
                    if (!cell.IsValid()) {
                        return false;
                    }
                }
            }
            IEnumerable<byte[]> rows  = Enumerable.Range(1, 9).Select(ExtractRow);
            IEnumerable<byte[]> cols  = Enumerable.Range(1, 9).Select(ExtractCol);
            IEnumerable<byte[]> grids = Enumerable.Range(1, 9).Select(n => ExtractGrid(n));

            return !Enumerable.Any(rows.Concat(cols).Concat(grids), Duplicate);
        }

        public bool IsInvalid() {
            return !IsValid();
        }

        public byte GetValue(int x, int y) {
            return data[y - 1, x - 1];
        }

        public bool IsAllowed(int x, int y, byte v) {
            return !Enumerable.Contains(ExtractRow(y), v)
                && !Enumerable.Contains(ExtractCol(x), v)
                && !Enumerable.Contains(ExtractGrid(x, y), v);
        }

        public Cell CalcCell(int x, int y) {
            byte v = GetValue(x, y);
            if (v != 0) {
                return new Cell(x, y, new byte[1] {v});
            } else {
                return new Cell(x, y, Enumerable.Range(1, 9).Select(n => (byte)n).Where(n => IsAllowed(x, y, n)).ToArray());
            }
        }

        public Sudoku Clone(int x, int y, byte v) {
            byte[,] newData = deepcopy(data);
            newData[y - 1, x - 1] = v;
            return new Sudoku(newData);
        }

        public Sudoku[] CalcNext() {
            if (IsSolved()) {
                return new Sudoku[] {this};
            } else if (IsInvalid()) {
                return new Sudoku[0];
            } 
            byte[,] newData = new byte[9, 9];
            bool changed = false;
            Cell targetCell = null;
            for (int x=1; x<10; x++) {
                for (int y=1; y<10; y++) {
                    Cell cell = CalcCell(x, y);
                    newData[y-1, x-1] = cell.ToByte();
                    if (newData[y-1, x-1] != data[y-1, x-1]) {
                        changed = true;
                    }
                    if (cell.Values.Length > 1 && (targetCell == null || targetCell.Values.Length > cell.Values.Length)) {
                        targetCell = cell;
                    }
                }
            }
            if (changed) {
                return new Sudoku(newData).CalcNext();
            }
            return targetCell.Values.SelectMany(v => Clone(targetCell.X, targetCell.Y, v).CalcNext()).ToArray();
        }

        public byte[,] Solve() {
            Sudoku[] ret = CalcNext();
            if (ret.Length == 1) {
                return ret[0].GetData();
            } else {
                throw new Exception("No answer");
            }
        }

        public void WriteData(byte[,] data)
        {
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    Console.Write(data[x, y]);
                }
                Console.Write('\n');
            }
            Console.Write('\n');
        }

        public void WriteLine(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Console.Write(data[i]);
            }
            Console.Write('\n');
        }
    }

    public class Cell {

        public Cell(int x, int y, byte[] values) {
            this.X = x;
            this.Y = y;
            this.Values = values;
        }

        public int X { get; set;}
        public int Y { get; set;}
        public byte[] Values { get; set;}

        public byte ToByte() {
            return Values.Length == 1 ? Values[0] : (byte)0;
        }

        public bool IsValid() {
            return Values.Length > 0;
        }
    }
}
