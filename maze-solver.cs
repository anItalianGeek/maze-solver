using System;
using System.IO;
using System.Threading;

namespace labirinto_con_le_matrici
{
    internal class Program
    {
        static void CheckMovements(ref bool up, ref bool down, ref bool dx, ref bool sx, ref int movementsAvailable, int posRow, int posColumn, char[,] matrix)
        {
            movementsAvailable = 4;

            // check movement above
            int checkRow = posRow - 1;
            if (checkRow == -1 || matrix[checkRow, posColumn] == '0')
            {
                up = false;
                movementsAvailable--;
            }

            // check movement below
            int checkRowBottom = posRow + 1;
            if (checkRowBottom == matrix.GetLength(0) || matrix[checkRowBottom, posColumn] == '0')
            {
                down = false;
                movementsAvailable--;
            }

            // check movement right
            int checkColumn = posColumn + 1;
            if (checkColumn == matrix.GetLength(1) || matrix[posRow, checkColumn] == '0')
            {
                dx = false;
                movementsAvailable--;
            }

            // check movimento left
            int checkColumnSin = posColumn - 1;
            if (checkColumnSin == -1 || matrix[posRow, checkColumnSin] == '0')
            {
                sx = false;
                movementsAvailable--;
            }

            return;
        }
        static void CheckIfThreeWayChoice(bool dx, bool sx, bool down, bool up, int[,] indexCrossroads, ref int pointerindexCrossroads, int indexHistory, int movementsAvailable)
        {
            // in case i could go in 3+ different ways, i save in the matrix "indexCrossroads" the coordinates of the current position, which will help me to go back there if i manage to get stuck somewhere
            // REMEMBER: this matrix is supposed to work just like a stack.
            
            if (movementsAvailable >= 3)
            {
                indexCrossroads[pointerindexCrossroads, 4] = indexHistory;

                if (dx) // based on the logic of the movements, whereever i can go i assign '1' to the direction (1 = true = movement available), '0' otherwise
                    indexCrossroads[pointerindexCrossroads, 0] = 1;
                else
                    indexCrossroads[pointerindexCrossroads, 0] = -1;

                if (down)
                    indexCrossroads[pointerindexCrossroads, 1] = 1;
                else
                    indexCrossroads[pointerindexCrossroads, 1] = -1;

                if (sx)
                    indexCrossroads[pointerindexCrossroads, 2] = 1;
                else
                    indexCrossroads[pointerindexCrossroads, 2] = -1;

                if (up)
                    indexCrossroads[pointerindexCrossroads, 3] = 1;
                else
                    indexCrossroads[pointerindexCrossroads, 3] = -1;
                
                // pointing at the next row in the matrix "indexCrossroads" so i can save another crossroad coordinate
                pointerindexCrossroads++;
            }

            return;
        }
        static void Move(bool up, bool down, bool right, bool left, char[,] matrix, int[,] history, ref int indexHistory, ref int currentPositionRow, ref int currentPositionColumn)
        {
            // saving in the matrix "history" the current coordinate
            history[indexHistory, 0] = currentPositionRow;
            history[indexHistory, 1] = currentPositionColumn;

            // "moving on" and exchanging the values in the matrix
            if (up)
            {
                matrix[currentPositionRow, currentPositionColumn] = '1';
                matrix[--currentPositionRow, currentPositionColumn] = 'x';
            }
            if (down)
            {
                matrix[currentPositionRow, currentPositionColumn] = '1';
                matrix[++currentPositionRow, currentPositionColumn] = 'x';
            }
            if (right)
            {
                matrix[currentPositionRow, currentPositionColumn] = '1';
                matrix[currentPositionRow, ++currentPositionColumn] = 'x';
            }
            if (left)
            {
                matrix[currentPositionRow, currentPositionColumn] = '1';
                matrix[currentPositionRow, --currentPositionColumn] = 'x';
            }

            // pointing to the next element, so i can save another move that i make 
            indexHistory++;

            return;
        }
        static int DetermineMove(ref bool up, ref bool down, ref bool right, ref bool left, char[,] matrix, int[,] indexCrossroads, ref int pointerindexCrossroads, ref int indexHistory, int[,] history, ref int currentPositionRow, ref int currentPositionColumn, ref int movementsAvailable)
        {
            // creating a temp vector to help the computer to "decide" the next move
            /*
             * moving right direction equals integer '0'
             * moving down equals integer '1'
             * moving left equals integer '2'
             * moving up equals integer '3'
             */
            int[] distances = { 0, 0, 0, 0 }; // a vector with 4 elements is just fine

            // before counting, i check if i reached a dead end, if so i go back to the previous crossroad and try a different path
            int posToReturn;
            if (CheckDeadEnd(up, down, right, left, history, indexCrossroads, ref pointerindexCrossroads, currentPositionRow, currentPositionColumn, indexHistory, out posToReturn))
                ReturnToCrossing(posToReturn, indexCrossroads, ref pointerindexCrossroads, matrix, ref currentPositionRow, ref currentPositionColumn, history, ref indexHistory, ref right, ref down, ref left, ref up, ref movementsAvailable);

            // before counting i have to avoid going back from where i came from (if i don't do so, i am going to get stuck in between 2 blocks)
            if (indexHistory > 0)
            {
                if (currentPositionColumn - 1 == history[indexHistory - 1, 1])
                    left = false;
                if (currentPositionColumn + 1 == history[indexHistory - 1, 1])
                    right = false;
                if (currentPositionRow - 1 == history[indexHistory - 1, 0])
                    up = false;
                if (currentPositionRow + 1 == history[indexHistory - 1, 0])
                    down = false;
            }

            // here i count the distance between my current position and the exit
            if (right)
            {
                currentPositionColumn++; // simulating a movement to the right

                // calculate the distance
                distances[0] += matrix.GetLength(0) - (currentPositionRow + 1);
                distances[0] += matrix.GetLength(1) - (currentPositionColumn + 1);
                
                // because "currentPositionColumn" is passed by reference, after the simulation i have to reset it to the previuos value to avoid wall-breaking
                currentPositionColumn--;
            }
            else
                distances[0] = Int32.MaxValue;

            if (down)
            {
                currentPositionRow++;

                distances[1] += matrix.GetLength(0) - (currentPositionRow + 1);
                distances[1] += matrix.GetLength(1) - (currentPositionColumn + 1);

                currentPositionRow--;
            }
            else
                distances[1] = Int32.MaxValue;

            if (left)
            {
                currentPositionColumn--;

                distances[2] += matrix.GetLength(0) - (currentPositionRow + 1);
                distances[2] += matrix.GetLength(1) - (currentPositionColumn + 1);

                currentPositionColumn++;
            }
            else
                distances[2] = Int32.MaxValue;

            if (up)
            {
                currentPositionRow--;

                distances[3] += matrix.GetLength(0) - (currentPositionRow + 1);
                distances[3] += matrix.GetLength(1) - (currentPositionColumn + 1);

                currentPositionRow++;
            }
            else
                distances[3] = Int32.MaxValue;

            // selecting the closest path to the exit
            // REMEMBER: if the distance of two possible paths is equal, the program is going to choose the one with the lowest integer assigned to a path (look at line 79)
            int returnValue = -1, tmp = Int32.MaxValue;
            for (int i = 0; i < 4; i++)
                if (distances[i] != Int32.MaxValue && tmp > distances[i])
                {
                    tmp = distances[i];
                    returnValue = i;
                }

            // setting the boolean variables
            switch (returnValue)
            {
                case 0:
                    down = false;
                    up = false;
                    left = false;
                    break;

                case 1:
                    up = false;
                    right = false;
                    left = false;
                    break;

                case 2:
                    up = false;
                    down = false;
                    right = false;
                    break;

                case 3:
                    down = false;
                    right = false;
                    left = false;
                    break;
            }

            // returning the value, rappresenting the move i am going to make
            return returnValue;
        }
        static bool CheckDeadEnd(bool up, bool down, bool right, bool left, int[,] history, int[,] indexCrossroads, ref int pointerindexCrossroads, int currentPositionRow, int currentPositionColumn, int indexhistory, out int positionToReturn)
        {
            // i check if i have only one move available, if there are more, i skip, to prevent any confusion
            int movesAvailable = 0;
            if (up) movesAvailable++;
            if (down) movesAvailable++;
            if (right) movesAvailable++;
            if (left) movesAvailable++;

            // this method performs a check that is similar to the "checkMovements" method, however this time, it returns true if i reached a dead end otherwise it returns false (if there are at least 2 moves available)
            if (movesAvailable == 1 && pointerindexCrossroads > 0)
            {
                if (up)
                    if (currentPositionRow - 1 == history[indexhistory - 1, 0])
                    {
                        positionToReturn = indexCrossroads[pointerindexCrossroads - 1, 4];
                        return true;
                    }

                if (down)
                    if (currentPositionRow + 1 == history[indexhistory - 1, 0])
                    {
                        positionToReturn = indexCrossroads[pointerindexCrossroads - 1, 4];
                        return true;
                    }

                if (right)
                    if (currentPositionColumn + 1 == history[indexhistory - 1, 1])
                    {
                        positionToReturn = indexCrossroads[pointerindexCrossroads - 1, 4];
                        return true;
                    }

                if (left)
                    if (currentPositionColumn - 1 == history[indexhistory - 1, 1])
                    {
                        positionToReturn = indexCrossroads[pointerindexCrossroads - 1, 4];
                        return true;
                    }
            }

            positionToReturn = -1;
            return false;
        }
        static void ReturnToCrossing(int positionToReturn, int[,] indexCrossroads, ref int pointerindexCrossroads, char[,] matrix, ref int currentPositionRow, ref int currentPositionColumn, int[,] history, ref int indexHistory, ref bool right, ref bool down, ref bool left, ref bool up, ref int movementsAvailable)
        {
            bool actuallyReturned = false;
            while (!actuallyReturned)
            {
                // creating local variables to store the coordinate of the last crossroad found
                bool isReturned = false;
                int rowToReturn = history[positionToReturn, 0];
                int columnToReturn = history[positionToReturn, 1];
                int nextRowToChoose, nextColumnToChoose;

                // repeating the process until i reach the crossroad
                while (!isReturned)
                {
                    // looking back at the previous move
                    nextRowToChoose = history[indexHistory - 1, 0];
                    nextColumnToChoose = history[indexHistory - 1, 1];

                    // decrement the index because i go back obviously
                    indexHistory--;

                    // update the matrix
                    matrix[currentPositionRow, currentPositionColumn] = '1';
                    matrix[nextRowToChoose, nextColumnToChoose] = 'x';

                    // creating two support variables that i use to verify in which direction i am going (they are used to check if i reached the crossroad)
                    int checkLastRow = currentPositionRow;
                    int checkLastColumn = currentPositionColumn;

                    currentPositionRow = nextRowToChoose;
                    currentPositionColumn = nextColumnToChoose;

                    PrintMatrix(matrix);
                    Console.Clear();

                    // checking if i reached the crossroad
                    if (matrix[rowToReturn, columnToReturn] == 'x')
                    {
                        isReturned = true;
                        actuallyReturned = true;
                        
                        // using a support variable so that i can the computer which direction was wrong
                        int valueToDeny = 104; // invalid number...
                        if (currentPositionColumn - 1 == checkLastColumn)
                            valueToDeny = 2;
                        if (currentPositionColumn + 1 == checkLastColumn)
                            valueToDeny = 0;
                        if (currentPositionRow - 1 == checkLastRow)
                            valueToDeny = 3;
                        if (currentPositionRow + 1 == checkLastRow)
                            valueToDeny = 1;

                        // telling the computer that the direction i took was wrong
                        indexCrossroads[pointerindexCrossroads - 1, valueToDeny] = -1;
                    }
                }

                /*
                 * I came back to the crossroad, however, i have to check if there are any more directions available, if i tried every direction, i have to repeat the process once again
                 * until i reach the previous crossroad
                */

                int howManyMovesUnavailable = 0;
                for (int i = 0; i < 4; i++) 
                    if (indexCrossroads[pointerindexCrossroads - 1, i] == -1)
                        howManyMovesUnavailable++;

                if (howManyMovesUnavailable == 3)
                {
                    actuallyReturned = false;
                    pointerindexCrossroads--;
                    positionToReturn = indexCrossroads[pointerindexCrossroads - 1, 4];
                }
            }

            /*
             * because everytime i make a move, the boolean variables get modified, i create another method to separate the checks (moves available and crossroads)
             * however, now to check where i can go, i have to call again the method "CheckMovements" so there is no problem if i reset the booleans to "true"
            */
            up = true;
            down = true;
            right = true;
            left = true;
            CheckMovements(ref up, ref down, ref right, ref left, ref movementsAvailable, currentPositionRow, currentPositionColumn, matrix);

            // correcting the booleans by checking which available road brings me to a dead end
            if (pointerindexCrossroads > 0)
            {
                if (indexCrossroads[pointerindexCrossroads - 1, 0] == -1)
                    right = false;
                if (indexCrossroads[pointerindexCrossroads - 1, 1] == -1)
                    down = false;
                if (indexCrossroads[pointerindexCrossroads - 1, 2] == -1)
                    left = false;
                if (indexCrossroads[pointerindexCrossroads - 1, 3] == -1)
                    up = false;
            }

            return;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("the maze is going to be read from a file.");
            Console.WriteLine("\nThe first move must contain a single integer (rows of the maze/matrix).");
            Console.WriteLine("\nThe second row must contain a single integer (columns of the maze/matrix).");
            Console.WriteLine("\nThe program will read as many rows and columns as the value of the integers.");
            Console.WriteLine("\nif the file does not follow the \"rules\" close the program and correct your input file");
            Console.WriteLine("\n\n\nPress any key to begin...");
            Console.ReadKey();

            // declaration and initialization of some varibles
            int[,] history = new int[1000, 2]; // here i save the moves performed by the computer (i assume that the levels won't take the computer longer than 1000 moves)
            int[,] indexCrossroads = new int[500, 5]; // i assume that the computer won't take more than 500 
            char[,] maze;

            int dimensionRows, dimensionColumns; // integers that store the dimension of the matrix

            // creating the object that holds the file
            StreamReader fileReader;
            try {
                fileReader = new StreamReader(@"maze.txt");
            } catch (Exception e) {
                return;
            }
            string sLine = "";

            // reading the information about the dimensions of the matrix
            dimensionRows = Convert.ToInt32(fileReader.ReadLine());
            dimensionColumns = Convert.ToInt32(fileReader.ReadLine());

            maze = new char[dimensionRows, dimensionColumns];

            // filling the matrix
            int rowCounter = 0;
            while (sLine != null)
            {
                sLine = fileReader.ReadLine();

                if (sLine != null) 
                    for (int columnCounter = 0, charPointer_string_sLine = 0; columnCounter < maze.GetLength(1); columnCounter++, charPointer_string_sLine++)
                        maze[rowCounter, columnCounter] = sLine[charPointer_string_sLine];

                rowCounter++;
            }
            fileReader.Close();

            // variables declaration
            int indexHistory = 0, currentPositionColumn = 0, currentPositionRow = 0, pointerindexCrossroads = 0, movementsAvailable = -1;

            bool notOut = true, up, down, right, left;

            PrintMatrix(maze);

            // process begins
            while (notOut)
            {
                // cleaning the terminal
                Console.Clear();

                // resetting the boolean variables
                up = true;
                down = true;
                right = true;
                left = true;

                // checking where can i go
                CheckMovements(ref up, ref down, ref right, ref left, ref movementsAvailable, currentPositionRow, currentPositionColumn, maze);
                CheckIfThreeWayChoice(right, left, down, up, indexCrossroads, ref pointerindexCrossroads, indexHistory, movementsAvailable);
                DetermineMove(ref up, ref down, ref right, ref left, maze, indexCrossroads, ref pointerindexCrossroads, ref indexHistory, history, ref currentPositionRow, ref currentPositionColumn, ref movementsAvailable);
                Move(up, down, right, left, maze, history, ref indexHistory, ref currentPositionRow, ref currentPositionColumn);
                
                PrintMatrix(maze);

                if (maze[maze.GetLength(0) - 1, maze.GetLength(1) - 1] == 'x')
                    notOut = false;
            }

            Console.ReadKey();
        }
        static void PrintMatrix(char[,] matrix) // method used to print the matrix to screen
        {
            for (int r = 0; r < matrix.GetLength(0); r++)
            {
                for (int c = 0; c < matrix.GetLength(1); c++)
                    switch (matrix[r, c])
                    {
                        case '0':
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(matrix[r, c] + " ");
                            break;
                        case '1':
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write(matrix[r, c] + " ");
                            break;
                        case 'x':
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(matrix[r, c] + " ");
                            break;
                    }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
            }
            Thread.Sleep(250);

            return;
        }
    }
}