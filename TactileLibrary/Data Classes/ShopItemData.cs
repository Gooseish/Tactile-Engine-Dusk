using System.IO;

namespace TactileLibrary
{
    public class ShopItemData : Item_Data
    {
        public static ShopItemData read(BinaryReader reader)
        {
            ShopItemData result;
            
            result = new ShopItemData(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
            reader.ReadBoolean(); // Needed for Drops flag

            return result;
        }

        public ShopItemData(Item_Data_Type type, int id, int uses)
            : base(type, id, uses) { }
        public ShopItemData(int type, int id, int uses)
            : base(type, id, uses) { }

        public override void consume_use()
        {
            Uses--;
        }
        public void add_stock()
        {
            Uses++;
        }
    }
}
