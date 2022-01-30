using Godot;
using System;
using KikiProject;

public class Start : Node2D
{
    public override void _Ready()
    {

    }

    void ChangeScene()
    {
        var levelNode = GetNode<Level>("/root/Level");
        var level = levelNode.levels[levelNode.currentLevel];
        // TODO: load level
        //levelNode.currentLevel++;
        LoadScene(level);
    }

    void LoadScene(string level)
    {
        GetTree().ChangeScene($"res://scenes/levels/{level}.tscn");
    }
}
