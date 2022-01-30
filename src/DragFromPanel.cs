using System.Collections.Generic;
using System.Security.Permissions;
using System.Linq;
using Godot;
using KikiProject.boards;
using KikiProject.tiles;
using KikiProject.ui;
using KikiProject.units;

namespace KikiProject
{
    public class DragFromPanel : Node
    {
        private Main _main;
        private UnitPanel _panel;
        private DynamicBoard _board;
        public bool Enabled = true;

        private DragFromBoard _dragFromBoard;
        private Timer timer;

        private Unit _currentUnit;
        private Control _currentControl;
        
        public override void _Ready()
        {
            _main = GetNode<Main>("../");
            _panel = GetNode<UnitPanel>("../Panel");
            _board = GetNode<DynamicBoard>("../Board");

            _panel.Connect(nameof(UnitPanel.UnitSelected), this, nameof(OnPanelUnitSelected));
            _dragFromBoard = GetNode<DragFromBoard>("../DragFromBoard");
            timer = GetNode<Timer>("../Timer");

        }
        private void OnPanelUnitSelected(Control control, Unit unit)
        {
            if (!Enabled) return;
            if (_currentControl != null && _currentUnit != null) return;
           _dragFromBoard.Enabled = false;
            control.RemoveChild(unit);
            _main.AddChild(unit);
            _currentControl = control;
            _currentUnit = unit;
            _currentUnit.SetUnitStatus(UnitStatus.OnDrag);
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
            _dragFromBoard.Enabled = true;
        }
        private void snap(Unit unit)
        {
            IList<Tile> tiles = _board.Tiles;
            IList<(Tile, SubUnit)> unitsOnOverTiles = (from tile in tiles
                from subunit in unit.SubUnits
                where overlappingTile(subunit, tile) && tile.UnitOver == null
                select (tile, subunit)).ToList();

            if (unitsOnOverTiles.Count() == unit.SubUnits.Count)
            {
                foreach (var tuple in unitsOnOverTiles)
                {
                    tuple.Item1.UnitOver = unit;
                    tuple.Item1.Dehighlight();
                }

                var mainTile = (from t in unitsOnOverTiles where t.Item2 == unit.SubUnits[0] select t.Item1).First();
                unit.GlobalPosition = mainTile.GlobalPosition;
                _currentControl.GetParent().RemoveChild(_currentControl);
                _currentControl = null;
                _currentUnit = null;

                //SendToBoard

                _board.RelocationUnit(unit);
            }
            else
            {
                SendUnitBackToOriginalPosition();
            }
        }
        private void SendUnitBackToOriginalPosition()
        {
            _currentUnit.GetParent().RemoveChild(_currentUnit);
            _panel.AddUnitToControl(_currentControl, _currentUnit);
            _currentUnit.SetUnitStatus(UnitStatus.OnDrop);

            _currentControl = null;
            _currentUnit = null;
        }
        bool overlappingTile(SubUnit subUnit, Tile tile)
        {
            Rect2 rect = new Rect2(tile.GlobalPosition, tile.SpriteSize() * tile.GlobalScale/2f);
            Rect2 unitRect = new Rect2(subUnit.GlobalPosition, ((RectangleShape2D) subUnit.Shape).Extents/2f);
            bool result = rect.Intersects(unitRect);
            return result;
        }
        
        public override void _Process(float delta)
        {
            if (_currentUnit?._unitStatus == UnitStatus.OnDrag)
            {
                _currentUnit.GlobalPosition = _main.GetGlobalMousePosition();
                HighlighOverlapTile();
            }
        }
        void HighlighOverlapTile()
        {
            var tiles = _board.Tiles;
            var unit = _currentUnit;
            var subUnits = unit.SubUnits;

            foreach (Tile tile in tiles)
            {
                IEnumerable<bool> overTileResultList = from subUnit in subUnits
                    where tile.UnitOver == null
                    select overlappingTile(subUnit, tile);
                bool doesAnySubUnitOverTile = overTileResultList.Any(isOver => isOver);

                if (doesAnySubUnitOverTile)
                {
                    tile.Highlight();
                    ;
                }
                else
                {
                    tile.Dehighlight();
                }
            }
        }
    }
}