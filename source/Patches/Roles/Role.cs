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

namespace TownOfUs.Roles
{
    public abstract class Role
    {
        public static readonly Dictionary<byte, Role> RoleDictionary = new Dictionary<byte, Role>();
        public static readonly List<KeyValuePair<byte, RoleEnum>> RoleHistory = new List<KeyValuePair<byte, RoleEnum>>();

        public static bool NobodyWins;
        public static bool SurvOnlyWins;
        public static bool VampireWins;
        public int BreadLeft = 1;
        public int Defense = 0;
        public bool Reaped = false;
        public bool Roleblocked = false;
        public GameObject DefenseButton = new GameObject();
        public DateTime LastBlood;
        public List<ArrowBehaviour> SnipeArrows = new List<ArrowBehaviour>();
        public bool ApocalypseWins { get; set; }

        public List<KillButton> ExtraButtons = new List<KillButton>();

        public Func<string> ImpostorText;
        public Func<string> TaskText;
        public TextMeshPro NotificationText;
        public DateTime NotificationEnds;
        public string NotificationString;
        public bool KilledByAbility;
        public GameObject ChatButton;
        public ChatType CurrentChat = ChatType.VanillaChat;
        public bool meeting = false;
        //public Dictionary<ChatType, ChatController> ChatControllers = new Dictionary<ChatType, ChatController>();

        protected Role(PlayerControl player)
        {
            Player = player;
            RoleDictionary.Add(player.PlayerId, this);
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
                PlayerControl.LocalPlayer.Is(ObjectiveEnum.ImpostorAgent)) && (CustomGameOptions.ImpostorSeeRoles || Player.Is(ObjectiveEnum.ImpostorAgent) || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ImpostorAgent))) return true;
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
            return false;
        }
        internal virtual bool CustomCriteria()
        {
            if (RoleType == RoleEnum.Mayor)
            {
                if (((Mayor)this).Revealed) return true;
            }
            else if (RoleType == RoleEnum.Deputy)
            {
                if (((Deputy)this).Revealed && CustomGameOptions.RevealDeputy) return true;
            }
            else if (RoleType == RoleEnum.Prosecutor)
            {
                if (((Prosecutor)this).Revealed && CustomGameOptions.RevealProsecutor) return true;
            }
            return false;
        }

        internal virtual bool SelfCriteria()
        {
            return GetRole(PlayerControl.LocalPlayer) == this;
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
                        if (player.Data.IsImpostor() || player.Is(ObjectiveEnum.ImpostorAgent)) impTeam.Add(player);
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

            VampireWins = true;

            Utils.Rpc(CustomRPC.VampireWin);
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
                var ga = (GuardianAngel) role;
                if (Player == ga.target && ((Player == PlayerControl.LocalPlayer && CustomGameOptions.GATargetKnows)
                    || (PlayerControl.LocalPlayer.Data.IsDead && !ga.Player.Data.IsDead)))
                {
                    PlayerName += "<color=#B3FFFFFF> ★</color>";
                }
            }

            foreach (var role in GetRoles(RoleEnum.Executioner))
            {
                var exe = (Executioner) role;
                if (Player == exe.target && PlayerControl.LocalPlayer.Data.IsDead && !exe.Player.Data.IsDead)
                {
                    PlayerName += "<color=#8C4005FF> X</color>";
                }
            }

            foreach (var role in GetRoles(RoleEnum.Inquisitor))
            {
                var inq = (Inquisitor)role;
                if (PlayerControl.LocalPlayer.Data.IsDead && !inq.Player.Data.IsDead && inq.heretics.Contains(Player.PlayerId)) //Error
                {
                    PlayerName += "<color=#821252FF> X</color>";
                }
            }

            foreach (var role in GetRoles(RoleEnum.Monarch))
            {
                var mon = (Monarch)role;
                if (mon.Knights.Contains(Player.PlayerId))
                {
                    PlayerName += "<color=#9628C8FF> +</color>";
                }
            }

            var modifier = Modifier.GetModifier(Player);
            if (modifier != null && modifier.GetColoredSymbol() != null)
            {
                if (revealModifier)
                    PlayerName += $" {modifier.GetColoredSymbol()}";
            }

            var objective = Objective.GetObjective(Player);
            if (objective != null && objective.GetColoredSymbol() != null)
            {
                if (objective.ObjectiveType == ObjectiveEnum.Lover && (revealModifier || revealLover))
                    PlayerName += $" {objective.GetColoredSymbol()}";
                else if (objective.ObjectiveType != ObjectiveEnum.Lover && revealModifier)
                    PlayerName += $" {objective.GetColoredSymbol()}";
            }

            if (revealTasks && (Faction == Faction.Crewmates || RoleType == RoleEnum.Phantom))
            {
                if ((PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.SeeTasksWhenDead) || (MeetingHud.Instance && CustomGameOptions.SeeTasksDuringMeeting) || (!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance && CustomGameOptions.SeeTasksDuringRound))
                {
                    PlayerName += $" ({TotalTasks - TasksLeft}/{TotalTasks})";
                }
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
                                    RoleType == RoleEnum.Trapper || RoleType == RoleEnum.Inspector || RoleType == RoleEnum.Lookout)
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
                        else if (RoleType == RoleEnum.Crewmate)
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
                        else if (RoleType == RoleEnum.Impostor)
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
        public static T GenModifier<T>(Type type, List<PlayerControl> players)
        {
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
                        || x == RLRoleEntry.ImpostorKilling || x == RLRoleEntry.ImpostorSupport || x == RLRoleEntry.Impostor || x == RLRoleEntry.Escapist
                        || x == RLRoleEntry.Grenadier || x == RLRoleEntry.Morphling || x == RLRoleEntry.Swooper || x == RLRoleEntry.Venerer
                        || x == RLRoleEntry.Bomber || x == RLRoleEntry.Warlock || x == RLRoleEntry.Poisoner || x == RLRoleEntry.Sniper
                        || x == RLRoleEntry.Blackmailer || x == RLRoleEntry.Janitor || x == RLRoleEntry.Miner || x == RLRoleEntry.Undertaker);
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
                        || x == RLRoleEntry.ImpostorKilling || x == RLRoleEntry.ImpostorSupport || x == RLRoleEntry.Impostor || x == RLRoleEntry.Escapist
                        || x == RLRoleEntry.Grenadier || x == RLRoleEntry.Morphling || x == RLRoleEntry.Swooper || x == RLRoleEntry.Venerer
                        || x == RLRoleEntry.Bomber || x == RLRoleEntry.Warlock || x == RLRoleEntry.Poisoner || x == RLRoleEntry.Sniper
                        || x == RLRoleEntry.Blackmailer || x == RLRoleEntry.Janitor || x == RLRoleEntry.Miner || x == RLRoleEntry.Undertaker);
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

                if (GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks && PlayerControl.AllPlayerControls.ToArray().Any(x => x.Is(Faction.Crewmates) && !x.Is(ObjectiveEnum.ImpostorAgent) && !x.Is(ObjectiveEnum.ApocalypseAgent) && x.Is(FactionOverride.None))) return true;
                
                var result = true;
                foreach (var role in AllRoles)
                {
                    var roleIsEnd = role.NeutralWin(__instance);
                    var modifier = Modifier.GetModifier(role.Player);
                    var objective = Objective.GetObjective(role.Player);
                    bool modifierIsEnd = true;
                    bool objectiveIsEnd = true;
                    var alives = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(ObjectiveEnum.ImpostorAgent) && !x.Is(RoleEnum.Witch) && !(x.Is(RoleEnum.Undercover) && Utils.UndercoverIsImpostor() && !CustomGameOptions.UndercoverKillEachother)).ToList();
                    var impsAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.Data.IsImpostor()).ToList();
                    var recruitImp = PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && x.Data.IsImpostor() && x.Is(FactionOverride.Recruit));
                    var traitorIsEnd = true;
                    var CKExists = alives.ToArray().Count(x => (x.Is(RoleEnum.Sheriff) || x.Is(RoleEnum.Vigilante) || x.Is(RoleEnum.Veteran) || x.Is(RoleEnum.VampireHunter)) && !x.Is(ObjectiveEnum.ImpostorAgent)) > 0;
                    bool stopImpOvertake = ((CustomGameOptions.OvertakeWin == OvertakeWin.Off || (CustomGameOptions.OvertakeWin == OvertakeWin.WithoutCK && CKExists) ? impsAlive.Count : impsAlive.Count * 2) < alives.Count) && impsAlive.Count != 0;
                    if (SetTraitor.WillBeTraitor != null)
                    {
                        traitorIsEnd = SetTraitor.WillBeTraitor.Data.IsDead || SetTraitor.WillBeTraitor.Data.Disconnected || alives.Count < CustomGameOptions.LatestSpawn || impsAlive.Count * 2 >= alives.Count;
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

                RoleDictionary.Clear();
                RoleHistory.Clear();
                Modifier.ModifierDictionary.Clear();
                Ability.AbilityDictionary.Clear();
                Objective.ObjectiveDictionary.Clear();
            }
        }

        [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames),
            typeof(Il2CppReferenceArray<Il2CppSystem.Object>))]
        public static class TranslationController_GetString
        {
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
                        __result += "\n" + (ImpsLeft > 0 && CustomGameOptions.ShowImpostorsRemaining ? ImpsLeftEjected + " Impostor" + (ImpsLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + (ApocLeft > 0 && CustomGameOptions.ShowApocalypseRemaining ? ApocLeftEjected + " Apocalypse Member" + (ApocLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + (UndeLeft > 0 && CustomGameOptions.ShowUndeadRemaining ? UndeLeftEjected + " Undead" + (UndeLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + (KillLeft > 0 && CustomGameOptions.ShowKillingRemaining ? KillLeftEjected + " Neutral Killing" + (KillLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + (ProsLeft > 0 && CustomGameOptions.ShowProselyteRemaining ? ProsLeftEjected + " Neutral Proselyte" + (ProsLeftEjected == 1 ? "" : "s") + " Remaining" : "");
                        return;
                    case StringNames.NoExileSkip:
                        {
                            __result += "\n" + (ImpsLeft > 0 && CustomGameOptions.ShowImpostorsRemaining ? ImpsLeftEjected + " Impostor" + (ImpsLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + (ApocLeft > 0 && CustomGameOptions.ShowApocalypseRemaining ? ApocLeftEjected + " Apocalypse Member" + (ApocLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + (UndeLeft > 0 && CustomGameOptions.ShowUndeadRemaining ? UndeLeftEjected + " Undead" + (UndeLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + (KillLeft > 0 && CustomGameOptions.ShowKillingRemaining ? KillLeftEjected + " Neutral Killing" + (KillLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + (ProsLeft > 0 && CustomGameOptions.ShowProselyteRemaining ? ProsLeftEjected + " Neutral Proselyte" + (ProsLeftEjected == 1 ? "" : "s") + " Remaining" : "");
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
                            __result += "\n" + (ImpsLeft > 0 && CustomGameOptions.ShowImpostorsRemaining ? ImpsLeftEjected + " Impostor" + (ImpsLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + (ApocLeft > 0 && CustomGameOptions.ShowApocalypseRemaining ? ApocLeftEjected + " Apocalypse Member" + (ApocLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + (UndeLeft > 0 && CustomGameOptions.ShowUndeadRemaining ? UndeLeftEjected + " Undead" + (UndeLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + (KillLeft > 0 && CustomGameOptions.ShowKillingRemaining ? KillLeftEjected + " Neutral Killing" + (KillLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + (ProsLeft > 0 && CustomGameOptions.ShowProselyteRemaining ? ProsLeftEjected + " Neutral Proselyte" + (ProsLeftEjected == 1 ? "" : "s") + " Remaining" : "");
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
                            __result += "\n" + (ImpsLeft > 0 && CustomGameOptions.ShowImpostorsRemaining ? ImpsLeftEjected + " Impostor" + (ImpsLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + (ApocLeft > 0 && CustomGameOptions.ShowApocalypseRemaining ? ApocLeftEjected + " Apocalypse Member" + (ApocLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + (UndeLeft > 0 && CustomGameOptions.ShowUndeadRemaining ? UndeLeftEjected + " Undead" + (UndeLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + (KillLeft > 0 && CustomGameOptions.ShowKillingRemaining ? KillLeftEjected + " Neutral Killing" + (KillLeftEjected == 1 ? "" : "s") + " Remaining\n" : "") + (ProsLeft > 0 && CustomGameOptions.ShowProselyteRemaining ? ProsLeftEjected + " Neutral Proselyte" + (ProsLeftEjected == 1 ? "" : "s") + " Remaining" : "");
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
                        if (role.Criteria())
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
                            bool customFlag = role.CustomCriteria();
                            player.NameText.text = role.NameText( //Error
                                selfFlag || deadFlag || role.Local,
                                selfFlag || deadFlag || impostorFlag || proselyteFlag || roleFlag || gaFlag || apocalypseFlag || customFlag,
                                selfFlag || deadFlag || agentFlag,
                                loverFlag,
                                witchFlag,
                                player
                            );
                            if (role.ColorCriteria())
                                player.NameText.color = role.Color;
                            if (role.RoleType == RoleEnum.Undercover)
                            {
                                if (((((Undercover)role).UndercoverImpostor && (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ImpostorAgent))) || (((Undercover)role).UndercoverApocalypse) && (PlayerControl.LocalPlayer.Is(Faction.NeutralApocalypse) || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ApocalypseAgent))))
                                    player.NameText.color = ((Undercover)role).UndercoverRole.GetRoleColor();
                            }
                            else if (role.Faction == Faction.Impostors && PlayerControl.LocalPlayer.Data.IsImpostor())
                                player.NameText.color = Patches.Colors.Impostor;
                        }
                        if (role.Player.IsKnight())
                        {
                            try
                            {
                                player.NameText.text = role.Player.GetDefaultOutfit().PlayerName + "<color=#9628C8FF> +</color>"; //Error
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            try
                            {
                                player.NameText.text = role.Player.GetDefaultOutfit().PlayerName;
                            }
                            catch
                            {
                            }
                        }
                        if (role.RoleType == RoleEnum.Undercover)
                        {
                            if (((((Undercover)role).UndercoverImpostor && (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ImpostorAgent))) || (((Undercover)role).UndercoverApocalypse) && (PlayerControl.LocalPlayer.Is(Faction.NeutralApocalypse) || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ApocalypseAgent))))
                                player.NameText.color = ((Undercover)role).UndercoverRole.GetRoleColor();
                        }
                        else if (role.Faction == Faction.Impostors && PlayerControl.LocalPlayer.Data.IsImpostor())
                            player.NameText.color = Patches.Colors.Impostor;
                    }
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
                    if (!(player.Data != null && player.Data.IsImpostor() && PlayerControl.LocalPlayer.Data.IsImpostor()))
                    {
                        player.nameText().text = player.name;
                        player.nameText().color = Color.white;
                    }
                    var role = GetRole(player);
                    if (role != null)
                    {
                        if (role.RoleType == RoleEnum.Undercover)
                        {
                            if (((((Undercover)role).UndercoverImpostor && (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ImpostorAgent))) || (((Undercover)role).UndercoverApocalypse) && (PlayerControl.LocalPlayer.Is(Faction.NeutralApocalypse) || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ApocalypseAgent))))
                                player.nameText().color = ((Undercover)role).UndercoverRole.GetRoleColor();
                        }
                        else if (role.Faction == Faction.Impostors && PlayerControl.LocalPlayer.Data.IsImpostor())
                            player.nameText().color = Patches.Colors.Impostor;
                    }
                    if (role != null)
                    {
                        if (role.Criteria())
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
                            bool customFlag = role.CustomCriteria();
                            player.nameText().text = role.NameText(
                                selfFlag || deadFlag || role.Local,
                                selfFlag || deadFlag || impostorFlag || proselyteFlag || roleFlag || gaFlag || apocalypseFlag || customFlag,
                                selfFlag || deadFlag || agentFlag,
                                loverFlag,
                                witchFlag
                             );
                            if (role.ColorCriteria())
                                player.nameText().color = role.Color;
                            if (role.RoleType == RoleEnum.Undercover)
                            {
                                if (((((Undercover)role).UndercoverImpostor && (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ImpostorAgent))) || (((Undercover)role).UndercoverApocalypse) && (PlayerControl.LocalPlayer.Is(Faction.NeutralApocalypse) || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ApocalypseAgent))))
                                    player.nameText().color = ((Undercover)role).UndercoverRole.GetRoleColor();
                            }
                            else if (role.Faction == Faction.Impostors && PlayerControl.LocalPlayer.Data.IsImpostor())
                                player.nameText().color = Patches.Colors.Impostor;
                        }
                        else if (player.IsKnight()) player.nameText().text += "<color=#9628C8FF> +</color>";
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
            foreach (var arrow in SnipeArrows)
            {
                if (arrow != null)
                    Object.Destroy(arrow);
                if (arrow.gameObject != null)
                    Object.Destroy(arrow.gameObject);
                SnipeArrows.Remove(arrow);
            }
        }

        public void Notification(string text, double milliseconds)
        {
            NotificationString = text;
            NotificationEnds = DateTime.UtcNow;
            NotificationEnds = NotificationEnds.AddMilliseconds(milliseconds);
        }
    }
}