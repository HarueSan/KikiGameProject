namespace KikiProject
{
    public class KikiBoard
    {
        public int Rows;
        public int Cols;

        public int PlayerRow;
        public int PlayerCol;

        public int GoalRow;
        public int GoalCol;
        
        public string[,] Board;
        public KikiBoard(KikiBoard old)
        {
            this.Rows = old.Rows;
            this.Cols = old.Cols;
            this.PlayerRow = old.PlayerRow;
            this.PlayerCol = old.PlayerCol;
            this.GoalRow = old.GoalRow;
            this.GoalCol = old.GoalCol;
            this.Board = (string[,]) old.Board.Clone();
        }

        public KikiBoard(int rows, int cols, int playerRow, int playerCol, int goalRow, int goalCol)
        {
            this.Rows = rows;
            this.Cols = cols;
            this.PlayerRow = playerRow;
            this.PlayerCol = playerCol;
            this.GoalRow = goalRow;
            this.GoalCol = goalCol;

            this.Board = new string[rows, cols];

            this.SetGoalPosition(goalRow, goalCol);
            this.SetPlayerPosition(playerRow, playerCol);
        }

        private void SetPlayerPosition(int row, int col)
        {
            Board[row, col] = KikiUnit.Player;
        }

        private void SetGoalPosition(int row, int col)
        {
            Board[row, col] = KikiUnit.Goal;
        }
         
        public override string ToString()
        {
            /*
             * This function convert object to string in encoding way.
             */
            string output = "";
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    output += Board[i, j] != null ? Board[i, j] : "_";
                }
            }

            return output;
        }

        public bool CanDrop(KikiUnit unit, int row, int col)
        {
            /*
             * This function use for check that can drop a unit on board.
             */
            var rows = Rows;
            var columns = Cols;


            switch (unit.Key)
            {
                case KikiUnit.Unit.Single:
                {
                    return Board[row, col] == null;
                }
                case KikiUnit.Unit.Horizontal:
                {
                    return col + 1 < columns && Board[row, col] == null && Board[row, col + 1] == null;
                }
                case KikiUnit.Unit.Vertical:
                {
                    return row + 1 < rows && Board[row, col] == null && Board[row + 1, col] == null;
                }
            }

            return false;
        }
    }
}