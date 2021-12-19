using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable] 
public class TooltipPokedexLine 
{ 
    public Sprite img;
    public string name;
    public Gender gender;
    public bool shiny;
}

public class HudToolTipPokedex : MonoBehaviour
{
    public static HudToolTipPokedex Instance;

    [SerializeField]    Animator                tooltipPokedexAnim;
    [SerializeField]    Image                   tooltipPokedexPokemonImage;
    [SerializeField]    TextMeshProUGUI         tooltipPokedexText;
    /// <summary>Use genders[Male,Female,Unknown]</summary>
    [SerializeField]    Image                       tooltipPokedexGenderImage;
    public              bool                        tooltipPokedexActive        =   false;
    [HideInInspector]
    public              bool                        tooltipPokedexCanActive     =   true;
    [HideInInspector]
    public              List<TooltipPokedexLine>    tooltipPokedexLine          =   new List<TooltipPokedexLine>();

    HudManager hM;

    private void Awake() 
    {
         if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);       
    }
    
    private void Start() 
    {
        hM = HudManager.Instance;
    }

    public void ToolTipPokedex(Sprite img,string name,Gender gender,bool shiny)
    {       
        if(img != null)
        {
            TooltipPokedexLine tt = new TooltipPokedexLine();
            tt.img    = img;
            tt.name   = name;
            tt.gender = gender;
            tt.shiny  = shiny;
            tooltipPokedexLine.Add(tt);
        }
            

        if(tooltipPokedexActive || tooltipPokedexLine.Count <= 0 || !tooltipPokedexCanActive)
            return; 

        Invoke("ShowToolTipPokedex", 1.0f);
    }

    void ShowToolTipPokedex()
    {   
        if(tooltipPokedexLine.Count < 1 || !tooltipPokedexCanActive)
            return;

        TooltipPokedexLine tt = tooltipPokedexLine[0];

        tooltipPokedexLine.RemoveAt(0);
        tooltipPokedexActive = true;
        tooltipPokedexPokemonImage.sprite = tt.img;
        tooltipPokedexText.text    = (tt.shiny ? "<color=yellow>"+tt.name.ToUpper()+"</color>" : tt.name.ToUpper())+"'s data was\nadded to the POKEDEX.";
        tooltipPokedexGenderImage.sprite = hM.genders[(int)tt.gender];

        tooltipPokedexAnim.SetTrigger("Show");

        Invoke("CloseToolTipPokedex", 7.0f);
    }

    void CloseToolTipPokedex()
    {
        tooltipPokedexAnim.SetTrigger("Close");

        if(tooltipPokedexLine.Count >= 1)
        {
            Invoke("ShowToolTipPokedex", 2.0f);
            return;
        }

        tooltipPokedexActive = false;   
    }
}
