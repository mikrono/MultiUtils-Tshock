using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using Microsoft.Xna.Framework;

namespace DamageMeter
{
    [ApiVersion(2, 1)]
    public class DamageMeter : TerrariaPlugin
    {
        public override string Author => "Test";
        public override string Description => "Show Damages";
        public override string Name => "DamageMeter";
        public override Version Version => new Version(1, 0, 0, 0);

        public DamageMeter(Main game) : base(game)
        {

        }



        public Dictionary<NPC, List<int>> bossData = new Dictionary<NPC, List<int>>();

        public override void Initialize()
        {
            ServerApi.Hooks.NpcSpawn.Register(this, bossChecker);
            ServerApi.Hooks.NpcStrike.Register(this, saveDamage);
            ServerApi.Hooks.NpcKilled.Register(this, BroadcastDamage);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.NpcSpawn.Deregister(this, bossChecker);
                ServerApi.Hooks.NpcStrike.Deregister(this, saveDamage);
                ServerApi.Hooks.NpcKilled.Deregister(this, BroadcastDamage);
            }
            base.Dispose(disposing);
        }




        public void bossChecker(NpcSpawnEventArgs args)
        {
            if (args.Handled)
                return;
            NPC nPC = Main.npc[args.NpcId];
            if (nPC.boss)
            {
                List<int> playerDamageList = new List<int>(new int[256]);
                bossData.Add(nPC, playerDamageList);
            }
        }

        public void saveDamage(NpcStrikeEventArgs args)
        {
            if (args.Handled)
                return;
            if (args.Npc.boss)
            {
                bossData[args.Npc][args.Player.whoAmI] += args.Damage;
            }
        }
        public void BroadcastDamage(NpcKilledEventArgs args)
        {
            if (args.npc.boss)
            {
                if (bossData.ContainsKey(args.npc))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Damage stats for " + Lang.GetNPCNameValue(args.npc.type) + ": ");
                    var playerDamageList = bossData[args.npc];
                    foreach (var Damage in playerDamageList.Select((value, index) => (value, index)))
                    {
                        if (Damage.value > 0)
                        {
                            if (Damage.index == 255)
                            {
                                sb.Append($"Traps/TownNPC: {Damage.value}, ");
                            }
                            else
                            {
                                sb.Append($"{Main.player[Damage.index].name}: {Damage.value}, ");
                            }
                        }
                    }
                    sb.Length -= 2;
                    Color messageColor = Color.Orange;

                    foreach (TSPlayer player in TShock.Players)
                    {
                        player.SendMessage(sb.ToString(), messageColor);
                    }
                    bossData.Remove(args.npc);
                }
            }
        }
    }
}