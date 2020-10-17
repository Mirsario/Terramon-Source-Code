﻿using DiscordRPC;
using DiscordRPC.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Razorwing.Framework.Configuration;
using Razorwing.Framework.IO.Stores;
using Razorwing.Framework.Localisation;
using Terramon.Items.Pokeballs.Inventory;
using Terramon.Network.Catching;
using Terramon.Network.Starter;
using Terramon.Pokemon;
using Terramon.Pokemon.Moves;
using Terramon.Razorwing.Framework.IO.Stores;
using Terramon.UI;
using Terramon.UI.Moveset;
using Terramon.UI.SidebarParty;
using Terramon.UI.Starter;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Terramon
{
    public class TerramonMod : Mod
    {
        // NEW DISCORD RICH PRESENCE INTEGRATION
        public DiscordRpcClient client;
        //

        internal ChooseStarter ChooseStarter;
        internal ChooseStarterBulbasaur ChooseStarterBulbasaur;
        internal ChooseStarterCharmander ChooseStarterCharmander;
        internal ChooseStarterSquirtle ChooseStarterSquirtle;

        public static bool PartyUITheme = true;
        public static bool PartyUIAutoMode = false;
        public static bool PartyUIReverseAutoMode = false;
        public static bool ShowHelpButton = true;
        public static bool HelpButtonInitialize = true;
        public int PartyUIThemeChanged = 0;

        // UI SIDEBAR //
        internal UISidebar UISidebar;
        internal Moves Moves;
        public PartySlots PartySlots { get; private set; }

        // UI SIDEBAR //

        internal PokegearUI PokegearUI;
        internal PokegearUIEvents PokegearUIEvents;
        internal EvolveUI evolveUI;
        public UserInterface _exampleUserInterface; // Choose Starter
        private UserInterface _exampleUserInterfaceNew; // Pokegear Main Menu
        private UserInterface PokegearUserInterfaceNew;
        private UserInterface evolveUserInterfaceNew; // Pokegear Events Menu
        private UserInterface _uiSidebar;
        private UserInterface _moves;
        public UserInterface _partySlots;

        public static ModHotKey PartyCycle;
        public static LocalisationManager Localisation;
        public static IResourceStore<byte[]> Store;

        //evolution


        public TerramonMod()
        {
            Instance = this;
            Localisation = new LocalisationManager(locale);
            Store = new ResourceStore<byte[]>(new EmbeddedStore());
            Localisation.AddLanguage(GameCulture.English.Name, new LocalisationStore(Store, GameCulture.English));
        }

        private static readonly string[] balls =
        {
            "Pokeball",
            "GreatBall",
            "UltraBall",
            "DuskBall",
            "PremierBall",
            "QuickBall",
            "TimerBall",
            "MasterBall",
            "ZeroBall"
        };

        // catch chance of the ball refers to the same index as the ball
        private static readonly float[][] catchChances =
        {
            new[] {.1190f}, //Pokeball
            new[] {.1785f}, //Great Ball
            new[] {.2380f}, //Ultra Ball
            new[]
            {
                .2380f, //Dusk Ball
                .1190f
            },
            new[] {.1190f}, //Premier Ball
            new[] {.2380f}, //Quick Ball
            new[] {.2380f}, //Timer Ball
            new[] {1f}, //Master Ball
            new[] {.1190f} //Zero Ball
        };

        public static string[] GetBallProjectiles()
        {
            string[] ballProjectiles = new string[balls.Length];
            for (int i = 0; i < balls.Length; i++) ballProjectiles[i] = balls[i] + "Projectile";

            return ballProjectiles;
        }

        Timestamps timestamp;

        public virtual void EnterWorldRP()
        {
                timestamp = Timestamps.Now;
                client.SetPresence(new RichPresence()
                {
                    Details = "In-Game",
                    State = "Testing v0.4.1",
                    Assets = new Assets()
                    {
                        LargeImageKey = "largeimage2",
                        LargeImageText = "Terramon Mod (Beta)",
                        SmallImageKey = "pokeball",
                        SmallImageText = "No Pokémon Selected"
                    },
                    Timestamps = timestamp
                });
        }

        public virtual void DisplayPokemonNameRP(string name, bool shinyness)
        {
            if (shinyness)
            {
                name = name += " ✨";
            }
                client.SetPresence(new RichPresence()
                {
                    Details = "In-Game",
                    State = "Testing v0.4.1",
                    Assets = new Assets()
                    {
                        LargeImageKey = "largeimage2",
                        LargeImageText = "Terramon Mod (Beta)",
                        SmallImageKey = "pokeball",
                        SmallImageText = "Using " + name
                    },
                    Timestamps = timestamp
                });
        }

        public virtual void RemoveDisplayPokemonNameRP()
        {
                client.SetPresence(new RichPresence()
                {
                    Details = "In-Game",
                    State = "Testing v0.4.1",
                    Assets = new Assets()
                    {
                        LargeImageKey = "largeimage2",
                        LargeImageText = "Terramon Mod (Beta)",
                        SmallImageKey = "pokeball",
                        SmallImageText = "No Pokémon Selected"
                    },
                    Timestamps = timestamp
                });
        }

        public override void PreSaveAndQuit()
        {
                client.SetPresence(new RichPresence()
                {
                    Details = "In Menu",
                    State = "Testing v0.4.1",
                    Assets = new Assets()
                    {
                        LargeImageKey = "largeimage2",
                        LargeImageText = "Terramon Mod (Beta)"
                    }
                });
        }
		
        protected DllResourceStore man;
        protected Bindable<string> locale = new Bindable<string>(Language.ActiveCulture.Name);

        public override void Load()
        {
                // Initalize Discord RP on Mod Load
                client = new DiscordRpcClient("749707767203233792");
                client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };
                //

                //Subscribe to events
                client.OnReady += (sender, e) =>
                {
                    Console.WriteLine("Received Ready from user {0}", e.User.Username);
                };

                client.OnPresenceUpdate += (sender, e) =>
                {
                    Console.WriteLine("Received Update! {0}", e.Presence);
                };

                //Connect to the RPC
                client.Initialize();

                client.SetPresence(new RichPresence()
                {
                    Details = "In Menu",
                    State = "Testing v0.4.1",
                    Assets = new Assets()
                    {
                        LargeImageKey = "largeimage2",
                        LargeImageText = "Terramon Mod (Beta)"
                    }
                });

            //Load all mons to a store
            LoadPokemons();

            if (Main.netMode != NetmodeID.Server)
            {

                if (Localisation == null)
                {
                    Localisation = new LocalisationManager(locale);
                }
                locale = new Bindable<string>(Language.ActiveCulture.Name);
                Store = new ResourceStore<byte[]>(new EmbeddedStore());
                Localisation.AddLanguage(GameCulture.English.Name, new LocalisationStore(Store, GameCulture.English));
                Localisation.AddLanguage(GameCulture.Russian.Name, new LocalisationStore(Store, GameCulture.Russian));
#if DEBUG
                var ss = Localisation.GetLocalisedString(new LocalisedString(("title","Powered by broken code")));//It's terrible checking in ui from phone, so i can ensure everything works from version string
                Main.versionNumber = ss.Value + "\n" + Main.versionNumber;
#endif

                ChooseStarter = new ChooseStarter();
                ChooseStarter.Activate();
                ChooseStarterBulbasaur = new ChooseStarterBulbasaur();
                ChooseStarterBulbasaur.Activate();
                ChooseStarterCharmander = new ChooseStarterCharmander();
                ChooseStarterCharmander.Activate();
                ChooseStarterSquirtle = new ChooseStarterSquirtle();
                ChooseStarterSquirtle.Activate();
                PokegearUI = new PokegearUI();
                PokegearUI.Activate();
                PokegearUIEvents = new PokegearUIEvents();
                PokegearUIEvents.Activate();
                evolveUI = new EvolveUI();
                evolveUI.Activate();
                UISidebar = new UISidebar();
                UISidebar.Activate();
                Moves = new Moves();
                Moves.Activate();
                PartySlots = new PartySlots();
                PartySlots.Activate();
                _exampleUserInterface = new UserInterface();
                _exampleUserInterfaceNew = new UserInterface();
                PokegearUserInterfaceNew = new UserInterface();
                evolveUserInterfaceNew = new UserInterface();
                _uiSidebar = new UserInterface();
                _moves = new UserInterface();
                _partySlots = new UserInterface();


                _exampleUserInterface.SetState(ChooseStarter); // Choose Starter
                _exampleUserInterfaceNew.SetState(PokegearUI); // Pokegear Main Menu
                PokegearUserInterfaceNew.SetState(PokegearUIEvents); // Pokegear Events Menu
                evolveUserInterfaceNew.SetState(evolveUI);
                _uiSidebar.SetState(UISidebar);
                _moves.SetState(Moves);
                _partySlots.SetState(PartySlots);

    
            }


            if (Main.dedServ)
                return;

            FirstPKMAbility = RegisterHotKey("First Pokémon Move", Keys.Z.ToString());
            SecondPKMAbility = RegisterHotKey("Second Pokémon Move", Keys.X.ToString());
            ThirdPKMAbility = RegisterHotKey("Third Pokémon Move", Keys.C.ToString());
            FourthPKMAbility = RegisterHotKey("Fourth Pokémon Move", Keys.V.ToString());


            PartyCycle = RegisterHotKey("Quick Spawn First Party Pokémon", Keys.RightAlt.ToString());
        }

        public override void Unload()
        {
            client.Dispose();
            client = null;
            Instance = null;
            _exampleUserInterface.SetState(null); // Choose Starter
            _exampleUserInterfaceNew.SetState(null); // Pokegear Main Menu
            PokegearUserInterfaceNew.SetState(null); // Pokegear Events Menu
            evolveUserInterfaceNew.SetState(null);
            _uiSidebar.SetState(null);
            _partySlots.SetState(null);
            _moves.SetState(null);
            PartySlots = null;
            pokemonStore = null;
            wildPokemonStore = null;
            movesStore = null;
            _exampleUserInterface = null;
            _exampleUserInterfaceNew = null;
            PokegearUserInterfaceNew = null;
            _uiSidebar = null;
            _partySlots = null;
            _moves = null;



            ChooseStarter.Deactivate();
            ChooseStarter = null;
            ChooseStarterBulbasaur.Deactivate();
            ChooseStarterBulbasaur = null;
            ChooseStarterCharmander.Deactivate();
            ChooseStarterCharmander = null;
            ChooseStarterSquirtle.Deactivate();
            ChooseStarterSquirtle = null;
            PokegearUI.Deactivate();
            PokegearUI = null;
            PokegearUIEvents.Deactivate();
            PokegearUIEvents = null;
            evolveUI.Deactivate();
            evolveUI = null;
            UISidebar.Deactivate();
            UISidebar = null;
            Moves.Deactivate();
            Moves = null;

            PartyCycle = null;
            FirstPKMAbility = null;
            SecondPKMAbility = null;
            ThirdPKMAbility = null;
            FourthPKMAbility = null;

            Localisation = null;
            Store = null;
        }

        //ModContent.GetInstance<TerramonMod>(). (grab instance)


        public static float[][] GetCatchChances()
        {
            return catchChances;
        }

        /* public override void Load()
		{
			Main.music[MusicID.OverworldDay] = GetMusic("Sounds/Music/overworldnew");
			Main.music[MusicID.Night] = GetMusic("Sounds/Music/nighttime");
			Main.music[MusicID.AltOverworldDay] = GetMusic("Sounds/Music/overworldnew");
		} */

        // UI STUFF DOWN HERE

        public override void UpdateUI(GameTime gameTime)
        {
            if (ChooseStarter.Visible) _exampleUserInterface?.Update(gameTime);
            if (PokegearUI.Visible) _exampleUserInterfaceNew?.Update(gameTime);
            if (PokegearUIEvents.Visible) PokegearUserInterfaceNew?.Update(gameTime);
            if (EvolveUI.Visible) evolveUserInterfaceNew?.Update(gameTime);
            //starters
            if (ChooseStarterBulbasaur.Visible) _exampleUserInterface?.Update(gameTime);
            if (ChooseStarterCharmander.Visible) _exampleUserInterface?.Update(gameTime);
            if (ChooseStarterSquirtle.Visible) _exampleUserInterface?.Update(gameTime);
            if (UISidebar.Visible) _uiSidebar?.Update(gameTime);
            if (Moves.Visible) _moves?.Update(gameTime);
            if (PartySlots.Visible) _partySlots?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            //int StarterSelectionLayer = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Interface Logic 1")); //Unused var
            if (mouseTextIndex != -1)
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Terramon: Pokemon Interfaces",
                    delegate
                    {
                        if (ChooseStarter.Visible) _exampleUserInterface.Draw(Main.spriteBatch, new GameTime());
                        if (PokegearUI.Visible) _exampleUserInterfaceNew.Draw(Main.spriteBatch, new GameTime());
                        if (PokegearUIEvents.Visible) PokegearUserInterfaceNew.Draw(Main.spriteBatch, new GameTime());
                        if (EvolveUI.Visible) evolveUserInterfaceNew.Draw(Main.spriteBatch, new GameTime());
                        if (ChooseStarterBulbasaur.Visible)
                            _exampleUserInterface.Draw(Main.spriteBatch, new GameTime());
                        if (ChooseStarterCharmander.Visible)
                            _exampleUserInterface.Draw(Main.spriteBatch, new GameTime());
                        if (ChooseStarterSquirtle.Visible) _exampleUserInterface.Draw(Main.spriteBatch, new GameTime());
                        if (UISidebar.Visible) _uiSidebar.Draw(Main.spriteBatch, new GameTime());
                        if (Moves.Visible) _moves.Draw(Main.spriteBatch, new GameTime());
                        if (PartySlots.Visible) _partySlots.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
        }

        public static bool MyUIStateActive(Player player)
        {
            return ChooseStarter.Visible;
        }

        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            if (Main.myPlayer == -1 || Main.gameMenu || !Main.LocalPlayer.active) return;

            if (MyUIStateActive(Main.LocalPlayer)) music = GetSoundSlot(SoundType.Music, null);
        }

        // END UI STUFF


#region HotKeys

        public ModHotKey FirstPKMAbility { get; private set; }
        public ModHotKey SecondPKMAbility { get; private set; }
        public ModHotKey ThirdPKMAbility { get; private set; }
        public ModHotKey FourthPKMAbility { get; private set; }

#endregion


        /// <summary>
        ///     Class used to save pokeball rarity when manipulating
        ///     items data;
        /// </summary>
        public static class PokeballFactory
        {
            public enum Pokebals : byte
            {
                Nothing = 0,
                Pokeball,
                GreatBall,
                UltraBall,
                MasterBall,
                DuskBall,
                PremierBall,
                QuickBall,
                TimerBall,
                ZeroBall
            }

            /// <summary>
            ///     Return type id for provided pokeball.
            ///     Mostly used for loading from saves
            /// </summary>
            /// <param name="item">Byte enum of save pokeball</param>
            /// <returns>Return item id or 0 if this is not a pokeball</returns>
            public static int GetPokeballType(Pokebals item)
            {
                switch (item)
                {
                    case Pokebals.Pokeball:
                        return ModContent.ItemType<PokeballCaught>();
                    case Pokebals.GreatBall:
                        return ModContent.ItemType<GreatBallCaught>();
                    case Pokebals.UltraBall:
                        return ModContent.ItemType<UltraBallCaught>();
                    case Pokebals.MasterBall:
                        return ModContent.ItemType<MasterBallCaught>();
                    case Pokebals.DuskBall:
                        return ModContent.ItemType<DuskBallCaught>();
                    case Pokebals.PremierBall:
                        return ModContent.ItemType<PremierBallCaught>();
                    case Pokebals.QuickBall:
                        return ModContent.ItemType<QuickBallCaught>();
                    case Pokebals.TimerBall:
                        return ModContent.ItemType<TimerBallCaught>();
                    case Pokebals.ZeroBall:
                        return ModContent.ItemType<ZeroBallCaught>();
                    default:
                        return 0;
                }
            }

            /// <summary>
            ///     Return enum byte for provided item.
            ///     Mostly used for saving
            /// </summary>
            /// <param name="item">ModItem of item</param>
            /// <returns>
            ///     Return byte enum or <see cref="Pokebals.Nothing" />
            ///     if provided item is not a pokeball
            /// </returns>
            public static Pokebals GetEnum(ModItem item)
            {
                if (item is PokeballCaught) return Pokebals.Pokeball;
                if (item is GreatBallCaught) return Pokebals.GreatBall;
                if (item is UltraBallCaught) return Pokebals.UltraBall;
                if (item is MasterBallCaught) return Pokebals.MasterBall;
                if (item is DuskBallCaught) return Pokebals.DuskBall;
                if (item is PremierBallCaught) return Pokebals.PremierBall;
                if (item is QuickBallCaught) return Pokebals.QuickBall;
                if (item is TimerBallCaught) return Pokebals.TimerBall;
                if (item is ZeroBallCaught) return Pokebals.ZeroBall;
                return Pokebals.Nothing;
            }

            /// <summary>
            ///     Return item type id from provided pokeball
            /// </summary>
            /// <param name="item">ModItem of item</param>
            /// <returns>Return item id or 0 if this is not a pokeball</returns>
            public static int GetPokeballType(ModItem item)
            {
                if (item is PokeballCaught) return ModContent.ItemType<PokeballCaught>();
                if (item is GreatBallCaught) return ModContent.ItemType<GreatBallCaught>();
                if (item is UltraBallCaught) return ModContent.ItemType<UltraBallCaught>();
                if (item is DuskBallCaught) return ModContent.ItemType<DuskBallCaught>();
                if (item is PremierBallCaught) return ModContent.ItemType<PremierBallCaught>();
                if (item is QuickBallCaught) return ModContent.ItemType<QuickBallCaught>();
                if (item is TimerBallCaught) return ModContent.ItemType<TimerBallCaught>();
                if (item is MasterBallCaught) return ModContent.ItemType<MasterBallCaught>();
                if (item is ZeroBallCaught) return ModContent.ItemType<ZeroBallCaught>();
                return 0;
            }
        }


        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            //In case i f*ck the code
            try
            {
                string type = reader.ReadString();
                switch (type)
                {
                    case SpawnStarterPacket.NAME:
                    {
                        //Server can't have any UI
                        if (whoAmI == 256)
                            return;
                        SpawnStarterPacket packet = new SpawnStarterPacket();
                        packet.HandleFromClient(reader, whoAmI);
                    }
                        break;
                    case BaseCatchPacket.NAME:
                    {
                        //Server should handle it from client
                        if (whoAmI == 256)
                            return;
                        BaseCatchPacket packet = new BaseCatchPacket();
                        packet.HandleFromClient(reader, whoAmI);
                    }
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.ErrorFormat(
                    "Exception appear in HandlePacket. Please, contact mod devs with folowing stacktrace:\n\n{0}\n\n{1}",
                    e.Message, e.StackTrace);
            }
        }

        public static TerramonMod Instance { get; private set; }

        public static IEnumerable<string> GetPokemonsNames()
        {
            return Instance.pokemonStore.Keys.ToList();
        }

        public static ParentPokemon GetPokemon(string monName)
        {
            if (monName == null) return null;
            if (Instance.pokemonStore != null && Instance.pokemonStore.ContainsKey(monName))
                return Instance.pokemonStore[monName];
            return null;
        }

        // ReSharper disable once UnusedMember.Global
        public static ParentPokemonNPC GetWildPokemon(string monName)
        {
            if (monName == null) return null;
            if (Instance.pokemonStore != null && Instance.pokemonStore.ContainsKey(monName))
                return Instance.wildPokemonStore[monName];
            return null;
        }

        public static BaseMove GetMove(string moveName)
        {
            if (string.IsNullOrEmpty(moveName)) return null;
            if (Instance.movesStore != null && Instance.movesStore.ContainsKey(moveName))
                return Instance.movesStore[moveName];
            return null;
        }


        private Dictionary<string, ParentPokemon> pokemonStore;
        private Dictionary<string, ParentPokemonNPC> wildPokemonStore;
        private Dictionary<string, BaseMove> movesStore;

        private void LoadPokemons()
        {
            pokemonStore = new Dictionary<string, ParentPokemon>();
            wildPokemonStore = new Dictionary<string, ParentPokemonNPC>();
            movesStore = new Dictionary<string, BaseMove>();
            foreach (TypeInfo it in GetType().Assembly.DefinedTypes)
            {
                var baseType = it.BaseType;
                if (it.IsAbstract)
                    continue;
                bool valid = false;
                if (baseType == typeof(ParentPokemon) || baseType == typeof(ParentPokemonNPC) ||
                    baseType == typeof(BaseMove))
                    valid = true;
                else
                    //Recurrent seek for our class
                    while (baseType != null && baseType != typeof(object))
                    {
                        if (baseType == typeof(ParentPokemon) || baseType == typeof(ParentPokemonNPC) ||
                            baseType == typeof(BaseMove))
                        {
                            valid = true;
                            break;
                        }

                        baseType = baseType.BaseType;
                    }

                if (valid)
                    try
                    {
                        if (baseType == typeof(ParentPokemon))
                            pokemonStore.Add(it.Name, (ParentPokemon) Activator.CreateInstance(it));
                        else if (baseType == typeof(ParentPokemonNPC))
                            wildPokemonStore.Add(it.Name, (ParentPokemonNPC) Activator.CreateInstance(it));
                        else if (baseType == typeof(BaseMove))
                            movesStore.Add(it.Name, (BaseMove) Activator.CreateInstance(it));
                    }
                    catch (Exception e)
                    {
                        Logger.Error(
                            "Exception caught in Events register loop. Report mod author with related stacktrace: \n" +
                            $"{e.Message}\n" +
                            $"{e.StackTrace}\n");
                    }
            }
        }
    }
}