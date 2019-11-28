using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory : IEnumerable<ItemObject>
{
    [SerializeField]
    [HideInInspector]
    private int xsize, ysize;

    [SerializeField]
    private ItemObject[] items;

    public Inventory(int xsize, int ysize)
    {
        this.xsize = xsize;
        this.ysize = ysize;
        items = new ItemObject[xsize * ysize];
    }
    public int XSize => xsize;
    public int YSize => ysize;
    public int Length => items.Length;
    public ItemObject this[int x, int y]
    {
        get {
            if (x < 0 || x >= xsize)
                return null;

            if (y < 0 || y >= ysize)
                return null;

            for (int y0 = 0; y0 <= y; y0++)
                for (int x0 = 0; x0 <= x; x0++)
                {
                    ItemObject i = items[x0 + y0 * xsize];

                    if (i == null)
                        continue;

                    if (x0 + i.InventorySizeX - 1 >= x && y0 + i.InventorySizeY - 1 >= y)
                        return i;
                }

            return null;
        }
    }

    public ItemObject Get(int x, int y, out int arrayX, out int arrayY)
    {
        if (x < 0 || x >= xsize || y < 0 || y >= ysize)
        {
            arrayX = -1;
            arrayY = -1;
            return null;
        }

        for (int y0 = 0; y0 <= y; y0++)
            for (int x0 = 0; x0 <= x; x0++)
            {
                ItemObject i = items[x0 + y0 * xsize];

                if (i == null)
                    continue;

                if (x0 + i.InventorySizeX - 1 >= x && y0 + i.InventorySizeY - 1 >= y)
                {
                    arrayX = x0;
                    arrayY = y0;
                    return i;
                }
            }

        arrayX = -1;
        arrayY = -1;
        return null;
    }

    public bool Empty(int x, int y, int w, int h)
    {
        if (x < 0 || x + w - 1 >= xsize)
            return false;

        if (y < 0 || y + h - 1 >= ysize)
            return false;

        for (int x1 = x; x1 < x + w; x1++)
            for (int y1 = y; y1 < y + h; y1++)
            {
                if (this[x1, y1])
                    return false;
            }

        return true;
    }

    public bool Contains(ItemObject item)
    {
        foreach (ItemObject i in items)
            if (i == item)
                return true;
        return false;
    }

    public bool Contains(ItemObject item, out int x, out int y)
    {
        x = -1;
        y = -1;
        if (item == null)
            return false;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == item)
            {
                x = i % xsize;
                y = i / xsize;
                return true;
            }
        }

        return false;
    }

    public ItemObject Remove(int x, int y)
    {
        ItemObject get = Get(x, y, out int arrayX, out int arrayY);
        if (get == null)
            return null;
        items[arrayX + arrayY * xsize] = null;
            return get;
    }

    public bool Add(ItemObject item)
    {
        return Add(item, out _, out _, true);
    }

    public bool Add(ItemObject item, out int arrayX, out int arrayY, bool confirm = true)
    {
        arrayX = -1;
        arrayY = -1;
        if (item == null)
            return false;

        if (item.InventorySizeX <= 0 || item.InventorySizeY <= 0)
            return false;

        for (int x = 0; x < xsize - item.InventorySizeX + 1; x++)
            for (int y = 0; y < ysize - item.InventorySizeY + 1; y++)
            {
                if (Empty(x, y, item.InventorySizeX, item.InventorySizeY))
                {
                    arrayX = x;
                    arrayY = y;
                    if (confirm) items[x + y * xsize] = item;
                    return true;
                }
            }
        return false;
    }

    public bool Add(ItemObject item, int x, int y)
    {
        return Add(item, x, y, out _, out _, out _, true);
    }

    public bool Add(ItemObject item, int x, int y, out ItemObject swap, out int swapX, out int swapY, bool confirm = true)
    {
        swap = null;
        swapX = -1;
        swapY = -1;

        if (item == null)
            return false;

        if (item.InventorySizeX <= 0 || item.InventorySizeY <= 0)
            return false;

        if (x < 0 || x + item.InventorySizeX > xsize)
            return false;

        if (y < 0 || y + item.InventorySizeY > ysize)
            return false;

        int arrayX = 0;
        int arrayY = 0;

        for (int x0 = x; x0 < x + item.InventorySizeX; x0++)
        {
            for (int y0 = y; y0 < y + item.InventorySizeY; y0++)
            {

                ItemObject i = Get(x0, y0, out arrayX, out arrayY);

                if (swap != null && i != null && swap != i)
                {
                    swap = null;
                    swapX = -1;
                    swapY = -1;
                    return false;
                }
                
                if (i != null)
                {
                    swap = i;
                    swapX = arrayX;
                    swapY = arrayY;
                }
            }
        }

        if (confirm)
        {
            if (swap != null)
                items[swapX + swapY * xsize] = null;
            items[x + y * xsize] = item;
        }

        return true;
    }

    public static IEnumerable<T> Get2D<T>(T[,] table)
    {
        for (int x = 0; x < table.GetLength(0); x++)
            for (int y = 0; y < table.GetLength(1); y++)
            yield return table[x, y];
    }

    public IEnumerator<ItemObject> GetEnumerator()
    {
        return ((IEnumerable<ItemObject>)items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<ItemObject>)items).GetEnumerator();
    }
}
