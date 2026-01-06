using System.Linq;

public static class InventoryHelper
{
    public static int GetAmount(string oreId)
    {
        if (SaveManager.I == null || SaveManager.I.Data == null)
            return 0;

        var list = SaveManager.I.Data.globalOreInventory;
        var stack = list.FirstOrDefault(o => o.oreId == oreId);
        return stack?.amount ?? 0;
    }
}
