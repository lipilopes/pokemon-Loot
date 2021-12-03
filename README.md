 [^1]: Repeated x1 -> +0.2%. max 15% or x75.
 [^2]: 1695 = Only Normal Pokemon's Loot - 1695/50.
 [^3]: 2540 = Only Shiny Pokemon's Loot - 2540/50.
 [^4]: Normal or Alolan
 [^5]: Male, Female or Unknown.
 [^6]: You will get exp only with not registered pokemons.
 
# Pokemon Loot
![GameScreen](https://user-images.githubusercontent.com/26468934/142738805-4e7487f6-2558-4a70-94a0-2c5d32dd93af.png)

# Play In
- [Windows](https://lipilopes.itch.io/pokeloot)
- [Web](https://lipilopes.itch.io/pokeloot)
 
# More About the Game
### Chances -> PokeBall -> DropPokeball.cs
 | Chance (Total Sum 100%) | Loot | Exp[^6] (shiny +5) | Color |
 | ------------ | ------------ | ------------ | ------------ |
 | 60% | Common | +5 exp | ![#FFFFFF](https://via.placeholder.com/15/FFFFFF/000000?text=+) `#FFFFFF` |
 | 30% | Rare | +10 exp | ![#14DB32](https://via.placeholder.com/15/14DB32/000000?text=+) `#14DB32` |
 | 9% | Epic | +15 exp | ![#1E44DD](https://via.placeholder.com/15/1E44DD/000000?text=+) `#14DB32` |
 | 1% | Legendary | +20 exp | ![#D10000](https://via.placeholder.com/15/D10000/000000?text=+) `#D10000` |

### Shiny -> LootScriptable
 | Chance | Detail |
 | ------------ | ------------ |
 | 1% | Shiny Chance |
 | +4% | [Bonus] Skip Anim Disable |
 | +5% | [Bonus] Complete Pokedex |
 | +0.2% - 15%[^1] | [Bonus] Repeated Pokemons x1 - 75[^1] |
 
 
### Exp/Level[^6] -> Manager -> ExpLevelManager.cs
 | Max Exp | Detail |
 | ------------ | ------------ |
 | 33.90[^2] | Level 50 or Less |
 | 50.80[^3] | Level 51 or More |
 > Max exp drop -> (Normal[1695]+
Shiny[2540]) = 4235 or Level 99.

# Exp Level Bonus -> ExpLevelManager.cs
### Loot Amount
 | Level | Loot Amount  |
 | ------------ | ------------ |
 | 01 - 10  | x1 |
 | 11 - 20  | x2 |
 | 21 - 30  | x3 |
 | 31 - 40  | x4 |
 | 41 - 50  | x5 |
 | 51 - 60  | x6 |
 | 61 - 70  | x7 |
 | 71 - 80  | x8 |
 | 81 - 99  | x9 |
 | 99       | x10 |
 ### Drop
 | Level | Drop |
 | ------------ | ------------ |
 | 01 - 30  |  Only Basic Stage. |
 | 31 - 49  | Only Basic or 1 Stages. |
 |   +50    | 1. All Stages.<br />2. Select Pokemon Type. |

 ### Grifts
 | Quest | Bonus |
 | ------------ | ------------ |
 | Complete Kanto's Dex  |  1. Can Loot Mew.<br />2. Can Loot MewTwo.<br />3. Loot x1 Mew. |

### PlayerPrefs Keys
 | Key | Example | type | Detail |
 | ------------ | ------------ | ------------ | ------------ |
 | "#"+id+"_"+form[^4]+gender[^5]+"_"+shiny | "#0_Normal_Male_False" | int | Save Pokemon Amount |
 | "LastPokemonLoot" | "LastPokemonLoot" | int | Last index pokemon loot |
 | "TotalLoot" | "TotalLoot" | int | Sum of total loots |
 | "CompleteDex" | "CompleteDex" | int (0 or 1) | ... |
 | "SkipAnim" | "SkipAnim" | int (0 or 1) | Toggle Skip Anim |
 | "fxVol" | "fxVol" | float (-20 to 20) | Effect Vol |
 | "musicVol" | "musicVol" | float (-20 to 20) | Music Vol |
 | "LootNumber" | "LootNumber" | int (1 to 10) | Player Count Loot |
 | "LootType" | (LootType)"LootType" | int | Current Type Loot |
 | "CompletePokedexKanto" | "CompletePokedexKanto" | int (0 or 1) | To Grifts |
 
# Game Scenes
|  Scenes       |  Caption |
| ------------ | ------------ |
| 1. Main Screen ![MainScreen](https://user-images.githubusercontent.com/26468934/142738622-319ccd85-214c-4816-9270-cfff47789e24.png) | 1. Config Button.<br />2. Pokemon's Name and Gender.<br />3. Pokedex Tooltip (Only for new registered pokemons).<br />4. Pokemon's Sprite.<br />5. Level Tooltip.<br />6. Pokedex Button.<br />7. Loot Button.<br />8. Skip Loot Animation.  
| 2. Pokedex Screen ![PokedexScreen](https://user-images.githubusercontent.com/26468934/140768460-cb3a16f0-a5d4-4887-b221-cf794bdc97bf.png) | 1. Pokemon's Name and Gender.<br />2. Pokemon's Sprite.<br />3. Loot Amount.<br />4. Shiny Effect.<br />5. Border Color Equal To Rarity.
| 3. Pokedex Detail ![PokedexDetail](https://user-images.githubusercontent.com/26468934/140772846-ac5163d3-2f83-47db-87af-836dd11dee61.png) | 1. Pokemon's ID and Name.<br />2. Change to Female/Male/Shiny or Alolan form.<br />3. Amount (Select Pokemon Gender,Form,...).<br />4. Description.<br />5. Evolution Button.  
| 4. Evolution Screen ![EvolutionScreen](https://user-images.githubusercontent.com/26468934/140770838-c690d1a9-0082-4663-8c76-4f3faf79ef62.png) ![EvolutionStart](https://user-images.githubusercontent.com/26468934/140771580-1bd1e23b-a4c7-4fba-b6d6-8aa98c271d42.png) ![EvolutionCompleted ](https://user-images.githubusercontent.com/26468934/140771765-caf1ab2e-f0ee-4fd2-8151-c601c45e0b41.png) | 1. Evolution Button.<br />2. Sprite Pokemon<br />3. Need To Evolve (Needed / Has)
| 5. Loot List Screen ![LootListScreen](https://user-images.githubusercontent.com/26468934/140770095-e5df80a6-27cf-4ee6-a108-3000db476793.png) | 1. Loots.<br />2. New Pokemon Icon. |
| 6. Config Screen ![ConfigScreen](https://user-images.githubusercontent.com/26468934/142738003-87175424-6bc7-4a23-9e2d-3056cb3ea0f5.png) | 1. Slider Sound Effect.<br />2. Credits Button.<br />3. Reset Button.(Double click to reset) |
| 6. Credits Screen ![CreditsScreen](https://user-images.githubusercontent.com/26468934/142738045-3a50d785-8ef8-4d9f-b448-cc4ffbd77973.png) | - |















