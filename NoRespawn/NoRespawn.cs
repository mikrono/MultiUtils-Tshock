using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using Microsoft.Xna.Framework;

namespace NoRespawn
{
    [ApiVersion(2, 1)]
    public class NoRespawn : TerrariaPlugin
    {
        public override string Author => "Test";
        public override string Description => "Prevent respawning while fighting with bosses.";
        public override string Name => "NoRespawn";
        public override Version Version => new Version(1, 0, 0, 0);

        public NoRespawn(Main game) : base(game)
        {

        }



        public Dictionary<NPC, List<int>> bossData = new Dictionary<NPC, List<int>>();

        public override void Initialize()
        {
            GetDataHandlers.PlayerUpdate += PreventRespawn;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GetDataHandlers.PlayerUpdate -= PreventRespawn;
            }
            base.Dispose(disposing);
        }

        private void PreventRespawn(object sender, GetDataHandlers.PlayerUpdateEventArgs args)
        {
            var player = TShock.Players[args.PlayerId];
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.boss)
                {
                    player.RespawnTimer = 10;
                }
            }
        }
    }
}