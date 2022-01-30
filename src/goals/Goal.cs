using System.Collections.Generic;
using Godot;
using KikiProject.tiles;
using KikiProject.units;


namespace KikiProject.goals
{
    public class Goal : Unit
    {
        public void MoveToTile(Tile tile)
        {
            GlobalPosition = tile.GlobalPosition;
        }

        public override List<SubUnit> SubUnits { get; }
        public override Vector2 GetUnitSize()
        {
            throw new System.NotImplementedException();
        }
    }
}