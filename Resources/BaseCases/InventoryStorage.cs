using Godot;
using System;

[GlobalClass]
public partial class InventoryStorage : Resource
{
    [Export]
    public Godot.Collections.Dictionary<int,int> inventory = [];

    public InventoryStorage() {}

}
