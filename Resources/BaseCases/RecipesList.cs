using Godot;
using System;

[GlobalClass]
public partial class RecipesList : Resource
{
    public RecipesList () {}

    /* 
        OKAY SO
        RECIPES IS GONNA BE: ID , INGREDIENTS
    
        INGREDIENTS ARE GONNA BE: ID, AMOUNT NEEDED FOR RECIPE
     */
    public Godot.Collections.Dictionary<Item,int> ingredients = [];
    [Export] public Godot.Collections.Dictionary<int,Godot.Collections.Dictionary<Item,int>> recipes = [];

}
