 
namespace LofiHollow {
    public class ItemDrop {
        public int ItemID; // The item to be dropped
        public int DropChance; // 1 in X chance to drop this
        public int DropQuantity; // 1 - X dropped items if this item is dropped

        public ItemDrop(int ID, int Chance, int Quantity) {
            ItemID = ID;
            DropChance = Chance;
            DropQuantity = Quantity;
        }
    }
}
