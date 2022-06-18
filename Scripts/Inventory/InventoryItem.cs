using Godot;
using Grate.Input;

namespace Grate.Inventory
{
    public interface IInventoryItem : IHasInputEvents<IInventoryItem>
    {
        void Move(Vector2 newPos);
    }

    public class InventoryItem : StaticBody2D, IInventoryItem
    {
        public InputEvents<IInventoryItem> InputEvents { get; private set; }

        public void Move(Vector2 newPos)
        {
            this.Position = newPos;
        }

        public override void _Ready()
        {
            InputEvents = new InputEvents<IInventoryItem>(this);
        }
    }
}
