using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public enum GriftsBonus
{
    CompletePokedexKanto,
    MewAppeared
}


public class ExpLevelManager : MonoBehaviour
{
    public static ExpLevelManager Instance;

    [ContextMenuItem("Test Bonus", "Bonus")]
    [SerializeField]    int level=1;
    public              int Level {get{return level;} }

    [SerializeField]    float exp;
    public              float Exp {get{return exp;} }

    [SerializeField]    float maxExp;
    public              float MaxExp {get{return maxExp;} }

    [SerializeField]    float expTotal;
    public              float ExpTotal {get{return expTotal;} }
    [Header("Bonus")]
    [Range(1,10)]
    [SerializeField]    int maxLootNumber = 1;
    public              int MaxLootNumber { get { return maxLootNumber; } }
    int lastMaxLootNumber = -1;

    bool enableMews     = false;
    bool enableLootType = false;
    bool enableMaxLoot  = false;

    DropPokeball dp;
    Pokedex pdx;

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
        dp  = DropPokeball.Instance;
        pdx = Pokedex.Instance;
        Load();
        Bonus();
    }

    void Load()
    {
        level       = PlayerPrefs.GetInt("Level",1);
        exp         = PlayerPrefs.GetFloat("Exp", 0);
        expTotal    = PlayerPrefs.GetFloat("TotalExp", 0);
        CalcMaxExp();

        HudManager.Instance.UpdateExpLevel(expTotal > 0,false);
        HudManager.Instance.UpdateExpLevelUP(false);                       
    }

    public void Bonus(bool levelUp=false)
    {
        if(!dp || !pdx)
            return;

        if(PlayerPrefs.GetInt("CompletePokedexKanto", 0) == 0)
        {
            if(Level <=30)
            {
                dp.LootType = LootType.OnlyBasicEvolution;
            }
            else
            {
                if(Level < 53 /*&& !HardMode || Level <=65 && HardMode*/)
                    dp.LootType = LootType.OnlyBasicOrFirstEvolution;
                else
                    dp.LootType = LootType.Normal;
            }
        }
        else
            if(!enableLootType)
            {
                enableLootType = true;
                HudManager.Instance.ActiveLootType();
                dp.LootType = (LootType)PlayerPrefs.GetInt("LootType", 0); //0 = Normal
                
                if(levelUp)
                    HudManager.Instance.ToolTipNewBonus("Now You can select a loot type!");
            }

        if(!enableMews && (
            PlayerPrefs.GetInt("MewAppeared", 0) == 1 ||
            pdx.GetNumberCompleteDex() >=  pdx.GetMaxIndexDrop))
        {
            if(PlayerPrefs.GetInt("MewAppeared", 0) == 0)
                PlayerPrefs.SetInt("MewAppeared",1);

            pdx.EnableDrop(150, true);
            pdx.EnableDrop(151, true);

            enableMews = true;

            if(levelUp) 
                HudManager.Instance.ToolTipNewBonus("<b>Mew</b> and <b>Mewtwo</b> can be looted now!!!");
        }       

        maxLootNumber = (int)System.Math.Ceiling(Level/10d);

        if(!enableMaxLoot && maxLootNumber > 1)
        {
            enableMaxLoot=true;
            HudManager.Instance.ActiveLootAmount();
            if(levelUp)
                HudManager.Instance.ToolTipNewBonus("Now You can increment your loot amount!");
        }

        if(Level < 99 && maxLootNumber == 10)
            maxLootNumber--; 

        if(lastMaxLootNumber == -1)
            lastMaxLootNumber = maxLootNumber;
        else
            if(lastMaxLootNumber != maxLootNumber)
            {
                if(PlayerPrefs.GetInt("LootNumber") == lastMaxLootNumber)
                    HudManager.Instance.LootAmountSumButton();

                HudManager.Instance.ToolTipNewBonus("New Maximum Loot Amount -> <color=green><b>"+maxLootNumber+"</b></color>");
                lastMaxLootNumber = maxLootNumber;               
            }     
    }

    public void Grifts(GriftsBonus g)
    {
        StartCoroutine(IGrift(g));
    }

    IEnumerator IGrift(GriftsBonus g,float time=8)
    {
        yield return new WaitForSeconds(time);

        switch (g)
        {         
            default:
            break;

            case GriftsBonus.CompletePokedexKanto:
                if(PlayerPrefs.GetInt("CompletePokedexKanto", 0) == 0)
                {
                    PlayerPrefs.SetInt("CompletePokedexKanto", 1);
                    HudManager.Instance.ToolTipNewBonus("What!");
                }    
            break;

            case GriftsBonus.MewAppeared:
                if(PlayerPrefs.GetInt("MewAppeared") == 0)
                {
                    PlayerPrefs.SetInt("MewAppeared",1);
                    HudManager.Instance.UpdateScene(pdx.GetPokemon(151));
                    HudManager.Instance.ToolTipNewBonus("<b>Wild Mew Appeared!</b>");  
                }                        
            break;
        }

        Bonus(true);
    }

    public int GetRarityExp(DropRarity rarity)
    {
        switch (rarity)
        {
            case DropRarity.Common:     return 5; 
            case DropRarity.Rare:       return 10; 
            case DropRarity.Epic:       return 15; 
            case DropRarity.Legendary:  return 20; 
            default: return 0;
        }  
    }
   
    void CalcMaxExp()
    {
        //Total Exp normal 1695 - shinys 2540 T= 4235
        if(level <= 50)//Only Normals
            maxExp = 33.9f;//1695/50
        else
        if(level == 99)
            maxExp = 49.9f;
        else
            maxExp = 50.8f;//2540/50
    }

    public void SetExp(float dropExp,bool bonus=true)
    {
        float leftoverExp = 0;
        float setExp = dropExp+(bonus ? 5 : 0);
        float exExp = setExp+exp;

         if(exExp > MaxExp)
        {               
            leftoverExp = exExp-MaxExp;  

            setExp -= leftoverExp;              
        }

        exp += setExp;
        
        PlayerPrefs.SetFloat("TotalExp", PlayerPrefs.GetFloat("TotalExp")+setExp);

        expTotal    = PlayerPrefs.GetFloat("TotalExp", 0);

        HudManager.Instance.UpdateExpLevel();

        if(exp >= MaxExp)
        {
            LevelUp(); 

            if(leftoverExp > 0)
                SetExp(leftoverExp,false);
        }

        
        PlayerPrefs.SetFloat("Exp",Exp);
    }

    public void LevelUp()
    {
        level++;

        PlayerPrefs.SetInt("Level",Level);

        Bonus(true);

        exp = 0;

        CalcMaxExp();
    }
}
