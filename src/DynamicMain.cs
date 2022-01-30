using Godot;
using System;
using KikiProject.boards;
using KikiProject.tiles;
using KikiProject.ui;
using KikiProject.units;

public class DynamicMain : Node2D
{
    private UnitPanel _panel;
    private DynamicBoard _board;
    private TextureButton _textureButtonReset;

    public override void _Ready()
    {
        _panel = GetNode<UnitPanel>("/root/DynamicPanel");
        _board = GetNode<DynamicBoard>("/root/DynamicBoard");
        _textureButtonReset = GetNode<TextureButton>("TextureButton");
        ResizePanel();
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