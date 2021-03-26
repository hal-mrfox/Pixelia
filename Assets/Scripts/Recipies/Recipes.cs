using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Recipes", menuName = "Recipes")]
public class Recipes : ScriptableObject
{
    public Recipe[] resourceRecipes;

    [System.Serializable]
    public class Recipe
    {
        public Resource type;
        public BuildingType building;
        public RequiredResource[] requiredResources;

        [System.Serializable]
        public struct RequiredResource
        {
            public Resource resource;
            public int amount;
        }
    }
}
