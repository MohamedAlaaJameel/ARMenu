using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class MealUiElemnt : MonoBehaviour
{
    public RawImage mealImageObject;
    public TMPro.TextMeshProUGUI TMPMealName;
    string description;
    [Inject]
    DescriptionPanel descriptionPanel;

    [Inject]
    public void SetMealData(Texture2D mealTexture, string mealName, string description)
    {
        if (mealImageObject==null|| TMPMealName==null)
        {
            Debug.LogWarning(" you didnot set all meal ui elemnt args object");
            return;
        }
        mealImageObject.texture = mealTexture;
        TMPMealName.text = mealName;
        this.description = description;
    }
    public void ShowMealDescription()
    {
        descriptionPanel.SetDescription(description);

    }


    public class Factory : PlaceholderFactory<Texture2D, string, string, MealUiElemnt> {}
}
