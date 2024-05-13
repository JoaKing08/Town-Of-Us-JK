using System.Collections.Generic;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Deputy : Role
    {
        public Deputy(PlayerControl player) : base(player)
        {
            Name = "Deputy";
            ImpostorText = () => "Kill Evils During Meetings";
            TaskText = () => "Shoot out evildoers in bright daylight";
            Color = Patches.Colors.Deputy;
            RoleType = RoleEnum.Deputy;
            AddToRoleHistory(RoleType);
            Revealed = false;
        }
        public bool Revealed { get; set; }

        public List<GameObject> ShootButtons = new List<GameObject>();

        internal override bool Criteria()
        {
            return Revealed && CustomGameOptions.RevealDeputy && !Player.Data.IsDead || base.Criteria();
        }

        internal override bool RoleCriteria()
        {
            if (!Player.Data.IsDead && CustomGameOptions.RevealDeputy) return Revealed || base.RoleCriteria();
            return false || base.RoleCriteria();
        }
    }
}