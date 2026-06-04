using Godot;
using System;

[GlobalClass]
public partial class ItemDictionary : Resource
{
    [Export]
    public Godot.Collections.Dictionary<int,Resource> items = new Godot.Collections.Dictionary<int,Resource>();


    public ItemDictionary() {}

    
}
