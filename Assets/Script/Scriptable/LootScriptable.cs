using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public enum PokemonType 
{
    Normal,
    Fire,
    Fighting,
    Water,
    Flying,
    Grass,
    Poison,
    Electric,
    Ground,
    Psychic,
    Rock,
    Ice,
    Bug,
    Dragon,
    Ghost,
    Dark,
    Steel,
    Fairy
}

[System.Serializable] 
public enum Evolution
{
    BasicEvolution,
    FirstEvolution,
    SecondEvolution
}

/// <summary>
/// ♂:♀
/// </summary>
[System.Serializable] 
public enum GenderRatio
{
    /// <summary>
    /// 1♂:7♀
    /// </summary>
    _1x7,
    /// <summary>
    /// 1♂:3♀
    /// </summary>
    _1x3,
    /// <summary>
    /// 1♂:1♀
    /// </summary>
    _1x1,
    /// <summary>
    /// 3♂:1♀
    /// </summary>
    _3x1,
    /// <summary>
    /// 7♂:1♀
    /// </summary>
    _7x1,
    /// <summary>
    /// ♀
    /// </summary>
    OnlyFemale,
    /// <summary>
    /// ♂
    /// </summary>
    OnlyMale,
    /// <summary>
    /// Unknown
    /// </summary>
    Genderless
}

[System.Serializable] 
public enum Gender
{
    Male,
    Female,
    Unknown
}

[System.Serializable] 
public enum Form
{
    Normal,
    Alolan
}

[CreateAssetMenu(fileName = "PokemonLoot", menuName = "Loot/PokemonLoot", order = 1)
//,HelpURL("https://docs.unity3d.com/ScriptReference/Editor.html")
]
public class LootScriptable : ScriptableObject
{
    [HideInInspector]
    public Sprite pokemon;
    [SerializeField,Tooltip("0[Front] Male, 1 Shiny, 3 Female*, 4 Female_Shiny*")]
    public Sprite[] sprites = {null,null };
    public Sprite[] Sprites { get {return sprites;}}
    [Space]
    [Tooltip("Id Pokedex")]
    public int id;
    public string Name;
    [TextArea]
    public string description;
    public AudioClip sound;
    [Space]
    public PokemonType[] types;
    [HideInInspector]
    public Gender gender;
    [Tooltip("♂:♀")]
    public GenderRatio genderRatio;
    [HideInInspector]
    public bool shiny = false;

    public Evolution evolution; 
    public Form form;
    [Header("Evolution")]
    public LootScriptable nextEvolution;
    public int            needAmountToEvolution = 4;

    protected float bonusShiny = 0;
    protected float bonusMale = 0;
    protected float bonusFemale = 0;

    bool testShiny = false;

    public bool GetType(PokemonType pkT)
    {
        int count = types.Length;
        for (int i = 0; i < count; i++)
        {
            if(types[i] == pkT)
             return true;
        }

        return false;
    }

    public Sprite GetSprite(Gender _gender,bool _shiny)
    {
        bool? GenderMale = null;

        if(genderRatio != GenderRatio.Genderless)
            switch (_gender)
            {
                case Gender.Male:
                    GenderMale = true;
                break;
                case Gender.Female:
                    GenderMale = false;
                break;
             }   

        if (GenderMale == true && _shiny==false || GenderMale == false && sprites.Length < 3 && _shiny == false || GenderMale == null && _shiny == false)
        {
            //Debug.Log("Male sprite");
           return sprites[0];
        }
        
        if (GenderMale == true && _shiny == true || GenderMale == null && _shiny == true || GenderMale == false && sprites.Length < 3 && _shiny == true)
        {
            //Debug.Log("Shiny Male sprite");
            return sprites[1];
        }
        
        if (GenderMale == false && sprites.Length >= 3 && _shiny == false)
        {
            //Debug.Log(" Female sprite");
            return sprites[2];
        }
       
        if (GenderMale == false && sprites.Length >= 3 && _shiny == true)
        {
            //Debug.Log("Shiny Female sprite");
            return sprites[3];
        }

        return sprites[0];
    }

    public Sprite GetSpriteNextEvolution(Gender _gender,bool _shiny)
    {
        bool? GenderMale = null;

        if(nextEvolution.genderRatio != GenderRatio.Genderless)
            switch (_gender)
            {
                default:
                    GenderMale = true;
                break;

                case Gender.Female:
                    GenderMale = false;
                break;
             }   

        if (GenderMale == true && _shiny==false | GenderMale == false && nextEvolution.sprites.Length < 3 && _shiny == false | GenderMale == null && _shiny == false)
        {
            Debug.Log("Male sprite");
           return nextEvolution.sprites[0];
        }
        
        if (GenderMale == true && _shiny == true | GenderMale == null && _shiny == true | GenderMale == false && nextEvolution.sprites.Length < 3 && _shiny == true)
        {
            Debug.Log("Shiny Male sprite");
            return nextEvolution.sprites[1];
        }
        
        if (GenderMale == false && nextEvolution.sprites.Length >= 3 && _shiny == false)
        {
            Debug.Log(" Female sprite");
            return nextEvolution.sprites[2];
        }
       
        if (GenderMale == false && nextEvolution.sprites.Length >= 3 && _shiny == true)
        {
            Debug.Log("Shiny Female sprite");
            return nextEvolution.sprites[3];
        }

        return nextEvolution.sprites[0];
    }

    protected void SetShiny()
    {
        if(testShiny)
            return;
     
        testShiny= true;

        int correctCatches = Pokedex.Instance.GetTotalCatches(this,true)+1;
        float   catches    = correctCatches>1 ? correctCatches : 0,
                catchBonus = (catches/500),
                completeDex = PlayerPrefs.GetInt("CompleteDex")==1 ? 0.05f : 0;

        if(catchBonus > 0.15)
            catchBonus = 0.15f;

        float random = Random.Range(0f, 1f),
              value  = 0.01f+completeDex+catchBonus+bonusShiny;

        if(catchBonus>0.0)
            Debug.LogWarning(Name+" Bonus Shiny "+(value*100).ToString("F2")+"% catchBonus["+(catchBonus*100).ToString("F3")+"%/"+catches+" - Limit 15%]");

        shiny = random <= value;       
    }

    protected void SetGender()
    {
        bool? male = null;

        GenderRatio _gender = genderRatio;

        if (_gender == GenderRatio.OnlyMale)
            male = true;
        else
        if (_gender == GenderRatio.OnlyFemale)
            male = false;
        else
        if (_gender == GenderRatio.Genderless)
            male = null;
        else
        {
            float _random = Random.Range(1, 252),
                  _g = 0;

            switch (_gender)
            {
                case GenderRatio._1x7:
                    _g = 225;
                break;

                case GenderRatio._1x3:
                    _g = 191;
                break;

                case GenderRatio._1x1:
                    _g = 127;
                break;

                case GenderRatio._3x1:
                    _g = 63;
                break;

                case GenderRatio._7x1:
                    _g = 31;
                break;
            }  

            male = (_random+bonusFemale <= _g+bonusMale);
        }

        if (male == false)
            gender = Gender.Female;
        else
        if (male == true)
            gender = Gender.Male;
        else
        if (male == null)
            gender = Gender.Unknown;
    }

    //protected void GetSprite()
    //{
        

        // bool? GenderMale = null;
        // bool? _shiny = shiny;


        // switch (gender)
        // {
        //     case Gender.Male:
        //         GenderMale = true;
        //         break;
        //     case Gender.Female:
        //         GenderMale = false;
        //         break;
        // }   

           
    //}

    public LootScriptable Get(float Bshiny = 0,float Bmale=0,float Bfemale=0)
    {      
        testShiny = false;

        bonusShiny  = Bshiny;
        bonusMale   = Bmale;
        bonusFemale = Bfemale;

        // Debug.Log("Shiny ["+(bonusShiny*100)+"%]");
        // Debug.Log("Male ["+((bonusMale/225)*100)+"%]");
        // Debug.Log("Female ["+((bonusFemale/225)*100)+"%]");

        SetShiny();
        SetGender();

        pokemon = GetSprite(gender, shiny);

        return this;
    }
}
