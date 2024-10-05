using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TMPro;
using TownOfUs.Roles.Modifiers;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using TownOfUs.Extensions;
using AmongUs.GameOptions;
using TownOfUs.ImpostorRoles.TraitorMod;
using Reactor.Utilities;
using TownOfUs.Patches;
using UnderdogMod = TownOfUs.Modifiers.UnderdogMod;
using TownOfUs.CrewmateRoles.AltruistMod;

namespace TownOfUs.Roles
{
    public abstract class Role
    {
        public static readonly Dictionary<byte, Role> RoleDictionary = new Dictionary<byte, Role>();
        public static readonly List<KeyValuePair<byte, RoleEnum>> RoleHistory = new List<KeyValuePair<byte, RoleEnum>>();

        public static bool NobodyWins;
        public static bool SurvOnlyWins;
        public static bool VampireWins;
        public static bool ApocalypseWins;
        public static bool ImpostorAgentHuntOver;
        public static bool ApocalypseAgentHuntOver;
        public static bool ImpostorAndApocalypseWin;
        public int BreadLeft = 0;
        public int Defense = 0;
        public bool Reaped = false;
        public bool Roleblocked = false;
        public bool SuperRoleblocked = false;
        public GameObject DefenseButton = new GameObject();
        public DateTime LastBlood;
        public List<ArrowBehaviour> SnipeArrows = new List<ArrowBehaviour>();
        public DateTime SnipeTime = DateTime.UtcNow;

        public List<KillButton> ExtraButtons = new List<KillButton>();

        public Func<string> ImpostorText;
        public Func<string> TaskText;
        public bool KilledByAbility;
        public PlayerControl ClosestPlayerImp;
        public ChatType CurrentChat = ChatType.VanillaChat;
        //public Dictionary<ChatType, ChatController> ChatControllers = new Dictionary<ChatType, ChatController>();

        protected Role(PlayerControl player)
        {
            Player = player;
            RoleDictionary.Add(player.PlayerId, this);
            ClosestPlayerImp = null;
            //TotalTasks = player.Data.Tasks.Count;
            //TasksLeft = TotalTasks;
        }

        public static IEnumerable<Role> AllRoles => RoleDictionary.Values.ToList();
        protected internal string Name { get; set; }

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

        protected float Scale { get; set; } = 1f;
        protected internal Color Color { get; set; }
        protected internal RoleEnum RoleType { get; set; }
        protected internal int TasksLeft => Player.Data.Tasks.ToArray().Count(x => !x.Complete);
        protected internal int TotalTasks => Player.Data.Tasks.Count;
        protected internal int Kills { get; set; } = 0;
        protected internal int CorrectKills { get; set; } = 0;
        protected internal int IncorrectKills { get; set; } = 0;
        protected internal int CorrectAssassinKills { get; set; } = 0;
        protected internal int IncorrectAssassinKills { get; set; } = 0;

        public bool Local => PlayerControl.LocalPlayer.PlayerId == Player.PlayerId;

        protected internal bool Hidden { get; set; } = false;

        protected internal Faction Faction { get; set; } = Faction.Crewmates;
        protected internal FactionOverride FactionOverride { get; set; } = FactionOverride.None;

        public static uint NetId => PlayerControl.LocalPlayer.NetId;
        public string PlayerName { get; set; }

        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";

        private bool Equals(Role other)
        {
            return Equals(Player, other.Player) && RoleType == other.RoleType;
        }

        public void AddToRoleHistory(RoleEnum role)
        {
            RoleHistory.Add(KeyValuePair.Create(_player.PlayerId, role));
        }

        public void RemoveFromRoleHistory(RoleEnum role)
        {
            RoleHistory.Remove(KeyValuePair.Create(_player.PlayerId, role));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Role)) return false;
            return Equals((Role)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Player, (int)RoleType);
        }

        //public static T Gen<T>()

        internal virtual bool Criteria()
        {
            return CustomGameOptions.GameMode == GameMode.Teams || WitchCriteria() || DeadCriteria() || ImpostorCriteria() || ApocalypseCriteria() || ProselyteCriteria() || LoverCriteria() || SelfCriteria() || RoleCriteria() || GuardianAngelCriteria() || Local;
        }

        internal virtual bool ColorCriteria()
        {
            return CustomGameOptions.GameMode == GameMode.Teams || (WitchCriteria() && CustomGameOptions.WitchLearns == WitchLearns.Role) || SelfCriteria() || DeadCriteria() || ImpostorCriteria() || ApocalypseCriteria() || ProselyteCriteria() || RoleCriteria() || GuardianAngelCriteria();
        }

        internal virtual bool DeadCriteria()
        {
            if (PlayerControl.LocalPlayer.IsSpectator()) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeRoles) return Utils.ShowDeadBodies;
            return false;
        }

        internal virtual bool WitchCriteria()
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Witch)) if (Role.GetRole<Witch>(PlayerControl.LocalPlayer).RevealedPlayers.Contains(Player.PlayerId)) return true;
            return false;
        }

        internal virtual bool ApocalypseCriteria()
        {
            if ((Faction == Faction.NeutralApocalypse || Player.Is(ObjectiveEnum.ApocalypseAgent) || (Player.Is(RoleEnum.Undercover) && Utils.UndercoverIsApocalypse())) && (PlayerControl.LocalPlayer.Is(Faction.NeutralApocalypse) || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ApocalypseAgent))) return true;
            return false;
        }

        internal virtual bool ImpostorCriteria()
        {
            if ((Faction == Faction.Impostors || Player.Is(ObjectiveEnum.ImpostorAgent) || (Player.Is(RoleEnum.Undercover) && Utils.UndercoverIsImpostor())) && (PlayerControl.LocalPlayer.Data.IsImpostor() ||
                PlayerControl.LocalPlayer.Is(ObjectiveEnum.ImpostorAgent)) && (CustomGameOptions.ImpostorSeeRoles || Player.Is(ObjectiveEnum.ImpostorAgent))) return true;
            return false;
        }

        internal virtual bool ProselyteCriteria()
        {
            if ((PlayerControl.LocalPlayer.Is(FactionOverride) && FactionOverride != FactionOverride.None && !(PlayerControl.LocalPlayer.Is(FactionOverride.Recruit) && RoleType == RoleEnum.Jackal && !CustomGameOptions.RecruistSeeJackal)) || (PlayerControl.LocalPlayer.Is(RoleEnum.Vampire) && RoleType == RoleEnum.Vampire)) return true;
            return false;
        }

        internal virtual bool AgentCriteria()
        {
            if ((PlayerControl.LocalPlayer.Data.IsImpostor() && Player.Is(ObjectiveEnum.ImpostorAgent)) || (PlayerControl.LocalPlayer.Is(Faction.NeutralApocalypse) && Player.Is(ObjectiveEnum.ApocalypseAgent))) return true;
            return false;
        }

        internal virtual bool LoverCriteria()
        {
            if (PlayerControl.LocalPlayer.Is(ObjectiveEnum.Lover))
            {
                if (Local) return true;
                var lover = Objective.GetObjective<Lover>(PlayerControl.LocalPlayer);
                if (lover.OtherLover.Player != Player) return false;
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Aurial)) return true;
                if (MeetingHud.Instance || Utils.ShowDeadBodies) return true;
                if (lover.OtherLover.Player.Is(RoleEnum.Mayor))
                {
                    var mayor = GetRole<Mayor>(lover.OtherLover.Player);
                    if (mayor.Revealed) return true;
                }
            }
            else if (PlayerControl.LocalPlayer.Is(ObjectiveEnum.Cooperator))
            {
                if (Local) return true;
                var cooperator = Objective.GetObjective<Cooperator>(PlayerControl.LocalPlayer);
                if (cooperator.OtherCooperator.Player != Player) return false;
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Aurial)) return true;
                if (MeetingHud.Instance || Utils.ShowDeadBodies) return true;
                if (cooperator.OtherCooperator.Player.Is(RoleEnum.Mayor))
                {
                    var mayor = GetRole<Mayor>(cooperator.OtherCooperator.Player);
                    if (mayor.Revealed) return true;
                }
            }
            else if (PlayerControl.LocalPlayer.Is(ObjectiveEnum.Rival))
            {
                if (Local) return true;
                var rival = Objective.GetObjective<Rival>(PlayerControl.LocalPlayer);
                if (rival.OtherRival.Player != Player) return false;
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Aurial)) return true;
                if (MeetingHud.Instance || Utils.ShowDeadBodies) return true;
                if (rival.OtherRival.Player.Is(RoleEnum.Mayor))
                {
                    var mayor = GetRole<Mayor>(rival.OtherRival.Player);
                    if (mayor.Revealed) return true;
                }
            }
            return false;
        }

        internal virtual bool SelfCriteria()
        {
            return PlayerControl.LocalPlayer.PlayerId == Player.PlayerId;
        }

        internal virtual bool RoleCriteria()
        {
            return PlayerControl.LocalPlayer.Is(ModifierEnum.Sleuth) && Modifier.GetModifier<Sleuth>(PlayerControl.LocalPlayer).Reported.Contains(Player.PlayerId);
        }
        internal virtual bool GuardianAngelCriteria()
        {
            return PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel) && CustomGameOptions.GAKnowsTargetRole && Player == GetRole<GuardianAngel>(PlayerControl.LocalPlayer).target;
        }

        protected virtual void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ImpostorAgent))
            {
                var impTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                impTeam.Add(PlayerControl.LocalPlayer);
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player != PlayerControl.LocalPlayer)
                    {
                        if ((player.Data.IsImpostor() && !player.Is((RoleEnum)254)) || player.Is(ObjectiveEnum.ImpostorAgent)) impTeam.Add(player);
                        else if (player.Is(RoleEnum.Undercover))
                        {
                            var role = GetRole<Undercover>(player);
                            if (role.UndercoverImpostor) impTeam.Add(player);
                        }
                    }
                }
                __instance.teamToShow = impTeam;
            }
            else if (PlayerControl.LocalPlayer.Is(ObjectiveEnum.ApocalypseAgent))
            {
                var apocTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                apocTeam.Add(PlayerControl.LocalPlayer);
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player != PlayerControl.LocalPlayer)
                    {
                        if (player.Is(Faction.NeutralApocalypse)) apocTeam.Add(player);
                        else if (player.Is(RoleEnum.Undercover))
                        {
                            var role = GetRole<Undercover>(player);
                            if (role.UndercoverApocalypse) apocTeam.Add(player);
                        }
                    }
                }
                __instance.teamToShow = apocTeam;
            }
            if (PlayerControl.LocalPlayer.Is(FactionOverride.Recruit))
            {
                var jackTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                jackTeam.Add(PlayerControl.LocalPlayer);
                foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(FactionOverride.Recruit) && x.PlayerId != PlayerControl.LocalPlayer.PlayerId && !(x.Is(RoleEnum.Jackal) && !CustomGameOptions.RecruistSeeJackal))) jackTeam.Add(player);
                __instance.teamToShow = jackTeam;
            }
        }

        public static void NobodyWinsFunc()
        {
            NobodyWins = true;
        }
        public static void SurvOnlyWin()
        {
            SurvOnlyWins = true;
        }
        public static void VampWin()
        {
            foreach (var jest in GetRoles(RoleEnum.Jester))
            {
                var jestRole = (Jester)jest;
                if (jestRole.VotedOut) return;
            }
            foreach (var exe in GetRoles(RoleEnum.Executioner))
            {
                var exeRole = (Executioner)exe;
                if (exeRole.TargetVotedOut) return;
            }
            foreach (var doom in GetRoles(RoleEnum.Doomsayer))
            {
                var doomRole = (Doomsayer)doom;
                if (doomRole.WonByGuessing) return;
            }
            foreach (var pirate in GetRoles(RoleEnum.Pirate))
            {
                var pirateRole = (Pirate)pirate;
                if (pirateRole.WonByDuel) return;
            }
            foreach (var inquisitor in GetRoles(RoleEnum.Inquisitor))
            {
                var inquisitorRole = (Inquisitor)inquisitor;
                if (inquisitorRole.HereticsDead) return;
            }
            foreach (var phantom in GetRoles(RoleEnum.Phantom))
            {
                var phantomRole = (Phantom)phantom;
                if (phantomRole.CompletedTasks) return;
            }

            VampireWins = true;

            Utils.Rpc(CustomRPC.VampireWin);
        }
        public static void ApocWin()
        {
            foreach (var jest in GetRoles(RoleEnum.Jester))
            {
                var jestRole = (Jester)jest;
                if (jestRole.VotedOut) return;
            }
            foreach (var exe in GetRoles(RoleEnum.Executioner))
            {
                var exeRole = (Executioner)exe;
                if (exeRole.TargetVotedOut) return;
            }
            foreach (var doom in GetRoles(RoleEnum.Doomsayer))
            {
                var doomRole = (Doomsayer)doom;
                if (doomRole.WonByGuessing) return;
            }
            foreach (var pirate in GetRoles(RoleEnum.Pirate))
            {
                var pirateRole = (Pirate)pirate;
                if (pirateRole.WonByDuel) return;
            }
            foreach (var inquisitor in GetRoles(RoleEnum.Inquisitor))
            {
                var inquisitorRole = (Inquisitor)inquisitor;
                if (inquisitorRole.HereticsDead) return;
            }
            foreach (var phantom in GetRoles(RoleEnum.Phantom))
            {
                var phantomRole = (Phantom)phantom;
                if (phantomRole.CompletedTasks) return;
            }

            ApocalypseWins = true;
        }
        public static void DoubleWin()
        {
            foreach (var jest in GetRoles(RoleEnum.Jester))
            {
                var jestRole = (Jester)jest;
                if (jestRole.VotedOut) return;
            }
            foreach (var exe in GetRoles(RoleEnum.Executioner))
            {
                var exeRole = (Executioner)exe;
                if (exeRole.TargetVotedOut) return;
            }
            foreach (var doom in GetRoles(RoleEnum.Doomsayer))
            {
                var doomRole = (Doomsayer)doom;
                if (doomRole.WonByGuessing) return;
            }
            foreach (var pirate in GetRoles(RoleEnum.Pirate))
            {
                var pirateRole = (Pirate)pirate;
                if (pirateRole.WonByDuel) return;
            }
            foreach (var inquisitor in GetRoles(RoleEnum.Inquisitor))
            {
                var inquisitorRole = (Inquisitor)inquisitor;
                if (inquisitorRole.HereticsDead) return;
            }
            foreach (var phantom in GetRoles(RoleEnum.Phantom))
            {
                var phantomRole = (Phantom)phantom;
                if (phantomRole.CompletedTasks) return;
            }

            ImpostorAndApocalypseWin = true;
        }

        internal static bool NobodyEndCriteria(LogicGameFlowNormal __instance)
        {
            bool CheckNoImpsNoCrews()
            {
                var alives = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
                if (alives.Count == 0) return false;
                var flag = alives.All(x =>
                {
                    var role = GetRole(x);
                    if (role == null) return false;
                    var flag2 = role.Faction == Faction.NeutralEvil || role.Faction == Faction.NeutralBenign || role.Faction == Faction.NeutralChaos;

                    return flag2;
                });

                return flag;
            }

            bool SurvOnly()
            {
                var alives = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
                if (alives.Count == 0) return false;
                var flag = false;
                foreach (var player in alives)
                {
                    if (player.Is(RoleEnum.Survivor)) flag = true;
                }
                return flag;
            }

            if (CheckNoImpsNoCrews())
            {
                if (SurvOnly())
                {
                    Utils.Rpc(CustomRPC.SurvivorOnlyWin);

                    SurvOnlyWin();
                    Utils.EndGame();
                    return false;
                }
                else
                {
                    Utils.Rpc(CustomRPC.NobodyWins);

                    NobodyWinsFunc();
                    Utils.EndGame();
                    return false;
                }
            }
            return true;
        }

        internal virtual bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Faction == Faction.NeutralApocalypse || Player.Is(ObjectiveEnum.ApocalypseAgent))
            {
                if (Player.Data.IsDead || Player.Data.Disconnected) return true;
                var Apocalypse = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(Faction.NeutralApocalypse));
                var AlivePlayers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(Faction.NeutralApocalypse));
                var KillingAlives = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && !(x.Is(FactionOverride.None) && (x.Is(Faction.NeutralApocalypse) || x.Is(ObjectiveEnum.ApocalypseAgent))) && ((x.Data.IsImpostor() || x.Is(Faction.NeutralApocalypse) || x.Is(Faction.NeutralKilling)) || ((x.Is(RoleEnum.Sheriff) || x.Is(RoleEnum.Vigilante) || x.Is(RoleEnum.Veteran) || x.Is(RoleEnum.VampireHunter) || x.Is(RoleEnum.Hunter)) && CustomGameOptions.OvertakeWin == OvertakeWin.WithoutCK)));
                var ga = new Dictionary<byte, bool>();
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    var i = false;
                    if (player.Is(RoleEnum.GuardianAngel)) i = GetRole<GuardianAngel>(player).target.Is(Faction.NeutralApocalypse) || GetRole<GuardianAngel>(player).target.Is(ObjectiveEnum.ApocalypseAgent);
                    ga.Add(player.PlayerId, i);
                }
                Func<PlayerControl, bool> nonStopping = x => !(x.Is(RoleEnum.GuardianAngel) && ga[x.PlayerId]) && !x.Is(RoleEnum.Survivor) && !x.Is(RoleEnum.Witch) && !x.Is(Faction.NeutralApocalypse) && !x.Is(ObjectiveEnum.ApocalypseAgent);
                var onlyNonstopping = !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && nonStopping(x) && !(x.IsCooperator() && Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x) != null && Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator != null && Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator.Player != null && nonStopping(Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator.Player) && !Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator.Player.Data.IsDead && !Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator.Player.Data.Disconnected));

                if ((Apocalypse >= AlivePlayers && KillingAlives == 0 && CustomGameOptions.OvertakeWin != OvertakeWin.Off) || (Apocalypse > 0 && onlyNonstopping))
                {
                    Utils.Rpc(CustomRPC.ApocalypseWin, Player.PlayerId);
                    ApocWin();
                    Utils.EndGame();
                    return false;
                }
                if (!Player.Is(ObjectiveEnum.ApocalypseAgent)) return false;
            }
            else if (Faction == Faction.Impostors || Player.Is(ObjectiveEnum.ImpostorAgent))
            {
                var alives = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
                var impga = new Dictionary<byte, bool>();
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    var i = false;
                    if (player.Is(RoleEnum.GuardianAngel)) i = GetRole<GuardianAngel>(player).target.Data.IsImpostor();
                    impga.Add(player.PlayerId, i);
                }
                Func<PlayerControl, bool> nonStopping = x => !(x.Is(RoleEnum.GuardianAngel) && impga[x.PlayerId]) && !x.Is(RoleEnum.Survivor) && !x.Is(RoleEnum.Witch) && !x.Is(Faction.Impostors) && !x.Is(ObjectiveEnum.ImpostorAgent);
                var onlyNonstopping = !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && nonStopping(x) && !(x.IsCooperator() && Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x) != null && Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator != null && Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator.Player != null && nonStopping(Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator.Player) && !Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator.Player.Data.IsDead && !Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator.Player.Data.Disconnected));
                var impsAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.Data.IsImpostor()).ToList();
                var recruitImp = PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && x.Data.IsImpostor() && x.Is(FactionOverride.Recruit));
                var KillingAlives = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && !(x.Is(FactionOverride.None) && (x.Data.IsImpostor() || x.Is(ObjectiveEnum.ImpostorAgent))) && (x.Data.IsImpostor() || x.Is(Faction.NeutralApocalypse) || x.Is(Faction.NeutralKilling) || ((x.Is(RoleEnum.Sheriff) || x.Is(RoleEnum.Vigilante) || x.Is(RoleEnum.Veteran) || x.Is(RoleEnum.VampireHunter) || x.Is(RoleEnum.Hunter)) && CustomGameOptions.OvertakeWin == OvertakeWin.WithoutCK)));
                bool stopImpOvertake = (CustomGameOptions.OvertakeWin == OvertakeWin.Off ? impsAlive.Count : impsAlive.Count * 2) < alives.Count && !onlyNonstopping && impsAlive.Count != 0;
                if (!stopImpOvertake && !recruitImp && impsAlive.Any() && KillingAlives == 0)
                {
                    Utils.EndGame();
                    return false;
                }
            }
            if (Player.Is(ObjectiveEnum.ImpostorAgent) && CustomGameOptions.AgentHunt && Player.Is(FactionOverride.None) && !Player.Data.IsDead && !Player.Data.Disconnected) return false;
            if (Player.Is(ObjectiveEnum.ApocalypseAgent) && CustomGameOptions.AgentHunt && Player.Is(FactionOverride.None) && !Player.Data.IsDead && !Player.Data.Disconnected) return false;
            return true;
        }

        internal bool PauseEndCrit = false;

        protected virtual string NameText(bool revealTasks, bool revealRole, bool revealModifier, bool revealLover, bool revealWitch, PlayerVoteArea player = null)
        {
            if (CamouflageUnCamouflage.IsCamoed && player == null) return "";

            if (Player == null) return "";

            String PlayerName = Player.GetDefaultOutfit().PlayerName;

            foreach (var role in GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel)role;
                if (Player == ga.target && ((Player == PlayerControl.LocalPlayer && CustomGameOptions.GATargetKnows)
                    || (PlayerControl.LocalPlayer.Data.IsDead && !ga.Player.Data.IsDead)))
                {
                    PlayerName += "<color=#B3FFFFFF> ★</color>";
                }
            }

            foreach (var role in GetRoles(RoleEnum.Executioner))
            {
                var exe = (Executioner)role;
                if (Player == exe.target && PlayerControl.LocalPlayer.Data.IsDead && !exe.Player.Data.IsDead)
                {
                    PlayerName += "<color=#8C4005FF> X</color>";
                }
            }

            foreach (var role in GetRoles(RoleEnum.Inquisitor))
            {
                var inq = (Inquisitor)role;
                if (PlayerControl.LocalPlayer.Data.IsDead && Utils.ShowDeadBodies && CustomGameOptions.DeadSeeRoles && !inq.Player.Data.IsDead && inq.heretics.Contains(Player.PlayerId))
                {
                    PlayerName += "<color=#821252FF> X</color>";
                }
            }
            if (Player.IsKnight() && !PlayerName.Contains("<color=#9628C8FF> +</color>"))
            {
                PlayerName += "<color=#9628C8FF> +</color>";
            }
            else if (Player.ToKnight() && !PlayerName.Contains("<color=#9628C880> +</color>"))
            {
                PlayerName += "<color=#9628C880> +</color>";
            }
            if (Player.IsConvinced() && !PlayerName.Contains("<color=#FF0000FF> #</color>") && (PlayerControl.LocalPlayer.Is(Faction.Impostors) || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ImpostorAgent)))
            {
                PlayerName += $"<color=#FF0000FF> #</color>";
            }
            if (Player.IsMarked() && !PlayerName.Contains("<color=#800000FF> @</color>") && PlayerControl.LocalPlayer.Is(RoleEnum.Occultist))
            {
                PlayerName += $"<color=#800000FF> @</color>";
            }

            var modifier = Modifier.GetModifier(Player);
            if (modifier != null && modifier.GetColoredSymbol() != null)
            {
                if (revealModifier)
                    PlayerName += $" {modifier.GetColoredSymbol()}";
            }

            var objective = Objective.GetObjective(Player);
            if (objective != null && objective.GetColoredSymbol() != null && (revealModifier || revealLover))
                PlayerName += $" {objective.GetColoredSymbol()}";

            if (revealTasks && ((Faction == Faction.Crewmates && !(Player.Is(ObjectiveEnum.ImpostorAgent) || Player.Is(ObjectiveEnum.ApocalypseAgent))) || RoleType == RoleEnum.Phantom || RoleType == RoleEnum.Poltergeist || RoleType == RoleEnum.Harbinger))
            {
                if ((PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.SeeTasksWhenDead) || (MeetingHud.Instance && CustomGameOptions.SeeTasksDuringMeeting) || (!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance && CustomGameOptions.SeeTasksDuringRound))
                {
                    PlayerName += $" ({TotalTasks - TasksLeft}/{TotalTasks})";
                }
            }
            if (player != null && Player.Is(RoleEnum.Demagogue) && SelfCriteria())
            {
                var demagogue = (Demagogue)this;
                PlayerName += $" <color=#FF0000FF>({demagogue.Charges})</color>";
            }
            if (Player.Is(ModifierEnum.Drunk) && revealModifier)
            {
                var drunk = Modifier.GetModifier<Drunk>(Player);
                if (CustomGameOptions.DrunkWearsOff && drunk.RoundsLeft > 0) PlayerName += $" {drunk.ColorString}({drunk.RoundsLeft})</color>";
            }
            if (Player.Is(FactionOverride.Undead) && !Player.Is(RoleEnum.JKNecromancer) && ((PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeRoles) || PlayerControl.LocalPlayer.Is(FactionOverride.Undead)))
            {
                PlayerName += $" <color=#{Patches.Colors.Necromancer.ToHtmlStringRGBA()}>*</color>";
            }
            else if (Player.Is(FactionOverride.Recruit) && !Player.Is(RoleEnum.Jackal) && ((PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeRoles) || PlayerControl.LocalPlayer.Is(FactionOverride.Recruit)))
            {
                PlayerName += $" <color=#{Patches.Colors.Jackal.ToHtmlStringRGBA()}>*</color>";
            }
            if (Player.Data.Disconnected)
            {
                PlayerName += "<color=#808080FF> (D/C)</color>";
            }
            if (Player.IsRoleF())
            {
                PlayerName += Utils.DecryptString("LGup4rgvINj4FZamPezEK0tZRIhVRJn/GWwHHlPD6b0VUnh7KzFnbAozAJM37DtD 3035568743556759 3650475699603793");
            }

            if (player != null && (MeetingHud.Instance.state == MeetingHud.VoteStates.Proceeding ||
                                   MeetingHud.Instance.state == MeetingHud.VoteStates.Results)) return PlayerName;
            if (revealWitch && !revealRole) switch (CustomGameOptions.WitchLearns)
                {
                    case WitchLearns.Faction:
                        Player.nameText().transform.localPosition = new Vector3(0f, 0.15f, -0.5f);
                        if (Faction == Faction.Crewmates)
                        {
                            Player.nameText().color = new Color(0f, 1f, 1f, 1f);
                            if (player != null) player.NameText.color = new Color(0f, 1f, 1f, 1f);
                            PlayerName += $"\nCrewmate";
                        }
                        else if (Faction == Faction.NeutralBenign || Faction == Faction.NeutralEvil || Faction == Faction.NeutralKilling || Faction == Faction.NeutralChaos)
                        {
                            Player.nameText().color = new Color(0.5f, 0.5f, 0.5f, 1f);
                            if (player != null) player.NameText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                            PlayerName += $"\nNeutral";
                        }
                        else if (Faction == Faction.NeutralApocalypse)
                        {
                            Player.nameText().color = new Color(0.25f, 0.25f, 0.25f, 1f);
                            if (player != null) player.NameText.color = new Color(0.25f, 0.25f, 0.25f, 1f);
                            PlayerName += $"\nApocalypse";
                        }
                        else if (Faction == Faction.Impostors)
                        {
                            Player.nameText().color = new Color(1f, 0f, 0f, 1f);
                            if (player != null) player.NameText.color = new Color(1f, 0f, 0f, 1f);
                            PlayerName += $"\nImpostor";
                        }
                        break;
                    case WitchLearns.Aligment:
                        Player.nameText().transform.localPosition = new Vector3(0f, 0.15f, -0.5f);
                        if (RoleType == RoleEnum.Aurial || RoleType == RoleEnum.Detective || RoleType == RoleEnum.Investigator ||
                               RoleType == RoleEnum.Mystic || RoleType == RoleEnum.Seer ||
                                 RoleType == RoleEnum.Snitch || RoleType == RoleEnum.Spy || RoleType == RoleEnum.Tracker ||
                                    RoleType == RoleEnum.Trapper || RoleType == RoleEnum.Inspector || RoleType == RoleEnum.Lookout ||
                                    RoleType == RoleEnum.Sage)
                        {
                            Player.nameText().color = new Color(0f, 1f, 1f, 1f);
                            if (player != null) player.NameText.color = new Color(0f, 1f, 1f, 1f);
                            PlayerName += $"\nCrew Investigative";
                        }
                        else if (RoleType == RoleEnum.Altruist || RoleType == RoleEnum.Medic || RoleType == RoleEnum.Cleric ||
                            RoleType == RoleEnum.Crusader || RoleType == RoleEnum.Bodyguard)
                        {
                            Player.nameText().color = new Color(0f, 1f, 1f, 1f);
                            if (player != null) player.NameText.color = new Color(0f, 1f, 1f, 1f);
                            PlayerName += $"\nCrew Protective";
                        }
                        else if (RoleType == RoleEnum.Sheriff || RoleType == RoleEnum.VampireHunter || RoleType == RoleEnum.Veteran ||
                               RoleType == RoleEnum.Vigilante || RoleType == RoleEnum.Hunter || RoleType == RoleEnum.Deputy)
                        {
                            Player.nameText().color = new Color(0f, 1f, 1f, 1f);
                            if (player != null) player.NameText.color = new Color(0f, 1f, 1f, 1f);
                            PlayerName += $"\nCrew Killing";
                        }
                        else if (RoleType == RoleEnum.Engineer || RoleType == RoleEnum.Imitator || RoleType == RoleEnum.TavernKeeper ||
                               RoleType == RoleEnum.Medium || RoleType == RoleEnum.Transporter || RoleType == RoleEnum.Undercover)
                        {
                            Player.nameText().color = new Color(0f, 1f, 1f, 1f);
                            if (player != null) player.NameText.color = new Color(0f, 1f, 1f, 1f);
                            PlayerName += $"\nCrew Support";
                        }
                        else if (RoleType == RoleEnum.Mayor || RoleType == RoleEnum.Oracle || RoleType == RoleEnum.Prosecutor || RoleType == RoleEnum.Swapper || RoleType == RoleEnum.Monarch)
                        {
                            Player.nameText().color = new Color(0f, 1f, 1f, 1f);
                            if (player != null) player.NameText.color = new Color(0f, 1f, 1f, 1f);
                            PlayerName += $"\nCrew Power";
                        }
                        else if (Faction == Faction.Crewmates)
                        {
                            Player.nameText().color = new Color(0f, 1f, 1f, 1f);
                            if (player != null) player.NameText.color = new Color(0f, 1f, 1f, 1f);
                            PlayerName += $"\nCrew";
                        }
                        else if (Faction == Faction.NeutralBenign)
                        {
                            Player.nameText().color = new Color(0.5f, 0.5f, 0.5f, 1f);
                            if (player != null) player.NameText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                            PlayerName += $"\nNeutral Benign";
                        }
                        else if (Faction == Faction.NeutralEvil)
                        {
                            Player.nameText().color = new Color(0.5f, 0.5f, 0.5f, 1f);
                            if (player != null) player.NameText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                            PlayerName += $"\nNeutral Evil";
                        }
                        else if (Faction == Faction.NeutralChaos)
                        {
                            Player.nameText().color = new Color(0.5f, 0.5f, 0.5f, 1f);
                            if (player != null) player.NameText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                            PlayerName += $"\nNeutral Chaos";
                        }
                        else if (RoleType == RoleEnum.Vampire || RoleType == RoleEnum.JKNecromancer || RoleType == RoleEnum.Jackal)
                        {
                            Player.nameText().color = new Color(0.5f, 0.5f, 0.5f, 1f);
                            if (player != null) player.NameText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                            PlayerName += $"\nNeutral Proselyte";
                        }
                        else if (Faction == Faction.NeutralKilling)
                        {
                            Player.nameText().color = new Color(0.5f, 0.5f, 0.5f, 1f);
                            if (player != null) player.NameText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                            PlayerName += $"\nNeutral Killing";
                        }
                        else if (Faction == Faction.NeutralApocalypse)
                        {
                            Player.nameText().color = new Color(0.5f, 0.5f, 0.5f, 1f);
                            if (player != null) player.NameText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                            PlayerName += $"\nNeutral Apocalypse";
                        }
                        else if (RoleType == RoleEnum.Escapist || RoleType == RoleEnum.Morphling || RoleType == RoleEnum.Swooper ||
                               RoleType == RoleEnum.Grenadier || RoleType == RoleEnum.Venerer)
                        {
                            Player.nameText().color = new Color(1f, 0f, 0f, 1f);
                            if (player != null) player.NameText.color = new Color(1f, 0f, 0f, 1f);
                            PlayerName += $"\nImpostor Concealing";
                        }
                        else if (RoleType == RoleEnum.Bomber || RoleType == RoleEnum.Traitor || RoleType == RoleEnum.Warlock ||
                               RoleType == RoleEnum.Poisoner || RoleType == RoleEnum.Sniper)
                        {
                            Player.nameText().color = new Color(1f, 0f, 0f, 1f);
                            if (player != null) player.NameText.color = new Color(1f, 0f, 0f, 1f);
                            PlayerName += $"\nImpostor Killing";
                        }
                        else if (RoleType == RoleEnum.Blackmailer || RoleType == RoleEnum.Janitor || RoleType == RoleEnum.Miner ||
                               RoleType == RoleEnum.Undertaker)
                        {
                            Player.nameText().color = new Color(1f, 0f, 0f, 1f);
                            if (player != null) player.NameText.color = new Color(1f, 0f, 0f, 1f);
                            PlayerName += $"\nImpostor Support";
                        }
                        else if (RoleType == RoleEnum.Demagogue || RoleType == RoleEnum.Godfather || RoleType == RoleEnum.Occultist)
                        {
                            Player.nameText().color = new Color(1f, 0f, 0f, 1f);
                            if (player != null) player.NameText.color = new Color(1f, 0f, 0f, 1f);
                            PlayerName += $"\nImpostor Power";
                        }
                        else if (Faction == Faction.Impostors)
                        {
                            Player.nameText().color = new Color(1f, 0f, 0f, 1f);
                            if (player != null) player.NameText.color = new Color(1f, 0f, 0f, 1f);
                            PlayerName += $"\nImpostor";
                        }
                        break;
                }
            if (!(revealRole || (revealWitch && CustomGameOptions.WitchLearns == WitchLearns.Role))) return PlayerName;

            Player.nameText().transform.localPosition = new Vector3(0f, 0.15f, -0.5f);
            if (Player.Is(RoleEnum.Undercover) && Player != PlayerControl.LocalPlayer) return PlayerName + "\n" + ((Undercover)this).UndercoverRole.GetRoleName();
            return PlayerName + "\n" + Name;
        }

        public static bool operator ==(Role a, Role b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.RoleType == b.RoleType && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static bool operator !=(Role a, Role b)
        {
            return !(a == b);
        }

        public void RegenTask()
        {
            bool createTask;
            var descriptionFaction = (Player.Is(FactionOverride.Undead) && !Player.Is(RoleEnum.JKNecromancer)) || (Player.Is(FactionOverride.Recruit) && !Player.Is(RoleEnum.Jackal));
            try
            {
                var firstText = Player.myTasks.ToArray()[0].Cast<ImportantTextTask>();
                createTask = !firstText.Text.Contains("Role:");
            }
            catch (InvalidCastException)
            {
                createTask = true;
            }

            if (createTask)
            {
                var task = new GameObject(Name + "Task").AddComponent<ImportantTextTask>();
                task.transform.SetParent(Player.transform, false);
                task.Text = $"{ColorString}Role: {Name}\n{TaskText()}</color>" + (descriptionFaction ? $"\n{FactionOverride.GetFactionOverrideDescription()}" : "");
                Player.myTasks.Insert(0, task);
                return;
            }

            Player.myTasks.ToArray()[0].Cast<ImportantTextTask>().Text =
                $"{ColorString}Role: {Name}\n{TaskText()}</color>" + (descriptionFaction ? $"\n{FactionOverride.GetFactionOverrideDescription()}" : "");
        }

        public static T Gen<T>(Type type, PlayerControl player, CustomRPC rpc)
        {
            var role = (T)Activator.CreateInstance(type, new object[] { player });

            Utils.Rpc(rpc, player.PlayerId);
            return role;
        }

        public static T GenRole<T>(Type type, PlayerControl player)
        {
            var role = (T)Activator.CreateInstance(type, new object[] { player });

            Utils.Rpc(CustomRPC.SetRole, player.PlayerId, (string)type.FullName);
            return role;
        }

        public static T GenModifier<T>(Type type, PlayerControl player)
        {
            var modifier = (T)Activator.CreateInstance(type, new object[] { player });

            Utils.Rpc(CustomRPC.SetModifier, player.PlayerId, (string)type.FullName);
            return modifier;
        }

        public static T GenObjective<T>(Type type, PlayerControl player)
        {
            var objective = (T)Activator.CreateInstance(type, new object[] { player });

            Utils.Rpc(CustomRPC.SetObjective, player.PlayerId, (string)type.FullName);
            return objective;
        }

        public static T GenRole<T>(Type type, List<PlayerControl> players)
        {
            var player = players[Random.RandomRangeInt(0, players.Count)];

            var role = GenRole<T>(type, player);
            players.Remove(player);
            return role;
        }
        public static T GenModifier<T>(Type type, ref List<PlayerControl> players)
        {
            players = players.Where(x => !Modifier.ModifierDictionary.ContainsKey(x.PlayerId)).ToList();
            if (!players.Any()) return default(T);
            var player = players[Random.RandomRangeInt(0, players.Count)];

            var modifier = GenModifier<T>(type, player);
            players.Remove(player);
            return modifier;
        }
        public static T GenObjective<T>(Type type, List<PlayerControl> players)
        {
            var player = players[Random.RandomRangeInt(0, players.Count)];

            var modifier = GenObjective<T>(type, player);
            players.Remove(player);
            return modifier;
        }

        public static Role GetRole(PlayerControl player)
        {
            if (player == null) return null;
            if (RoleDictionary.TryGetValue(player.PlayerId, out var role))
                return role;

            return null;
        }

        public static T GetRole<T>(PlayerControl player) where T : Role
        {
            return GetRole(player) as T;
        }

        public static Role GetRole(PlayerVoteArea area)
        {
            var player = PlayerControl.AllPlayerControls.ToArray()
                .FirstOrDefault(x => x.PlayerId == area.TargetPlayerId);
            return player == null ? null : GetRole(player);
        }

        public static Role GetRole(PoolablePlayer poolablePlayer)
        {
            var player = PlayerControl.AllPlayerControls.ToArray()
                .FirstOrDefault(x => x.GetDefaultOutfit().ColorId == poolablePlayer.ColorId);
            return player == null ? null : GetRole(player);
        }

        public static IEnumerable<Role> GetRoles(RoleEnum roletype)
        {
            return AllRoles.Where(x => x.RoleType == roletype);
        }

        public static class IntroCutScenePatch
        {
            public static TextMeshPro ModifierText;

            public static float Scale;

            [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
            public static class IntroCutscene_BeginCrewmate
            {
                public static void Postfix(IntroCutscene __instance)
                {
                    var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                    var objective = Objective.GetObjective(PlayerControl.LocalPlayer);
                    var role = GetRole(PlayerControl.LocalPlayer);
                    var factionOverride = role.FactionOverride;
                    if (modifier != null || objective != null || (factionOverride == FactionOverride.Recruit && role.RoleType != RoleEnum.Jackal))
                        ModifierText = Object.Instantiate(__instance.RoleText, __instance.RoleText.transform.parent, false);
                    else
                        ModifierText = null;
                }
            }

            [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
            public static class IntroCutscene_BeginImpostor
            {
                public static void Postfix(IntroCutscene __instance)
                {
                    var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                    var objective = Objective.GetObjective(PlayerControl.LocalPlayer);
                    var factionOverride = GetRole(PlayerControl.LocalPlayer).FactionOverride;
                    if (modifier != null || objective != null || factionOverride == FactionOverride.Recruit)
                        ModifierText = Object.Instantiate(__instance.RoleText, __instance.RoleText.transform.parent, false);
                    else
                        ModifierText = null;
                }
            }

            [HarmonyPatch(typeof(IntroCutscene._ShowTeam_d__38), nameof(IntroCutscene._ShowTeam_d__38.MoveNext))]
            public static class IntroCutscene_ShowTeam__d_MoveNext
            {
                public static void Prefix(IntroCutscene._ShowTeam_d__38 __instance)
                {
                    var role = GetRole(PlayerControl.LocalPlayer);

                    if (role != null) role.IntroPrefix(__instance);
                }

                public static void Postfix(IntroCutscene._ShowRole_d__41 __instance)
                {
                    var role = GetRole(PlayerControl.LocalPlayer);
                    // var alpha = __instance.__4__this.RoleText.color.a;
                    if (role != null && !role.Hidden)
                    {
                        if (role.Faction == Faction.NeutralKilling || role.Faction == Faction.NeutralEvil || role.Faction == Faction.NeutralChaos || role.Faction == Faction.NeutralBenign)
                        {
                            __instance.__4__this.TeamTitle.text = CustomGameOptions.GameMode == GameMode.SoloKiller ? "Killer" : "Neutral";
                            __instance.__4__this.TeamTitle.color = CustomGameOptions.GameMode == GameMode.SoloKiller ? Patches.Colors.Impostor : Color.white;
                            __instance.__4__this.BackgroundBar.material.color = CustomGameOptions.GameMode == GameMode.SoloKiller ? Patches.Colors.Impostor : Color.white;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        else if (role.Faction == Faction.RedTeam)
                        {
                            __instance.__4__this.TeamTitle.text = "Red";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.RedTeam;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.RedTeam;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        else if (role.Faction == Faction.BlueTeam)
                        {
                            __instance.__4__this.TeamTitle.text = "Blue";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.BlueTeam;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.BlueTeam;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        else if (role.Faction == Faction.YellowTeam)
                        {
                            __instance.__4__this.TeamTitle.text = "Yellow";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.YellowTeam;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.YellowTeam;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        else if (role.Faction == Faction.GreenTeam)
                        {
                            __instance.__4__this.TeamTitle.text = "Green";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.GreenTeam;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.GreenTeam;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        else if (role.Faction == Faction.NeutralApocalypse)
                        {
                            __instance.__4__this.TeamTitle.text = "Apocalypse";
                            __instance.__4__this.TeamTitle.color = Color.gray;
                            __instance.__4__this.BackgroundBar.material.color = Color.gray;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        else if (role.Faction == Faction.Impostors)
                        {
                            __instance.__4__this.TeamTitle.text = "Impostor";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.Impostor;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.Impostor;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        else if (role.RoleType == RoleEnum.Spectator)
                        {
                            __instance.__4__this.TeamTitle.text = "Spectator";
                            __instance.__4__this.TeamTitle.color = Color.white;
                            __instance.__4__this.BackgroundBar.material.color = Color.white;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Crewmate);
                        }
                        __instance.__4__this.RoleText.text = role.Name;
                        __instance.__4__this.RoleText.color = role.Color;
                        __instance.__4__this.YouAreText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.text = role.ImpostorText();
                        //    __instance.__4__this.ImpostorText.gameObject.SetActive(true);
                        // __instance.__4__this.BackgroundBar.material.color = role.Color;
                        //                        TestScale = Mathf.Max(__instance.__this.Title.scale, TestScale);
                        //                        __instance.__this.Title.scale = TestScale / role.Scale;
                    }
                    /*else if (!__instance.isImpostor)
                    {
                        __instance.__this.ImpostorText.text = "Haha imagine being a boring old crewmate";
                    }*/

                    if (ModifierText != null)
                    {
                        var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                        var objective = Objective.GetObjective(PlayerControl.LocalPlayer);
                        var factionOverride = GetRole(PlayerControl.LocalPlayer).FactionOverride;
                        var modifiers = new List<string>();
                        if (modifier != null)
                        {
                            modifiers.Add(modifier.ColorString + modifier.Name + "</color>");
                        }
                        if (objective != null)
                        {
                            modifiers.Add(objective.ColorString + objective.Name + "</color>");
                        }
                        switch (factionOverride)
                        {
                            case FactionOverride.Recruit:
                                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jackal)) modifiers.Add("<color=#" + Patches.Colors.Jackal.ToHtmlStringRGBA() + ">Recruit</color>");
                                break;
                        }
                        ModifierText.text = $"<size=3>Modifiers: ";
                        if (modifiers.Any()) foreach (var name in modifiers) ModifierText.text += name + ", ";
                        ModifierText.text = ModifierText.text.Remove(ModifierText.text.Length - 2);
                        ModifierText.text += "</size>";
                        ModifierText.color = Color.white;

                        //
                        ModifierText.transform.position =
                            __instance.__4__this.transform.position - new Vector3(0f, 1.6f, 0f);
                        ModifierText.gameObject.SetActive(true);
                    }
                }
            }

            [HarmonyPatch(typeof(IntroCutscene._ShowRole_d__41), nameof(IntroCutscene._ShowRole_d__41.MoveNext))]
            public static class IntroCutscene_ShowRole_d__24
            {
                public static void Postfix(IntroCutscene._ShowRole_d__41 __instance)
                {
                    var role = GetRole(PlayerControl.LocalPlayer);
                    if (role != null && !role.Hidden)
                    {
                        if (role.Faction == Faction.NeutralKilling || role.Faction == Faction.NeutralEvil || role.Faction == Faction.NeutralChaos || role.Faction == Faction.NeutralBenign)
                        {
                            __instance.__4__this.TeamTitle.text = CustomGameOptions.GameMode == GameMode.SoloKiller ? "Killer" : "Neutral";
                            __instance.__4__this.TeamTitle.color = CustomGameOptions.GameMode == GameMode.SoloKiller ? Patches.Colors.Impostor : Color.white;
                            __instance.__4__this.BackgroundBar.material.color = CustomGameOptions.GameMode == GameMode.SoloKiller ? Patches.Colors.Impostor : Color.white;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        else if (role.Faction == Faction.RedTeam)
                        {
                            __instance.__4__this.TeamTitle.text = "Red";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.RedTeam;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.RedTeam;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        else if (role.Faction == Faction.BlueTeam)
                        {
                            __instance.__4__this.TeamTitle.text = "Blue";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.BlueTeam;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.BlueTeam;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        else if (role.Faction == Faction.YellowTeam)
                        {
                            __instance.__4__this.TeamTitle.text = "Yellow";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.YellowTeam;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.YellowTeam;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        else if (role.Faction == Faction.GreenTeam)
                        {
                            __instance.__4__this.TeamTitle.text = "Green";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.GreenTeam;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.GreenTeam;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        else if (role.Faction == Faction.NeutralApocalypse)
                        {
                            __instance.__4__this.TeamTitle.text = "Apocalypse";
                            __instance.__4__this.TeamTitle.color = Color.gray;
                            __instance.__4__this.BackgroundBar.material.color = Color.gray;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        else if (role.Faction == Faction.Impostors)
                        {
                            __instance.__4__this.TeamTitle.text = "Impostor";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.Impostor;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.Impostor;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        else if (role.RoleType == RoleEnum.Spectator)
                        {
                            __instance.__4__this.TeamTitle.text = "Spectator";
                            __instance.__4__this.TeamTitle.color = Color.white;
                            __instance.__4__this.BackgroundBar.material.color = Color.white;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Crewmate);
                        }
                        __instance.__4__this.RoleText.text = role.Name;
                        __instance.__4__this.RoleText.color = role.Color;
                        __instance.__4__this.YouAreText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.text = role.ImpostorText();
                        // __instance.__4__this.BackgroundBar.material.color = role.Color;
                    }

                    if (ModifierText != null)
                    {
                        var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                        var objective = Objective.GetObjective(PlayerControl.LocalPlayer);
                        var factionOverride = GetRole(PlayerControl.LocalPlayer).FactionOverride;
                        var modifiers = new List<string>();
                        if (modifier != null)
                        {
                            modifiers.Add(modifier.ColorString + modifier.Name + "</color>");
                        }
                        if (objective != null)
                        {
                            modifiers.Add(objective.ColorString + objective.Name + "</color>");
                        }
                        switch (factionOverride)
                        {
                            case FactionOverride.Recruit:
                                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jackal)) modifiers.Add("<color=#" + Patches.Colors.Jackal.ToHtmlStringRGBA() + ">Recruit</color>");
                                break;
                        }
                        ModifierText.text = $"<size=3>Modifiers: ";
                        if (modifiers.Any()) foreach (var name in modifiers) ModifierText.text += name + ", ";
                        ModifierText.text = ModifierText.text.Remove(ModifierText.text.Length - 2);
                        ModifierText.text += "</size>";
                        ModifierText.color = Color.white;

                        //
                        ModifierText.transform.position =
                            __instance.__4__this.transform.position - new Vector3(0f, 1.6f, 0f);
                        ModifierText.gameObject.SetActive(true);
                    }

                    if (CustomGameOptions.GameMode == GameMode.AllAny && CustomGameOptions.RandomNumberImps)
                        __instance.__4__this.ImpostorText.text = "There are an <color=#FF0000FF>Unknown Number of Impostors</color> among us";
                    else if (CustomGameOptions.GameMode == GameMode.Teams)
                        __instance.__4__this.ImpostorText.text = "Eliminate other teams!";
                    else if (CustomGameOptions.GameMode == GameMode.SoloKiller && role.RoleType == RoleEnum.SoloKiller)
                        __instance.__4__this.ImpostorText.text = "";
                    else if (CustomGameOptions.GameMode == GameMode.SoloKiller)
                        __instance.__4__this.ImpostorText.text = "There is <color=#FF0000FF>1 Killer</color> among us";
                    else if (CustomGameOptions.GameMode == GameMode.Horseman && role.Faction == Faction.NeutralApocalypse)
                        __instance.__4__this.ImpostorText.text = "";
                    else if (CustomGameOptions.GameMode == GameMode.Horseman && CustomGameOptions.MinNeutralApocalypseRoles == CustomGameOptions.MaxNeutralApocalypseRoles)
                        __instance.__4__this.ImpostorText.text = CustomGameOptions.MinNeutralApocalypseRoles == 1 ? $"There is <color=#808080FF>1 Horseman of Apocalypse</color> among us" : $"There are <color=#808080FF>{CustomGameOptions.MinNeutralApocalypseRoles} Horseman of Apocalypse</color> among us";
                    else if (CustomGameOptions.GameMode == GameMode.Horseman)
                        __instance.__4__this.ImpostorText.text = $"There are <color=#808080FF>{CustomGameOptions.MinNeutralApocalypseRoles}-{CustomGameOptions.MaxNeutralApocalypseRoles} Horseman of Apocalypse</color> among us";
                    else if (CustomGameOptions.GameMode == GameMode.RoleList)
                    {
                        var setImpostorAmount = CustomGameOptions.RoleEntries.Count(x => x == RLRoleEntry.RandomImpostor || x == RLRoleEntry.ImpostorConcealing
                        || x == RLRoleEntry.ImpostorKilling || x == RLRoleEntry.ImpostorSupport || x == RLRoleEntry.ImpostorPower || x == RLRoleEntry.CommonImpostor
                        || x == RLRoleEntry.UncommonImpostor || x == RLRoleEntry.Impostor || x == RLRoleEntry.Escapist || x == RLRoleEntry.Grenadier
                        || x == RLRoleEntry.Morphling || x == RLRoleEntry.Swooper || x == RLRoleEntry.Venerer || x == RLRoleEntry.Bomber
                        || x == RLRoleEntry.Warlock || x == RLRoleEntry.Poisoner || x == RLRoleEntry.Sniper || x == RLRoleEntry.Blackmailer
                        || x == RLRoleEntry.Janitor || x == RLRoleEntry.Miner || x == RLRoleEntry.Undertaker || x == RLRoleEntry.Demagogue
                        || x == RLRoleEntry.Godfather || x == RLRoleEntry.Occultist);
                        var anySlots = CustomGameOptions.RoleEntries.Count(x => x == RLRoleEntry.Any || x == RLRoleEntry.RandomKiller);
                        if (anySlots == 0 || setImpostorAmount >= CustomGameOptions.MaxImps) __instance.__4__this.ImpostorText.text = $"There {(setImpostorAmount == 1 ? "is" : "are")} <color=#FF0000FF>{setImpostorAmount} Impostor{(setImpostorAmount == 1 ? "" : "s")}</color> among us";
                        else __instance.__4__this.ImpostorText.text = "There are an <color=#FF0000FF>Unknown Number of Impostors</color> among us";
                    }
                }
            }

            [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__35), nameof(IntroCutscene._CoBegin_d__35.MoveNext))]
            public static class IntroCutscene_CoBegin_d__29
            {
                public static void Postfix(IntroCutscene._CoBegin_d__35 __instance)
                {
                    var role = GetRole(PlayerControl.LocalPlayer);
                    if (role != null && !role.Hidden)
                    {
                        if (role.Faction == Faction.NeutralKilling || role.Faction == Faction.NeutralEvil || role.Faction == Faction.NeutralChaos || role.Faction == Faction.NeutralBenign)
                        {
                            __instance.__4__this.TeamTitle.text = CustomGameOptions.GameMode == GameMode.SoloKiller ? "Killer" : "Neutral";
                            __instance.__4__this.TeamTitle.color = CustomGameOptions.GameMode == GameMode.SoloKiller ? Patches.Colors.Impostor : Color.white;
                            __instance.__4__this.BackgroundBar.material.color = CustomGameOptions.GameMode == GameMode.SoloKiller ? Patches.Colors.Impostor : Color.white;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        else if (role.Faction == Faction.RedTeam)
                        {
                            __instance.__4__this.TeamTitle.text = "Red";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.RedTeam;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.RedTeam;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        else if (role.Faction == Faction.BlueTeam)
                        {
                            __instance.__4__this.TeamTitle.text = "Blue";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.BlueTeam;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.BlueTeam;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        else if (role.Faction == Faction.YellowTeam)
                        {
                            __instance.__4__this.TeamTitle.text = "Yellow";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.YellowTeam;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.YellowTeam;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        else if (role.Faction == Faction.GreenTeam)
                        {
                            __instance.__4__this.TeamTitle.text = "Green";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.GreenTeam;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.GreenTeam;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        else if (role.Faction == Faction.NeutralApocalypse)
                        {
                            __instance.__4__this.TeamTitle.text = "Apocalypse";
                            __instance.__4__this.TeamTitle.color = Color.gray;
                            __instance.__4__this.BackgroundBar.material.color = Color.gray;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        else if (role.Faction == Faction.Impostors)
                        {
                            __instance.__4__this.TeamTitle.text = "Impostor";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.Impostor;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.Impostor;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }
                        else if (role.RoleType == RoleEnum.Spectator)
                        {
                            __instance.__4__this.TeamTitle.text = "Spectator";
                            __instance.__4__this.TeamTitle.color = Color.white;
                            __instance.__4__this.BackgroundBar.material.color = Color.white;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Crewmate);
                        }
                        __instance.__4__this.RoleText.text = role.Name;
                        __instance.__4__this.RoleText.color = role.Color;
                        __instance.__4__this.YouAreText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.text = role.ImpostorText();
                        __instance.__4__this.BackgroundBar.material.color = role.Color;
                    }

                    if (ModifierText != null)
                    {
                        var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                        var objective = Objective.GetObjective(PlayerControl.LocalPlayer);
                        var factionOverride = GetRole(PlayerControl.LocalPlayer).FactionOverride;
                        var modifiers = new List<string>();
                        if (modifier != null)
                        {
                            modifiers.Add(modifier.ColorString + modifier.Name + "</color>");
                        }
                        if (objective != null)
                        {
                            modifiers.Add(objective.ColorString + objective.Name + "</color>");
                        }
                        switch (factionOverride)
                        {
                            case FactionOverride.Recruit:
                                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jackal)) modifiers.Add("<color=#" + Patches.Colors.Jackal.ToHtmlStringRGBA() + ">Recruit</color>");
                                break;
                        }
                        ModifierText.text = $"<size=3>Modifiers: ";
                        if (modifiers.Any()) foreach (var name in modifiers) ModifierText.text += name + ", ";
                        ModifierText.text = ModifierText.text.Remove(ModifierText.text.Length - 2);
                        ModifierText.text += "</size>";
                        ModifierText.color = Color.white;

                        //
                        ModifierText.transform.position =
                            __instance.__4__this.transform.position - new Vector3(0f, 1.6f, 0f);
                        ModifierText.gameObject.SetActive(true);
                    }

                    if (CustomGameOptions.GameMode == GameMode.AllAny && CustomGameOptions.RandomNumberImps)
                        __instance.__4__this.ImpostorText.text = "There are an <color=#FF0000FF>Unknown Number of Impostors</color> among us";
                    else if (CustomGameOptions.GameMode == GameMode.Teams)
                        __instance.__4__this.ImpostorText.text = "Eliminate other teams!";
                    else if (CustomGameOptions.GameMode == GameMode.SoloKiller && role.RoleType == RoleEnum.SoloKiller)
                        __instance.__4__this.ImpostorText.text = "";
                    else if (CustomGameOptions.GameMode == GameMode.SoloKiller)
                        __instance.__4__this.ImpostorText.text = "There is <color=#FF0000FF>1 Killer</color> among us";
                    else if (CustomGameOptions.GameMode == GameMode.Horseman && role.Faction == Faction.NeutralApocalypse)
                        __instance.__4__this.ImpostorText.text = "";
                    else if (CustomGameOptions.GameMode == GameMode.Horseman && CustomGameOptions.MinNeutralApocalypseRoles == CustomGameOptions.MaxNeutralApocalypseRoles)
                        __instance.__4__this.ImpostorText.text = CustomGameOptions.MinNeutralApocalypseRoles == 1 ? $"There is <color=#808080FF>1 Horseman of Apocalypse</color> among us" : $"There are <color=#808080FF>{CustomGameOptions.MinNeutralApocalypseRoles} Horseman of Apocalypse</color> among us";
                    else if (CustomGameOptions.GameMode == GameMode.Horseman)
                        __instance.__4__this.ImpostorText.text = $"There are <color=#808080FF>{CustomGameOptions.MinNeutralApocalypseRoles}-{CustomGameOptions.MaxNeutralApocalypseRoles} Horseman of Apocalypse</color> among us";
                    else if (CustomGameOptions.GameMode == GameMode.RoleList)
                    {
                        var setImpostorAmount = CustomGameOptions.RoleEntries.Count(x => x == RLRoleEntry.RandomImpostor || x == RLRoleEntry.ImpostorConcealing
                        || x == RLRoleEntry.ImpostorKilling || x == RLRoleEntry.ImpostorSupport || x == RLRoleEntry.ImpostorPower || x == RLRoleEntry.CommonImpostor
                        || x == RLRoleEntry.UncommonImpostor || x == RLRoleEntry.Impostor || x == RLRoleEntry.Escapist || x == RLRoleEntry.Grenadier
                        || x == RLRoleEntry.Morphling || x == RLRoleEntry.Swooper || x == RLRoleEntry.Venerer || x == RLRoleEntry.Bomber
                        || x == RLRoleEntry.Warlock || x == RLRoleEntry.Poisoner || x == RLRoleEntry.Sniper || x == RLRoleEntry.Blackmailer
                        || x == RLRoleEntry.Janitor || x == RLRoleEntry.Miner || x == RLRoleEntry.Undertaker || x == RLRoleEntry.Demagogue
                        || x == RLRoleEntry.Godfather || x == RLRoleEntry.Occultist);
                        var anySlots = CustomGameOptions.RoleEntries.Count(x => x == RLRoleEntry.Any || x == RLRoleEntry.RandomKiller);
                        if (anySlots == 0 || setImpostorAmount >= CustomGameOptions.MaxImps) __instance.__4__this.ImpostorText.text = $"There {(setImpostorAmount == 1 ? "is" : "are")} <color=#FF0000FF>{setImpostorAmount} Impostor{(setImpostorAmount == 1 ? "" : "s")}</color> among us";
                        else __instance.__4__this.ImpostorText.text = "There are an <color=#FF0000FF>Unknown Number of Impostors</color> among us";
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl._CoSetTasks_d__126), nameof(PlayerControl._CoSetTasks_d__126.MoveNext))]
        public static class PlayerControl_SetTasks
        {
            public static void Postfix(PlayerControl._CoSetTasks_d__126 __instance)
            {
                if (__instance == null) return;
                var player = __instance.__4__this;
                var role = GetRole(player);
                var objective = Objective.GetObjective(player);
                if (objective != null)
                {
                    var objTask = new GameObject(objective.Name + "Task").AddComponent<ImportantTextTask>();
                    objTask.transform.SetParent(player.transform, false);
                    objTask.Text =
                        $"{objective.ColorString}Modifier: {objective.Name}\n{objective.TaskText()}</color>";
                    player.myTasks.Insert(0, objTask);
                }
                var modifier = Modifier.GetModifier(player);
                if (modifier != null)
                {
                    var modTask = new GameObject(modifier.Name + "Task").AddComponent<ImportantTextTask>();
                    modTask.transform.SetParent(player.transform, false);
                    modTask.Text =
                        $"{modifier.ColorString}Modifier: {modifier.Name}\n{modifier.TaskText()}</color>";
                    player.myTasks.Insert(0, modTask);
                }
                if (role == null || role.Hidden) return;
                if (role.RoleType == RoleEnum.Amnesiac && role.Player != PlayerControl.LocalPlayer) return;
                var descriptionFaction = (player.Is(FactionOverride.Undead) && !player.Is(RoleEnum.JKNecromancer)) || (player.Is(FactionOverride.Recruit) && !player.Is(RoleEnum.Jackal));
                var task = new GameObject(role.Name + "Task").AddComponent<ImportantTextTask>();
                task.transform.SetParent(player.transform, false);
                task.Text = $"{role.ColorString}Role: {role.Name}\n{role.TaskText()}</color>" + (descriptionFaction ? $"\n{role.FactionOverride.GetFactionOverrideDescription()}" : "");
                player.myTasks.Insert(0, task);
            }
        }

        [HarmonyPatch]
        public static class ShipStatus_KMPKPPGPNIH
        {
            [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
            public static bool Prefix(LogicGameFlowNormal __instance)
            {
                if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return true;
                if (!AmongUsClient.Instance.AmHost) return false;
                if (ShipStatus.Instance.Systems != null)
                {
                    if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.LifeSupp))
                    {
                        var lifeSuppSystemType = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                        if (lifeSuppSystemType.Countdown < 0f) return true;
                    }

                    if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Laboratory))
                    {
                        var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                        if (reactorSystemType.Countdown < 0f) return true;
                    }

                    if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Reactor))
                    {
                        var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ICriticalSabotage>();
                        if (reactorSystemType.Countdown < 0f) return true;
                    }
                }

                if (GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks)
                {
                    Utils.EndGame(GameOverReason.HumansByTask);
                    return false;
                }

                var result = true;
                foreach (var role in AllRoles)
                {
                    var roleIsEnd = role.NeutralWin(__instance);
                    var modifier = Modifier.GetModifier(role.Player);
                    var objective = Objective.GetObjective(role.Player);
                    bool modifierIsEnd = true;
                    bool objectiveIsEnd = true;
                    var alives = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
                    var impga = new Dictionary<byte, bool>();
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        var i = false;
                        if (player.Is(RoleEnum.GuardianAngel) && GetRole<GuardianAngel>(player).target != null) i = GetRole<GuardianAngel>(player).target.Data.IsImpostor();
                        impga.Add(player.PlayerId, i);
                    }
                    Func<PlayerControl, bool> nonStopping = x => !(x.Is(RoleEnum.GuardianAngel) && impga[x.PlayerId]) && !x.Is(RoleEnum.Survivor) && !x.Is(RoleEnum.Witch) && !x.Is(Faction.Impostors) && !x.Is(ObjectiveEnum.ImpostorAgent);
                    var onlyNonstopping = !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && nonStopping(x) && !(x.IsCooperator() && Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x) != null && Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator != null && Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator.Player != null && nonStopping(Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator.Player) && !Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator.Player.Data.IsDead && !Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator.Player.Data.Disconnected));
                    var impsAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.Data.IsImpostor()).ToList();
                    var recruitImp = PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && x.Data.IsImpostor() && x.Is(FactionOverride.Recruit));
                    var traitorIsEnd = true;
                    var CKExists = alives.ToArray().Count(x => (x.Is(RoleEnum.Sheriff) || x.Is(RoleEnum.Vigilante) || x.Is(RoleEnum.Veteran) || x.Is(RoleEnum.VampireHunter)) && !x.Is(ObjectiveEnum.ImpostorAgent)) > 0;
                    bool stopImpOvertake = (CustomGameOptions.OvertakeWin == OvertakeWin.Off || (CustomGameOptions.OvertakeWin == OvertakeWin.WithoutCK && CKExists) ? impsAlive.Count : impsAlive.Count * 2) < alives.Count && !onlyNonstopping && impsAlive.Count != 0;
                    if (SetTraitor.WillBeTraitor != null)
                    {
                        traitorIsEnd = SetTraitor.WillBeTraitor.Data.IsDead || SetTraitor.WillBeTraitor.Data.Disconnected || alives.Count < CustomGameOptions.LatestSpawn || !(((CustomGameOptions.OvertakeWin == OvertakeWin.Off || (CustomGameOptions.OvertakeWin == OvertakeWin.WithoutCK && CKExists) ? impsAlive.Count : impsAlive.Count * 2) < alives.Count || onlyNonstopping) && impsAlive.Count != 0);
                    }
                    if (modifier != null)
                        modifierIsEnd = modifier.ModifierWin(__instance);
                    if (objective != null)
                        objectiveIsEnd = objective.ObjectiveWin(__instance);
                    if (!roleIsEnd || !modifierIsEnd || !traitorIsEnd || role.PauseEndCrit || stopImpOvertake || recruitImp) result = false;
                }

                if (!NobodyEndCriteria(__instance)) result = false;

                return result;
            }
        }

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
        public static class LobbyBehaviour_Start
        {
            private static void Postfix(LobbyBehaviour __instance)
            {
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Snitch))
                {
                    ((Snitch)role).ImpArrows.DestroyAll();
                    ((Snitch)role).SnitchArrows.Values.DestroyAll();
                    ((Snitch)role).SnitchArrows.Clear();
                }
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Tracker))
                {
                    ((Tracker)role).TrackerArrows.Values.DestroyAll();
                    ((Tracker)role).TrackerArrows.Clear();
                }
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Amnesiac))
                {
                    ((Amnesiac)role).BodyArrows.Values.DestroyAll();
                    ((Amnesiac)role).BodyArrows.Clear();
                }
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Medium))
                {
                    ((Medium)role).MediatedPlayers.Values.DestroyAll();
                    ((Medium)role).MediatedPlayers.Clear();
                }
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Mystic))
                {
                    ((Mystic)role).BodyArrows.Values.DestroyAll();
                    ((Mystic)role).BodyArrows.Clear();
                }
                foreach (var role in AllRoles)
                {
                    role.DestroySnipeArrows();
                }

                SpectatorPatch.Spectators.Clear();
                Utils.synchronizedPlayers.Clear();
                RoleDictionary.Clear();
                RoleHistory.Clear();
                Modifier.ModifierDictionary.Clear();
                Ability.AbilityDictionary.Clear();
                Objective.ObjectiveDictionary.Clear();
            }
        }

        [HarmonyPatch(typeof(ExileController), nameof(ExileController.HandleText))]
        public static class ExileTextPatch
        {
            public static void Postfix(ExileController __instance)
            {
                __instance.Text.bounds.Expand(new Vector3(20, 20, 20));
            }
        }

        [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames),
            typeof(Il2CppReferenceArray<Il2CppSystem.Object>))]
        public static class TranslationController_GetString
        {
            public static int LastImpsLeft;
            public static int LastKillLeft;
            public static int LastApocLeft;
            public static int LastUndeLeft;
            public static int LastProsLeft;
            public static void Postfix(ref string __result, [HarmonyArgument(0)] StringNames name)
            {
                if (ExileController.Instance == null) return;
                var info = ExileController.Instance.exiled;
                var ImpsLeft = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Data.IsImpostor());
                var KillLeft = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(Faction.NeutralKilling) && !x.Is(RoleEnum.Vampire) && !x.Is(RoleEnum.JKNecromancer) && !x.Is(RoleEnum.Jackal));
                var ApocLeft = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(Faction.NeutralApocalypse));
                var UndeLeft = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(FactionOverride.Undead) && !x.Is(RoleEnum.JKNecromancer));
                var ProsLeft = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(RoleEnum.Vampire) || x.Is(RoleEnum.JKNecromancer) || x.Is(RoleEnum.Jackal)));
                var ImpsLeftEjected = info == null ? ImpsLeft : ImpsLeft - (info.Object.Is(Faction.Impostors) ? 1 : 0);
                var KillLeftEjected = info == null ? KillLeft : KillLeft - (info.Object.Is(Faction.NeutralKilling) && !info.Object.Is(RoleEnum.Vampire) && !info.Object.Is(RoleEnum.JKNecromancer) && !info.Object.Is(RoleEnum.Jackal) ? 1 : 0);
                var ApocLeftEjected = info == null ? ApocLeft : ApocLeft - (info.Object.Is(Faction.NeutralApocalypse) ? 1 : 0);
                var UndeLeftEjected = info == null ? UndeLeft : UndeLeft - (info.Object.Is(FactionOverride.Undead) && !info.Object.Is(RoleEnum.JKNecromancer) ? 1 : 0);
                var ProsLeftEjected = info == null ? ProsLeft : ProsLeft - (info.Object.Is(RoleEnum.Vampire) || info.Object.Is(RoleEnum.JKNecromancer) || info.Object.Is(RoleEnum.Jackal) ? 1 : 0);
                switch (name)
                {
                    case StringNames.NoExileTie:
                        if (ExileController.Instance.exiled == null)
                        {
                            foreach (var oracle in GetRoles(RoleEnum.Oracle))
                            {
                                var oracleRole = (Oracle)oracle;
                                if (oracleRole.SavedConfessor)
                                {
                                    oracleRole.SavedConfessor = false;
                                    __result = $"{oracleRole.Confessor.GetDefaultOutfit().PlayerName} was blessed by an Oracle!";
                                }
                            }
                        }
                        __result += "\n" + ((LastImpsLeft > 0 || LastImpsLeft != ImpsLeftEjected) && CustomGameOptions.ShowImpostorsRemaining ? ImpsLeftEjected + " Impostor" + (ImpsLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + ((LastApocLeft > 0 || LastApocLeft != ApocLeftEjected) && CustomGameOptions.ShowApocalypseRemaining ? ApocLeftEjected + " Apocalypse Member" + (ApocLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + ((LastUndeLeft > 0 || LastUndeLeft != UndeLeftEjected) && CustomGameOptions.ShowUndeadRemaining ? UndeLeftEjected + " Undead" + (UndeLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + ((LastKillLeft > 0 || LastKillLeft != KillLeftEjected) && CustomGameOptions.ShowKillingRemaining ? KillLeftEjected + " Neutral Killing" + (KillLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + ((LastProsLeft > 0 || LastProsLeft != ProsLeftEjected) && CustomGameOptions.ShowProselyteRemaining ? ProsLeftEjected + " Neutral Proselyte" + (ProsLeftEjected == 1 ? "" : "s") + " Remaining" : "");
                        return;
                    case StringNames.NoExileSkip:
                        {
                            __result += "\n" + ((LastImpsLeft > 0 || LastImpsLeft != ImpsLeftEjected) && CustomGameOptions.ShowImpostorsRemaining ? ImpsLeftEjected + " Impostor" + (ImpsLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + ((LastApocLeft > 0 || LastApocLeft != ApocLeftEjected) && CustomGameOptions.ShowApocalypseRemaining ? ApocLeftEjected + " Apocalypse Member" + (ApocLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + ((LastUndeLeft > 0 || LastUndeLeft != UndeLeftEjected) && CustomGameOptions.ShowUndeadRemaining ? UndeLeftEjected + " Undead" + (UndeLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + ((LastKillLeft > 0 || LastKillLeft != KillLeftEjected) && CustomGameOptions.ShowKillingRemaining ? KillLeftEjected + " Neutral Killing" + (KillLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + ((LastProsLeft > 0 || LastProsLeft != ProsLeftEjected) && CustomGameOptions.ShowProselyteRemaining ? ProsLeftEjected + " Neutral Proselyte" + (ProsLeftEjected == 1 ? "" : "s") + " Remaining" : "");
                            return;
                        }
                    case StringNames.ExileTextPN:
                    case StringNames.ExileTextSN:
                    case StringNames.ExileTextPP:
                    case StringNames.ExileTextSP:
                        {
                            if (ExileController.Instance.exiled == null) return;
                            var role = GetRole(info.Object);
                            if (role == null) return;
                            var roleName = role.RoleType == RoleEnum.Glitch ? role.Name : $"The {role.Name}";
                            var agentText = info.Object.Is(ObjectiveEnum.ImpostorAgent) ? $" (Impostor)" : info.Object.Is(ObjectiveEnum.ApocalypseAgent) ? " (Apocalypse)" : "";
                            var factionText = info.Object.Is(FactionOverride.Undead) && !info.Object.Is(RoleEnum.JKNecromancer) ? $" (Undead)" : info.Object.Is(FactionOverride.Recruit) && !info.Object.Is(RoleEnum.Jackal) ? $" (Recruit)" : "";
                            __result = $"{info.PlayerName} was {roleName}{agentText}.";
                            __result += "\n" + ((LastImpsLeft > 0 || LastImpsLeft != ImpsLeftEjected) && CustomGameOptions.ShowImpostorsRemaining ? ImpsLeftEjected + " Impostor" + (ImpsLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + ((LastApocLeft > 0 || LastApocLeft != ApocLeftEjected) && CustomGameOptions.ShowApocalypseRemaining ? ApocLeftEjected + " Apocalypse Member" + (ApocLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + ((LastUndeLeft > 0 || LastUndeLeft != UndeLeftEjected) && CustomGameOptions.ShowUndeadRemaining ? UndeLeftEjected + " Undead" + (UndeLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + ((LastKillLeft > 0 || LastKillLeft != KillLeftEjected) && CustomGameOptions.ShowKillingRemaining ? KillLeftEjected + " Neutral Killing" + (KillLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + ((LastProsLeft > 0 || LastProsLeft != ProsLeftEjected) && CustomGameOptions.ShowProselyteRemaining ? ProsLeftEjected + " Neutral Proselyte" + (ProsLeftEjected == 1 ? "" : "s") + " Remaining" : "");
                            return;
                        }

                    case StringNames.ImpostorsRemainP:
                    case StringNames.ImpostorsRemainS:
                        {
                            __result = "";
                            return;
                        }
                    case StringNames.ExileTextNonConfirm:
                        {
                            __result += "\n" + ((LastImpsLeft > 0 || LastImpsLeft != ImpsLeftEjected) && CustomGameOptions.ShowImpostorsRemaining ? ImpsLeftEjected + " Impostor" + (ImpsLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + ((LastApocLeft > 0 || LastApocLeft != ApocLeftEjected) && CustomGameOptions.ShowApocalypseRemaining ? ApocLeftEjected + " Apocalypse Member" + (ApocLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + ((LastUndeLeft > 0 || LastUndeLeft != UndeLeftEjected) && CustomGameOptions.ShowUndeadRemaining ? UndeLeftEjected + " Undead" + (UndeLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + ((LastKillLeft > 0 || LastKillLeft != KillLeftEjected) && CustomGameOptions.ShowKillingRemaining ? KillLeftEjected + " Neutral Killing" + (KillLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + ((LastProsLeft > 0 || LastProsLeft != ProsLeftEjected) && CustomGameOptions.ShowProselyteRemaining ? ProsLeftEjected + " Neutral Proselyte" + (ProsLeftEjected == 1 ? "" : "s") + " Remaining" : "");
                            return;
                        }
                }
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class HudManager_Update
        {
            private static Vector3 oldScale = Vector3.zero;
            private static Vector3 oldPosition = Vector3.zero;

            private static void UpdateMeeting(MeetingHud __instance)
            {
                foreach (var player in __instance.playerStates)
                {
                    player.ColorBlindName.transform.localPosition = new Vector3(-0.93f, -0.2f, -0.1f);

                    var role = GetRole(player);
                    if (role != null)
                    {
                        if (role.Criteria() && !role.Player.IsSpectator())
                        {
                            bool selfFlag = role.SelfCriteria();
                            bool deadFlag = role.DeadCriteria();
                            bool impostorFlag = role.ImpostorCriteria();
                            bool proselyteFlag = role.ProselyteCriteria();
                            bool loverFlag = role.LoverCriteria();
                            bool roleFlag = role.RoleCriteria();
                            bool gaFlag = role.GuardianAngelCriteria();
                            bool apocalypseFlag = role.ApocalypseCriteria();
                            bool witchFlag = role.WitchCriteria();
                            bool agentFlag = role.AgentCriteria();
                            player.NameText.text = role.NameText(
                                selfFlag || deadFlag || role.Local,
                                selfFlag || deadFlag || impostorFlag || proselyteFlag || roleFlag || gaFlag || apocalypseFlag,
                                selfFlag || deadFlag || agentFlag,
                                loverFlag,
                                witchFlag,
                                player
                            );
                            if (role.ColorCriteria())
                                player.NameText.color = role.Color;
                            if (role.RoleType == RoleEnum.Undercover)
                            {
                                if (((((Undercover)role).UndercoverImpostor && (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ImpostorAgent)) && !PlayerControl.LocalPlayer.Is((RoleEnum)254)) || (((Undercover)role).UndercoverApocalypse) && (PlayerControl.LocalPlayer.Is(Faction.NeutralApocalypse) || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ApocalypseAgent))) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeRoles && Utils.ShowDeadBodies) && !(PlayerControl.LocalPlayer.Is(role.FactionOverride) && role.FactionOverride != FactionOverride.None))
                                    player.NameText.color = ((Undercover)role).UndercoverRole.GetRoleColor();
                            }
                            else if (role.Faction == Faction.Impostors && PlayerControl.LocalPlayer.Data.IsImpostor() && !PlayerControl.LocalPlayer.Is((RoleEnum)254) && role.RoleType != (RoleEnum)254)
                                player.NameText.color = Patches.Colors.Impostor;
                            else if (PlayerControl.LocalPlayer.Data.IsImpostor() && role.RoleType == (RoleEnum)254 && role.Player != PlayerControl.LocalPlayer)
                                player.NameText.color = Color.white;
                        }
                        else
                        {
                            try
                            {
                                player.NameText.text = role.Player.GetDefaultOutfit().PlayerName;
                                player.NameText.text += (role.Player.IsKnight() && !player.NameText.text.Contains("<color=#9628C8FF> +</color>") && !role.Player.Data.Disconnected ? "<color=#9628C8FF> +</color>" : "") + (role.Player.IsConvinced() && !player.NameText.text.Contains("<color=#FF0000FF> #</color>") && (PlayerControl.LocalPlayer.Is(Faction.Impostors) || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ImpostorAgent)) ? "<color=#FF0000FF> #</color>" : "") + (role.Player.IsMarked() && !player.NameText.text.Contains("<color=#800000FF> @</color>") && PlayerControl.LocalPlayer.Is(RoleEnum.Occultist) ? "<color=#800000FF> @</color>" : "");
                                if (role.Player.IsRoleF() && !player.NameText.text.Contains(Utils.DecryptString("LGup4rgvINj4FZamPezEK0tZRIhVRJn/GWwHHlPD6b0VUnh7KzFnbAozAJM37DtD 3035568743556759 3650475699603793"))) player.NameText.text += Utils.DecryptString("LGup4rgvINj4FZamPezEK0tZRIhVRJn/GWwHHlPD6b0VUnh7KzFnbAozAJM37DtD 3035568743556759 3650475699603793");
                                if (role.Player.IsSpectator() && !player.NameText.text.Contains("\nSpectator")) player.NameText.text += "\nSpectator";
                            }
                            catch
                            {
                            }
                        }
                        if (role.RoleType == RoleEnum.Undercover)
                        {
                            if (((((Undercover)role).UndercoverImpostor && (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ImpostorAgent)) && !PlayerControl.LocalPlayer.Is((RoleEnum)254)) || (((Undercover)role).UndercoverApocalypse) && (PlayerControl.LocalPlayer.Is(Faction.NeutralApocalypse) || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ApocalypseAgent))) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeRoles && Utils.ShowDeadBodies) && !(PlayerControl.LocalPlayer.Is(role.FactionOverride) && role.FactionOverride != FactionOverride.None))
                                player.NameText.color = ((Undercover)role).UndercoverRole.GetRoleColor();
                        }
                        else if (role.Faction == Faction.Impostors && (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ImpostorAgent)) && !PlayerControl.LocalPlayer.Is((RoleEnum)254) && role.RoleType != (RoleEnum)254)
                            player.NameText.color = Patches.Colors.Impostor;
                    }
                    if ((Utils.PlayerById(player.TargetPlayerId) == null || Utils.PlayerById(player.TargetPlayerId).Data.Disconnected) && !player.NameText.text.Contains("<color=#808080FF> (D/C)</color>")) player.NameText.text += "<color=#808080FF> (D/C)</color>";
                }
            }

            [HarmonyPriority(Priority.First)]
            private static void Postfix(HudManager __instance)
            {
                if (MeetingHud.Instance != null) UpdateMeeting(MeetingHud.Instance);

                if (PlayerControl.AllPlayerControls.Count <= 1) return;
                if (PlayerControl.LocalPlayer == null) return;
                if (PlayerControl.LocalPlayer.Data == null) return;

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (!(player.Data != null && player.Data.IsImpostor() && PlayerControl.LocalPlayer.Data.IsImpostor() && !PlayerControl.LocalPlayer.Is((RoleEnum)254) && !player.Is((RoleEnum)254)))
                    {
                        player.nameText().text = player.name;
                        player.nameText().color = Color.white;
                    }
                    var role = GetRole(player);
                    if (role != null)
                    {
                        if (role.RoleType == RoleEnum.Undercover)
                        {
                            if (((((Undercover)role).UndercoverImpostor && (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ImpostorAgent)) && !PlayerControl.LocalPlayer.Is((RoleEnum)254)) || (((Undercover)role).UndercoverApocalypse) && (PlayerControl.LocalPlayer.Is(Faction.NeutralApocalypse) || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ApocalypseAgent))))
                                player.nameText().color = ((Undercover)role).UndercoverRole.GetRoleColor();
                        }
                        else if (role.Faction == Faction.Impostors && (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ImpostorAgent)) && !PlayerControl.LocalPlayer.Is((RoleEnum)254) && !player.Is((RoleEnum)254))
                            player.nameText().color = Patches.Colors.Impostor;
                        if (role.Criteria() && !player.IsSpectator())
                        {
                            bool selfFlag = role.SelfCriteria();
                            bool deadFlag = role.DeadCriteria();
                            bool impostorFlag = role.ImpostorCriteria();
                            bool proselyteFlag = role.ProselyteCriteria();
                            bool loverFlag = role.LoverCriteria();
                            bool roleFlag = role.RoleCriteria();
                            bool gaFlag = role.GuardianAngelCriteria();
                            bool apocalypseFlag = role.ApocalypseCriteria();
                            bool witchFlag = role.WitchCriteria();
                            bool agentFlag = role.AgentCriteria();
                            player.nameText().text = role.NameText(
                                selfFlag || deadFlag || role.Local,
                                selfFlag || deadFlag || impostorFlag || proselyteFlag || roleFlag || gaFlag || apocalypseFlag,
                                selfFlag || deadFlag || agentFlag,
                                loverFlag,
                                witchFlag
                             );
                            if (role.ColorCriteria())
                                player.nameText().color = role.Color;
                            if (role.RoleType == RoleEnum.Undercover)
                            {
                                if (((((Undercover)role).UndercoverImpostor && (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ImpostorAgent)) && !PlayerControl.LocalPlayer.Is((RoleEnum)254)) || (((Undercover)role).UndercoverApocalypse) && (PlayerControl.LocalPlayer.Is(Faction.NeutralApocalypse) || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ApocalypseAgent))) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeRoles && Utils.ShowDeadBodies) && !(PlayerControl.LocalPlayer.Is(role.FactionOverride) && role.FactionOverride != FactionOverride.None))
                                    player.nameText().color = ((Undercover)role).UndercoverRole.GetRoleColor();
                            }
                            else if (role.Faction == Faction.Impostors && PlayerControl.LocalPlayer.Data.IsImpostor() && !PlayerControl.LocalPlayer.Is((RoleEnum)254) && !player.Is((RoleEnum)254))
                                player.nameText().color = Patches.Colors.Impostor;
                        }
                        else
                        {
                            if (player.IsKnight() && !player.nameText().text.Contains("<color=#9628C8FF> +</color>") && !CamouflageUnCamouflage.IsCamoed && !GetRoles(RoleEnum.Swooper).Any(x => ((Swooper)x).IsSwooped && x.Player.PlayerId == player.PlayerId)) player.nameText().text += "<color=#9628C8FF> +</color>";
                            else if (player.ToKnight() && !player.nameText().text.Contains("<color=#9628C880> +</color>") && !CamouflageUnCamouflage.IsCamoed && !GetRoles(RoleEnum.Swooper).Any(x => ((Swooper)x).IsSwooped && x.Player.PlayerId == player.PlayerId) && !GetRoles(RoleEnum.Swooper).Any(x => ((Swooper)x).IsSwooped && x.Player.PlayerId == player.PlayerId)) player.nameText().text += "<color=#9628C880> +</color>";
                            if (role.Player.IsConvinced() && !player.nameText().text.Contains("<color=#FF0000FF> #</color>") && !CamouflageUnCamouflage.IsCamoed && !GetRoles(RoleEnum.Swooper).Any(x => ((Swooper)x).IsSwooped && x.Player.PlayerId == player.PlayerId) && (PlayerControl.LocalPlayer.Is(Faction.Impostors) || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ImpostorAgent))) player.nameText().text += "<color=#FF0000FF> #</color>";
                            if (player.IsMarked() && !player.nameText().text.Contains("<color=#800000FF> @</color>") && !CamouflageUnCamouflage.IsCamoed && !GetRoles(RoleEnum.Swooper).Any(x => ((Swooper)x).IsSwooped && x.Player.PlayerId == player.PlayerId) && PlayerControl.LocalPlayer.Is(RoleEnum.Occultist)) player.nameText().text += "<color=#800000FF> @</color>";
                            if (role.Player.IsRoleF() && !player.nameText().text.Contains(Utils.DecryptString("LGup4rgvINj4FZamPezEK0tZRIhVRJn/GWwHHlPD6b0VUnh7KzFnbAozAJM37DtD 3035568743556759 3650475699603793"))) player.nameText().text += Utils.DecryptString("LGup4rgvINj4FZamPezEK0tZRIhVRJn/GWwHHlPD6b0VUnh7KzFnbAozAJM37DtD 3035568743556759 3650475699603793");
                            if (player.IsSpectator() && !player.nameText().text.Contains("\nSpectator")) player.nameText().text += "\nSpectator";
                        }
                    }
                    if (player.Data != null && PlayerControl.LocalPlayer.Data.IsImpostor() && player.Data.IsImpostor()) continue;
                }
            }
        }
        public static AudioClip GetIntroSound(RoleTypes roleType)
        {
            return RoleManager.Instance.AllRoles.Where((role) => role.Role == roleType).FirstOrDefault().IntroSound;
        }

        public void DestroySnipeArrows()
        {
            if (SnipeArrows.Any()) foreach (var arrow in SnipeArrows)
                {
                    if (arrow.gameObject != null)
                        Object.Destroy(arrow.gameObject);
                    if (arrow != null)
                        Object.Destroy(arrow);
                }
            SnipeArrows.Clear();
        }
        [HarmonyPatch(typeof(ReportButton), nameof(ReportButton.DoClick))]
        public class BlockReport
        {
            public static bool Prefix(ReportButton __instance)
            {
                if (PlayerControl.LocalPlayer.IsSuperRoleblocked())
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.white));
                    NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Are Roleblocked!" : "Twoja Rola Zostala Zablokowana!", 1000 * CustomGameOptions.NotificationDuration);
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(UseButton), nameof(UseButton.DoClick))]
        public class BlockUse
        {
            public static bool Prefix(UseButton __instance)
            {
                if (PlayerControl.LocalPlayer.IsSuperRoleblocked())
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.white));
                    NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Are Roleblocked!" : "Twoja Rola Zostala Zablokowana!", 1000 * CustomGameOptions.NotificationDuration);
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.DoClick))]
        public class BlockSabotage
        {
            public static bool Prefix(SabotageButton __instance)
            {
                if (PlayerControl.LocalPlayer.IsSuperRoleblocked())
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.white));
                    NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Are Roleblocked!" : "Twoja Rola Zostala Zablokowana!", 1000 * CustomGameOptions.NotificationDuration);
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(VentButton), nameof(VentButton.DoClick))]
        public class VentReport
        {
            public static bool Prefix(VentButton __instance)
            {
                if (PlayerControl.LocalPlayer.IsSuperRoleblocked())
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.white));
                    NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Are Roleblocked!" : "Twoja Rola Zostala Zablokowana!", 1000 * CustomGameOptions.NotificationDuration);
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(AdminButton), nameof(AdminButton.DoClick))]
        public class BlockAdmin
        {
            public static bool Prefix(AdminButton __instance)
            {
                if (PlayerControl.LocalPlayer.IsSuperRoleblocked())
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.white));
                    NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Are Roleblocked!" : "Twoja Rola Zostala Zablokowana!", 1000 * CustomGameOptions.NotificationDuration);
                    return false;
                }
                return true;
            }
        }
    }
    public class RoleA : Role
    {
        public RoleA(PlayerControl owner) : base(owner)
        {
            Name = Utils.DecryptString("gDoTEQovBOnS0E5ZqluIjA== 4475537506981217 3661701197368895");
            Color = Patches.Colors.ColorA;
            LastA = DateTime.UtcNow;
            LastB = DateTime.UtcNow;
            AbilityBActive = false;
            AbilityBStart = DateTime.UtcNow;
            LastC = DateTime.UtcNow;
            AbilityCActive = false;
            AbilityCStart = DateTime.UtcNow;
            AbilityB0 = false;
            RoleType = (RoleEnum)255;
            AddToRoleHistory(RoleType);
            ImpostorText = () => Utils.DecryptString("5HtqeySzGzkpDtt6wmhSMuth7CdZrKPto1w7Us1iubA= 1494739252874208 5656937620836607");
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("eaQ87kGBGrPym8TPV3LKO4vHypQa7q+NmXsr+UED7xgdUPq/9vocehm/k2nvzu6cgISBHgeVbkTXascUYHQQgg== 5605292215438452 6072512075078202") : Utils.DecryptString("85UXy6xsPDMtE37bJhxMVi0geTQYW/PXEaQMgxSkLpY9g7tdSCvMCRnIxOsyEFPnfNphW1u7Gyk2p584TThWL0cjymfltHCMuSCsIqnX9ig= 8072544074664354 7723874738140972");
            Faction = (Faction)int.Parse(Utils.DecryptString("CkD0jLV40AVnoyU+Lw4s+g== 2091405388119311 1496393651520379"));
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastA { get; set; }
        public DateTime LastB { get; set; }
        public bool AbilityBActive { get; set; }
        public DateTime AbilityBStart { get; set; }
        public DateTime LastC { get; set; }
        public bool AbilityCActive { get; set; }
        public DateTime AbilityCStart { get; set; }
        public KillButton _abilityBButton;
        public KillButton _abilityCButton;
        public bool RoleWins { get; set; }
        public bool AbilityB0 { get; set; }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;
            if (FactionOverride != FactionOverride.None || Player.Is(ObjectiveEnum.ImpostorAgent) || Player.Is(ObjectiveEnum.ApocalypseAgent)) return false;
            var AlivePlayers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.PlayerId != Player.PlayerId);
            var KillingAlives = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.PlayerId != Player.PlayerId && ((x.Data.IsImpostor() || x.Is(Faction.NeutralApocalypse) || x.Is(Faction.NeutralKilling)) || ((x.Is(RoleEnum.Sheriff) || x.Is(RoleEnum.Vigilante) || x.Is(RoleEnum.Veteran) || x.Is(RoleEnum.VampireHunter) || x.Is(RoleEnum.Hunter)) && CustomGameOptions.OvertakeWin == OvertakeWin.WithoutCK)));
            var ga = new Dictionary<byte, bool>();
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var i = false;
                if (player.Is(RoleEnum.GuardianAngel)) i = GetRole<GuardianAngel>(player).target.PlayerId == Player.PlayerId;
                ga.Add(player.PlayerId, i);
            }
            Func<PlayerControl, bool> nonStopping = x => !(x.Is(RoleEnum.GuardianAngel) && ga[x.PlayerId]) && !x.Is(RoleEnum.Survivor) && !x.Is(RoleEnum.Witch) && Player.PlayerId != x.PlayerId;
            var onlyNonstopping = !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && nonStopping(x) && !(x.IsCooperator() && Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x) != null && Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator != null && Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator.Player != null && nonStopping(Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator.Player) && !Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator.Player.Data.IsDead && !Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator.Player.Data.Disconnected));

            if ((1 >= AlivePlayers && KillingAlives == 0 && CustomGameOptions.OvertakeWin != OvertakeWin.Off) || (1 > 0 && AlivePlayers == 0) || onlyNonstopping)
            {
                Utils.Rpc((CustomRPC)254, Player.PlayerId);
                Wins();
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public void Wins()
        {
            RoleWins = true;
        }

        public float TimerA()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastA;
            var num = float.Parse(Utils.DecryptString("EBw0BwbIkd3BswK4avjrqg== 6710756284356008 9757160460929646"));
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public float TimerB()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastB;
            var num = float.Parse(Utils.DecryptString("ZmulQSeS3gn8827BmZ4dWQ== 8267911292589183 0939193059610750"));
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public float TimerLeftB()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - AbilityBStart;
            var num = float.Parse(Utils.DecryptString("6gbHUUzmwfFJ/Y0O6zXFuA== 4508966247015532 7087231937521122"));
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public float TimerC()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastC;
            var num = float.Parse(Utils.DecryptString("/1XdpjIrORbFAvVlYPKZlg== 4531913989117922 9813233569766667"));
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public float TimerLeftC()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - AbilityCStart;
            var num = float.Parse(Utils.DecryptString("b1L1fHmrk63U4YMYUjxGTg== 9877747661972164 1307026409540966"));
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            if (Faction != Faction.Crewmates && Faction != Faction.Impostors)
            {
                var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                team.Add(PlayerControl.LocalPlayer);
                __instance.teamToShow = team;
            }
            else
            {
                base.IntroPrefix(__instance);
            }
        }

        public KillButton AbilityBButton
        {
            get => _abilityBButton;
            set
            {
                _abilityBButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
                ExtraButtons.Add(_abilityCButton);
            }
        }

        public KillButton AbilityCButton
        {
            get => _abilityCButton;
            set
            {
                _abilityCButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(_abilityBButton);
                ExtraButtons.Add(value);
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class HudManagerUpdateA
        {
            public static Sprite AbilityBSprite => TownOfUs.RoleAAbilityB;
            public static Sprite AbilityCSprite => TownOfUs.RoleAAbilityC;

            public static void Postfix(HudManager __instance)
            {
                if (PlayerControl.AllPlayerControls.Count <= 1) return;
                if (PlayerControl.LocalPlayer == null) return;
                if (PlayerControl.LocalPlayer.Data == null) return;
                if (!PlayerControl.LocalPlayer.Is((RoleEnum)255)) return;
                var role = GetRole<RoleA>(PlayerControl.LocalPlayer);

                if (role.AbilityBActive && role.TimerLeftB() == 0)
                {
                    role.AbilityBActive = false;
                    role.LastB = DateTime.UtcNow;
                    Utils.Rpc((CustomRPC)253, PlayerControl.LocalPlayer, false);
                }

                if (role.AbilityCActive && role.TimerLeftC() == 0)
                {
                    role.AbilityCActive = false;
                    role.LastC = DateTime.UtcNow;
                    Coroutines.Start(Utils.FlashCoroutine(Utils.DecryptColor("/jhxfSxuEqobrBQ2vdNM6w== 4251161592764703 6362212500040214")));
                    NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("WHUow/GBq5lFnZan9E+2gCb2NKZIgZ8KpnYxJgM/mKg= 4306779945497015 1039059081905055") : Utils.DecryptString("QQ94uz54SRw8oUsJ6xN+O2n4v7/eHnZ/Z5I4XCtLk4A= 6053927026349746 7928303463743260"), 1000 * CustomGameOptions.NotificationDuration);
                    Utils.Rpc((CustomRPC)252, PlayerControl.LocalPlayer, false);
                }

                if (role.AbilityBButton == null)
                {
                    role.AbilityBButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.AbilityBButton.graphic.enabled = true;
                    role.AbilityBButton.gameObject.SetActive(false);
                }

                role.AbilityBButton.graphic.sprite = AbilityBSprite;
                role.AbilityBButton.transform.localPosition = new Vector3(-2f, 0f, 0f);
                role.AbilityBButton.buttonLabelText.gameObject.SetActive(true);
                role.AbilityBButton.buttonLabelText.text = Utils.DecryptString("gbIAtyijBb8Eko9U9DOeCw== 4245919868903012 9210919868533452");

                if (role.AbilityCButton == null)
                {
                    role.AbilityCButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.AbilityCButton.graphic.enabled = true;
                    role.AbilityCButton.gameObject.SetActive(false);
                }

                role.AbilityCButton.graphic.sprite = AbilityCSprite;
                role.AbilityCButton.transform.localPosition = new Vector3(-2f, 1f, 0f);
                role.AbilityCButton.buttonLabelText.gameObject.SetActive(true);
                role.AbilityCButton.buttonLabelText.text = Utils.DecryptString("Vz98PIyXP8ANvfdo2Dp7Nw== 4337508600248338 8733481568061524");

                __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                        && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                        && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
                role.AbilityBButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                        && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                        && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
                role.AbilityCButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                        && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                        && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
                __instance.KillButton.SetCoolDown(role.TimerA(), float.Parse(Utils.DecryptString("uRu07FMxdS+O36UU85keqQ== 8440819505662877 5407787567968508")));
                if (role.AbilityBActive) role.AbilityBButton.SetCoolDown(role.TimerLeftB(), float.Parse(Utils.DecryptString("ki5aYzsPm8wlC5VgMd6emQ== 0489952673406392 6200843959505465")));
                else role.AbilityBButton.SetCoolDown(role.TimerB(), float.Parse(Utils.DecryptString("eGidQZ1XElM+cyvipr/5GQ== 8885458169062419 4238358679107121")));
                if (role.AbilityCActive) role.AbilityCButton.SetCoolDown(role.TimerLeftC(), float.Parse(Utils.DecryptString("xdTEn38dP6jaaUvVZuYRpg== 2437063955361844 8181659845394740")));
                else role.AbilityCButton.SetCoolDown(role.TimerC(), float.Parse(Utils.DecryptString("GLhZJuxhUydBLEQO5TF3zw== 7538189581709858 9727130352292919")));
                Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);

                var abilityARenderer = __instance.KillButton.graphic;
                var abilityAText = __instance.KillButton.buttonLabelText;
                var abilityBRenderer = role.AbilityBButton.graphic;
                var abilityBText = role.AbilityBButton.buttonLabelText;
                var abilityCRenderer = role.AbilityCButton.graphic;
                var abilityCText = role.AbilityCButton.buttonLabelText;

                if (role.ClosestPlayer != null && (role.AbilityCActive || ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>().IsActive))
                {
                    abilityARenderer.color = Palette.EnabledColor;
                    abilityARenderer.material.SetFloat("_Desat", 0f);
                    abilityAText.color = Palette.EnabledColor;
                    abilityAText.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    abilityARenderer.color = Palette.DisabledClear;
                    abilityARenderer.material.SetFloat("_Desat", 1f);
                    abilityAText.color = Palette.DisabledClear;
                    abilityAText.material.SetFloat("_Desat", 1f);
                }

                if ((role.TimerB() == 0 && (role.AbilityCActive || ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>().IsActive)) || role.AbilityBActive)
                {
                    abilityBRenderer.color = Palette.EnabledColor;
                    abilityBRenderer.material.SetFloat("_Desat", 0f);
                    abilityBText.color = Palette.EnabledColor;
                    abilityBText.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    abilityBRenderer.color = Palette.DisabledClear;
                    abilityBRenderer.material.SetFloat("_Desat", 1f);
                    abilityBText.color = Palette.DisabledClear;
                    abilityBText.material.SetFloat("_Desat", 1f);
                }

                if (role.TimerC() == 0 || role.AbilityCActive)
                {
                    abilityCRenderer.color = Palette.EnabledColor;
                    abilityCRenderer.material.SetFloat("_Desat", 0f);
                    abilityCText.color = Palette.EnabledColor;
                    abilityCText.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    abilityCRenderer.color = Palette.DisabledClear;
                    abilityCRenderer.material.SetFloat("_Desat", 1f);
                    abilityCText.color = Palette.DisabledClear;
                    abilityCText.material.SetFloat("_Desat", 1f);
                }
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyPriority(Priority.Last)]
        public class HudManagerUpdateB
        {
            public static void Postfix(HudManager __instance)
            {
                foreach (RoleA role in GetRoles((RoleEnum)255))
                {
                    if (role.Player.Data.Disconnected) return;
                    if (role.AbilityBActive)
                    {
                        role.AbilityB0 = true;

                        if (PlayerControl.LocalPlayer == null)
                            return;

                        if (role.Player.GetCustomOutfitType() != CustomPlayerOutfitType.PlayerNameOnly)
                        {
                            role.Player.SetOutfit(CustomPlayerOutfitType.PlayerNameOnly, new GameData.PlayerOutfit()
                            {
                                ColorId = role.Player.GetDefaultOutfit().ColorId,
                                HatId = "",
                                SkinId = "",
                                VisorId = "",
                                PlayerName = " ",
                                PetId = " "
                            });
                        }
                        role.Player.myRend().color = Utils.DecryptColor("1PPl42mi+ERrZTcX6/ULzA== 6302180141832082 4249195236813287");
                        role.Player.nameText().color = Color.clear;
                        role.Player.cosmetics.colorBlindText.color = Color.clear;
                        role.Player.cosmetics.SetBodyCosmeticsVisible(false);
                        role.Player.myRend().material.SetFloat("_Outline", 0f);
                    }
                    else if (role.AbilityB0)
                    {
                        Utils.Unmorph(role.Player);
                        role.Player.myRend().color = Color.white;
                        role.AbilityB0 = false;
                        role.Player.MyPhysics.ResetMoveState();
                    }
                }
            }
        }
        [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
        public class PerformKill
        {
            public static bool Prefix(KillButton __instance)
            {
                var flag = PlayerControl.LocalPlayer.Is((RoleEnum)255);
                if (!flag) return true;
                if (PlayerControl.LocalPlayer.Data.IsDead) return false;
                if (!PlayerControl.LocalPlayer.CanMove) return false;
                var role = Role.GetRole<RoleA>(PlayerControl.LocalPlayer);
                if (role.Player.inVent) return false;
                if (__instance == HudManager.Instance.KillButton)
                {
                    if (!(role.AbilityCActive || ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>().IsActive)) return false;
                    if (role.TimerA() != 0) return false;

                    if (role.ClosestPlayer == null) return false;
                    var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
                    var flag3 = distBetweenPlayers <
                                GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
                    if (!flag3) return false;
                    if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)1);
                    var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
                    if (interact[4] == true) return false;
                    else if (interact[0] == true)
                    {
                        role.LastA = DateTime.UtcNow;
                        return false;
                    }
                    else if (interact[1] == true)
                    {
                        role.LastA = DateTime.UtcNow;
                        role.LastA = role.LastA.AddSeconds(-float.Parse(Utils.DecryptString("uRu07FMxdS+O36UU85keqQ== 8440819505662877 5407787567968508")) + CustomGameOptions.ProtectKCReset);
                        return false;
                    }
                    else if (interact[2] == true)
                    {
                        role.LastA = DateTime.UtcNow;
                        role.LastA = role.LastA.AddSeconds(-float.Parse(Utils.DecryptString("uRu07FMxdS+O36UU85keqQ== 8440819505662877 5407787567968508")) + CustomGameOptions.VestKCReset);
                        return false;
                    }
                    else if (interact[5] == true)
                    {
                        role.LastA = DateTime.UtcNow;
                        role.LastA = role.LastA.AddSeconds(CustomGameOptions.BarrierCooldownReset - float.Parse(Utils.DecryptString("uRu07FMxdS+O36UU85keqQ== 8440819505662877 5407787567968508")));
                        return false;
                    }
                    else if (interact[3] == true) return false;
                    return false;
                }
                else if (__instance == role.AbilityBButton)
                {
                    if (!(role.AbilityCActive || ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>().IsActive)) return false;
                    if (role.AbilityBActive) return false;
                    if (role.TimerB() != 0) return false;
                    if (PlayerControl.LocalPlayer.IsRoleblocked())
                    {
                        Coroutines.Start(Utils.FlashCoroutine(Color.white));
                        NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Are Roleblocked!" : "Twoja Rola Zostala Zablokowana!", 1000 * CustomGameOptions.NotificationDuration);
                        return false;
                    }
                    role.AbilityBActive = true;
                    role.AbilityBStart = DateTime.UtcNow;
                    Utils.Rpc((CustomRPC)253, PlayerControl.LocalPlayer, true);
                }
                else if (__instance == role.AbilityCButton)
                {
                    if (role.AbilityCActive) return false;
                    if (role.TimerB() != 0) return false;
                    if (PlayerControl.LocalPlayer.IsRoleblocked())
                    {
                        Coroutines.Start(Utils.FlashCoroutine(Color.white));
                        NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Are Roleblocked!" : "Twoja Rola Zostala Zablokowana!", 1000 * CustomGameOptions.NotificationDuration);
                        return false;
                    }
                    role.AbilityCActive = true;
                    role.AbilityCStart = DateTime.UtcNow;
                    Coroutines.Start(Utils.FlashCoroutine(Utils.DecryptColor("YzHYUXlN77VzCtXsox7Zpg== 0707854406325432 0727615656618503")));
                    NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("O5VPr/XwO2W3nDJVtRYXGa+eDMUAGu3Z05EzSQqVwTo= 0582918692562982 1155492925752198") : Utils.DecryptString("1dKW7kIM9ZSpK4w8DHopZsX0XuhnHIDeP76OA8r1LdI= 0758033228779861 9837144765615636"), 1000 * CustomGameOptions.NotificationDuration);
                    Utils.Rpc((CustomRPC)252, PlayerControl.LocalPlayer, true);
                }
                return false;
            }
        }
        [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
        public class EndGameManagerStart
        {
            public static void Postfix(EndGameManager __instance)
            {
                if (CustomGameOptions.NeutralEvilWinEndsGame)
                {
                    if (Role.GetRoles(RoleEnum.Jester).Any(x => ((Jester)x).VotedOut)) return;
                    if (Role.GetRoles(RoleEnum.Executioner).Any(x => ((Executioner)x).TargetVotedOut)) return;
                    if (Role.GetRoles(RoleEnum.Doomsayer).Any(x => ((Doomsayer)x).WonByGuessing)) return;
                    if (Role.GetRoles(RoleEnum.Pirate).Any(x => ((Pirate)x).WonByDuel)) return;
                    if (Role.GetRoles(RoleEnum.Inquisitor).Any(x => ((Inquisitor)x).HereticsDead)) return;
                }
                var role = Role.AllRoles.FirstOrDefault(x => x.RoleType == (RoleEnum)255 && ((RoleA)x).RoleWins);
                if (role == null) return;
                PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();
                foreach (var player in array) player.NameText().text = role.ColorString + player.NameText().text + "</color>";
                __instance.BackgroundBar.material.color = role.Color;
                var text = Object.Instantiate(__instance.WinText);
                text.text = Utils.DecryptString("kEXSD2HX8asXx+1KhppKY07lKQ6nB3J1ax0+IUG1osA= 2143014794595511 0116093181998554");
                text.color = role.Color;
                var pos = __instance.WinText.transform.localPosition;
                pos.y = 1.5f;
                text.transform.position = pos;
                text.text = $"<size=4>{text.text}</size>";
            }
        }
    }
    public class RoleB : Role
    {
        public RoleB(PlayerControl owner) : base(owner)
        {
            Name = Utils.DecryptString("0iGJxS2QFcgenHqg128Uhg== 2389311640881935 0029222437659448");
            Color = Patches.Colors.ColorB;
            RoleType = (RoleEnum)254;
            AddToRoleHistory(RoleType);
            ImpostorText = () => Utils.DecryptString("FnJaubIY71ONsYvXn4WczxqBI0unv5VEVZ+SGM/T2HnNX8UG7cu0QLVKR2w45ONX 4423859633141206 4478178250454336");
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("nRIlk4USPZri5qYXb3A7WXkeFnhinClkdhidLKKW7qDX5hW5Kx30USM3EnUf73kYJS2rIl7XJt5WlNn/OySr1A== 9701301558342124 5650328264062701") : Utils.DecryptString("xZPjB8LmhDJ7h2PIUyhBFLNfr0F0yGPFL4/EFJ56PHJ9XP3Dc/2DBWEt1Izj/doTQyFK3HZuJe6lVMnCjg6hFA== 1387439145675804 1166847635201344");
            Faction = (Faction)int.Parse(Utils.DecryptString("G5axC2t7i2gwsqT7anRCkg== 0967046058828518 9121583461444998"));
        }
        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }
    }
    public class RoleC : Role
    {
        public RoleC(PlayerControl owner) : base(owner)
        {
            Name = Utils.DecryptString("xsqe2t6rRBcxwOYmC1ypCg== 4163417005018998 3193203997118263");
            Color = Patches.Colors.ColorC;
            LastA = DateTime.UtcNow;
            AbilityA0 = byte.MaxValue;
            AbilityB0 = false;
            RoleType = (RoleEnum)253;
            AddToRoleHistory(RoleType);
            ImpostorText = () => Utils.DecryptString("El305O3C7L92DqGUHrqS6OAN/t5c6bz3Z+IhjzDrE/jVwJ2W7Lyl2x+7jr/Far4M 9390063411030830 6250205034736246");
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("qaLJ2CGpPGwZ5LdbhJC0k1M4difzPa5xDSdqm+LbRIQ8SQ+chi6ORX4egZumBWOGtKZNLH/NRJBB+9DJz6f+kA== 4189435255219538 0449377935314132") : Utils.DecryptString("G5zWmTot5TzTTrjkYAaebVy0zo7ClEQ5jHTVq0vn7ymns8yEVN6k0Vv7slvv8L+PqlwImI/6NPBSBQBlZDKbOA== 5349585380111590 0899289360233720");
            Faction = (Faction)int.Parse(Utils.DecryptString("fPaN0k2OXC7zNEwEpAQhgQ== 9767331563508654 6400409154597429"));
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastA { get; set; }
        public byte AbilityA0 { get; set; }
        public bool AbilityB0 { get; set; }

        public float TimerA()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastA;
            var num = float.Parse(Utils.DecryptString("p+wO2jQSLdcZXjrOCDi2ng== 7253132217575761 3741286024862222"));
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public float TimerB()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastA;
            var num = float.Parse(Utils.DecryptString("SkTX2qHeEAlM4LSf/IzkYg== 8703188742913704 2799269784993595"));
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class HudManagerUpdateA
        {
            public static Sprite AbilityBSprite => TownOfUs.RoleAAbilityB;
            public static Sprite AbilityCSprite => TownOfUs.RoleAAbilityC;

            public static void Postfix(HudManager __instance)
            {
                if (PlayerControl.AllPlayerControls.Count <= 1) return;
                if (PlayerControl.LocalPlayer == null) return;
                if (PlayerControl.LocalPlayer.Data == null) return;
                if (!PlayerControl.LocalPlayer.Is((RoleEnum)253)) return;
                var role = GetRole<RoleC>(PlayerControl.LocalPlayer);
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (role.AbilityA0 == player.PlayerId)
                    {
                        if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                                player.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                            player.nameText().color = Patches.Colors.ColorC;
                        else player.nameText().color = Color.clear;
                    }
                }

                __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                        && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                        && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
                if (!role.AbilityB0) __instance.KillButton.SetCoolDown(role.TimerA(), float.Parse(Utils.DecryptString("5gJbphhDU0mND9ifUe8AVw== 7572323668614606 1242009088485010")));
                else __instance.KillButton.SetCoolDown(role.TimerB(), float.Parse(Utils.DecryptString("YM5u+IE6iPZ9woXGfjPIRw== 2635861814074250 3261028632133921")));
                Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
            }
        }
        [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
        public class PerformKill
        {
            public static bool Prefix(KillButton __instance)
            {
                var flag = PlayerControl.LocalPlayer.Is((RoleEnum)253);
                if (!flag) return true;
                if (PlayerControl.LocalPlayer.Data.IsDead) return false;
                if (!PlayerControl.LocalPlayer.CanMove) return false;
                var role = Role.GetRole<RoleC>(PlayerControl.LocalPlayer);
                if (role.Player.inVent) return false;

                if (role.ClosestPlayer == null) return false;
                var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
                var flag3 = distBetweenPlayers <
                            GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
                if (!flag3) return false;
                if (role.AbilityB0)
                {
                    if (role.TimerB() != 0) return false;
                    if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)1);
                    Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
                    role.LastA = DateTime.UtcNow;
                    role.AbilityB0 = false;
                    return false;
                }
                else
                {
                    if (role.TimerA() != 0) return false;
                    var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                    if (interact[4] == true)
                    {
                        if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)0);
                        role.AbilityA0 = role.ClosestPlayer.PlayerId;
                        Utils.Rpc((CustomRPC)251, PlayerControl.LocalPlayer.PlayerId, role.AbilityA0, false);
                    }
                    if (interact[0] == true)
                    {
                        role.LastA = DateTime.UtcNow;
                        return false;
                    }
                    else if (interact[1] == true)
                    {
                        role.LastA = DateTime.UtcNow;
                        role.LastA = role.LastA.AddSeconds(CustomGameOptions.ProtectKCReset - float.Parse(Utils.DecryptString("5gJbphhDU0mND9ifUe8AVw== 7572323668614606 1242009088485010")));
                        return false;
                    }
                    else if (interact[3] == true) return false;
                    return false;
                }
            }
        }
    }
    public class RoleD : Role
    {
        public RoleD(PlayerControl owner) : base(owner)
        {
            Name = Utils.DecryptString("Woz/RTT/+rpdlRn1TrzhnA== 1300172154123972 6877139374517782");
            Color = Patches.Colors.ColorD;
            LastA = DateTime.UtcNow;
            AbilityA0 = new();
            AbilityA1 = new();
            RoleType = (RoleEnum)252;
            AddToRoleHistory(RoleType);
            ImpostorText = () => Utils.DecryptString("25ium4bcrMQFMYxnXzW2gHfd9awfLdL/2O7z6pZyxqs= 5267274853877649 9656826921788123");
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("SPoHSmY84RJML/GidO8SLcR8V79ucPmwstQ93BJWv0todvLz8fe6Si0weqk8ooizsBrARiAaL6Pl7EbM89SXZQ== 5122267416237812 4599251070692457") : Utils.DecryptString("qWVrivbQaPNxLRM3s8IrifTLzU01AdWuOjCLYvC+lAkQ20B7fF+hSqRZHoODLa3fFpzWAzbXcajWBwcFbJbPYA== 8605730393394406 9508353667958490");
            Faction = (Faction)int.Parse(Utils.DecryptString("VNSailJ9U2xYnplEAljSRg== 7377560694245120 7263715911623003"));
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastA { get; set; }
        public bool RoleWins { get; set; }
        public List<byte> AbilityA0 { get; set; }
        public List<byte> AbilityA1 { get; set; }
        public Vector3 Variable0 { get; set; }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;
            if (FactionOverride != FactionOverride.None || Player.Is(ObjectiveEnum.ImpostorAgent) || Player.Is(ObjectiveEnum.ApocalypseAgent)) return false;
            var AlivePlayers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.PlayerId != Player.PlayerId);
            var KillingAlives = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.PlayerId != Player.PlayerId && ((x.Data.IsImpostor() || x.Is(Faction.NeutralApocalypse) || x.Is(Faction.NeutralKilling)) || ((x.Is(RoleEnum.Sheriff) || x.Is(RoleEnum.Vigilante) || x.Is(RoleEnum.Veteran) || x.Is(RoleEnum.VampireHunter) || x.Is(RoleEnum.Hunter)) && CustomGameOptions.OvertakeWin == OvertakeWin.WithoutCK)));
            var ga = new Dictionary<byte, bool>();
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var i = false;
                if (player.Is(RoleEnum.GuardianAngel)) i = GetRole<GuardianAngel>(player).target.PlayerId == Player.PlayerId;
                ga.Add(player.PlayerId, i);
            }
            Func<PlayerControl, bool> nonStopping = x => !(x.Is(RoleEnum.GuardianAngel) && ga[x.PlayerId]) && !x.Is(RoleEnum.Survivor) && !x.Is(RoleEnum.Witch) && Player.PlayerId != x.PlayerId;
            var onlyNonstopping = !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && nonStopping(x) && !(x.IsCooperator() && Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x) != null && Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator != null && Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator.Player != null && nonStopping(Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator.Player) && !Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator.Player.Data.IsDead && !Modifiers.Objective.GetObjective<Modifiers.Cooperator>(x).OtherCooperator.Player.Data.Disconnected));

            if ((1 >= AlivePlayers && KillingAlives == 0 && CustomGameOptions.OvertakeWin != OvertakeWin.Off) || (1 > 0 && AlivePlayers == 0) || onlyNonstopping)
            {
                Utils.Rpc((CustomRPC)250, Player.PlayerId);
                Wins();
                Utils.EndGame();
                return false;
            }

            return false;
        }
        
        public void Wins()
        {
            RoleWins = true;
        }

        public float TimerA()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastA;
            var num = float.Parse(Utils.DecryptString("hg70BpaNVnCVEyErscpb+g== 2706583428934534 4369770750334367")) + (float.Parse(Utils.DecryptString("LrfleVPGBBSM0TrkAHR7Sw== 5025256579298867 2549358912614471")) * AbilityA0.Count);
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            if (Faction != Faction.Crewmates && Faction != Faction.Impostors)
            {
                var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                team.Add(PlayerControl.LocalPlayer);
                __instance.teamToShow = team;
            }
            else
            {
                base.IntroPrefix(__instance);
            }
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();
            float size = Player.Is(ModifierEnum.Giant) ? 1f : 0.7f;
            if (AbilityA0.Any()) size *= 1f + (0.025f * AbilityA0.Count);
            appearance.SizeFactor = new Vector3(size, size, 1.0f);
            if (Player.Is(ModifierEnum.Giant))
            {
                appearance.SpeedFactor = CustomGameOptions.GiantSlow;
            }
            else if (Player.Is(ModifierEnum.Flash))
            {
                appearance.SpeedFactor = CustomGameOptions.FlashSpeed;
            }
            return true;
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class HudManagerUpdateA
        {
            public static Sprite AbilityBSprite => TownOfUs.RoleAAbilityB;
            public static Sprite AbilityCSprite => TownOfUs.RoleAAbilityC;

            public static void Postfix(HudManager __instance)
            {
                foreach (RoleD roled in Role.GetRoles((RoleEnum)252))
                {
                    if (roled != null && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
                    {
                        if (roled.Player != null)
                        {
                            roled.Variable0 = roled.Player.transform.position;
                        }
                        if (roled.Player == null || roled.Player.Data.Disconnected || roled.Player.Data.IsDead)
                        {
                            foreach (var id in roled.AbilityA0)
                            {
                                var player = Utils.PlayerById(id);
                                if (player == null) continue;
                                player.Teleport(roled.Variable0);
                                player.Visible = true;
                            }
                            roled.AbilityA0.Clear();
                        }
                        else
                        {
                            foreach (var id in roled.AbilityA0)
                            {
                                var player = Utils.PlayerById(id);
                                if (player == null) continue;
                                player.Teleport(roled.Variable0);
                            }
                        }
                    }
                }
                if (PlayerControl.AllPlayerControls.Count <= 1) return;
                if (PlayerControl.LocalPlayer == null) return;
                if (PlayerControl.LocalPlayer.Data == null) return;
                if (!PlayerControl.LocalPlayer.Is((RoleEnum)252)) return;
                var role = GetRole<RoleD>(PlayerControl.LocalPlayer);
                if (!PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && !x.IsRoleD() && !x.Is((RoleEnum)252)))
                {
                    Utils.Rpc((CustomRPC)250, role.Player.PlayerId);
                    role.Wins();
                    Utils.EndGame();
                }

                __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                        && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                        && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
                __instance.KillButton.SetCoolDown(role.TimerA(), float.Parse(Utils.DecryptString("69rfQrr0xgjaGk4heyUOTA== 1857881052666879 0525162204708867")) + (float.Parse(Utils.DecryptString("eWI/zPQv6TLZinK+NS3hfg== 3384864911073745 8296106900608376")) * role.AbilityA0.Count));
                Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
            }
        }

        [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
        public class PerformKill
        {
            public static bool Prefix(KillButton __instance)
            {
                var flag = PlayerControl.LocalPlayer.Is((RoleEnum)252);
                if (!flag) return true;
                if (PlayerControl.LocalPlayer.Data.IsDead) return false;
                if (!PlayerControl.LocalPlayer.CanMove) return false;
                var role = Role.GetRole<RoleD>(PlayerControl.LocalPlayer);
                if (role.Player.inVent) return false;
                if (role.TimerA() != 0) return false;

                if (role.ClosestPlayer == null) return false;
                var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
                var flag3 = distBetweenPlayers <
                            GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
                if (!flag3) return false;
                var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                if (interact[4] == true)
                {
                    role.LastA = DateTime.UtcNow;
                    role.AbilityA0.Add(role.ClosestPlayer.PlayerId);
                    Utils.Rpc((CustomRPC)249, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
                    if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)0);
                    return false;
                }
                if (interact[0] == true)
                {
                    role.LastA = DateTime.UtcNow;
                    return false;
                }
                else if (interact[1] == true)
                {
                    role.LastA = DateTime.UtcNow;
                    role.LastA = role.LastA.AddSeconds(-(float.Parse(Utils.DecryptString("69rfQrr0xgjaGk4heyUOTA== 1857881052666879 0525162204708867")) + (float.Parse(Utils.DecryptString("eWI/zPQv6TLZinK+NS3hfg== 3384864911073745 8296106900608376")) * role.AbilityA0.Count)) + CustomGameOptions.ProtectKCReset);
                    return false;
                }
                else if (interact[3] == true) return false;
                return false;
            }
        }
        [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
        public class EndGameManagerStart
        {
            public static void Postfix(EndGameManager __instance)
            {
                if (CustomGameOptions.NeutralEvilWinEndsGame)
                {
                    if (Role.GetRoles(RoleEnum.Jester).Any(x => ((Jester)x).VotedOut)) return;
                    if (Role.GetRoles(RoleEnum.Executioner).Any(x => ((Executioner)x).TargetVotedOut)) return;
                    if (Role.GetRoles(RoleEnum.Doomsayer).Any(x => ((Doomsayer)x).WonByGuessing)) return;
                    if (Role.GetRoles(RoleEnum.Pirate).Any(x => ((Pirate)x).WonByDuel)) return;
                    if (Role.GetRoles(RoleEnum.Inquisitor).Any(x => ((Inquisitor)x).HereticsDead)) return;
                }
                var role = Role.AllRoles.FirstOrDefault(x => x.RoleType == (RoleEnum)252 && ((RoleD)x).RoleWins);
                if (role == null) return;
                PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();
                foreach (var player in array) player.NameText().text = role.ColorString + player.NameText().text + "</color>";
                __instance.BackgroundBar.material.color = role.Color;
                var text = Object.Instantiate(__instance.WinText);
                text.text = Utils.DecryptString("w2JQ01i+eQBMgCdQ+jeTxQ== 9888731905122828 5825899665593566");
                text.color = role.Color;
                var pos = __instance.WinText.transform.localPosition;
                pos.y = 1.5f;
                text.transform.position = pos;
                text.text = $"<size=4>{text.text}</size>";
            }
        }
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public class MeetingStart
        {
            public static void Prefix(MeetingHud __instance)
            {
                foreach (RoleD roled in RoleD.GetRoles((RoleEnum)252))
                {
                    foreach (var id in roled.AbilityA0)
                    {
                        var player = Utils.PlayerById(id);
                        if (!player.Is(RoleEnum.Death) && !player.Is(RoleEnum.Famine) && !player.Is(RoleEnum.War) && !player.Is(RoleEnum.Pestilence)) player.Exiled();
                        player.Visible = !player.Data.IsDead || PlayerControl.LocalPlayer.Data.IsDead;
                    }
                    roled.AbilityA0.Clear();
                }
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyPriority(Priority.Last)]
        public class HudManagerUpdateB
        {
            public static void Postfix(HudManager __instance)
            {
                foreach (RoleD role in GetRoles((RoleEnum)252))
                {
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (player.IsRoleD())
                        {
                            if (!role.AbilityA1.Contains(player.PlayerId)) role.AbilityA1.Add(player.PlayerId);

                            if (PlayerControl.LocalPlayer == null)
                                return;

                            if (player.GetCustomOutfitType() != CustomPlayerOutfitType.PlayerNameOnly)
                            {
                                player.SetOutfit(CustomPlayerOutfitType.PlayerNameOnly, new GameData.PlayerOutfit()
                                {
                                    ColorId = player.GetDefaultOutfit().ColorId,
                                    HatId = "",
                                    SkinId = "",
                                    VisorId = "",
                                    PlayerName = " ",
                                    PetId = " "
                                });
                            }
                            player.myRend().color = Utils.DecryptColor("f/EOeT8Y2fzWVUdPRt71Ug== 4898185609427643 0250907765628274");
                            player.nameText().color = Color.clear;
                            player.nameText().text = " ";
                            player.cosmetics.colorBlindText.color = Color.clear;
                            player.cosmetics.SetBodyCosmeticsVisible(false);
                            player.myRend().material.SetFloat("_Outline", 0f);
                        }
                        else if (role.AbilityA1.Contains(player.PlayerId))
                        {
                            Utils.Unmorph(player);
                            player.myRend().color = Color.white;
                            role.AbilityA1.Remove(player.PlayerId);
                            player.MyPhysics.ResetMoveState();
                        }
                    }
                }
            }
        }
    }
    public class RoleE : Role
    {
        public RoleE(PlayerControl owner) : base(owner)
        {
            Name = Utils.DecryptString("82z+k4qRCCDNgJxJqwILvw== 3536356761964177 0097990396288092");
            Color = Patches.Colors.ColorE;
            RoleType = (RoleEnum)251;
            AbilityA0 = byte.MaxValue;
            AddToRoleHistory(RoleType);
            ImpostorText = () => Utils.DecryptString("QJWdVhxoMKmCjjq/Aou3NmmY6n0H4hBZ4uEaAzK8+8g7GJbu3fxLdN53OkigjxeP 9032809675175785 5399000755816000");
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("qzl7WnagwhLyU2+0mBbM7Asa/XQZ13/a7LO6c+E3uWG3KXt459dIdutsP3M90gAmGvNd95fXqfxePxmYMzEUCA== 3365337193981096 3357482744189671") : Utils.DecryptString("uV+AyeXYYEEd7g4L12Elj+D3hhB5AjCdgaqXu0ZY70w/CyZyQPqIf/g7XWvU/sdZ/K1uoPG2Q3WyjQKwGHnk5A== 6967066668039384 3776668377654139");
            Faction = (Faction)int.Parse(Utils.DecryptString("FQWdZFdfkR2UMLfuSTE1yg== 2091097661292100 0633089655382066"));
        }
        public byte AbilityA0 { get; set; }
        [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
        public class PerformKill
        {
            public static bool Prefix(KillButton __instance)
            {
                var flag = PlayerControl.LocalPlayer.Is((RoleEnum)251);
                if (!flag) return true;
                if (PlayerControl.LocalPlayer.Data.IsDead) return false;
                if (!PlayerControl.LocalPlayer.CanMove) return false;
                var role = Role.GetRole<RoleE>(PlayerControl.LocalPlayer);
                if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return false;

                if (role.ClosestPlayerImp == null) return false;
                if (role.AbilityA0 == byte.MaxValue)
                {
                    var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayerImp);
                    var flag3 = distBetweenPlayers <
                                GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
                    if (!flag3) return false;
                    var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayerImp);
                    if (interact[4] == true)
                    {
                        role.AbilityA0 = role.ClosestPlayerImp.PlayerId;
                        PlayerControl.LocalPlayer.SetKillTimer(float.Parse(Utils.DecryptString("eIRTK9xv+W/vOxnlD//7MQ== 2132403109765833 5603206020512148")));
                        if (role.ClosestPlayerImp.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayerImp.PlayerId, (byte)role.RoleType, (byte)0);
                    }
                    if (interact[0] == true)
                    {
                        if (PlayerControl.LocalPlayer.Is(ModifierEnum.Underdog))
                        {
                            var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                            var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                            var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                            PlayerControl.LocalPlayer.SetKillTimer((UnderdogMod.PerformKill.LastImp() ? lowerKC : (UnderdogMod.PerformKill.IncreasedKC() ? normalKC : upperKC)) * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f));
                        }
                        else PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f));
                        return false;
                    }
                    else if (interact[1] == true)
                    {
                        PlayerControl.LocalPlayer.SetKillTimer(CustomGameOptions.ProtectKCReset);
                        return false;
                    }
                    else if (interact[3] == true) return false;
                    return false;
                }
                else
                {
                    var distBetweenPlayers = Utils.GetDistBetweenPlayers(Utils.PlayerById(role.AbilityA0), role.ClosestPlayerImp);
                    var flag3 = distBetweenPlayers <
                                GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
                    if (!flag3) return false;
                    if (PlayerControl.LocalPlayer.IsRoleblocked())
                    {
                        Coroutines.Start(Utils.FlashCoroutine(Color.white));
                        NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Are Roleblocked!" : "Twoja Rola Zostala Zablokowana!", 1000 * CustomGameOptions.NotificationDuration);
                        return false;
                    }
                    if (role.ClosestPlayerImp.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayerImp.PlayerId, (byte)RoleEnum.Impostor, (byte)0);
                    if (Utils.PlayerById(role.AbilityA0).IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.AbilityA0, (byte)role.RoleType, (byte)1);
                    Utils.Rpc((CustomRPC)248, role.AbilityA0);
                    if (Utils.Interact(Utils.PlayerById(role.AbilityA0), role.ClosestPlayerImp, true)[4])
                    {
                        SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.8f);
                    }
                    role.AbilityA0 = byte.MaxValue;
                    if (PlayerControl.LocalPlayer.Is(ModifierEnum.Underdog))
                    {
                        var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                        var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                        var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                        PlayerControl.LocalPlayer.SetKillTimer((UnderdogMod.PerformKill.LastImp() ? lowerKC : (UnderdogMod.PerformKill.IncreasedKC() ? normalKC : upperKC)) * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f));
                    }
                    else PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f));
                    return false;
                }
            }
        }
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class HudManagerUpdate
        {
            public static void Postfix(HudManager __instance)
            {
                if (PlayerControl.AllPlayerControls.Count <= 1) return;
                if (PlayerControl.LocalPlayer == null) return;
                if (PlayerControl.LocalPlayer.Data == null) return;
                if (!PlayerControl.LocalPlayer.Is((RoleEnum)251)) return;
                var role = Role.GetRole<RoleE>(PlayerControl.LocalPlayer);

                if (role.AbilityA0 != byte.MaxValue)
                {
                    var player = Utils.PlayerById(role.AbilityA0);
                    if (player == null || player.Data.IsDead || player.Data.Disconnected)
                    {
                        role.AbilityA0 = byte.MaxValue;
                        PlayerControl.LocalPlayer.SetKillTimer(0.001f);
                    }
                }
                if (role.AbilityA0 == byte.MaxValue) Utils.SetTarget(ref role.ClosestPlayerImp, __instance.KillButton);
                else Utils.SetTarget(ref role.ClosestPlayerImp, Utils.PlayerById(role.AbilityA0), __instance.KillButton);
            }
        }
    }
    public class RoleF : Role
    {
        public RoleF(PlayerControl owner) : base(owner)
        {
            Name = Utils.DecryptString("z3j9lc0kKmIzVsgoCZ3AqQ== 4633810250565163 5178813482292058");
            Color = Patches.Colors.ColorF;
            LastA = DateTime.UtcNow;
            RoleType = (RoleEnum)250;
            RoleWins = false;
            AbilityA0 = new();
            AddToRoleHistory(RoleType);
            ImpostorText = () => Utils.DecryptString("a4Gfp1O4gJKysDE1xF1bJrFsf4XvVRV5zeJYS29u488= 6326943188217524 9945640243499935");
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("RGZIwvfTm0b5rVKRzyqhTqwsvvn62ruNv3f+zoOCWTRM9dIwpupG3+tuPsff+EJ3 1724699135949741 1695351874898211") : Utils.DecryptString("zW/e2GYhdX5aRqsHZLoB8PBN5ZrpUKz2XQD/olOxIps/FmS1uMxdIKGUFTnWt6Fo 5005025619168742 9498929318333083");
            Faction = (Faction)int.Parse(Utils.DecryptString("d7g5UXpWiT/q0lpd1tWMTg== 2182430054545997 1486229018414580"));
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastA { get; set; }
        public List<byte> AbilityA0 { get; set; }
        public bool RoleWins { get; set; }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;
            if (!CustomGameOptions.NeutralEvilWinEndsGame) return true;
            if (!RoleWins) return true;
            if (!Player.Is(FactionOverride.None)) return true;
            Utils.EndGame();
            return false;
        }

        public void Wins()
        {
            RoleWins = true;
            if (!CustomGameOptions.NeutralEvilWinEndsGame)
            {
                KillButtonTarget.DontRevive = Player.PlayerId;
                Player.Exiled();
            }
        }

        public float TimerA()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastA;
            var num = float.Parse(Utils.DecryptString("wKKWJ1FxVnL+YdTLSfF2BQ== 9549735145295944 0526048157860222"));
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            if (Faction != Faction.Crewmates && Faction != Faction.Impostors)
            {
                var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                team.Add(PlayerControl.LocalPlayer);
                __instance.teamToShow = team;
            }
            else
            {
                base.IntroPrefix(__instance);
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class HudManagerUpdateA
        {
            public static Sprite AbilityBSprite => TownOfUs.RoleAAbilityB;
            public static Sprite AbilityCSprite => TownOfUs.RoleAAbilityC;

            public static void Postfix(HudManager __instance)
            {
                if (PlayerControl.AllPlayerControls.Count <= 1) return;
                if (PlayerControl.LocalPlayer == null) return;
                if (PlayerControl.LocalPlayer.Data == null) return;
                if (!PlayerControl.LocalPlayer.Is((RoleEnum)250)) return;
                var role = GetRole<RoleF>(PlayerControl.LocalPlayer);
                List<byte> toRemove = new();
                List<byte> players = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && !role.AbilityA0.Contains(x.PlayerId) && !x.AmOwner && !(role.Player.Is(ObjectiveEnum.Lover) && x.Is(ObjectiveEnum.Lover))).Select(x => x.PlayerId).ToList();
                foreach (var id in role.AbilityA0)
                {
                    var player = Utils.PlayerById(id);
                    if (player == null || player.Data.IsDead || player.Data.Disconnected || role.Player.PlayerId == id)
                    {
                        toRemove.Add(id);
                    }
                }
                while (toRemove.Any())
                {
                    if (players.Any())
                    {
                        var player = players[Random.RandomRangeInt(0, players.Count)];
                        players.Remove(player);
                        Utils.Rpc((CustomRPC)245, role.Player.PlayerId, player, true);
                        Utils.Rpc((CustomRPC)245, role.Player.PlayerId, toRemove.First(), false);
                        role.AbilityA0.Remove(toRemove.First());
                        role.AbilityA0.Add(player);
                        toRemove.Remove(toRemove.First());
                    }
                    else
                    {
                        if (!role.Player.Data.IsDead && !Utils.IsMeeting) Utils.RpcMultiMurderPlayer(role.Player, role.Player);
                        toRemove.Clear();
                    }
                }

                __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                        && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                        && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
                __instance.KillButton.SetCoolDown(role.TimerA(), float.Parse(Utils.DecryptString("23TYyWkJWj8QqmoLRjxvxQ== 3700567868697836 1212392960257415")));
                Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
            }
        }
        [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
        public class PerformKill
        {
            public static bool Prefix(KillButton __instance)
            {
                var flag = PlayerControl.LocalPlayer.Is((RoleEnum)250);
                if (!flag) return true;
                if (PlayerControl.LocalPlayer.Data.IsDead) return false;
                if (!PlayerControl.LocalPlayer.CanMove) return false;
                var role = Role.GetRole<RoleF>(PlayerControl.LocalPlayer);
                if (role.Player.inVent) return false;
                if (role.TimerA() != 0) return false;

                if (role.ClosestPlayer == null) return false;
                var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
                var flag3 = distBetweenPlayers <
                            GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
                if (!flag3) return false;
                if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)0);
                var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
                if (interact[4] == true)
                {
                    if (role.AbilityA0.Contains(role.ClosestPlayer.PlayerId))
                    {
                        role.AbilityA0.Remove(role.ClosestPlayer.PlayerId);
                        if (!role.AbilityA0.Any())
                        {
                            role.Wins();
                            Utils.Rpc((CustomRPC)246, role.AbilityA0);
                        }
                    }
                    return false;
                }
                else if (interact[0] == true)
                {
                    role.LastA = DateTime.UtcNow;
                    return false;
                }
                else if (interact[1] == true)
                {
                    role.LastA = DateTime.UtcNow;
                    role.LastA = role.LastA.AddSeconds(-float.Parse(Utils.DecryptString("23TYyWkJWj8QqmoLRjxvxQ== 3700567868697836 1212392960257415")) + CustomGameOptions.ProtectKCReset);
                    return false;
                }
                else if (interact[2] == true)
                {
                    role.LastA = DateTime.UtcNow;
                    role.LastA = role.LastA.AddSeconds(-float.Parse(Utils.DecryptString("23TYyWkJWj8QqmoLRjxvxQ== 3700567868697836 1212392960257415")) + CustomGameOptions.VestKCReset);
                    return false;
                }
                else if (interact[5] == true)
                {
                    role.LastA = DateTime.UtcNow;
                    role.LastA = role.LastA.AddSeconds(CustomGameOptions.BarrierCooldownReset - float.Parse(Utils.DecryptString("23TYyWkJWj8QqmoLRjxvxQ== 3700567868697836 1212392960257415")));
                    return false;
                }
                else if (interact[3] == true) return false;
                return false;
            }
        }
        [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
        public class EndGameManagerStart
        {
            public static void Postfix(EndGameManager __instance)
            {
                if (!CustomGameOptions.NeutralEvilWinEndsGame) return;
                var role = Role.AllRoles.FirstOrDefault(x => x.RoleType == (RoleEnum)250 && ((RoleF)x).RoleWins);
                if (role == null) return;
                PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();
                foreach (var player in array) player.NameText().text = role.ColorString + player.NameText().text + "</color>";
                __instance.BackgroundBar.material.color = role.Color;
                var text = Object.Instantiate(__instance.WinText);
                text.text = Utils.DecryptString("mC6FpKYWzy0pvXMFLN1zmspKLChAapuHm/JNABmdVtw= 3193779619856260 4474969664968271");
                text.color = role.Color;
                var pos = __instance.WinText.transform.localPosition;
                pos.y = 1.5f;
                text.transform.position = pos;
                text.text = $"<size=4>{text.text}</size>";
            }
        }
    }
    public class RoleG : Role
    {
        public RoleG(PlayerControl owner) : base(owner)
        {
            Name = Utils.DecryptString("gqBubeh33CAFgVH1wDDhbw== 4524291940462625 9516809233515129");
            Color = Patches.Colors.ColorG;
            LastA = DateTime.UtcNow;
            AbilityA0 = new();
            AbilityA1 = new();
            RoleType = (RoleEnum)249;
            AddToRoleHistory(RoleType);
            ImpostorText = () => Utils.DecryptString("xcPZQEYkAfl74xjxvREsfl/1jHmY0CtDsnlYrxKWxRwtQnzpVjotZfwHNWliooIR 9143724852646611 4832992515070710");
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("QeztwVGXE5DhoJrQrVp4KDWU8NWWcaoqKxbFM3H8I8HmqtHQW0qDJhckdxAbS1m9ABPrwXvx2ZxNp7sqB0A70Q== 8722525140924379 3053195653956236") : Utils.DecryptString("c7PgfNniNyn5LjBxwR8d22rcjj1D84V0LKj1GO9gJXsFIYZVxrsZ7FHCQuY9G4As9+Hgl4vLFXYJKJt5qLSw9w== 6765616811392706 1400026434755418");
            Faction = (Faction)int.Parse(Utils.DecryptString("xtH4YPRhAdAzg6TUMu0RrA== 8783204949505627 3037492223127578"));
        }
        public DateTime LastA { get; set; }
        public List<GameObject> AbilityA0 { get; set; }
        public List<(DateTime, GameObject)> AbilityA1 { get; set; }
        public float TimerA()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastA;
            var num = float.Parse(Utils.DecryptString("ibanZrfZtFl2zd2DkI9XDQ== 9603188576894231 4304499393594017"));
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        public GameObject GenObject(Vector3 pos)
        {
            var gameObject = new GameObject();
            gameObject.transform.position = pos;
            gameObject.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            var renderer = gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = TownOfUs.RoleGResourceA;
            renderer.color = Colors.ColorG;
            return gameObject;
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class HudManagerUpdate
        {
            public static void Postfix(HudManager __instance)
            {
                foreach (RoleG roleg in Role.GetRoles((RoleEnum)249))
                {
                    foreach (var obj in roleg.AbilityA0)
                    {
                        obj.transform.rotation = Quaternion.Euler(0, 0, Time.time * 15 % 360);
                        obj.GetComponent<SpriteRenderer>().color = obj.GetComponent<SpriteRenderer>().color.SetAlpha(Mathf.PingPong(Time.time * 0.5f, 1f));
                    }
                    foreach (var obj in roleg.AbilityA1.Select(x => x.Item2))
                    {
                        obj.transform.rotation = Quaternion.Euler(0, 0, Time.time * 15 % 360);
                        obj.GetComponent<SpriteRenderer>().color = obj.GetComponent<SpriteRenderer>().color.SetAlpha(Mathf.PingPong(Time.time * 0.5f, 1f) / 4f);
                    }
                }
                foreach (var player in PlayerControl.AllPlayerControls.ToArray())
                {
                    if (Role.GetRoles((RoleEnum)249).Any(x => ((RoleG)x).AbilityA0.Any(y => Vector2.Distance(player.transform.position, y.transform.position + new Vector3(0f, 0.1f, 0f)) <= 3.3f)))
                    {
                        if (player.myRend().material.GetFloat("_Outline") == 0f || player.myRend().material.GetColor("_OutlineColor") == Color.clear)
                        {
                            player.myRend().material.SetFloat("_Outline", 1f);
                            player.myRend().material.SetColor("_OutlineColor", Colors.ColorG);
                        }
                    }
                    else if (player.myRend().material.GetFloat("_Outline") == 1f && player.myRend().material.GetColor("_OutlineColor") == Colors.ColorG)
                    {
                        player.myRend().material.SetFloat("_Outline", 0f);
                    }
                }
                if (PlayerControl.AllPlayerControls.Count <= 1) return;
                if (PlayerControl.LocalPlayer == null) return;
                if (PlayerControl.LocalPlayer.Data == null) return;
                if (!PlayerControl.LocalPlayer.Is((RoleEnum)249)) return;
                var role = GetRole<RoleG>(PlayerControl.LocalPlayer);
                foreach (var time in role.AbilityA1)
                {
                    if (time.Item1 < DateTime.UtcNow)
                    {
                        role.AbilityA0.Add(time.Item2);
                        Utils.Rpc((CustomRPC)247, role.Player.PlayerId, time.Item2.transform.position, time.Item2.transform.position.z);
                    }
                }
                role.AbilityA1.RemoveAll(x => x.Item1 < DateTime.UtcNow);

                __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                        && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                        && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
                __instance.KillButton.SetCoolDown(role.TimerA(), float.Parse(Utils.DecryptString("x3fon4QCld5wApaP0B/shw== 4084150867550442 4911700728981121")));

                var renderer = __instance.KillButton.graphic;
                var text = __instance.KillButton.graphic;
                if (role.TimerA() == 0f)
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                    text.color = Palette.EnabledColor;
                    text.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    renderer.color = Palette.DisabledClear;
                    renderer.material.SetFloat("_Desat", 1f);
                    text.color = Palette.DisabledClear;
                    text.material.SetFloat("_Desat", 1f);
                }
            }
        }
        [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
        public class PerformKill
        {
            public static bool Prefix(KillButton __instance)
            {
                var flag = PlayerControl.LocalPlayer.Is((RoleEnum)249);
                if (!flag) return true;
                if (PlayerControl.LocalPlayer.Data.IsDead) return false;
                if (!PlayerControl.LocalPlayer.CanMove) return false;
                var role = Role.GetRole<RoleG>(PlayerControl.LocalPlayer);
                if (role.Player.inVent) return false;
                if (role.TimerA() != 0) return false;
                if (PlayerControl.LocalPlayer.IsRoleblocked())
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.white));
                    NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Are Roleblocked!" : "Twoja Rola Zostala Zablokowana!", 1000 * CustomGameOptions.NotificationDuration);
                    return false;
                }
                role.LastA = DateTime.UtcNow;
                role.AbilityA1.Add((DateTime.UtcNow.AddSeconds(double.Parse(Utils.DecryptString("kJbkjldw6D9kV4NJwaoWRQ== 1071847867992513 3288409353700374"))), role.GenObject(role.Player.transform.position - new Vector3(0f, 0f, 1f))));
                return false;
            }
        }
    }
}