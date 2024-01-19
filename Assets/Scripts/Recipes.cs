using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipes
{
    int id;
    List<Recipes> recipesList = new List<Recipes>();
    string object1;
    string object2;
    string makeObject;
    bool use;
    public Recipes()
    {
    }
    public bool Add(int id, string object1, string object2, string makeObject)
    {

        Recipes recipe = new Recipes(id, object1, object2, makeObject);
        recipesList.Add(recipe);
        return true;
    }
    public Recipes(int id, string object1, string object2, string makeObject)
    {
        this.id = id;
        this.object1 = object1;
        this.object2 = object2;
        this.makeObject = makeObject;
        this.use = false;
    }
    public int getSize()
    {
        return recipesList.Count;
    }
    public string getRecipes(int id)
    {
        for (int i = 0; i < getSize(); i++)
        {
            if (id == recipesList[i].id)
            {
                return recipesList[i].object1 + recipesList[i].object2;
            }
        }
        return "";
    }
    public string getNewObject(int id)
    {
        for (int i = 0; i < getSize(); i++)
        {
            if (id == recipesList[i].id)
            {
                return recipesList[i].makeObject;
            }
        }
        return "";
    }
    public void useID(int id)
    {
        for (int i = 0; i < getSize(); i++)
        {
            if (id == recipesList[i].id)
            {
                recipesList[i].use = true;
            }
        }
    }
    public bool checkUseID(int id)
    {
        for (int i = 0; i < getSize(); i++)
        {
            if (id == recipesList[i].id)
            {
                return recipesList[i].use;
            }
        }
        return true;
    }
}
