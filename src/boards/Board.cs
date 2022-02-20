using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using KikiProject.goals;
using KikiProject.players;
using KikiProject.tiles;
using KikiProject.ui;
using KikiProject.units;


namespace KikiProject.boards
{
    public class Board : Node2D
    {
        [Export] public int Rows = 0;
        [Export] public int Columns = 0;
        [Export] public Vector2 GoalPosition;
        [Export] private Vector2 PlayerStartPosition;

        public Tile[,] LayerTile;

        [Signal]
        public delegate void UnitSelected(Control control, Unit unit);

        public List<Tile> Tiles => LayerTile.Cast<Tile>().ToList();
        public Player Player;
        public Goal Goal;
        private Node2D _tilesNode;
        private Node2D _playersNode;
        private Node2D _goalsNode;
        private Node2D _unitsNode;
        private UnitPanel _unitPanel;
        [Export] private NodePath _unitPanelPath;

        private PackedScene tileScene;
        private PackedScene playerScene;
        private PackedScene goalScene;


        public override void _Ready()
        {
       
            
            tileScene = ResourceLoader.Load<PackedScene>("res://scenes/tiles/Tile.tscn");
            playerScene = ResourceLoader.Load<PackedScene>("res://scenes/players/Player.tscn");
            goalScene = ResourceLoader.Load<PackedScene>("res://scenes/goals/Goal.tscn");

            LayerTile = new Tile[Rows, Columns];

            _tilesNode = GetNode<Node2D>("Tiles");
            _playersNode = GetNode<Node2D>("Players");
            _unitsNode = GetNode<Node2D>("Units");
            _goalsNode = GetNode<Node2D>("Goals");
            _unitPanel = GetNode<UnitPanel>(_unitPanelPath);

            GenerateBoard();
            InitializePlayer();
            InitializeGoal();
        }

        void InitializeGoal()
        {
            Goal = goalScene.Instance<Goal>();
            _goalsNode.AddChild(Goal);
            var tile = LayerTile[(int) GoalPosition.y, (int) GoalPosition.x];
            tile.UnitOver = Goal;
            Goal.MoveToTile(tile);
        }

        void InitializePlayer()
        {
            Player = playerScene.Instance<Player>();
            _playersNode.AddChild(Player);
            var tile = LayerTile[(int) PlayerStartPosition.y, (int) PlayerStartPosition.x];
            Player.MoveToTile(tile);
            tile.UnitOver = Player;
        }

        public void ResetBoard()
        {
            Tiles.ForEach(tile => tile.UnitOver = null);
            var StartTile = LayerTile[(int) PlayerStartPosition.y, (int) PlayerStartPosition.x];
            Player.MoveToTile(StartTile);
            StartTile.UnitOver = Player;

            foreach (Unit currentUnit in _unitsNode.GetChildren())
            {
                currentUnit.GetParent().RemoveChild(currentUnit);
                _unitPanel.AddUnit(currentUnit);
            }
        }

        private void GenerateBoard()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    Tile tile = tileScene.Instance<Tile>();
                    LayerTile[row, col] = tile;
                    _tilesNode.AddChild(tile);
                    tile.Position = tile.SpriteSize() * new Vector2(col, row) + tile.SpriteSize() / 2;
                }
            }
        }

        public void RelocationUnit(Unit unit)
        {
            var position = unit.GlobalPosition;
            unit.GetParent().RemoveChild(unit);
            _unitsNode.AddChild(unit);
            unit.GlobalPosition = position;
            unit.Connect(nameof(Unit.UnitDragging), this, nameof(OnUnitSelected));
        }

        void OnUnitSelected(Unit unit)
        {
            EmitSignal(nameof(Board.UnitSelected), unit);
        }

        public void RemoveUnit(Unit unit)
        {
            unit.Disconnect(nameof(Unit.UnitDragging), this, nameof(OnUnitSelected));

            foreach (var tile in Tiles)
            {
                if (tile.UnitOver == unit)
                {
                    tile.UnitOver = null;
                }
            }
        }

        public BoardPosition GetPlayerPosition()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (LayerTile[row, col].UnitOver == Player)
                    {
                        return new BoardPosition(row, col);
                    }
                }
            }

            return null;
        }
    }
}