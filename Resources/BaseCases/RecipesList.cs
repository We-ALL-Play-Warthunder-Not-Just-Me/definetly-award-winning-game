using Godot;
using System;

[GlobalClass]
public partial class RecipesList : Resource
{
    public RecipesList () {}

    /* 
        OKAY SO
        RECIPES IS GONNA BE: ID , INGREDIENTS
    
        INGREDIENTS ARE GONNA BE: Item/ ID , AMOUNT NEEDED FOR RECIPE
     */
    [Export] public Godot.Collections.Dictionary<int,Ingredients> recipes = [];

}
