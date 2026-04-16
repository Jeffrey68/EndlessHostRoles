using System.Collections.Generic;
using static EHR.Options;

namespace EHR.Roles;

internal class Rio : IAddon
{
    private static readonly Dictionary<byte, bool> Applied = [];
    public AddonTypes Type => AddonTypes.Mixed;

    public void SetupCustomOption()
    {
        SetupAdtRoleOptions(653000, CustomRoles.Rio, canSetNum: false, teamSpawnOptions: true);
    }

    public static void ApplyRio(PlayerControl pc)
    {
        const byte purple = 4;

        pc.SetColor(purple);

        if (GameStates.CurrentServerType != GameStates.ServerType.Vanilla)
        {
            pc.RpcSetColor(purple);
        }
        else
        {
            var sender = CustomRpcSender.Create($"Rio.SetColor({pc.Data.PlayerName})");

            sender.AutoStartRpc(pc.NetId, RpcCalls.SetColor)
                .Write(pc.Data.NetId)
                .Write(purple)
                .EndRpc();

            sender.SendMessage();
        }
    }

    public static void OnFixedUpdate(PlayerControl pc)
    {
        if (!pc.Is(CustomRoles.Rio) || !GameStates.IsInTask || ExileController.Instance ||
            AntiBlackout.SkipTasks || pc.IsShifted() || Camouflage.IsCamouflage ||
            pc.inVent || pc.MyPhysics.Animations.IsPlayingEnterVentAnimation() ||
            pc.walkingToVent || pc.onLadder ||
            pc.MyPhysics.Animations.IsPlayingAnyLadderAnimation() || pc.inMovingPlat)
            return;

        // Only apply once per round per player
        if (Applied.TryGetValue(pc.PlayerId, out bool done) && done)
            return;

        ApplyRio(pc);
        Applied[pc.PlayerId] = true;
    }
}

