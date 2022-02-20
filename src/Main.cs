using Godot;
using System;
using KikiProject;
using KikiProject.boards;
using KikiProject.tiles;
using KikiProject.ui;
using KikiProject.units;

public class Main : Node2D
{
    private UnitPanel _panel;
    private DynamicBoard _board;
    private TextureButton _textureButtonReset;


    private KikiMCTS ai;

    public override void _Ready()
    {
        _board = GetNode<DynamicBoard>("Board");
        _panel = GetNode<UnitPanel>("Panel");
        _panel.GenerateUnit((_board.Rows * _board.Columns) / 2);
        _textureButtonReset = GetNode<TextureButton>("resetBtn");

        ResizePanel();

        ai = new KikiMCTS(_board, _panel);
        var result = ai.Run();

        GD.Print("ai result = ", result);

        if (result == false)
        {
            GetTree().ReloadCurrentScene();
        }
    }

    void ResizePanel()
    {
        var panelSizeY = (_panel.ScrollContainer.RectSize.y / (OS.WindowSize.y * this.Scale.y)) * OS.WindowSize.y;
        _panel.ScrollContainer.RectSize = new Vector2(_panel.ScrollContainer.RectSize.x, panelSizeY);
    }

    void Reset()
    {
        _board.ResetBoard();
    }
}