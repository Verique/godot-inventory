using Godot;
using Grate.Services;
using Grate.Types;

namespace Grate.Inventory
{
    public interface IInventoryService : IService { }

    public class InventoryService : Reference, IInventoryService
    {
        private Grid _grid;
        private InventoryModel _model;

        public InventoryService(CanvasLayer canvasLayer)
        {
            var testButton = canvasLayer.GetNode<Button>("Button");
            var node = canvasLayer.GetNode<InventoryNode>("Inventory");

            _grid = node.Grid;
            _model = new InventoryModel(_grid.GridSize);
            testButton.Connect("button_up", this, nameof(Add));

            _model.ItemAdded += node.CreateItemNode;
            node.LeftMouseButtonUp += OnLeftMouseButtonUp;
        }

        private void OnLeftMouseButtonUp(Vector2 pos)
        {
            var gridPos = (_grid.HasPoint(pos)) ? _grid.LocalToGrid(pos) : null;

            if (_model.PickedItem != null)
            {
                if (gridPos == null)
                    _model.DeletePickedItem();
                else
                    OnItemPut(gridPos);
            }
            else
            {
                if (gridPos != null)
                    OnItemPicked(gridPos);
            }
        }

        private void OnItemPut(Vector2Int pos)
        {
            if (_model.CanPut(pos)) _model.Put(pos);
        }

        private void OnItemPicked(Vector2Int pos)
        {
            if (_model.CheckCoordinatesValid(pos) && _model.HasItemAt(pos)) _model.Pick(pos);
        }

        private void Add()
        {
            var item = new InventoryItem(Vector2Int.Zero);

            for (int x = 0; x < _model.Size.x; x++)
                for (int y = 0; y < _model.Size.y; y++)
                {
                    var newPos = new Vector2Int(x, y);
                    if (_model.CanPlace(item, newPos))
                    {
                        _model.Add(item, newPos);
                        return;
                    }
                }
        }
    }
}
