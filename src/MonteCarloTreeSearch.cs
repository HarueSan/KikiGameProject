using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;


namespace DefaultNamespace
{
    // generate action -> return action list

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

        static List<State> states = new List<State>();


        public static void Run()
        {
            State s = states[0];

            s.generateActions();

            foreach (string action in s.Actions)
            {
                State nextState = new State(s, action);
                if (s.addChild(nextState, states, action))
                {
                    states.Add(nextState);
                }
            }
            
        }

        public static void PrintTree(State root, int level)
        {
            for (int i = 0; i < level; i++)
            {
                Console.Write("-");
            }

            Console.Write("[" + root.createdAction + "] " );
            root.printBoard();

            foreach (var child in root.Children)
            {
                PrintTree(child, level + 1);
            }
        }


        public static void Main()
        {
            Console.WriteLine("unitPosition row column");
            List<string> units = new List<string>() {"1", "2", "3"};
            int row = 1;
            int column = 5;

            State rootState = new State(units, row, column, 0, 4, 0, 0);
            rootState.dropUnit("1", 0, 1);

            states.Add(rootState);

            Run();
            PrintTree(rootState,0);
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

        public string createdAction = null;


        public List<string> Actions
        {
            get { return actions; }
        }

        public List<State> Children => childrenStates;


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
            this.playerRow = parent.playerRow;
            this.playerColumn = parent.playerColumn;
            this.actions = new List<string>();

            var actions = action.Split(' ');
            var nextRow = Convert.ToInt32(actions[1]);
            var nextCol = Convert.ToInt32(actions[2]);
            this.createdAction = action;
            switch (actions[0])
            {
                case "U":
                case "L":
                case "R":
                case "D":

                    // Console.WriteLine("TEST " + this.board[nextRow, nextCol]);

                    this.board[playerRow, playerColumn] = null;
                    playerRow = nextRow;
                    playerColumn = nextCol;
                    this.board[playerRow, playerColumn] = this.board[playerRow, playerColumn] != "G" ? "P" : "W";
                    break;
                default:
                    var isNumeric = int.TryParse(actions[0], out var unitIndex);

                    if (isNumeric)
                    {
                        var currentUnit = units[unitIndex];
                        switch (currentUnit)
                        {
                            case "1":
                                this.board[nextRow, nextCol] = currentUnit;
                                break;
                            case "2":
                                this.board[nextRow, nextCol] = currentUnit;
                                this.board[nextRow, nextCol + 1] = currentUnit;
                                break;
                            case "3":
                                this.board[nextRow, nextCol] = currentUnit;
                                this.board[nextRow + 1, nextCol] = currentUnit;
                                break;
                        }

                        units.RemoveAt(unitIndex);
                    }

                    break;
            }
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

            // "1": one unit
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
                        if (checkUsedUnit[1]) continue;
                        checkUsedUnit[1] = true;
                        break;
                    case "2":
                        if (checkUsedUnit[2]) continue;
                        checkUsedUnit[2] = true;
                        break;
                    case "3":
                        if (checkUsedUnit[3]) continue;
                        checkUsedUnit[3] = true;
                        break;
                }

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (board[i, j] == null)
                        {
                            //check unit type and then generate action to action list
                            if (canDrop(units[k], i, j))
                            {
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
                    if (board[i, playerColumn] == null || board[i, playerColumn] == "G")
                    {
                        nextRow = i;
                        break;
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
                    if (board[i, playerColumn] == null || board[i, playerColumn] == "G")
                    {
                        nextRow = i;
                        break;
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
            else
            {
                int nextColumn = -1;
                for (int i = playerColumn - 1; i >= 0; i--)
                {
                    if (board[playerRow, i] == null || board[playerRow, i] == "G")
                    {
                        nextColumn = i;
                        break;
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
            else
            {
                int nextColumn = -1;
                for (int i = playerColumn + 1; i < columns; i++)
                {
                    if (board[playerRow, i] == null || board[playerRow, i] == "G")
                    {
                        nextColumn = i;
                        break;
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


        protected bool Equals(State other)
        {
            return this.board.Equals(other.board);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((State) obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }


        public bool addChild(State newState, List<State> states, string action)
        {
            var actions = action.Split(' ');
            if (!"UDLR".Contains(actions[0]))
            {
                childrenStates.Add(newState);
                return true;
            }

            bool foundDuplicate = false;
            //check duplicate state from states
            foreach (var state in states)
            {
                if (this.Equals(state))
                {
                    foundDuplicate = true;
                    break;
                }
            }

            if (foundDuplicate == false)
            {
                this.childrenStates.Add(newState);
                return true;
            }

            return false;
        }

        public void printBoard()
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] == null)
                    {
                        Console.Write("_" + " ");
                    }
                    else
                    {
                        Console.Write(board[i, j] + " ");
                    }
                }

                Console.WriteLine();
            }
        }
    }
}