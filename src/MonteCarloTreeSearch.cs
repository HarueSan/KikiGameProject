using System;
using System.Collections.Generic;
using Godot;

namespace DefaultNamespace
{
    // TODO: generate action -> return action list
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
        // TODO: assume initial state

        public static void Main()
        {
            Console.WriteLine("Hello World");
            List<string> units = new List<string>(){"1"};
            int row = 1;
            int column = 3;


            State state = new State(units, row, column, 0, 0, 0, 2);
            state.generateActions();
        }
        
        
        
    }

    public class State
    {
        private List<string> units;
        private string [,] board;
        private List<State> childrenStates;
        private State parentState;
        private List<string> actions;
        private int playerRow = -1;
        private int playerColumn = -1;
    
        public void generateActions()
        {
            for (int k = 0; k < units.Count; k++)
            {
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    for (int j = 0; j < board.GetLength(1); j++)
                    {
                        Console.WriteLine(board[i,j] == null);
                        if (board[i, j] == null)
                        {
                            //TODO: check unit type and then generate action to action list
                            Console.WriteLine($"{k} {i} {j}");
                        }
                    }
                }
            }
            //TODO: add action up down left right than append action to action list
            // move up
            if (board[playerRow-1, playerColumn] == null);
            else
            {
                int nextRow = -1;
                for (int i = playerRow-1; i >= 0; i--)
                {
                    if (board[i, playerColumn] == null)
                    {
                        nextRow = i;
                    }
                }
                if (nextRow != -1)
                {
                    Console.WriteLine("UP");
                }
            }
            
            // TODO: move down
            // TODO: move left
            // TODO: move right
            
            
        }
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
        public State(List<string> units, int row, int column, int goalRow, int goalColumn, int playerRow, int playerColumn) : this(units, row, column)
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
    }
}