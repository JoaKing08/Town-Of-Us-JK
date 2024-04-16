using Reactor.Utilities.Extensions;
using System;
using System.Collections.Generic;
using TownOfUs.Patches.ScreenEffects;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Undercover : Role
    {
        public RoleEnum? UndercoverRole;
        public bool UndercoverImpostor => UndercoverRole == null ? false : !(UndercoverRole == RoleEnum.Plaguebearer || UndercoverRole == RoleEnum.Baker || UndercoverRole == RoleEnum.Berserker || UndercoverRole == RoleEnum.SoulCollector);
        public bool UndercoverApocalypse => UndercoverRole == null ? false : (UndercoverRole == RoleEnum.Plaguebearer || UndercoverRole == RoleEnum.Baker || UndercoverRole == RoleEnum.Berserker || UndercoverRole == RoleEnum.SoulCollector);
        public Undercover(PlayerControl player) : base(player)
        {
            Name = "Undercover";
            ImpostorText = () => UndercoverRole == null ? "You Don't Have A Undercover... Wierd..." : UndercoverImpostor ? "Disguise As The <color=#FF0000FF>Impostor</color> To Find Who They Are" : "Disguise As Member Of The <color=#808080FF>Apocalypse</color> To Find Who They Are";
            TaskText = () => UndercoverRole == null ? "You don't have a undercover... wierd..." : $"Disguise as evildoers to find out who they are\nUndercover role: <color=#{UndercoverRole.GetRoleColor().ToHtmlStringRGBA()}>{UndercoverRole.GetRoleName()}</color>";
            Color = Patches.Colors.Undercover;
            RoleType = RoleEnum.Undercover;
            AddToRoleHistory(RoleType);
        }
    }
}
