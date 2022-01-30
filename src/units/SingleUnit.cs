using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace KikiProject.units
{
    public class SingleUnit : Unit
    {
        [Export] private PackedScene[] _spritesScenes;

        private Random _random = new Random();
        private Sprite _sprite;


        public override void _Ready()
        {
            _sprite = GetNode<Sprite>("Sprite");

            RandomSprite();
            // AdjustCollisionShape();
        }

        //TODO:  random when new game not when reset
        void RandomSprite()
        {
            int index = _random.Next(0, _spritesScenes.Length);
            PackedScene spriteScene = _spritesScenes[index];
            Sprite sprite = spriteScene.Instance<Sprite>();
            sprite.Name = "Sprite";
            RemoveChild(_sprite);
            AddChild(sprite);
            _sprite = sprite;
        }

        public override List<SubUnit> SubUnits => GetNode<Area2D>("Area2D").GetChildren().Cast<SubUnit>().ToList();

        public override Vector2 GetUnitSize()
        {
            var _sprite = GetNode<Sprite>("Sprite");
            return _sprite.GetRect().Size * _sprite.Transform.Scale;
        }

        public void OnInputEvent(Node viewport, InputEvent @event, int shapeIdx)
        {
            if (@event.IsActionPressed("MouseLeftClick"))
            {
                if (_unitStatus == UnitStatus.InActive || _unitStatus == UnitStatus.OnDrop)
                {
                    _unitStatus = UnitStatus.OnDrag;
                    EmitSignal(nameof(UnitDragging), this);
                }
                else
                {
                    EmitSignal(nameof(UnitDrop), this);
                    _unitStatus = UnitStatus.OnDrop;
                }
            }
        }
    }
}