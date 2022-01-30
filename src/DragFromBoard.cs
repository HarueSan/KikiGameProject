using System.Collections.Generic;
using Godot;
using KikiProject.boards;
using KikiProject.tiles;
using KikiProject.ui;
using KikiProject.units;
using System.Linq;

namespace KikiProject
{
    public class DragFromBoard : Node
    {
        private Main _main;
        private UnitPanel _panel;
        private DynamicBoard _board;

        public bool Enabled = true;

        private DragFromPanel _dragFromPanel;

        private Unit currentUnit = null;
        private Vector2 dragPosition;
        private Timer timer;

        public override void _Ready()
        {
            _main = GetNode<Main>("../");
            _panel = GetNode<UnitPanel>("../Panel");
            _board = GetNode<DynamicBoard>("../Board");

            //_board.Connect(nameof(Board.UnitSelected), this, nameof(OnUnitSelected));
            
            _dragFromPanel = GetNode<DragFromPanel>("../DragFromPanel");
            timer = GetNode<Timer>("../Timer");

        }
        void OnUnitSelected(Unit unit)
        {
            if (!Enabled) return;
            if (currentUnit != null) return;
            _dragFromPanel.Enabled = false;
            Enabled = false;
            currentUnit = unit;
            dragPosition = unit.GlobalPosition;
            unit.SetUnitStatus(UnitStatus.OnDrag);
            _board.RemoveUnit(unit);
            unit.Connect(nameof(Unit.UnitDrop), this, nameof(OnUnitDropped));
        }
        void OnUnitDropped(Unit unit)
        {
            unit.Disconnect(nameof(Unit.UnitDrop), this, nameof(OnUnitDropped));
            GD.Print("DragFromPanel@OnUnitDropped");
            snap(unit);
            timer.WaitTime = 0.1f;
            timer.Start();
            timer.Connect("timeout", this, nameof(OnTimerTimeout));
        }
        void OnTimerTimeout()
        {
            timer.Disconnect("timeout",this,nameof(OnTimerTimeout));
            _dragFromPanel.Enabled = true;
            Enabled = true;
        }
        private void snap(Unit unit)
        {
            IList<Tile> tiles = _board.Tiles;
            IList<(Tile, SubUnit)> unitsOnOverTiles = (from tile in tiles
                from  subunit in unit.SubUnits
                where overlappingTile(subunit, tile) && tile.UnitOver == null
                select (tile, subunit)).ToList();
            
            GD.Print($"test 1 : {unitsOnOverTiles.Count()} : {unit.SubUnits.Count}  ");
            if (unitsOnOverTiles.Count() == unit.SubUnits.Count)
            {
                GD.Print("test 2");
                foreach (var tuple in unitsOnOverTiles)
                {
                    tuple.Item1.UnitOver = unit;
                    tuple.Item1.Dehighlight();
                }

                var mainTile = (from t in unitsOnOverTiles where t.Item2 == unit.SubUnits[0] select t.Item1).First();
                unit.GlobalPosition = mainTile.GlobalPosition;
                currentUnit = null;
                
                //SendToBoard
                _board.RelocationUnit(unit);
                
            }else
            {
                SendUnitBackToOriginalPosition();
            }
        }
        private void SendUnitBackToOriginalPosition()
        {
            currentUnit.GetParent().RemoveChild(currentUnit);
            _panel.AddUnit(currentUnit);
            currentUnit = null;
        }
        public override void _Process(float delta)
        {
            if (currentUnit?._unitStatus == UnitStatus.OnDrag)
            {
                currentUnit.GlobalPosition = _main.GetGlobalMousePosition();
                HighlighOverlapTile();
            }
        }
        void HighlighOverlapTile()
        {
            var tiles = _board.Tiles;
            var unit = currentUnit;
            var subUnits = unit.SubUnits;

            foreach(Tile tile in tiles)
            {
                IEnumerable<bool> overTileResultList = from subUnit in subUnits where tile.UnitOver==null select overlappingTile(subUnit, tile);
                bool doesAnySubUnitOverTile = overTileResultList.Any(isOver => isOver);
            
                if (doesAnySubUnitOverTile)
                {
                    tile.Highlight();;
                }
                else
                {
                    tile.Dehighlight();
                }
            }
        }
        bool overlappingTile(SubUnit subUnit, Tile tile)
        {
            Rect2 rect = new Rect2(tile.GlobalPosition, tile.SpriteSize() * tile.GlobalScale/2f);
            Rect2 unitRect = new Rect2(subUnit.GlobalPosition, ((RectangleShape2D) subUnit.Shape).Extents/2f);
            bool result = rect.Intersects(unitRect);
            return result;
        }
    }
}
