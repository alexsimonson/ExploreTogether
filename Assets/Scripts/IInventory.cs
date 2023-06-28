using UnityEngine;
namespace ExploreTogether {
    public interface IInventory{
        void Initialize();
        void AddItem(Item new_item, bool isStorage);
    }
}
