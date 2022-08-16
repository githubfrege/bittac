using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace TicTacToeConsole
{

    static class Program
    {
        const int Cols = 4;
        const int Rows = 4;
        const int NInARow = 3;
        public static List<List<int>> Diagonals = new List<List<int>>();
        public static long[] HorizontalOptions;
        public static List<List<int>> VerticalNumbers = new List<List<int>>();
        public static Dictionary<ulong, int> OldAnswers = new Dictionary<ulong, int>();
        //public static long[] VerticalOptions;
        public static long NewLineMask;
        public static int[] Values;
        public static int Highest = Rows * Cols;
        public static int MovesForWin = (NInARow * 2) - 1;


        public static int evaluate(long desiredMarks, long undesiredMarks, long newMark)
        {
            //blocker
           
            bool nextMoveContainsFork(long marks)
            {
                bool containsFork(long marks)
                {
                    int options = 0;
                    foreach (int value in Values)
                    {
                        if (HasWon(desiredMarks | 1L << value))
                        {
                            options++;
                        }
                    }
                    return options >= 2;
                }
                foreach (int value in Values)
                {
                    if (containsFork(desiredMarks | 1L << value))
                    {
                        return true;
                    }
                }
                return false;
            }

            /*if (!HasWon(undesiredMarks) && !nextMoveContainsFork(desiredMarks) && nextMoveContainsFork(desiredMarks | newMark))
            {
                return 150;
            }*/
            if (!HasWon(undesiredMarks) && HasWon(undesiredMarks | newMark))
            {
                return 100;
            }

            if (HasWon(desiredMarks))
            {
                return 10;
            }
            else if (HasWon(undesiredMarks))
            {
                return -10;
            }

            return 0;
        }
        public static int minimax(long desiredMarks, long undesiredMarks, int depth, bool isMax, int alpha, int beta, long newMark)
        {
            //Console.WriteLine(isMax);
            ulong state = (ulong)((newMark << Highest * 2) | (desiredMarks << Highest) | undesiredMarks);
            bool checkFullBoard()
            {
                foreach (int value in Values)
                {
                    if (((desiredMarks | undesiredMarks) & (1L << value)) == 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            if (OldAnswers.TryGetValue(state, out int oldAnswer))
            {
                return oldAnswer;
            }

          
            //int score = HammingWeight(desiredMarks | undesiredMarks) >= MovesForWin ? evaluate(desiredMarks, undesiredMarks) : 0;
            int score = evaluate(desiredMarks, undesiredMarks,newMark);
            if (score == 10)
            {
                OldAnswers[state] = score;
                return score;
            }
            if (score == -10)
            {
                OldAnswers[state] = score;
                return score;
            }
            if (score == 100)
            {
                OldAnswers[state] = score;
                return score;
            }

            if (checkFullBoard())
            {
                OldAnswers[state] = 0;
                return 0;
            }
            if (isMax)
            {
                int best = -1000;
                foreach (int value in Values)
                {
                   
                    if (((desiredMarks | undesiredMarks) & (1L << value)) == 0)
                    {
                        desiredMarks |= (1L << value);
                        best = Math.Max(best, minimax(desiredMarks, undesiredMarks, depth + 1, !isMax, alpha, beta,1L<<value));
                        alpha = Math.Max(alpha, best);
                        desiredMarks ^= (1L << value);
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }
                OldAnswers[state] = best;
                return best;
            }
            else
            {

                int best = 1000;
                foreach (int value in Values)
                {
                    if (((desiredMarks | undesiredMarks) & (1L << value)) == 0)
                    {
                        undesiredMarks |= (1L << value);
                        best = Math.Min(best, minimax(desiredMarks, undesiredMarks, depth + 1, !isMax, alpha, beta,1L<<value));
                        beta = Math.Min(beta, best);
                        undesiredMarks ^= (1L << value);
                        if (beta <= alpha)
                        {

                            break;
                        }
                    }
                }
                OldAnswers[state] = best;
                return best;
            }

        }
        public static long findBestMove(long desiredMarks, long undesiredMarks)
        {
            int bestVal = -1000;
            long bestMove = 0;
            foreach (int value in Values)
            {
                if (((desiredMarks | undesiredMarks) & (1L << value)) == 0)
                {
                    desiredMarks |= (1L << value);
                    int moveVal = minimax(desiredMarks, undesiredMarks, 0, false, -1000, 1000,1L<<value);
                    //Console.WriteLine(moveVal);
                    desiredMarks ^= (1L << value);
                    if (moveVal > bestVal)
                    {
                        bestMove = (1L << value);
                        bestVal = moveVal;
                    }
                }
            }
            return bestMove;
        }
        public static int HammingWeight(long n)
        {
            int ret = 0;
            while (n != 0)
            {
                n &= (n - 1);
                ret++;
            }
            return ret;
        }
        public static int MaxConsecutiveOnes(long x)
        {

            // Initialize result
            int count = 0;

            // Count the number of iterations
            // to reach x = 0.
            while (x != 0)
            {

                // This operation reduces length
                // of every sequence of 1s by one.
                x = (x & (x << 1));

                count++;
            }

            return count;
        }

        public static void GetHorizontalOptions()
        {
            long[] horizontal = new long[Rows];
            for (int i = 1; i <= Rows; i++)
            {
                long rowValue = 0;
                for (int j = 1; j <= Cols; j++)
                {
                    long currentVal = (1L << ((Cols * (i - 1)) + j));
                    rowValue |= currentVal;

                }
                horizontal[i - 1] = rowValue;


            }
            HorizontalOptions = horizontal;

        }

        public static long Mark(int row, int col)
        {

            int idx = (Cols * (row - 1)) + col;

            return (long)(1L << idx);
        }

        public static void antidiagonal(int[,] A)
        {

            // For each column start row is 0
            for (int col = 0; col < Cols; col++)
            {
                List<int> myDiagonal = new List<int>();
                int startcol = col, startrow = 0;

                while (startcol >= 0 && startrow < Rows)
                {
                    myDiagonal.Add(A[startcol, startrow]);
                    startcol--;
                    startrow++;
                }
                if (myDiagonal.Count() >= NInARow)
                {
                    Diagonals.Add(myDiagonal);
                }

            }

            // For each row start column is N-1
            for (int row = 1; row < Rows; row++)
            {
                List<int> myDiagonal = new List<int>();
                int startrow = row, startcol = Cols - 1;

                while (startrow < Rows && startcol >= 0)
                {
                    myDiagonal.Add(A[startcol, startrow]);
                    startcol--;
                    startrow++;
                }
                if (myDiagonal.Count() >= NInARow)
                {
                    Diagonals.Add(myDiagonal);
                }
            }
        }
        public static void GetDiagonalOptions()
        {
            int[,] matrix = GetMatrix();
            antidiagonal(matrix);
            diagonal(matrix);
        }
        static void diagonal(int[,] A)
        {


            // For each column start row is 0
            for (int col = 0; col < Cols; col++)
            {
                List<int> myDiagonal = new List<int>();
                int startcol = col, startrow = 0;

                while (startcol < Cols && startrow < Rows)
                {
                    myDiagonal.Add(A[startcol, startrow]);
                    startcol++;
                    startrow++;
                }
                if (myDiagonal.Count() >= NInARow)
                {
                    Diagonals.Add(myDiagonal);

                }
            }

            // For each row start column is 0
            for (int row = 1; row < Rows; row++)
            {
                int startrow = row, startcol = 0;
                List<int> myDiagonal = new List<int>();
                while (startrow < Rows && startcol < Cols)
                {
                    myDiagonal.Add(A[startcol, startrow]);
                    startcol++;
                    startrow++;
                }
                if (myDiagonal.Count() >= NInARow)
                {
                    Diagonals.Add(myDiagonal);
                }
            }
        }
        public static int[,] GetMatrix()
        {
            int[,] myMatrix = new int[Cols, Rows];
            int n = 0;
            for (int i = 0; i < Cols; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    n++;
                    myMatrix[i, j] = n;
                }


            }
            return myMatrix;
        }
        public static void GetVerticalNumbers()
        {
            List<List<int>> vertical = new List<List<int>>();
            for (int i = 0; i < Cols; i++)
            {
                long myColumn = NewLineMask >> i;
                List<int> numbersForColumn = new List<int>();
                foreach (int val in Values)
                {
                    if ((myColumn & (1L << val)) != 0)
                    {
                        numbersForColumn.Add(val);

                    }

                }
                vertical.Add(numbersForColumn);



            }
            VerticalNumbers = vertical;
        }
        public static bool HasWon(long marks)
        {
            //check horizontal
            foreach (long option in HorizontalOptions)
            {
                if (MaxConsecutiveOnes((marks & option)) >= NInARow)
                {
                    return true;
                }
            }
            //check vertical
            foreach (List<int> column in VerticalNumbers)
            {
                int adj = 0;
                foreach (int value in column)
                {
                    if ((marks & (1L << value)) != 0)
                    {
                        adj++;
                        if (adj >= NInARow)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        adj = 0;
                    }
                }

            }
            //check diagonals
            foreach (List<int> diagonal in Diagonals)
            {
                int adj = 0;
                foreach (int value in diagonal)
                {
                    if ((marks & (1L << value)) != 0)
                    {
                        adj++;
                        if (adj >= NInARow)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        adj = 0;
                    }
                }

            }
            return false;
        }
        public static void GenerateNewLineMask()
        {
            long mask = 0;
            for (int i = 1; i <= Rows; i++)
            {
                mask |= (long)(1L << (i * Cols));
                Console.WriteLine(1L << (i * Cols));
                Console.WriteLine(Convert.ToString(mask, toBase: 2));

            }
            NewLineMask = mask;
        }
        public static void RenderBoard(long marks1, long marks2, string symbol1, string symbol2)
        {

            for (int i = 0; i < Rows * Cols; i++)
            {

                if ((NewLineMask & (1L << (i))) != 0)
                {
                    Console.WriteLine("\n");

                }
                if ((marks1 & (1L << (i + 1))) != 0)
                {
                    Console.Write(symbol1 + " ");
                }
                else if ((marks2 & (1L << (i + 1))) != 0)
                {
                    Console.Write(symbol2 + " ");
                }
                else
                {
                    Console.Write("# ");
                }
            }
            Console.WriteLine("\n\n");
        }
        public static void GenerateValues()
        {
            int[] myValues = new int[Rows * Cols];
            int idx = 0;
            for (int i = 1; i <= Rows * Cols; i++)
            {
                myValues[idx] = i;
                idx++;
            }
            Values = myValues;
        }
        public static void Play(ref long marks, long otherMarks, string symbol1, string symbol2)
        {
            string str = Console.ReadLine();
            long myMark = Mark(int.Parse(str[0].ToString()), int.Parse(str[1].ToString()));
            Console.WriteLine(Convert.ToString(myMark, toBase: 2));
            if ((myMark & (marks | otherMarks)) == 0)
            {
                marks |= myMark;
            }
            else
            {
                Console.WriteLine("tile occupied. try again");
                Play(ref marks, otherMarks, symbol1, symbol2);
                return;

            }
            RenderBoard(marks, otherMarks, symbol1, symbol2);
            if (HasWon(marks))
            {
                Console.WriteLine("You won!");
            }
        }
        static void Main(string[] args)
        {
            long playerMarks = 0;
            long opponentMarks = 0;
            GetHorizontalOptions();
            GenerateNewLineMask();
            GenerateValues();
            GetDiagonalOptions();
            GetVerticalNumbers();
            Console.WriteLine("your turn");
            while (true)
            {
                Play(ref playerMarks, opponentMarks, "O", "X");
                opponentMarks |= findBestMove(opponentMarks, playerMarks);
                RenderBoard(playerMarks, opponentMarks, "O", "X");
            }





        }
    }
}
