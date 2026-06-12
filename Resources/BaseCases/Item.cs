using Godot;
using System;


[GlobalClass] public partial class Item : Resource
{
   [Export] public int ID;
   [Export] public string Name;
   [Export] public int qty;
   [Export] public int max_qty;
   [Export] public Texture2D icon;
   [Export] public bool important = false;
   [Export] public bool consumable = true;

    public Item() : this(0,null,0,0,null)
    {
        
    }

    public Item(int ID, string name, int qty, int max_qty, Texture2D icon, bool important = false, bool consumable = true)
    {
        this.ID = ID;
        this.Name = name;
        this.qty = qty;
        this.max_qty = max_qty;
        this.icon = icon;
        this.important = important;
        this.consumable = consumable;
    }

    public Item shallowCopy()
    {
        return (Item)MemberwiseClone();
    }

    public Item shallowCopy(int newqty)
    {
        Item tempItem = (Item)MemberwiseClone();
        tempItem.qty = newqty;
        return tempItem;
    }

    // I don't have an Object inside my Object
    // public Item deepCopy()
    // {
    //     Item other = (Item)MemberwiseClone();
    //     other.
    //     return other;
    // }

}
