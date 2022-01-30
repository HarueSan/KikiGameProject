using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Collections;
using KikiProject.units;
using Container = Godot.Container;

namespace KikiProject.ui
{
    public class DynamicUnitPanel : Node2D
    {
        [Export()] private PackedScene []_unitScenes;
        [Export()] private Vector2 _containerMinSize;
        [Export()] private NodePath _mainNode;

        public ScrollContainer ScrollContainer;
        public VBoxContainer UnitContainers;
        public Sprite Sprite;
        private Main _main;

        public override void _Ready()
        {
            Init();
            InitUnits();
        }

        void Init()
        {
            ScrollContainer = GetNode<ScrollContainer>("ScrollContainer");
            UnitContainers= GetNode<VBoxContainer>("ScrollContainer/UnitContainers");
            Sprite = GetNode<Sprite>("Sprite");
            _main = GetNode<Main>(_mainNode);
            ScrollContainer.GetVScrollbar().RectScale = Vector2.Zero;
            
        }

        void InitUnits()
        {
            foreach (var unitScene in _unitScenes)
            {
                Unit unit = unitScene.Instance<Unit>();
                AddUnit(unit);
            }
        }

        public void AddUnit(Unit unit)
        {
            Control container = new Control();
            container.MouseFilter = Control.MouseFilterEnum.Pass;
            container.RectMinSize = unit.GetUnitSize();
            AddUnitToControl(container,unit);
            UnitContainers.AddChild(container);
            container.Connect("gui_input", this, nameof(OnGuiInput),new Array(){container,unit});
        }

        public void AddUnitToControl(Control control, Unit unit)
        {
            // float x = ScrollContainer.RectSize.x;
            control.AddChild(unit);
            if (unit is HorizontalUnit)
            {
                unit.Position = new Vector2(64,64);    
            }
            else if (unit is VerticleUnit)
            {
                GD.Print("Vertical");
                unit.Position = new Vector2(64,64);
            }
            else
            {
                unit.Position = new Vector2(64,64);
            }
            
            unit.Name = "Unit";
        }

        private Control _currentControl;
        private Unit _currentUnit;

        [Signal]
        public delegate void UnitSelected(Control control, Unit unit);
        
        private void OnGuiInput(InputEvent @event,Control control,Unit unit)
        {
            if (@event.IsActionPressed("MouseLeftClick"))
            {
                EmitSignal(nameof(UnitSelected),control,unit);
            }
        }

        public void RemoveUnit(Unit unit)
        {
            Control removeControl = null;
            foreach (var control in UnitContainers.GetChildren().Cast<Control>())
            {
                if (control.GetChild<Unit>(0) == unit)
                {
                    removeControl = control;
                    break;
                }
            }
            RemoveControl(removeControl);
        }
        private void RemoveControl(Control control)
        {
            UnitContainers.RemoveChild(control);
            control.QueueFree();
        }

    }
}
