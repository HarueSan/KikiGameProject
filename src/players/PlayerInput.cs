using System;
using System.Runtime.InteropServices;
using Godot;
using KikiProject.boards;
using KikiProject.tiles;


namespace KikiProject.players
{
    public class PlayerInput : Node
    {
        private Player _player;
        private DynamicBoard _board;
        private AnimationNodeStateMachinePlayback _stateMachine;
        private AnimationPlayer _animationPlayer;
        private Tween _tween;
        private Timer _timer;

        public override void _Ready()
        {
           
            
        }

        public void Initialize()
        {
            _board = GetNode<DynamicBoard>("/root/Main/Board");
            _player = GetNode<Player>("../");
            GD.Print(_player);
            _stateMachine = _player.AnimationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback;
            _animationPlayer = _player.AnimationPlayer;
            _tween = GetNode<Tween>("../Tween");
            _timer = GetNode<Timer>("../Timer");
        }

        public override void _Input(InputEvent inputEvent)
        {
            if (inputEvent.IsActionPressed("player_left"))
            {
                _stateMachine.Travel("idleLeft");
            }
            else if (inputEvent.IsActionPressed("player_right"))
            {
                _stateMachine.Travel("idleRight");
            }
            else if (inputEvent.IsActionPressed("player_up"))
            {
                _stateMachine.Travel("idleBack");
            }
            else if (inputEvent.IsActionPressed("player_down"))
            {
                _stateMachine.Travel("idleFront");
            }
            else if (inputEvent.IsActionPressed("player_move"))
            {
                var currentState = _stateMachine.GetCurrentNode();
                switch (currentState)
                {
                    case "idleLeft":
                        _stateMachine.Travel("jumpLeft");
                        break;
                    case "idleRight":
                        _stateMachine.Travel("jumpRight");
                        break;
                    case "idleFront":
                        _stateMachine.Travel("jumpFront");
                        break;
                    case "idleBack":
                        _stateMachine.Travel("jumpBack");
                        break;
                        

                }


            }

        }

        void MoveLeft()
        {
            BoardPosition playerPosition = _board.GetPlayerPosition();
            if (playerPosition == null) return;
            Tile fromTile = _board.LayerTile[playerPosition.Row,playerPosition.Column];;
            Tile nextFirstTile = null;
            Tile nextTile = null;
            
            for (int i = playerPosition.Column - 1; i >= 0; i--)
            {
                var tile = (_board.LayerTile[playerPosition.Row, i]);
                if (nextFirstTile == null) { nextFirstTile = tile; }
                if ((nextFirstTile.UnitOver != null && nextFirstTile.UnitOver != _board.Goal) && (tile.UnitOver == null || tile.UnitOver == _board.Goal))
                {
                    nextTile = _board.LayerTile[playerPosition.Row, i];
                    break;

                }
            }
            
            MovePlayerFromTileToTile(fromTile,nextTile);
        }
        void MoveRight()
        {
            BoardPosition playerPosition = _board.GetPlayerPosition();
            if (playerPosition == null) return;
            Tile fromTile = _board.LayerTile[playerPosition.Row,playerPosition.Column];
            Tile nextFirstTile = null;
            Tile nextTile = null;
            
            for (int i = playerPosition.Column + 1; i < _board.Columns; i++)
            {
                var tile = (_board.LayerTile[playerPosition.Row, i]);
                if (nextFirstTile == null) { nextFirstTile = tile; }
                if ((nextFirstTile.UnitOver != null && nextFirstTile.UnitOver != _board.Goal) && (tile.UnitOver == null || tile.UnitOver == _board.Goal))
                {
                    nextTile = _board.LayerTile[playerPosition.Row, i];
                    break;

                }
            }
            
            MovePlayerFromTileToTile(fromTile,nextTile);
        }
        
        void MoveDown()
        {
            BoardPosition playerPosition = _board.GetPlayerPosition();
            if (playerPosition == null) return;
            Tile fromTile = _board.LayerTile[playerPosition.Row,playerPosition.Column];
            Tile nextFirstTile = null;
            Tile nextTile = null;
            
            for (int i = playerPosition.Row + 1; i < _board.Rows; i++)
            {
                var tile = (_board.LayerTile[i, playerPosition.Column]);
                if (nextFirstTile == null) { nextFirstTile = tile; }
                if ((nextFirstTile.UnitOver != null && nextFirstTile.UnitOver != _board.Goal) && (tile.UnitOver == null || tile.UnitOver == _board.Goal))
                {
                    nextTile = _board.LayerTile[i, playerPosition.Column];
                    break;

                }
            }
            
            MovePlayerFromTileToTile(fromTile,nextTile);
        }
        
        void MoveUp()
        {
            BoardPosition playerPosition = _board.GetPlayerPosition();
            if (playerPosition == null) return;
            Tile fromTile = _board.LayerTile[playerPosition.Row,playerPosition.Column];
            Tile nextFirstTile = null;
            Tile nextTile = null;
            
            for (int i = playerPosition.Row - 1; i >= 0; i--)
            {
                var tile = (_board.LayerTile[i, playerPosition.Column]);
                if (nextFirstTile == null) { nextFirstTile = tile; }
                if ((nextFirstTile.UnitOver != null && nextFirstTile.UnitOver != _board.Goal) && (tile.UnitOver == null || tile.UnitOver == _board.Goal))
                {
                    nextTile = _board.LayerTile[i, playerPosition.Column];
                    break;

                }
            }
            
            MovePlayerFromTileToTile(fromTile,nextTile);
        }
        
         void MovePlayerFromTileToTile(Tile from, Tile to)
        {
            if (to == null)
            {
                BackToIdle();
            }
            else
            {
                from.UnitOver = null;
                to.UnitOver = _player;
                _tween.InterpolateProperty(_player, "global_position", _player.GlobalPosition,
                    _player.GetPlayerOnTilePosition(to), 0.5f);
                _tween.Start();
                _tween.Connect("tween_all_completed", this, nameof(BackToIdle));
            }   
            
        }
        void BackToIdle()
        {
            string state = _stateMachine.GetCurrentNode();
            switch (state)
            {
                case "rideRight" :
                    _stateMachine.Travel("idleRight");
                    break;
                case "rideLeft":
                    _stateMachine.Travel("idleLeft");
                    break;
                case "rideFront":
                    _stateMachine.Travel("idleFront");
                    break;
                case "rideBack":
                    _stateMachine.Travel("idleBack");
                    break;
            }

            
            _timer.Connect("timeout", this, nameof(DisplayWin));
            _timer.WaitTime = 1f;
            _timer.Start();

        }

        private void DisplayWin()
        {
            _timer.Disconnect("timeout",this,nameof(DisplayWin));

            
            _timer.WaitTime = 1f;
            _timer.Connect("timeout", this, nameof(BackToStart));
            _timer.Start();
        }

        private void BackToStart()
        {
            _timer.Disconnect("timeout",this,nameof(BackToStart));
            var playerPosition = _board.GetPlayerPosition();
            GD.Print(playerPosition.toVector2(), _board.GoalPosition);
            if (playerPosition.toVector2() == _board.GoalPosition)
            {
                GetTree().ChangeScene("res://scenes/Start.tscn");
            }
        }
    }
}
