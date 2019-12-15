﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Terramon.Items.Pokeballs.Inventory
{
    public sealed class QuickBallIItem : BasePokeballItem
    {
        public QuickBallIItem() : base(Constants.Pokeballs.UnlocalizedNames.QUICK_BALL,
            new Dictionary<GameCulture, string>()
            {
                { GameCulture.English, "Quick Ball" },
                { GameCulture.French, "Rapide Ball" }
            },
            new Dictionary<GameCulture, string>()
            {
                { GameCulture.English, "A somewhat different Poké Ball that provides a better catch rate if it is used at the start of a wild encounter." }
            },
            Item.sellPrice(copper: 0), ItemRarityID.Orange, Constants.Pokeballs.CatchRates.QUICK_BALL)
        {
        }
    }
}