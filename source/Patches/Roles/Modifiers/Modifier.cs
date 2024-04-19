using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    public abstract class Modifier
    {
        public static readonly Dictionary<byte, Modifier> ModifierDictionary = new Dictionary<byte, Modifier>();
        public Func<string> TaskText;

        protected Modifier(PlayerControl player)
        {
            Player = player;
            ModifierDictionary.Add(player.PlayerId, this);
        }

        public static IEnumerable<Modifier> AllModifiers => ModifierDictionary.Values.ToList();
        protected internal string Name { get; set; }
        protected internal string SymbolName { get; set; }

        protected internal string GetColoredSymbol()
        {
            if (SymbolName == null) return null;

            return $"{ColorString}{SymbolName}</color>";
        }

        public string PlayerName { get; set; }
        private PlayerControl _player { get; set; }
        public PlayerControl Player
        {
            get => _player;
            set
            {
                if (_player != null) _player.nameText().color = Color.white;

                _player = value;
                PlayerName = value.Data.PlayerName;
            }
        }
        protected internal Color Color { get; set; }
        protected internal ModifierEnum ModifierType { get; set; }
        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";

        private bool Equals(Modifier other)
        {
            return Equals(Player, other.Player) && ModifierType == other.ModifierType;
        }

        internal virtual bool ModifierWin(LogicGameFlowNormal __instance)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Modifier)) return false;
            return Equals((Modifier) obj);
        }


        public override int GetHashCode()
        {
            return HashCode.Combine(Player, (int) ModifierType);
        }


        public static bool operator ==(Modifier a, Modifier b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.ModifierType == b.ModifierType && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static bool operator !=(Modifier a, Modifier b)
        {
            return !(a == b);
        }

        public static Modifier GetModifier(PlayerControl player)
        {
            return (from entry in ModifierDictionary where entry.Key == player.PlayerId select entry.Value)
                .FirstOrDefault();
        }

        public static IEnumerable<Modifier> GetModifiers(ModifierEnum modifiertype)
        {
            return AllModifiers.Where(x => x.ModifierType == modifiertype);
        }

        public virtual List<PlayerControl> GetTeammates()
        {
            var team = new List<PlayerControl>();
            return team;
        }

        public static T GetModifier<T>(PlayerControl player) where T : Modifier
        {
            return GetModifier(player) as T;
        }

        public static Modifier GetModifier(PlayerVoteArea area)
        {
            var player = PlayerControl.AllPlayerControls.ToArray()
                .FirstOrDefault(x => x.PlayerId == area.TargetPlayerId);
            return player == null ? null : GetModifier(player);
        }
    }

    public abstract class Ability
    {
        public static readonly Dictionary<byte, Ability> AbilityDictionary = new Dictionary<byte, Ability>();
        public Func<string> TaskText;

        protected Ability(PlayerControl player)
        {
            Player = player;
            AbilityDictionary.Add(player.PlayerId, this);
        }

        public static IEnumerable<Ability> AllAbilities => AbilityDictionary.Values.ToList();
        protected internal string Name { get; set; }

        public string PlayerName { get; set; }
        private PlayerControl _player { get; set; }
        public PlayerControl Player
        {
            get => _player;
            set
            {
                if (_player != null) _player.nameText().color = Color.white;

                _player = value;
                PlayerName = value.Data.PlayerName;
            }
        }
        protected internal Color Color { get; set; }
        protected internal AbilityEnum AbilityType { get; set; }
        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";

        private bool Equals(Ability other)
        {
            return Equals(Player, other.Player) && AbilityType == other.AbilityType;
        }

        internal virtual bool EABBNOODFGL(ShipStatus __instance)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Ability)) return false;
            return Equals((Ability)obj);
        }


        public override int GetHashCode()
        {
            return HashCode.Combine(Player, (int)AbilityType);
        }


        public static bool operator ==(Ability a, Ability b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.AbilityType == b.AbilityType && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static bool operator !=(Ability a, Ability b)
        {
            return !(a == b);
        }

        public static Ability GetAbility(PlayerControl player)
        {
            return (from entry in AbilityDictionary where entry.Key == player.PlayerId select entry.Value)
                .FirstOrDefault();
        }

        public static T GetAbility<T>(PlayerControl player) where T : Ability
        {
            return GetAbility(player) as T;
        }

        public static Ability GetAbility(PlayerVoteArea area)
        {
            var player = PlayerControl.AllPlayerControls.ToArray()
                .FirstOrDefault(x => x.PlayerId == area.TargetPlayerId);
            return player == null ? null : GetAbility(player);
        }

        public static IEnumerable<Ability> GetAbilities(AbilityEnum abilitytype)
        {
            return AllAbilities.Where(x => x.AbilityType == abilitytype);
        }
    }
    public abstract class Objective
    {
        public static readonly Dictionary<byte, Objective> ObjectiveDictionary = new Dictionary<byte, Objective>();
        public Func<string> TaskText;

        protected Objective(PlayerControl player)
        {
            Player = player;
            ObjectiveDictionary.Add(player.PlayerId, this);
        }

        public static IEnumerable<Objective> AllObjectives => ObjectiveDictionary.Values.ToList();
        protected internal string Name { get; set; }
        protected internal string SymbolName { get; set; }

        protected internal string GetColoredSymbol()
        {
            if (SymbolName == null) return null;

            return $"{ColorString}{SymbolName}</color>";
        }

        public string PlayerName { get; set; }
        private PlayerControl _player { get; set; }
        public PlayerControl Player
        {
            get => _player;
            set
            {
                if (_player != null) _player.nameText().color = Color.white;

                _player = value;
                PlayerName = value.Data.PlayerName;
            }
        }
        protected internal Color Color { get; set; }
        protected internal ObjectiveEnum ObjectiveType { get; set; }
        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";

        private bool Equals(Objective other)
        {
            return Equals(Player, other.Player) && ObjectiveType == other.ObjectiveType;
        }

        internal virtual bool ObjectiveWin(LogicGameFlowNormal __instance)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Objective)) return false;
            return Equals((Objective)obj);
        }


        public override int GetHashCode()
        {
            return HashCode.Combine(Player, (int)ObjectiveType);
        }


        public static bool operator ==(Objective a, Objective b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.ObjectiveType == b.ObjectiveType && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static bool operator !=(Objective a, Objective b)
        {
            return !(a == b);
        }

        public static Objective GetObjective(PlayerControl player)
        {
            return (from entry in ObjectiveDictionary where entry.Key == player.PlayerId select entry.Value)
                .FirstOrDefault();
        }

        public static IEnumerable<Objective> GetObjectives(ObjectiveEnum objectivetype)
        {
            return AllObjectives.Where(x => x.ObjectiveType == objectivetype);
        }

        public virtual List<PlayerControl> GetTeammates()
        {
            var team = new List<PlayerControl>();
            return team;
        }

        public static T GetObjective<T>(PlayerControl player) where T : Objective
        {
            return GetObjective(player) as T;
        }

        public static Objective GetObjective(PlayerVoteArea area)
        {
            var player = PlayerControl.AllPlayerControls.ToArray()
                .FirstOrDefault(x => x.PlayerId == area.TargetPlayerId);
            return player == null ? null : GetObjective(player);
        }
    }
}