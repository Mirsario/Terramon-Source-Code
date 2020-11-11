﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terramon.Network.Sync.Battle;
using Terramon.Players;
using Terramon.Pokemon;
using Terraria;

namespace Terramon.Network.Sync
{
    public class RequestSyncPacket :Packet
    {
        public override string PacketName => "net_reqsync";

        public void Send()
        {
            GetPacket(TerramonMod.Instance).Send();
        }

        public override void HandleFromClient(BinaryReader reader, int whoAmI)
        {
            var p = GetPacket(TerramonMod.Instance);
            p.Write(whoAmI);
            p.Send(ignoreClient: whoAmI);
        } 

        public override void HandleFromServer(BinaryReader reader)
        {
            var id = reader.ReadInt32();
            var pl = Main.LocalPlayer.GetModPlayer<TerramonPlayer>();
            var side = new PlayerSidebarSync();
            side.Send(TerramonMod.Instance, pl);
            if (pl.ActivePet != null)
            {
                var p = new ActivePetSync();
                p.Send(TerramonMod.Instance, pl);
            }
            if (pl.ActivePetId != -1 && Main.projectile[pl.ActivePetId].active &&
                Main.projectile[pl.ActivePetId].modProjectile is ParentPokemon)
            {
                var p = new PetIDSyncPacket();
                p.Send(TerramonMod.Instance, pl.ActivePetId);
            }

            if (pl.Battle != null)
            {
                new BaseBattleSyncPacket().Send(pl);
            }
        }
    }
}
