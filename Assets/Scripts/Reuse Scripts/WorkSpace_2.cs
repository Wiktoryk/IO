using UnityEngine;

namespace Reuse_Scripts {
    public class WorkSpace : MonoBehaviour
    {
        Recipes recipes = new Recipes();
        int actualId = -1;
        void Start()
        {
            recipes.Add(0, "Glass", "Paint", "Vase");
            recipes.Add(1, "Can", "String", "Canstring");
            recipes.Add(2, "BottleCap", "String", "Necklace");
            recipes.Add(3, "Vase", "String", "HangVase");
        }
        public bool checkRecipe(string name1, string name2)
        {
            for (int i = 0; i < recipes.getSize();i++)
            {
                if (recipes.getRecipes(i) == name1+name2)
                {
                    actualId = i;
                    return true;
                }
            }
            return false;
        }
        public bool RecipeId()
        {
            bool rec = recipes.checkUseID(actualId);
            recipes.useID(actualId);
            return rec;
        }
        public string getNewObject(string name1, string name2)
        {
            for (int i = 0; i < recipes.getSize(); i++)
            {
                if (recipes.getRecipes(i) == name1 + name2)
                {
                    return recipes.getNewObject(i);
                }
            }
            return "";
        }
    }
}
