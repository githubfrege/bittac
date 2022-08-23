using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;

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
        //public static Dictionary<long, bool> WinTable = new Dictionary<long, bool>();
        public static HashSet<long> WinHashSet = new HashSet<long>();
        //public static long[] VerticalOptions;
        public static long NewLineMask;
        public static int[] Values;
        public static int Highest = Rows * Cols;
        //public static int MovesForWin = (NInARow * 2) - 1;
        public static long FullBoardMask;

        public static void GenerateFullBoardMask()
        {
            long fullBoard = 0;
            for (int i = 1; i <= Highest; i++)
            {
                fullBoard |= 1L << i;
            }
            FullBoardMask = fullBoard;
        }
        public static bool CheckWinInAlgorithm(long marks)
        {
            
            for (int i = 1; i <= Highest; i++)
            {
                
                    if (WinHashSet.Contains(marks & ~(1L << i)))
                    {
                    //RenderBoard(marks, 0, "A", "");

                    //RenderBoard(marks & ~(1L << i), 0, "A", "");
                        return true;
                    }
        
               
            }
            return false;
            
          
        }
        public static bool nextMoveBlocksFork(long newMark,long desiredMarks, long undesiredMarks)
        {
            /*bool containsFork(long marks)
            {
                int options = 0;
                for (int i = 1; i <= Highest; i++)
                {
                    if (HasWon(marks | (1L << i)) && (((boardMarks) & (1L << i)) == 0))
                    {
                        options++;
                    }
                }
                return options >= 2;
            }
            for (int i = 1; i <= Highest; i++)
            {
                if (containsFork(marks | 1L << i))
                {
                    return true;
                }
            }*/
            bool containsFork(long marks)
            {
                int options = 0;
                for (int i = 1; i <= Highest; i++)
                {
                    if (HasWon(marks | (1L << i)) && (((desiredMarks | undesiredMarks) & (1L << i)) == 0))
                    {
                        options++;
                    }
                }
                return options >= 2;
            }
            return !containsFork(undesiredMarks) && containsFork(undesiredMarks | newMark);
        }
        public static int evaluate(long desiredMarks, long undesiredMarks)
        {
            //blocker
           
           
            
            /*if (!undesiredWon && !nextMoveContainsFork(undesiredMarks,desiredMarks | undesiredMarks) && nextMoveContainsFork(undesiredMarks | newMark, desiredMarks | undesiredMarks))
             {
                 return 150;
             }*/
                //if (!undesiredWon && HasWon(undesiredMarks | newMark))
                //{
                    /*RenderBoard(undesiredMarks, 0, "O", "");*/
                    //RenderBoard(undesiredMarks, newMark, "X", "A");

                //return 100;
                //}
            
                if (WinHashSet.Contains(desiredMarks) || CheckWinInAlgorithm(desiredMarks))
                {
                //RenderBoard(desiredMarks, 0, "D", "");
                    WinHashSet.Add(desiredMarks);
                    return 10;
                }
                if (WinHashSet.Contains(undesiredMarks) || CheckWinInAlgorithm(undesiredMarks))
                {
                //RenderBoard(undesiredMarks, 0, "U", "");
                WinHashSet.Add(undesiredMarks);
                return -10;
                }

            

            return 0;
        }
        public static int minimax(long desiredMarks, long undesiredMarks, int depth, bool isMax, int alpha, int beta)
        {
            //Console.WriteLine(isMax);
            Console.WriteLine(depth);
            ulong state = (ulong) ((desiredMarks << Highest) | undesiredMarks);
            /*bool checkFullBoard()
            {
                foreach (int value in Values)
                {
                    if (((desiredMarks | undesiredMarks) & (1L << value)) == 0)
                    {
                        return false;
                    }
                }
                for (int i = 1;i<=Highest;i++)
                {
                    if (((desiredMarks | undesiredMarks) & (1L << i)) == 0)
                    {
                        return false;
                    }
                }
                
                return true;
            }*/
            if (OldAnswers.TryGetValue(state, out int oldAnswer))
            {
                return oldAnswer;
            }
          
            int score = evaluate(desiredMarks, undesiredMarks);
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
            if (score == 150)
            {
                OldAnswers[state] = score;
                return score;
            }

            if ((desiredMarks | undesiredMarks) == FullBoardMask)
            {
                OldAnswers[state] = 0;
                return 0;
            }
            if (isMax)
            {
                int best = -1000;
                for (int i = 1;i<=Highest;i++)//foreach (int value in Values)
                {
                   
                    if (((desiredMarks | undesiredMarks) & (1L << i)) == 0)
                    {
                        desiredMarks |= (1L << i);
                        best = Math.Max(best, minimax(desiredMarks, undesiredMarks, depth + 1, !isMax, alpha, beta));
                        alpha = Math.Max(alpha, best);
                        desiredMarks &= ~(1L << i);
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
                for (int i = 1;i<=Highest;i++)//foreach (int value in Values)
                {
                    if (((desiredMarks | undesiredMarks) & (1L << i)) == 0)
                    {
                        undesiredMarks |= (1L << i);
                        best = Math.Min(best, minimax(desiredMarks, undesiredMarks, depth + 1, !isMax, alpha, beta));
                        beta = Math.Min(beta, best);
                        undesiredMarks &= ~(1L << i);
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
            for (int i = 1;i<=Highest;i++)//foreach (int value in Values)
            {
                if (((desiredMarks | undesiredMarks) & (1L << i)) == 0)
                {
                    desiredMarks |= (1L << i);
                    int moveVal = minimax(desiredMarks, undesiredMarks, 0, false, -1000, 1000);
                    if (nextMoveBlocksFork(1L<<i,desiredMarks,undesiredMarks))
                    {
                        Console.WriteLine("This is a fork blocking move");
                        RenderBoard(undesiredMarks, 1L << i, "F", "B");
                        moveVal = 150;
                    }
                    if (!CheckWinInAlgorithm(undesiredMarks) && HasWon(undesiredMarks | 1L << i))
                    {
                        Console.WriteLine("This is a blocking move");
                        RenderBoard(undesiredMarks, 1L << i, "U", "B");
                        moveVal = 100;
                    }
                        //Console.WriteLine(moveVal);
                        desiredMarks &= ~(1L << i);
                    if (moveVal > bestVal)
                    {
                        bestMove = (1L << i);
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
        public static void GenerateWinTable(int n,long marks)
        {
            /*if (n == NInARow)
            {
                WinTable[marks] = HasWon(marks);
                return;
            }
            for (int i = 1; i <= Highest; i++)
            {
                GenerateWinTable(n + 1, marks | (1L << i));
            }*/
            for (int i = 1; i <= Highest; i++)
            {

            }
        }

        public static void GenerateWinHashSetPrototype()
        {
            /*if (n == NInARow)
            {
                WinTable[marks] = HasWon(marks);
                return;
            }
            for (int i = 1; i <= Highest; i++)
            {
                GenerateWinTable(n + 1, marks | (1L << i));
            }*/
            for (int i = 1; i <= Highest; i++)
            {
                long hypotheticalMarks = 0;
                hypotheticalMarks |= (1L << i);
                for (int j = 1; j <= Highest; j++)
                {
                    if ((hypotheticalMarks & (1L << j)) == 0)
                    {

                        //Console.WriteLine("free mark at " + j);

                        hypotheticalMarks |= (1L << j);
                        for (int n = 1; n <= Highest; n++)
                        {

                            if ((hypotheticalMarks & (1L << n)) == 0)
                            {

                                hypotheticalMarks |= (1L << n);
                                if (HasWon(hypotheticalMarks))
                                {
                                    WinHashSet.Add(hypotheticalMarks);
                                    //RenderBoard(hypotheticalMarks, 0, "X", "");

                                }
                                hypotheticalMarks &= ~(1L << n);
                            }
                        }
                        hypotheticalMarks &= ~(1L << j);
                    }
                }
                hypotheticalMarks &= ~(1L << i);

            }
            //Console.WriteLine("Herre's 4 as a board");
            //RenderBoard(4, 0, "X", "");
        }
        public static bool HasWon(long marks)
        {
            //check horizontal
            if (WinHashSet.Contains(marks))
            {
                return true;
            }
            WinHashSet.Add(marks);
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
            WinHashSet.Remove(marks);
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
            GenerateFullBoardMask();
            GetHorizontalOptions();
            GenerateNewLineMask();
            GenerateValues();
            GetDiagonalOptions();
            GetVerticalNumbers();
            GenerateWinHashSetPrototype();
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
