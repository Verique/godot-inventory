using Godot;
using Grate.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Grate.Inventory
{
    public class InventoryItemModuleNode : TextureRect
    {
        public InventoryItemModuleNode(Color color, Vector2 position)
        {
            RectPosition = position;
            this.Texture = ResourceLoader.Load<StreamTexture>("res://sprites/item1x1.png");
            this.Modulate = color;
            this.MouseFilter = MouseFilterEnum.Ignore;
        }
    }

    public class InventoryItemNode : Control
    {
        public event Action<InventoryItemNode, Vector2, bool> ItemClicked;

        private Vector2 _offset;
        private Dictionary<Vector2Int, InventoryItemModuleNode> _modulesByLayout;

        public override bool HasPoint(Vector2 point)
        {
            return _modulesByLayout.Any(pair => pair.Value.HasPoint(point));
        }

        public InventoryItemNode(IInventoryItemInfo item, InventoryNode inventory, Vector2Int gridPosition)
        {
            this.MouseFilter = MouseFilterEnum.Ignore;
            _modulesByLayout = new Dictionary<Vector2Int, InventoryItemModuleNode>();
            foreach (var point in item.Layout)
            {
                var pos = (point) * inventory.CellSize;
                var module = new InventoryItemModuleNode(item.Color, pos.ToVector2());
                this.AddChild(module);
                _offset = Vector2.One * inventory.CellSize / 2;
                _modulesByLayout.Add(point, module);
            }
        }

        public void Pick()
        {
            var rng = new RandomNumberGenerator();
            rng.Randomize();
            var color = Color.Color8((byte)rng.RandiRange(0, 255), (byte)rng.RandiRange(0, 255), (byte)rng.RandiRange(0, 255));
            foreach (var (_, module) in _modulesByLayout)
            {
                module.Modulate = color;
            }
        }

        public void Put(Vector2 pos)
        {
            RectPosition = pos;
        }

        // TODO: Move all logic here, make modules stupid | maybe in future we'll need to link module to module?
    }
}

