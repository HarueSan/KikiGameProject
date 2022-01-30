using System.Collections.Generic;
using Godot;
using KikiProject.tiles;
using KikiProject.units;


namespace KikiProject.players
{
    public class Player : Unit
    {
        public void MoveToTile(Tile tile)
        {
            GlobalPosition = tile.GlobalPosition + Vector2.Up*20 ;
        }
        public Vector2 GetPlayerOnTilePosition(Tile tile)
        {
            return tile.GlobalPosition + Vector2.Up * 20;
        }
        public override List<SubUnit> SubUnits { get; }
        public override Vector2 GetUnitSize()
        {
            throw new System.NotImplementedException();
        }
        public override void _Ready()
        {
        }
        public AnimationPlayer AnimationPlayer => GetNode<AnimationPlayer>("AnimationPlayer");
        public AnimationTree AnimationTree => GetNode<AnimationTree>("AnimationTree");

        public void Initialize()
        {
            var playerInput = GetNode<PlayerInput>("PlayerInput");
            playerInput.Initialize();
        }
    }
}
