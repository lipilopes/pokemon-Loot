using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class LootDrop 
{ 
    public LootScriptable loot;
    public DropRarity     rarity;
    public bool           canDrop=true;
}

public class Pokedex : MonoBehaviour
{
    public static Pokedex Instance;

    #if UNITY_EDITOR
    [ContextMenuItem("Check % Dex", "GetPokedexStatus")]
    public bool DeleteThisVarAfter;

    void GetPokedexStatus()
    {
        int count               = pokemons.Count;
        float registered        = 0;
        float shinyRegistered   = 0;

        float C=0,R=0,E=0,L=0,S=0;

        if(PlayerPrefs.GetInt("TotalLoot") > 0)
            for (int i = 0; i < count; i++)
            {
                LootScriptable pk   = pokemons[i].loot;
                int maleLoot        = GetIntNamePlayerPrefs(pk);
                int shinyMaleLoot   = GetIntNamePlayerPrefs(pk,shiny: true);
                int femaleLoot      = 0;
                int shinyFemaleLoot = 0;
                int loots           = maleLoot + shinyMaleLoot;          

                if(pk.genderRatio != GenderRatio.OnlyMale && pk.genderRatio != GenderRatio.Genderless)
                {
                    femaleLoot  = GetIntNamePlayerPrefs(pk,Gender.Female);
                    loots       += femaleLoot;

                    shinyFemaleLoot = GetIntNamePlayerPrefs(pk,Gender.Female, true);
                    loots           += shinyFemaleLoot;
                }

                if(pk.shiny) S+= shinyFemaleLoot+shinyMaleLoot;

                if(pokemons[i].rarity == DropRarity.Common) C+=loots;
                if(pokemons[i].rarity == DropRarity.Rare) R+=loots;
                if(pokemons[i].rarity == DropRarity.Epic) E+=loots;
                if(pokemons[i].rarity == DropRarity.Legendary) L+=loots; 

                if(loots > 0)//GetTotalCatches
                {
                    registered++;
                    Debug.Log(pk.Name+"["+pokemons[i].rarity+"] <color=blue>♂["+maleLoot+" / <color=yellow><b>"+shinyMaleLoot+"</b></color>]</color> <color=magenta>♀["+femaleLoot+" / <color=yellow><b>"+shinyFemaleLoot+"</b></color>]</color> - "+loots);
                
                    shinyRegistered += shinyFemaleLoot + shinyMaleLoot;
                }
                else
                    Debug.LogError(pk.Name+" ["+pokemons[i].rarity+"] Not Found");
            }

        int total = PlayerPrefs.GetInt("TotalLoot",0);
        Debug.LogWarning("Common ["+((C/total)*100)+"%] "+C+"/"+total);
        Debug.LogWarning("Rare ["+((R/total)*100)+"%] "+R+"/"+total);
        Debug.LogWarning("Epic ["+((E/total)*100)+"%] "+E+"/"+total);
        Debug.LogWarning("Legendary ["+((L/total)*100)+"%] "+L+"/"+total);
        Debug.LogWarning("Shiny ["+((S/total)*100)+"%] "+S+"/"+total);
        Debug.LogWarning("Pokedex["+((registered/count)*100).ToString("F2")+"%] "+registered+" of "+count+" - <color=yellow>"+shinyRegistered+"</color>/"+total);
    }
    #endif
    [SerializeField]
    List<LootDrop> pokemons = new List<LootDrop>();

    public List<LootDrop> GetAllPokemons { get{ return pokemons;}}

    int maxID=0;
    public int GetMaxIndex{ get{ return maxID; }}

    int getMaxIndexDrop = 0;
    public int GetMaxIndexDrop{ get{ return getMaxIndexDrop; }}

    bool countMax = false;

    ExpLevelManager expM;

    private void Awake() 
    {
         if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this); 

        CountMaxIndex();
    }

    private void Start() 
    {
        expM = ExpLevelManager.Instance;
    }

    void CountMaxIndex()
    {
        int count       =   pokemons.Count,
            currentId   =   0,
            BackId      =   -1;

        for (int i = 0; i < count; i++)
        {
            int id = pokemons[i].loot.id;

            currentId = id;

            if(currentId != BackId)
            {
                maxID++;
                BackId =  currentId;

                if(pokemons[i].canDrop)
                    getMaxIndexDrop++;
                #if UNITY_EDITOR
                else
                    Debug.LogWarning(pokemons[i].loot.Name+" is Not Dropped!!");
                #endif
            }
        }

        countMax = true;
    }

    #region Enable Drop
    public void EnableDrop(LootDrop loot,bool enable = true)
    {
        loot.canDrop = enable;        
    }

    public void EnableDrop(LootScriptable pokemon,bool enable = true)
    {
        LootDrop loot = GetLootDrop(pokemon);
       
        loot.canDrop  = enable;        
    }

    public void EnableDrop(int index,bool enable = true)
    {
        LootDrop loot = GetLootDrop(index);

        loot.canDrop  = enable;   
    }
    #endregion

    #region Get Loot
    public LootDrop GetLootDrop(string name,Form? form = null)
    {
        if(form ==null)
            return pokemons.Find(x => x.loot.Name == name);
        else
            return pokemons.Find(x => x.loot.Name == name && x.loot.form == form);
    }

    public List<LootDrop> GetLoot(LootType lootType)
    {
        List<LootDrop> r = new List<LootDrop>();

            switch (lootType)
            {
                default://DropPokeball  -> OnlyShiny,Commom,Rare,Epic,Legendary 
                    r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop;}
                    );
                break;

                case LootType.OnlyAlolan:
                     r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.form == Form.Alolan;}
                    );
                break;

                case LootType.OnlyMale:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.genderRatio == GenderRatio.OnlyMale;}
                    );
                break;

                case LootType.OnlyFemale:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.genderRatio == GenderRatio.OnlyFemale;}
                    );
                break;

                case LootType.OnlyBasicEvolution:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.evolution == Evolution.BasicEvolution;}
                    );
                break;

                case LootType.OnlyBasicOrFirstEvolution:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && (ld.loot.evolution == Evolution.BasicEvolution || ld.loot.evolution == Evolution.FirstEvolution);}
                    );
                break;

                case LootType.OnlyFirstEvolution:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.evolution == Evolution.FirstEvolution;}
                    );
                break;

                case LootType.OnlyFirstOrSecondEvolution:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && (ld.loot.evolution == Evolution.FirstEvolution || ld.loot.evolution == Evolution.SecondEvolution);}
                    );
                break;

                case LootType.OnlySecondEvolution:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.evolution == Evolution.SecondEvolution;}
                    );
                break;

                case LootType.OnlyNormal:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Normal);}
                    );
                break;

                case LootType.OnlyFire:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Fire);}
                    );
                break;

                case LootType.OnlyFighting:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Fighting);}
                    );
                break;

                case LootType.OnlyWater:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Water);}
                    );
                break;

                case LootType.OnlyFlying:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Flying);}
                    );
                break;

                case LootType.OnlyGrass:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Grass);}
                    );
                break;

                case LootType.OnlyPoison:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Poison);}
                    );
                break;

                case LootType.OnlyElectric:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Electric);}
                    );
                break;

                case LootType.OnlyGround:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Ground);}
                    );
                break;

                case LootType.OnlyPsychic:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Psychic);}
                    );
                break;

                case LootType.OnlyRock:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Rock);}
                    );
                break;

                case LootType.OnlyIce:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Ice);}
                    );
                break;

                case LootType.OnlyBug:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Bug);}
                    );
                break;

                case LootType.OnlyDragon:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Dragon);}
                    );
                break;

                case LootType.OnlyGhost:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Ghost);}
                    );
                break;

                case LootType.OnlyDark:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Dark);}
                    );
                break;

                case LootType.OnlySteel:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Steel);}
                    );
                break;

                case LootType.OnlyFairy:
                  r =  GetAllPokemons.FindAll(
                        delegate(LootDrop ld)
                            {return ld.canDrop && ld.loot.GetType(PokemonType.Fairy);}
                    );
                break;
            }

        return r;
    }

    public LootDrop GetLootDrop(int Id,Form? form = null)
    {
         if(form ==null)
            return pokemons.Find(x => x.loot.id == Id);
        else
            return pokemons.Find(x => x.loot.id == Id && x.loot.form == form);
    }

    public LootDrop GetLootDrop(LootScriptable loot,Form? form = null)
    {
        if(form ==null)
            return pokemons.Find(x => x.loot == loot);
        else
            return pokemons.Find(x => x.loot == loot && x.loot.form == form);
    }
    #endregion

    #region Get Pokemon(s)
    public LootScriptable GetPokemon(string name,Form? form = null)
    {
        if(form ==null)
            return pokemons.Find(x => x.loot.Name == name).loot;
        else
            return pokemons.Find(x => x.loot.Name == name && x.loot.form == form).loot;
    }

    public LootScriptable GetPokemon(int Id,Form? form = null)
    {
        if(form ==null)
            return pokemons.Find(x => x.loot.id == Id).loot;
        else
            return pokemons.Find(x => x.loot.id == Id && x.loot.form == form).loot;
    }

    public LootScriptable GetPokemonLoot(string key)
    {
        return pokemons.Find(x => x.loot.ToString() == key).loot;
    }

    public List<LootScriptable> GetPokemons(int Id)
    {
        List<LootScriptable> r = new List<LootScriptable>();
        r.Add(pokemons.Find(x => x.loot.id == Id  /*&& x.loot.form == Form.Normal*/).loot);
        return r;
    }

    public List<LootScriptable> GetPokemons(LootScriptable loot)
    {
        List<LootScriptable> r = new List<LootScriptable>();
        r.Add(pokemons.Find(x => x.loot == loot).loot);
        return r;
    }
    #endregion

    public void SaveInPokedex(LootScriptable pk)
    {
        Debug.Log("SaveInPokedex( "+pk+" )");

        if(pk == null)
            return;

        PlayerPrefs.SetString("LastPokemonLoot",""+pk);
        PlayerPrefs.SetInt("TotalLoot", PlayerPrefs.GetInt("TotalLoot",0)+1);

        string key = GetKeyNamePlayerPrefs(pk);

        if(GetPokemonRegistered(pk))
        {           
            int number = PlayerPrefs.GetInt(key) + 1;
            PlayerPrefs.SetInt(key, number);
        }
        else
        {
            PlayerPrefs.SetInt(key, 1);

            HudManager.Instance.ToolTipPokedex(pk.pokemon,pk.Name,pk.gender,pk.shiny);
           
            DropRarity r = GetLootDrop(pk).rarity;
            expM.SetExp(expM.GetRarityExp(r),pk.shiny);
        }

        CompleteDex();
      
        #if UNITY_EDITOR
            DropRarity rarity = GetLootDrop(pk).rarity;
            string[] colors = {"white","green","blue","red"};
            Debug.Log("<color="+colors[(int)rarity]+">"+(pk.shiny ? "<color=yellow><b>*</b></color> " : "" )+pk.Name+" ["+rarity+"] -> "+key+" = x"+PlayerPrefs.GetInt(key)+" / "+PlayerPrefs.GetInt("TotalLoot")+"</color>");
        #endif

        if(CanEvolved(pk))
        {
            LootScriptable pkFirstEvolution     = pk.nextEvolution;
            bool pkFE = pkFirstEvolution != null ? !(GetTotalCatches(pkFirstEvolution,pk.shiny) > 0) : false;

            if(pkFirstEvolution == null || !pkFE)
                return;

            LootScriptable pkSecondEvolution    = pkFirstEvolution.nextEvolution;
            bool pkSE = pkSecondEvolution != null ? !(GetTotalCatches(pkSecondEvolution,pk.shiny) > 0) : false;

            if(pkFE || pkSE)
            {
                HudManager.Instance.OpenEvolutionScene(pk);
                Debug.LogWarning("You dont Have a "+pkFirstEvolution.Name+(pkSE ? " or/and "+pkSecondEvolution.Name : ""));
            }           
        }       
    }

    #region Evolved
    public bool CanEvolved(LootScriptable pk)
    {
        if(pk.nextEvolution != null)
        {
            int amount = GetIntNamePlayerPrefs(pk);

            if(amount >= pk.needAmountToEvolution)
                return true;
        }

        return false;
    }

    public LootScriptable Evolved(LootScriptable pk,Gender gender,bool shiny)
    {
        int amount = pk.needAmountToEvolution;

        string key = GetKeyNamePlayerPrefs(pk);

        int number = PlayerPrefs.GetInt(key) - amount;

        if(number <= 0)
            number = 1;

        PlayerPrefs.SetInt(key, number);

        LootScriptable pkE = pk.nextEvolution;

        pkE.pokemon = pkE.GetSprite(gender,shiny);

        pkE.gender = gender;
        pkE.shiny  = shiny;

        SaveInPokedex(pkE);

        return pkE;
    }
    #endregion

    #region Complete Dex
    public int GetNumberCompleteDex()
    {
        if(!countMax)
            CountMaxIndex();

        int count   = pokemons.Count;

        if(PlayerPrefs.GetInt("CompleteDex") == 1)
            return GetMaxIndex;

        int r = 0;
        int loot = 0;
        string name = "";

        for (int i = 0; i < count; i++)
        {                   
            LootScriptable pk = pokemons[i].loot;

            if(pk.Name != name)
            {
                if(loot > 0)
                    r++; 

                loot = GetTotalCatches(pk,true,form: true);
                name = pk.Name;               
            }
            else
            {
                loot += GetTotalCatches(pk,true);  
            }                     
        }  

        if(loot > 0)
            r++; 

        if(r == GetMaxIndexDrop && PlayerPrefs.GetInt("MewAppeared",0) == 0)
        {
            //Gift mew + Enable new bonus
            ExpLevelManager.Instance.Grifts(GriftsBonus.MewAppeared);
        }

        if(r == GetMaxIndex)
        {     
            PlayerPrefs.SetInt("CompleteDex", 1);
            Debug.LogError("DEX COMPLETs"); 
            ExpLevelManager.Instance.Grifts(GriftsBonus.CompletePokedexKanto);
        }

        return r;
    }

    public int GetNumberCompleteShinyDex()
    {
        int count   = pokemons.Count;

        int r = 0;
        int loot = GetTotalCatches(pokemons[0].loot,onlyShiny: true);
        string name = "";

        for (int i = 0; i < count; i++)
        {                   
            LootScriptable pk = pokemons[i].loot;

            if(pk.Name != name)
            {
                if(loot > 0)
                    r++; 

                loot = GetTotalCatches(pk,onlyShiny: true);
                name = pk.Name;               
            }
            else
            {
                loot += GetTotalCatches(pk,onlyShiny: true);  
            }                     
        }   

        return r;
    }

    public bool CompleteDex()
    {
        int count   = pokemons.Count;

        if(PlayerPrefs.GetInt("TotalLoot") < count)
            return false;

         GetNumberCompleteDex();//Set PlayerPrefs -> CompleteDex = 1

        if(PlayerPrefs.GetInt("CompleteDex") == 1)
            return true;

        return false;
    }
    #endregion

    #region Get Pokemon's PlayerPrefs
    public bool GetPokemonRegistered(LootScriptable pk)
    {
        return GetTotalCatches(pk,true) > 0;
    }

    public int GetTotalCatches(LootScriptable pk,bool shiny = false,bool onlyShiny=false,bool form =false)
    {
        int r = 0;
        
        if(!onlyShiny)
            r += GetIntNamePlayerPrefs(pk,Gender.Male,false)+GetIntNamePlayerPrefs(pk,Gender.Female,false)+GetIntNamePlayerPrefs(pk,Gender.Unknown,false);

        if(shiny || onlyShiny)
            r += GetIntNamePlayerPrefs(pk,Gender.Male,true)+GetIntNamePlayerPrefs(pk,Gender.Female,true)+GetIntNamePlayerPrefs(pk,Gender.Unknown,true);

        if(form)
        {
            if(!onlyShiny)
                r += GetIntNamePlayerPrefs(pk,Gender.Male,false,Form.Alolan)+GetIntNamePlayerPrefs(pk,Gender.Female,false,Form.Alolan)+GetIntNamePlayerPrefs(pk,Gender.Unknown,false,Form.Alolan);

            if(shiny || onlyShiny)
                r+= GetIntNamePlayerPrefs(pk,Gender.Male,true,Form.Alolan)+GetIntNamePlayerPrefs(pk,Gender.Female,true,Form.Alolan)+GetIntNamePlayerPrefs(pk,Gender.Unknown,true,Form.Alolan);
        }

        return r;
    }

     public int GetTotalCatchesGender(LootScriptable pk,Gender gender,bool shiny = false,bool onlyShiny=false,bool form =false)
    {
        int r = 0;
        
        if(!onlyShiny)
            r += GetIntNamePlayerPrefs(pk,gender,false);

        if(shiny || onlyShiny)
            r += GetIntNamePlayerPrefs(pk,gender,true);

        if(form)
        {
            if(!onlyShiny)
                r += GetIntNamePlayerPrefs(pk,gender,false,Form.Alolan);

            if(shiny || onlyShiny)
                r+= GetIntNamePlayerPrefs(pk,gender,true,Form.Alolan);
        }

        return r;
    }

    public int GetTotalCatchesAlolanRegistered(LootScriptable pk,bool shiny = false,bool onlyShiny=false)
    {
        int r = 0;
        
        if(!onlyShiny)
            r += GetIntNamePlayerPrefs(pk,Gender.Male,false,Form.Alolan)+GetIntNamePlayerPrefs(pk,Gender.Female,false,Form.Alolan)+GetIntNamePlayerPrefs(pk,Gender.Unknown,false,Form.Alolan);

        if(shiny || onlyShiny)
            r += GetIntNamePlayerPrefs(pk,Gender.Male,true,Form.Alolan)+GetIntNamePlayerPrefs(pk,Gender.Female,true,Form.Alolan)+GetIntNamePlayerPrefs(pk,Gender.Unknown,true,Form.Alolan);

        return r;
    }

    public string GetKeyNamePlayerPrefs(LootScriptable pk,Gender? gender = null,bool? shiny= null,Form? form = null)
    {
        if (gender == null)
        {
            if(pk.genderRatio != GenderRatio.Genderless)
                gender = pk.gender == Gender.Male ?  Gender.Male  :  Gender.Female;
            else
                gender = Gender.Unknown;
        }
        
        if (form == null)
            form = Form.Normal;
        
        if (shiny == null)
            shiny = pk.shiny;

        string f   = (form   != Form.Normal ? pk.form+"_" : "");

        string key = "#"+pk.id+"_"+f+gender+"_"+shiny;

        return key;
    }

    public int GetIntNamePlayerPrefs(LootScriptable pk,Gender? gender = null,bool? shiny= null,Form? form = null)
    {
        return PlayerPrefs.GetInt(GetKeyNamePlayerPrefs(pk,gender,shiny,form));
    }
    #endregion
}
