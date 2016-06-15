using System.Collections.Generic;

namespace Game.Model
{
    public class InventoryModel
    {
        readonly List<ItemModel> _items = new List<ItemModel>();

        public void AddItem(ItemModel item)
        {
            if(!_items.Contains(item))
            {
                _items.Add(item);
            }
        }

        public bool RemoveItem(ItemModel item)
        {
            if(_items.Contains(item))
            {
                _items.Remove(item);
                return true;
            }
            return false;
        }

        public bool Contains(ItemModel item)
        {
            return _items.Contains(item);
        }
    }
}