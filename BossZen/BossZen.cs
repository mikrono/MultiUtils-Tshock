using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace BossZen
{
    [ApiVersion(2, 1)]
    public class BossZen : TerrariaPlugin
    {
        public override string Author => "Test";
        public override string Description => "Boss Zen.";
        public override string Name => "BossZen";
        public override Version Version => new Version(1, 0, 0, 0);

        public BossZen(Main game) : base(game)
        {

        }



        public bool isBoss;

        public override void Initialize()
        {
            ServerApi.Hooks.NpcSpawn.Register(this, NpcSpawn);
            isBoss = false;
            ServerApi.Hooks.NpcKilled.Register(this, NpcKilled);
            ServerApi.Hooks.GameUpdate.Register(this, gameupdate);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.NpcSpawn.Deregister(this, NpcSpawn);
                ServerApi.Hooks.NpcKilled.Deregister(this, NpcKilled);
                ServerApi.Hooks.NpcTransform.Deregister(this, gameupdate);
            }
            base.Dispose(disposing);
        }

        public void NpcSpawn(NpcSpawnEventArgs args)
        {
            if (args.Handled) return;
            NPC nPC = Main.npc[args.NpcId];
            if (nPC.boss)
            {
                isBoss = true;
            }
            else
            {
                if (isBoss)
                {
                    args.Handled = true;
                    Main.npc[args.NpcId].active = false;
                    args.NpcId = Main.maxNPCs;
                }
            }
        }

        public void NpcKilled(NpcKilledEventArgs args)
        {
            if (args.npc.boss)
            {
                isBoss = false;
            }
        }
        public void gameupdate(EventArgs args)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active)
                    return;
            }
            isBoss = false;
        }
    }
}