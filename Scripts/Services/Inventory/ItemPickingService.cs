using Grate.Inventory;
using Godot;
using Grate.Services.Input;

namespace Grate.Services
{
    public interface IItemPickingService : IService
    {
        void RegisterItem(IInventoryItem item);
    }

    public class ItemPickingService : IItemPickingService
    {
        private bool isBusy = false;
        private IInventoryItem pickedItem = null;

        public ItemPickingService(IInputService inputService)
        {
            inputService.MouseMoved += OnMouseMoved;
        }

        public void RegisterItem(IInventoryItem item)
        {
            item.InputEvents.OnLeftMouseUp += Pick;
        }

        private void Pick(IInventoryItem item)
        {
            if (isBusy)
            {
                if (item == pickedItem)
                {
                    PutItem();
                }
                else return;
            }
            else
            {
                PickItem(item);
            }
        }

        private void PutItem()
        {
            isBusy = false;
            pickedItem = null;
        }

        private void PickItem(IInventoryItem item)
        {
            isBusy = true;
            pickedItem = item;
        }

        private void OnMouseMoved(Vector2 mousePos)
        {
            if (isBusy)
            {
                pickedItem.Move(mousePos);
            }
        }
    }
}
