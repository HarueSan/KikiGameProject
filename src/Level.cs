using Godot;

namespace KikiProject
{
    public class Level : Node
    {
        public string[] levels = new[]
        {
            "Level1",
            "Level2",
            "Level3",
            "Level4",
            "Level5",
            "Level6"
        };

        public int currentLevel = 0;
    }
}