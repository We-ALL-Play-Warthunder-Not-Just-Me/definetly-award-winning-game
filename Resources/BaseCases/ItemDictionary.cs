using Godot;
using System;

[GlobalClass]
public partial class InventoryDictionary : Resource
{
    [Export]
    public Godot.Collections.Dictionary<int,Resource> items = new Godot.Collections.Dictionary<int,Resource>();


    public InventoryDictionary() {}

    
}
