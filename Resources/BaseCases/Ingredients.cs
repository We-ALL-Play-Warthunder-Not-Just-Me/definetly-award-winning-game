using Godot;
using System;

[GlobalClass]
public partial class Ingredients : Resource
{
    public Ingredients () {}

    /* 
        OKAY SO
        RECIPES IS GONNA BE: ID , INGREDIENTS
    
        INGREDIENTS ARE GONNA BE: Item/ID, AMOUNT NEEDED FOR RECIPE
     */
    [Export] public Godot.Collections.Dictionary<Item,int> ingredients = [];
   

}
