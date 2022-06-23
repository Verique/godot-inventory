using System;

namespace Grate.Models.Inventory{
    public class ItemsContainer{
        private InventoryItem[,] Grid {get; set;} = new InventoryItem[10, 10];
        
        public void AddItem(InventoryItem item, int x, int y){
            if (Grid[x,y] != null) Grid[x,y] = item;
            else throw new Exception("there is an item");
        }

        public InventoryItem this[int x, int y]{
            get => Grid[x, y];
        }
    }
}
