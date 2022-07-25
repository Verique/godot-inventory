using System;
using Godot;
using Grate.Services;
using Grate.Types;

namespace Grate.Inventory
{
    public interface IInventoryService : IService { }

    public class InventoryService : Reference, IInventoryService
    {
        private InventoryModel model;

        public InventoryService(InventoryNode node, Button testButton)
        {
            model = new InventoryModel(node.GridSize.ToVector2Int());
            testButton.Connect("button_up", this, nameof(Add));

            model.ModelChanged += node.HandleModelChange;
            node.ItemPut += OnItemPut;
            node.ItemPicked += OnItemPicked;
        }

        // TODO Move all non relevant to view thigs as size and conversion methods
        private void OnItemPut(Vector2Int pos)
        {
            model.Put(pos);
        }

        private void OnItemPicked(Vector2Int pos)
        {
            if (model.CheckCoordinatesValid(pos) && model[pos] != null)
                model.Pick(pos);
        }

        private void Add()
        {
            var item = new InventoryItem(DateTime.Now.ToShortDateString());

            for (int x = 0; x < model.Size.x; x++)
                for (int y = 0; y < model.Size.y; y++)
                {
                    item.MainPos = new Vector2Int(x, y);
                    if (model.CanPlace(item))
                    {
                        model.Add(item);
                        return;
                    }
                }
        }
    }
}
