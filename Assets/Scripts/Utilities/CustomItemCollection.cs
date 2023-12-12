using System.Collections.Generic;
using System.Linq;

public class CustomItemCollection<T>
{
    private List<T> itemList;

    public CustomItemCollection()
    {
        itemList = new List<T>();
    }

    public void AddItem(T item)
    {
        itemList.Add(item);
    }

    public T GetLastItem()
    {
        return itemList.Last();
    }

    public int GetSumOfLastItems(int itemsCount)
    {
        int sum = 0;

        int count = itemList.Count;

        if (itemsCount > 0)
        {
            for (int i = count - 1; i >= 0 && i >= count - itemsCount; i--)
            {
                if (itemList[i] is int intValue)
                {
                    sum += intValue;
                }
            }
        }

        return sum;
    }

    public int GetSumOfAllItems()
    {
        int sum = 0;

        foreach (var item in itemList)
        {
            if (item is int intValue)
            {
                sum += intValue;
            }
        }

        return sum;
    }

    public void Clear()
    {
        itemList.Clear();
    }
}
