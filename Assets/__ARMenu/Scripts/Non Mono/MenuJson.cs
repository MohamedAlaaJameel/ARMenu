using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARMenu
{
    public Category[] categories { get; set; }
}

public class Category
{
    public string idCategory { get; set; }
    public string strCategory { get; set; }
    public string strCategoryThumb { get; set; }
    public string strCategoryDescription { get; set; }
    [JsonIgnore]
    public Texture2D texture { get; set; } = null;
    [JsonIgnore]
    public bool downloadedTexture { get; set; } 
    [JsonIgnore]
    public MealsData mealsData { get; set; } = null;
}

public class MealsData
{
    public Meal[] meals { get; set; }
}

public class Meal
{
    public string strMeal { get; set; }
    public string strMealThumb { get; set; }
    public string idMeal { get; set; }
    [JsonIgnore]
    public Texture2D texture { get; set; } = null;
    [JsonIgnore]
    public bool downloadedTexture { get; set; }
}
