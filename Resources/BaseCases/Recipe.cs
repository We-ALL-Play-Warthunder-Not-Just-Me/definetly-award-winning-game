using Godot;
using System;

[GlobalClass]
public partial class Recipe : Resource
{
    public Recipe () {}

    /* 
        OKAY SO
        RECIPES IS GONNA BE: ID , INGREDIENTS
    
        INGREDIENTS ARE GONNA BE: ID, AMOUNT NEEDED FOR RECIPE
     */
    [Export] public Godot.Collections.Dictionary<int,Godot.Collections.Dictionary<int,int>> recipes = [];

}
