using System;
using Godot;
using KikiProject.players;
using KikiProject.units;

namespace KikiProject.tiles
{
    public class Tile : Node2D
    {
        [Export] private Texture[] _texture;
        private Sprite _sprite;
        public Unit UnitOver = null;
        public Player PlayerOver = null;

        Random _random = new Random();

        public override void _Ready()
        {
            _sprite = GetNode<Sprite>("Sprite");
            _sprite.Texture = RandomTexture();
        }

        private Texture RandomTexture()
        {
            int index = _random.Next(0, _texture.Length);
            return _texture[index];
        }

        public Vector2 SpriteSize()
        {
            return _sprite.GetRect().Size * _sprite.Transform.Scale;
        }

        public void Highlight()
        {
            Modulate = Color.Color8(150, 150, 150, 255);
        }

        public void Dehighlight()
        {
            Modulate = Color.Color8(255, 255, 255, 255);
        }
    }
}