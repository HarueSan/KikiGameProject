using System.Collections.Generic;
using Godot;

namespace KikiProject.units
{
    public enum UnitStatus
    {
        InActive,
        OnDrag,
        OnDrop,
    }

    public abstract class Unit : Node2D
    {
        public UnitStatus _unitStatus = UnitStatus.InActive;

        public abstract List<SubUnit> SubUnits { get; }

        [Signal]
        public delegate void UnitDragging(Unit unit);
        [Signal]
        public delegate void UnitDrop(Unit unit);
        
        public abstract Vector2 GetUnitSize();
        public void SetUnitStatus(UnitStatus status)
        {
            _unitStatus = status;
        }
    }
}