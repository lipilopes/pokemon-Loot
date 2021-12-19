using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public enum LootType
{
    Normal,
    OnlyMale,
    OnlyFemale,
    OnlyShiny,

    OnlyBasicEvolution,
    OnlyBasicOrFirstEvolution,
    OnlyFirstEvolution,
    OnlyFirstOrSecondEvolution,
    OnlySecondEvolution,

    OnlyNormal,
    OnlyFire,
    OnlyFighting,
    OnlyWater,
    OnlyFlying,
    OnlyGrass,
    OnlyPoison,
    OnlyElectric,
    OnlyGround,
    OnlyPsychic,
    OnlyRock,
    OnlyIce,
    OnlyBug,
    OnlyDragon,
    OnlyGhost,
    OnlyDark,
    OnlySteel,
    OnlyFairy,

    OnlyCommon,
    OnlyRare,
    OnlyEpic,
    OnlyLegendary,

    OnlyAlolan
}

[System.Serializable] 
public enum DropRarity
{
    Common,
    Rare,
    Epic,
    Legendary 
}

public class DropPokeball : MonoBehaviour
{
    public static DropPokeball Instance;

    #if UNITY_EDITOR
    [ContextMenuItem("Complete Dex Test", "DROPLOOTTESTE")]
    [ContextMenuItem("Fast Complete Dex", "FASTCOMPLETEDEXTESTE")]
    [SerializeField] bool FastCompleteDexOnlyShiny = false;

    void DROPLOOTTESTE()
    {
        int count = 0;
        float C=0,R=0,E=0,L=0,S=0;
        while(Pokedex.Instance.CompleteDex() == false)
        {
            GetLootType();
            GetDropRate();

            LootScriptable pk = GetLoot(shinyBonus,maleBonus,femaleBonus);

            Pokedex.Instance.SaveInPokedex(pk);

            if(pk.shiny) S++;

            if(currentDropRate == DropRarity.Common) C++;
            if(currentDropRate == DropRarity.Rare) R++;
            if(currentDropRate == DropRarity.Epic) E++;
            if(currentDropRate == DropRarity.Legendary) L++;           

            ResetLootType();
            count++;
        }

        Debug.LogWarning("Needed "+count+" loots to complete");
        Debug.LogWarning("Common ["+((C/count)*100)+"%] "+C+"/"+count);
        Debug.LogWarning("Rare ["+((R/count)*100)+"%] "+R+"/"+count);
        Debug.LogWarning("Epic ["+((E/count)*100)+"%] "+E+"/"+count);
        Debug.LogWarning("Legendary ["+((L/count)*100)+"%] "+L+"/"+count);
        Debug.LogWarning("Shiny ["+((S/count)*100)+"%] "+S+"/"+count);
    }

    void FASTCOMPLETEDEXTESTE()
    {
        Pokedex pdx = Pokedex.Instance;
        int count = pdx.GetAllPokemons.Count;

        for (int i = 0; i < count; i++)
        {
            LootScriptable pk = pdx.GetAllPokemons[i].loot.Get();

            pk.shiny = FastCompleteDexOnlyShiny;

            if(pk.genderRatio == GenderRatio.OnlyMale ||
               pk.genderRatio == GenderRatio.OnlyFemale ||
               pk.genderRatio == GenderRatio.Genderless)
            {
                Pokedex.Instance.SaveInPokedex(pk);
            }
            else
            {
                pk.gender = Gender.Male;
                Pokedex.Instance.SaveInPokedex(pk);

                pk.gender = Gender.Female;
                Pokedex.Instance.SaveInPokedex(pk);
            }
        }             
    }
    #endif

    [SerializeField]
    int lootNumber = 1;
    public int LootNumber { set{ lootNumber = value; } }  
    WaitForSeconds waitDropLoot = new WaitForSeconds(1.2f);
    WaitForSeconds waitDropLootShiny = new WaitForSeconds(0.5f+0.4f);//time shiny effect + delay
    WaitForSeconds waitDropLootWhile = new WaitForSeconds(0);

    [Header("Drop Rate")]
    [SerializeField,Range(0,1)]
    float dropRateCommon;
    float minDropRateCommon;
    float maxDropRateCommon;

    [SerializeField,Range(0,1)]
    float dropRateRare;
    float minDropRateRare;
    float maxDropRateRare;

    [SerializeField,Range(0,1)]
    float dropRateEpic;
    float minDropRateEpic;
    float maxDropRateEpic;

    [SerializeField,Range(0,1)]
    float dropRateLegendary;
    float minDropRateLegendary;
    float maxDropRateLegendary;

    DropRarity currentDropRate;
    [SerializeField]
    LootType        lootType;
    public LootType LootType{ set { lootType = value; }}

    #region Bonus
        //[SerializeField,Range(0,1)]
        float shinyBonus = 0;
        public float ShinyBonus 
        { 
            set 
            { 
                shinyBonus += value;
                if(shinyBonus < 0) 
                    shinyBonus = 0;
            }
        }
        //[SerializeField,Range(0,225)]
        float maleBonus = 0;
        //[SerializeField,Range(0,225)]
        float femaleBonus = 0;
    #endregion

    bool generateDropRate = false;

    Animator anim;

    void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);

        anim = GetComponent<Animator>();

        minDropRateCommon = 0;
        maxDropRateCommon = minDropRateCommon + dropRateCommon - 0.01f;

        minDropRateRare = maxDropRateCommon + 0.01f;
        maxDropRateRare = minDropRateRare + dropRateRare - 0.01f;

        minDropRateEpic = maxDropRateRare + 0.01f;
        maxDropRateEpic = minDropRateEpic + dropRateEpic - 0.01f;

        minDropRateLegendary = maxDropRateEpic + 0.01f;
        maxDropRateLegendary = minDropRateLegendary + dropRateLegendary;

        LootNumber = PlayerPrefs.GetInt("LootNumber", 1);
    }

    public void OpenPokeball()
    {      
        HudManager.Instance.UpdateCurrentScene = CurrentScene.LootPokemonScene;
        HudManager.Instance.DisableMainScreen();
        //HudManager.Instance.ResetMainPokemon();

        if(lootNumber > 1)
            StartCoroutine(DropLootNumber(lootNumber));
        else
        if(HudManager.Instance.SkipAnim)
            Drop();
        else
            anim.SetTrigger("Open");
    }

    IEnumerator DropLootNumber(int lootNumber=-1,bool promo=false)
    {
        HudToolTipPokedex.Instance.tooltipPokedexCanActive = false;

        float C=0,R=0,E=0,L=0,S=0;

        List<LootScriptable> dropList = new List<LootScriptable>();
        LootScriptable pk = null;

        for (int i = 0; i < lootNumber; i++)
        {
            Debug.Log(i+" / "+lootNumber);
            //if(!promo)
                anim.SetTrigger("Open");
            
            GetLootType();
            GetDropRate();

            pk = GetLoot(shinyBonus,maleBonus,femaleBonus);

            Debug.Log(pk+"["+i+"/"+lootNumber+"]");

            while(pk == null)
            {
                GetDropRate();
                pk = GetLoot(shinyBonus,maleBonus,femaleBonus);
                yield return waitDropLootWhile;
            }

            HudManager.Instance.UpdateScene(pk,false,playAudio: (!HudManager.Instance.SkipAnim || i == (lootNumber-1)));

            dropList.Add(pk);

            if(pk.shiny)
            {
               S++; 

               if(!HudManager.Instance.SkipAnim)
                    yield return waitDropLootShiny;
            } 

            if(currentDropRate == DropRarity.Common) C++;
            if(currentDropRate == DropRarity.Rare) R++;
            if(currentDropRate == DropRarity.Epic) E++;
            if(currentDropRate == DropRarity.Legendary) L++;           

            ResetLootType();

            if(!HudManager.Instance.SkipAnim /*&& !promo*/)
                yield return waitDropLoot;
        }

        HudManager.Instance.LootDropList(dropList,C,R,E,L,S);
        HudManager.Instance.DisableMainScreen(false);

        Debug.LogWarning("Common ["+((C/lootNumber)*100)+"%] "+C+"/"+lootNumber);
        Debug.LogWarning("Rare ["+((R/lootNumber)*100)+"%] "+R+"/"+lootNumber);
        Debug.LogWarning("Epic ["+((E/lootNumber)*100)+"%] "+E+"/"+lootNumber);
        Debug.LogWarning("Legendary ["+((L/lootNumber)*100)+"%] "+L+"/"+lootNumber);
        Debug.LogWarning("Shiny ["+((S/lootNumber)*100)+"%] "+S+"/"+lootNumber);
    }

    void Drop()
    {
        if(lootNumber > 1)
            return;

        generateDropRate = false;

        GetDropRate();

        LootScriptable loot = GetLoot(shinyBonus,maleBonus,femaleBonus);

        if(loot != null)
        {
            HudManager.Instance.UpdateScene(loot);
            ResetLootType();
        }
        else
            if(lootType != LootType.Normal)
            {
                Debug.LogError(lootType+" Not Found ["+currentDropRate+"]");
                lootNumber = 6;
                LootType oldLootType = lootType;
                lootType = LootType.Normal;
                StartCoroutine(DropLootNumber(lootNumber,true));
                lootType = oldLootType;
                lootNumber = 1;
            }

        HudManager.Instance.UpdateCurrentScene = CurrentScene.MainScene;
    }

    DropRarity? GetLootType()
    {
        switch (lootType)
        {
            case LootType.OnlyMale:   maleBonus = 225; break;

            case LootType.OnlyFemale: femaleBonus = 225; break;

            case LootType.OnlyShiny:  shinyBonus = 1; break;

            case LootType.OnlyAlolan: 
                generateDropRate = true;
                currentDropRate  = DropRarity.Epic;
            break;

            #region rarity
            case LootType.OnlyCommon:  
                generateDropRate = true;
                currentDropRate  = DropRarity.Common;
            break;

            case LootType.OnlyRare:  
                generateDropRate = true;
                currentDropRate = DropRarity.Rare;
            break;

            case LootType.OnlyEpic:  
                generateDropRate = true;
                currentDropRate  = DropRarity.Epic;
            break;

            case LootType.OnlyLegendary:
                generateDropRate = true;
                currentDropRate = DropRarity.Legendary;
            break;
            #endregion
        
            case LootType.OnlyGhost: 
                return DropRarity.Rare;

            case LootType.OnlyDragon: 
                return DropRarity.Epic;
        }

        return null;
    }

    void ResetLootType()
    {
        switch (lootType)
        {
            case LootType.OnlyMale:   maleBonus = 0; break;

            case LootType.OnlyFemale: femaleBonus = 0; break;

            case LootType.OnlyShiny:  shinyBonus = 0; break;           
        }
    }

    void GetDropRate()
    {
      DropRarity? minDrop = GetLootType();

        if(generateDropRate)
            return;   

        float minDropRandom = 0;     

        switch (minDrop)
        {
            case DropRarity.Rare: minDropRandom = minDropRateRare; break;
            case DropRarity.Epic: minDropRandom = minDropRateEpic; break;
            case DropRarity.Legendary: minDropRandom = minDropRateLegendary; break;
            default: break;
        }

        float randomNumber = Random.Range(minDropRandom,1f);

        if(randomNumber >= minDropRateCommon && randomNumber <= maxDropRateCommon)
        {
            currentDropRate = DropRarity.Common;
        }
        else
        if(randomNumber >= minDropRateRare     && randomNumber <= maxDropRateRare)
        {
            currentDropRate = DropRarity.Rare;
        }
        else
        if(randomNumber >= minDropRateEpic      && randomNumber <= maxDropRateEpic)
        {
            currentDropRate = DropRarity.Epic;
        }
        else
        if(randomNumber >= minDropRateLegendary && randomNumber <= maxDropRateLegendary)
        {
            currentDropRate = DropRarity.Legendary;
        }
        else
            currentDropRate = DropRarity.Legendary;
    }

    LootScriptable GetLoot(float Bshiny = 0,float Bmale=0,float Bfemale=0)
    {      
        LootScriptable pk = null;
        List<LootDrop> funnel =  Pokedex.Instance.GetLoot(lootType).FindAll(
            delegate(LootDrop ld)
            {
                return ld.rarity == currentDropRate;
            }
            );

        
        
        if(funnel.Count > 0)
             pk = funnel[Random.Range(0,funnel.Count)].loot.Get(shinyBonus,maleBonus,femaleBonus);

        return pk;
    }
}
