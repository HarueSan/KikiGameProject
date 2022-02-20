using System;
using System.Collections.Generic;


namespace KikiProject
{
    public class KikiNode
    {
        public bool FiniteNode { get; set; } = false;
        public bool WinStatus { get; set; } = false;
        private string _code;
        public string ActionPerform;

        public List<string> ActionList;
        public List<KikiNode> Children;
        public KikiNode Parent;
        public KikiUnitList Unit;
        public KikiBoard Board;

        public string Code
        {
            /*
             * This function using for generate state code.
             */
            get
            {
                if (_code == null)
                {
                    string unitCode = Unit.Encode();
                    string boardCode = Board.ToString();
                    _code = boardCode + "-" + unitCode;
                }


                return _code;
            }
        }
        
        public KikiNode(KikiUnitList unit, KikiBoard board)
        {
            ActionList = new List<string>();
            Children = new List<KikiNode>();
            Unit = unit;
            Board = board;
            
            GenerateActionList();

        }

        public KikiNode(KikiNode parent, string actionPerform)
        {
            ActionList = new List<string>();
            Children = new List<KikiNode>();

            Unit = new KikiUnitList(parent.Unit.Encode());
            Board = new KikiBoard(parent.Board);
            this.Parent = parent;
            this.Parent.Children.Add(this);
            this.ActionPerform = actionPerform;
            
            PerformAction(actionPerform);
            GenerateActionList();
        }

        public KikiNode(KikiNode parent, string actionPerform, List<KikiNode> nodes) : this(parent, actionPerform)
        {
            //  check duplicate if true set finite node to be true
            // else  add this node to nodes
            
            var duplicate = false;
            foreach (KikiNode kikiNode in nodes)
            {
                if (kikiNode.Code == this.Code)
                {
                    Console.WriteLine("DUP : " + this.Code + kikiNode.Code);
                    duplicate = true;
                    break;
                }
                
            }

            if (duplicate)
            {
                FiniteNode = true;
                this.ActionList.Clear();
            }
            else
            {
                nodes.Add(this);
            }
            
        }

        private void PerformAction(string action)
        {
            var actions = action.Split(' ');
            var nextRow = Convert.ToInt32(actions[1]);
            var nextCol = Convert.ToInt32(actions[2]);

            switch (actions[0])
            {
                case "U":
                case "L":
                case "R":
                case "D":
                {
                    // Console.WriteLine("TEST " + this.board[nextRow, nextCol]);

                    this.Board.Board[Board.PlayerRow, Board.PlayerCol] = null;
                    Board.PlayerRow = nextRow;
                    Board.PlayerCol = nextCol;
                    this.Board.Board[Board.PlayerRow, Board.PlayerCol] =
                        this.Board.Board[Board.PlayerRow, Board.PlayerCol] != KikiUnit.Goal
                            ? KikiUnit.Player
                            : KikiUnit.Win;

                    if (this.Board.Board[Board.PlayerRow, Board.PlayerCol] == KikiUnit.Win)
                    {
                        FiniteNode = true;
                        WinStatus = true;
                    }
                    
                    Parent.ActionList.Remove(action);
                    break;
                }
                // unit
                default:
                {
                    var isNumeric = int.TryParse(actions[0], out var unitIndex);
                    if (isNumeric)
                    {
                        var currentUnit = Unit[unitIndex];
                        string currentUnitStr = ((int) currentUnit.Key).ToString(); 
                        switch (currentUnit.Key)
                        {
                            case KikiUnit.Unit.Single:
                                this.Board.Board[nextRow, nextCol] = currentUnitStr;
                                break;
                            case KikiUnit.Unit.Horizontal:
                                this.Board.Board[nextRow, nextCol] = currentUnitStr;
                                this.Board.Board[nextRow, nextCol + 1] = currentUnitStr;
                                break;
                            case KikiUnit.Unit.Vertical:
                                this.Board.Board[nextRow, nextCol] = currentUnitStr;
                                this.Board.Board[nextRow + 1, nextCol] = currentUnitStr;
                                break;
                        }

                        Parent.ActionList.Remove(action);
                        Unit.RemoveAt(unitIndex);
                    }

                    break;
                }
            }
        }


        public void GenerateActionList()
        {
            var rows = Board.Rows;
            var columns = Board.Cols;

            string[,] board = Board.Board;

            bool[] checkUsedUnit = new[] {true, false, false, false};

            for (int k = 0; k < Unit.Count; k++)
            {
                if (checkUsedUnit[(int) Unit[k].Key]) continue;
                checkUsedUnit[(int) Unit[k].Key] = true;

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (board[i, j] == null)
                        {
                            //check unit type and then generate action to action list
                            if (Board.CanDrop(Unit[k], i, j))
                            {
                                ActionList.Add($"{k} {i} {j}");
                            }
                        }
                    }
                }
            }

            // move up
            if (Board.PlayerRow - 1 < 0 || board[Board.PlayerRow - 1, Board.PlayerCol] == null || board[Board.PlayerRow - 1, Board.PlayerCol] == KikiUnit.Goal) ;
            else
            {
                int nextRow = -1;
                for (int i = Board.PlayerRow - 1; i >= 0; i--)
                {
                    if (board[i, Board.PlayerCol] == null || board[i, Board.PlayerCol] == KikiUnit.Goal)
                    {
                        nextRow = i;
                        break;
                    }
                }

                if (nextRow != -1)
                {
                    //Console.WriteLine($"U {nextRow} {playerColumn}");
                    ActionList.Add($"U {nextRow} {Board.PlayerCol}");
                }
            }

            // move down
            if (Board.PlayerRow + 1 >= rows || board[Board.PlayerRow + 1, Board.PlayerCol] == null || board[Board.PlayerRow + 1, Board.PlayerCol] == KikiUnit.Goal) ;
            else
            {
                int nextRow = -1;
                for (int i = Board.PlayerRow + 1; i < rows; i++)
                {
                    if (board[i, Board.PlayerCol] == null || board[i, Board.PlayerCol] == KikiUnit.Goal)
                    {
                        nextRow = i;
                        break;
                    }
                }

                if (nextRow != -1)
                {
                    //Console.WriteLine($"D {nextRow} {playerColumn}");
                    ActionList.Add($"D {nextRow} {Board.PlayerCol}");
                }
            }

            // move left
            if (Board.PlayerCol - 1 < 0 || board[Board.PlayerRow, Board.PlayerCol - 1] == null || board[Board.PlayerRow, Board.PlayerCol - 1] == KikiUnit.Goal) ;
            else
            {
                int nextColumn = -1;
                for (int i = Board.PlayerCol - 1; i >= 0; i--)
                {
                    if (board[Board.PlayerRow, i] == null || board[Board.PlayerRow, i] == KikiUnit.Goal)
                    {
                        nextColumn = i;
                        break;
                    }
                }

                if (nextColumn != -1)
                {
                    //Console.WriteLine($"L {playerRow} {nextColumn}");
                    ActionList.Add($"L {Board.PlayerRow} {nextColumn}");
                }
            }

            // move right
            
            if (Board.PlayerCol + 1 >= columns || board[Board.PlayerRow, Board.PlayerCol + 1] == null || board[Board.PlayerRow, Board.PlayerCol + 1] == KikiUnit.Goal) ;
            else
            {
                int nextColumn = -1;
                for (int i = Board.PlayerCol + 1; i < columns; i++)
                {
                    if (board[Board.PlayerRow, i] == null || board[Board.PlayerRow, i] == KikiUnit.Goal)
                    {
                        nextColumn = i;
                        break;
                    }
                }

                if (nextColumn != -1)
                {
                    // Console.WriteLine($"R {playerRow} {nextColumn}");
                    ActionList.Add($"R {Board.PlayerRow} {nextColumn}");
                }
            }
        }
    }
    
}