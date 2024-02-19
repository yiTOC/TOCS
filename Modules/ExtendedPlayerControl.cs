using AmongUs.Data;
using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOCS
{
    static class ExtendedPlayerControl
    {
        public static int GetClientId(this PlayerControl player)
        {
            if (player == null) return -1;
            var client = player.GetClient();
            return client == null ? -1 : client.Id;
        }
        
        public static ClientData GetClient(this PlayerControl player)
        {
            try
            {
                var client = AmongUsClient.Instance.allClients.ToArray().Where(cd => cd.Character.PlayerId == player.PlayerId).FirstOrDefault();
                return client;
            }
            catch
            {
                return null;
            }
        }
    }
}