using Godot;

namespace KikiProject
{
    public class BackToMenuBtn : TextureButton
    {
        public override void _Ready()
        {
            Connect("pressed", this, nameof(OnPressed));
        }

        void OnPressed()
        {
            GetTree().ChangeScene("res://scenes/Start.tscn");
        }
    }
}