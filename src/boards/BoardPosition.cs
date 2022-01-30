using System;
using Godot;


namespace KikiProject.boards
{
    public class BoardPosition : Tuple<int, int>
    {
        public int Row => Item1;
        public int Column => Item2;
        
        public BoardPosition(int item1, int item2) : base(item1, item2)
        {
            
        }

        public Vector2 toVector2()
        {
            return new Vector2(Column, Row);
        }
    }
}