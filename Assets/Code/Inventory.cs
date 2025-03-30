using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Inventory
{
    public List<Item> items;

    public void init()
    {
        items = new List<Item>();
    }

}
