using System;
using System.Collections.Generic;
using Godot;

namespace DefaultNamespace
{
    // generate action -> return action list
    // TODO: generate state

    /*
     * action
     * - drop: D
     * - walk -> top: T, down: D, left: L, right: R
     */

    /*
     * state
     * - list of unit -> one onit:
     * - board
     */
    public class MonteCarloTreeSearch
    {
        // assume initial state

        public static void Main()
        {
            Console.WriteLine("Hello World");
            List<string> units = new List<string>() {"1", "1", "2"};
            int row = 4;
            int column = 4;


            State state = new State(units, row, column, 2, 2, 0, 1);
            state.dropUnit("1", 1, 1);
            // state.dropUnit("1", 0, 0);
            state.dropUnit("1", 0, 2);
            state.generateActions();

            state.printActions();
        }
    }

    public class State
    {
        private List<string> actions;
        private string[,] board;
        private List<State> childrenStates;
        private State parentState;
        private int playerColumn = -1;
        private int playerRow = -1;
        private List<string> units;

        public State(int row, int column)
        {
            units = new List<string>();
            board = new string[row, column];
            this.childrenStates = new List<State>();
            actions = new List<string>();
        }

        public State(List<string> units, int row, int column) : this(row, column)
        {
            this.units = units;
        }

        public State(List<string> units, int row, int column, int goalRow, int goalColumn, int playerRow,
            int playerColumn) : this(units, row, column)
        {
            board[goalRow, goalColumn] = "G";
            board[playerRow, playerColumn] = "P";
            this.playerRow = playerRow;
            this.playerColumn = playerColumn;
        }

        public State(List<string> units, string[,] board, int playerRow, int playerColumn)
        {
            this.units = units;
            this.board = board;
            this.playerRow = playerRow;
            this.playerColumn = playerColumn;
            board[playerRow, playerColumn] = "P";
        }

        public State(State parent, string action)
        {
            this.parentState = parent;
            this.childrenStates = new List<State>();
            this.board = (string[,]) parent.board.Clone();
            this.units = new List<string>(parent.units);

            //TODO: implement new state
        }

        public void dropUnit(string unit, int row, int column)
        {
            var rows = board.GetLength(0);
            var columns = board.GetLength(1);

            if (canDrop(unit, row, column))
            {
                if (unit == "1")
                {
                    board[row, column] = unit;
                }
                // "2": horizontal unit
                else if (unit == "2")
                {
                    board[row, column] = unit;
                    board[row, column + 1] = unit;
                }
                // "3": vertical unit
                else if (unit == "3")
                {
                    board[row, column] = unit;
                    board[row + 1, column] = unit;
                }

            }
        }

        public bool canDrop(string unit, int row, int column)
        {
            var rows = board.GetLength(0);
            var columns = board.GetLength(1);

            if (unit == "1")
            {
                return board[row, column] == null;
            }
            // "2": horizontal unit
            else if (unit == "2")
            {
                return column + 1 < columns && board[row, column] == null && board[row, column + 1] == null;
            }
            // "3": vertical unit
            else if (unit == "3")
            {
                return row + 1 < rows && board[row, column] == null && board[row + 1, column] == null;
            }

            return false;
        }

        public void generateActions()
        {
            var rows = board.GetLength(0);
            var columns = board.GetLength(1);
            bool[] checkUsedUnit = new[] {true, false, false, false};

            for (int k = 0; k < units.Count; k++)
            {
                switch (units[k])
                {
                    case "1":
                        if(checkUsedUnit[1]) continue;
                        checkUsedUnit[1] = true; break;
                    case "2":
                        if(checkUsedUnit[2]) continue;
                        checkUsedUnit[2] = true; break;
                    case "3":
                        if(checkUsedUnit[3]) continue;
                        checkUsedUnit[3] = true; break;
                }
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        // Console.WriteLine(board[i,j] == null);
                        if (board[i, j] == null)
                        {
                            //check unit type and then generate action to action list
                            if (canDrop(units[k], i, j))
                            {
                                //Console.WriteLine($"{k} {i} {j}");
                                actions.Add($"{k} {i} {j}");
                            }
                        }
                    }
                }
            }
            
            // move up
            if (playerRow - 1 < 0 || board[playerRow - 1, playerColumn] == null) ;
            else
            {
                int nextRow = -1;
                for (int i = playerRow - 1; i >= 0; i--)
                {
                    if (board[i, playerColumn] == null)
                    {
                        nextRow = i;
                    }
                }

                if (nextRow != -1)
                {
                    //Console.WriteLine($"U {nextRow} {playerColumn}");
                    actions.Add($"U {nextRow} {playerColumn}");
                }
            }

            // move down
            if (playerRow + 1 >= rows || board[playerRow + 1, playerColumn] == null) ;
            else
            {
                int nextRow = -1;
                for (int i = playerRow + 1; i < rows; i++)
                {
                    if (board[i, playerColumn] == null)
                    {
                        nextRow = i;
                    }
                }

                if (nextRow != -1)
                {
                    //Console.WriteLine($"D {nextRow} {playerColumn}");
                    actions.Add($"D {nextRow} {playerColumn}");
                }
            }

            // move left
            if (playerColumn - 1 < 0 || board[playerRow, playerColumn - 1] == null) ;
            {
                int nextColumn = -1;
                for (int i = playerColumn - 1; i >= 0; i--)
                {
                    if (board[playerRow, i] == null)
                    {
                        nextColumn = i;
                    }
                }

                if (nextColumn != -1)
                {
                    //Console.WriteLine($"L {playerRow} {nextColumn}");
                    actions.Add($"L {playerRow} {nextColumn}");
                }
            }
            // move right
            if (playerColumn + 1 >= columns || board[playerRow, playerColumn + 1] == null) ;
            {
                int nextColumn = -1;
                for (int i = playerColumn + 1; i < columns; i++)
                {
                    if (board[playerRow, i] == null)
                    {
                        nextColumn = i;
                    }
                }

                if (nextColumn != -1)
                {
                    // Console.WriteLine($"R {playerRow} {nextColumn}");
                    actions.Add($"R {playerRow} {nextColumn}");
                }
            }
        }

        public void printActions()
        {
            foreach (var action in actions)
            {
                Console.WriteLine(action);
            }
        }
    }
}