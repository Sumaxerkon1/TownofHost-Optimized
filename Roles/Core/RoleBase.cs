﻿using AmongUs.GameOptions;
using Hazel;
using InnerNet;
using TOHE.Roles.Core;
using TOHE.Roles.Neutral;
using UnityEngine;

namespace TOHE;

public abstract class RoleBase
{
    public PlayerState _state;
#pragma warning disable IDE1006
    public PlayerControl _Player => Utils.GetPlayerById(_state.PlayerId);
    public List<byte> _playerIdList => Main.PlayerStates.Values.Where(x => x.MainRole == _state.MainRole).Select(x => x.PlayerId).Cast<byte>().ToList();
#pragma warning restore IDE1006

    public float AbilityLimit { get; set; } = -100;
    public virtual bool IsEnable { get; set; } = false;
    public void OnInit() // CustomRoleManager.RoleClass executes this
    {
        IsEnable = false;
        Init();
    }

    public void OnAdd(byte playerid) // The player with the class executes this
    {
        _state = Main.PlayerStates.Values.FirstOrDefault(state => state.PlayerId == playerid);
        try {
            CustomRoleManager.RoleClass.FirstOrDefault(r => r.Key == _state.MainRole).Value.IsEnable = true;
            this.IsEnable = true; // Not supposed to be used, but some methods may have still implemented that check.
        } catch { } // temporary try catch


        Add(playerid);
    }


    /// <summary>
    /// Variable resets when the game starts.
    /// </summary>
    public virtual void Init()
    { }
    /// <summary>
    /// When role is applied in the game, beginning or during the game.
    /// </summary>
    public virtual void Add(byte playerId)
    { }

    /// <summary>
    /// If role has to be removed from player
    /// </summary>
    public virtual void Remove(byte playerId)
    { }

    /// <summary>
    /// Used to Determine the CustomRole's BASE
    /// </summary>
    public abstract CustomRoles ThisRoleBase { get; }

    /// <summary>
    /// Defines the role type
    /// </summary>
    public abstract Custom_RoleType ThisRoleType { get; }

    /// <summary>
    /// A generic method to set if a impostor/SS base may use kill button.
    /// </summary>
    public virtual bool CanUseKillButton(PlayerControl pc) => pc.Is(Custom_Team.Impostor) && pc.IsAlive();

    /// <summary>
    /// A generic method to set if a impostor/SS base may vent.
    /// </summary>
    public virtual bool CanUseImpostorVentButton(PlayerControl pc) => pc.Is(Custom_Team.Impostor) && pc.IsAlive();

    /// <summary>
    /// A generic method to set if the role can use sabotage.
    /// </summary>
    public virtual bool CanUseSabotage(PlayerControl pc) => pc.Is(Custom_Team.Impostor);
    /// <summary>
    /// When the player presses the sabotage button
    /// </summary>
    public virtual bool OnSabotage(PlayerControl pc) => pc != null;

    public virtual void SetupCustomOption()
    { }

    public virtual bool IsExperimental => false;

    /// <summary>
    /// A generic method to send a CustomRole's Gameoptions.
    /// </summary>
    public virtual void ApplyGameOptions(IGameOptions opt, byte playerId)
    {
        // Set vision
        opt.SetVision(false);
    }

    /// <summary>
    /// Set a specific kill cooldown
    /// </summary>
    public virtual void SetKillCooldown(byte id) => Main.AllPlayerKillCooldown[id] = Options.DefaultKillCooldown;

    /// <summary>
    /// A local method to check conditions during gameplay, 30 times each second
    /// </summary>
    public virtual void OnFixedUpdate(PlayerControl pc)
    { }
    /// <summary>
    /// A local method to check conditions during gameplay, which aren't prioritized
    /// </summary>
    public virtual void OnFixedUpdateLowLoad(PlayerControl pc)
    { }

    /// <summary>
    /// Player completes a task
    /// </summary>
    public virtual bool OnTaskComplete(PlayerControl player, int completedTaskCount, int totalTaskCount) => true;
    /// <summary>
    /// Other player complete A Marked task
    /// </summary>
    public virtual void OnOthersTaskComplete(PlayerControl pc, PlayerTask task)
    { }
    /// <summary>
    /// The role's tasks are needed for a task win
    /// </summary>
    public virtual bool HasTasks(GameData.PlayerInfo player, CustomRoles role, bool ForRecompute) => role.IsCrewmate() && !role.IsTasklessCrewmate() && (!ForRecompute || !player.Object.IsAnySubRole(x => x.IsConverted()));

    /// <summary>
    /// A generic method to check a Guardian Angel protecting someone.
    /// </summary>
    public virtual bool OnCheckProtect(PlayerControl angel, PlayerControl target) => angel != null && target != null;

    /// <summary>
    /// A method for activating actions where the role starts playing an animation when entering a vent
    /// </summary>
    public virtual void OnEnterVent(PlayerControl pc, Vent vent)
    { }
    /// <summary>
    /// When role need force boot from vent
    /// </summary>
    public virtual bool CheckBootFromVent(PlayerPhysics physics, int ventId) => physics == null;
    /// <summary>
    /// A method for activating actions when role is already in vent
    /// </summary>
    public virtual bool OnCoEnterVentOthers(PlayerPhysics physics, int ventId) => physics == null;
    /// <summary>
    /// A method for activating actions when role is already in vent
    /// </summary>
    public virtual void OnCoEnterVent(PlayerPhysics physics, int ventId)
    { }
    /// <summary>
    /// A generic method to activate actions once (CustomRole)player exists vent.
    /// </summary>
    public virtual void OnExitVent(PlayerControl pc, int ventId)
    { }

    /// <summary>
    /// When role try fix any sabotage or open doors
    /// </summary>
    public virtual void UpdateSystem(ShipStatus __instance, SystemTypes systemType, byte amount, PlayerControl player)
    { }
    /// <summary>
    /// When role try fix electrical
    /// </summary>
    public virtual void SwitchSystemUpdate(SwitchSystem __instance, byte amount, PlayerControl player)
    { }

    /// <summary>
    ///  When role based on Impostors need force check target
    /// </summary>
    public virtual bool ForcedCheckMurderAsKiller(PlayerControl killer, PlayerControl target) => target != null && killer != null;
    /// <summary>
    /// When role the target requires a kill check
    /// </summary>
    /// <returns>If the target doesn't require a kill cancel, always use "return true"</returns>
    public virtual bool OnCheckMurderAsTarget(PlayerControl killer, PlayerControl target) => target != null && killer != null;
    /// <summary>
    /// When role the target requires a kill check
    /// </summary>
    /// <returns>If the target needs to cancel kill, always use "return true"</returns>
    public virtual bool CheckMurderOnOthersTarget(PlayerControl killer, PlayerControl target) => target == null || killer == null;
    /// <summary>
    ///  When role the killer requires a kill check
    /// </summary>
    public virtual bool OnCheckMurderAsKiller(PlayerControl killer, PlayerControl target) => target != null && killer != null;

    /// <summary>
    /// When the killer murder his target
    /// </summary>
    public virtual void OnMurderPlayerAsKiller(PlayerControl killer, PlayerControl target, bool inMeeting, bool isSuicide)
    { }
    /// <summary>
    /// When the target role died by killer
    /// </summary>
    public virtual void OnMurderPlayerAsTarget(PlayerControl killer, PlayerControl target, bool inMeeting, bool isSuicide)
    { }

    /// <summary>
    /// A method to always check the state when target has died (murder, exiled, execute etc..)
    /// </summary>
    public virtual void OnOtherTargetsReducedToAtoms(PlayerControl DeadPlayer)
    { }

    /// <summary>
    /// When the target role died and need run kill flash
    /// </summary>
    public virtual bool KillFlashCheck(PlayerControl killer, PlayerControl target, PlayerControl seer) => false;

    /// <summary>
    /// Shapeshift animation only from itself
    /// </summary>
    public virtual bool CanDesyncShapeshift => false;

    /// <summary>
    /// Called when checking for shapeshift
    /// Also can be used not activate shapeshift animate
    /// </summary>
    /// <param name="target">Transformation target</param>
    /// <param name="animate">Whether to play the shapeshift animation</param>
    /// <returns>return false for cancel the shapeshift transformation</returns>
    public virtual bool OnCheckShapeshift(PlayerControl shapeshifter, PlayerControl target, ref bool resetCooldown, ref bool shouldAnimate) => true;

    /// <summary>
    /// Called after check shapeshift
    /// </summary>
    public virtual void OnShapeshift(PlayerControl shapeshifter, PlayerControl target, bool IsAnimate, bool shapeshifting)
    { }

    /// <summary>
    /// Check start meeting by press meeting button
    /// </summary>
    public virtual bool OnCheckStartMeeting(PlayerControl reporter) => reporter.IsAlive();
    /// <summary>
    /// Check start meeting by dead body
    /// </summary>
    public virtual bool OnCheckReportDeadBody(PlayerControl reporter, GameData.PlayerInfo deadBody, PlayerControl killer) => reporter.IsAlive();
    /// <summary>
    /// When the meeting was start by report dead body or press meeting button
    /// </summary>
    public virtual void OnReportDeadBody(PlayerControl reporter, PlayerControl target)
    { }

    /// <summary>
    /// When guesser need check guess (Check limit or Cannot guess а role/add-on)
    /// </summary>
    public virtual bool GuessCheck(bool isUI, PlayerControl guesser, PlayerControl target, CustomRoles role, ref bool guesserSuicide) => target == null;
    /// <summary>
    /// When guesser trying guess target a role
    /// </summary>
    public virtual bool OnRoleGuess(bool isUI, PlayerControl target, PlayerControl guesser, CustomRoles role, ref bool guesserSuicide) => target == null;

    /// <summary>
    /// When guesser was misguessed
    /// </summary>
    public virtual bool CheckMisGuessed(bool isUI, PlayerControl guesser, PlayerControl target, CustomRoles role, ref bool guesserSuicide) => target == null;

    /// <summary>
    /// Check exile role
    /// </summary>
    public virtual void CheckExileTarget(GameData.PlayerInfo exiled, ref bool DecidedWinner, bool isMeetingHud, ref string name)
    { }
    /// <summary>
    /// When player was exiled
    /// </summary>
    public virtual void OnPlayerExiled(PlayerControl player, GameData.PlayerInfo exiled)
    { }

    /// <summary>
    /// When the meeting hud is loaded for others
    /// </summary>
    public virtual void OnOthersMeetingHudStart(PlayerControl pc)
    { }
    /// <summary>
    /// When the meeting hud is loaded 
    /// </summary>
    public virtual void OnMeetingHudStart(PlayerControl pc)
    { }
    /// <summary>
    /// Clears the initial meetinghud message
    /// </summary>
    public virtual void MeetingHudClear()
    { }
    /// <summary>
    /// Notify the playername for modded clients OnMeeting
    /// </summary>
    public virtual string PVANameText(PlayerVoteArea pva, PlayerControl seer, PlayerControl target) => string.Empty;

    /// <summary>
    /// Notify a specific role about something after the meeting was ended.
    /// </summary>
    public virtual void NotifyAfterMeeting()
    { }
    /// <summary>
    /// A generic method to activate actions after a meeting has ended.
    /// </summary>
    public virtual void AfterMeetingTasks()
    { }

    /// <summary>
    /// When player left the game
    /// </summary>
    public virtual void OnPlayerLeft(ClientData clientData)
    { }

    /// <summary>
    /// When the game starts to ending
    /// </summary>
    public virtual void OnCoEndGame()
    { }


    /// <summary>
    /// If role wants to return the vote to the player during meeting. Can also work to check any abilities during meeting.
    /// </summary>
    public virtual bool CheckVote(PlayerControl voter, PlayerControl target) => voter != null && target != null;

    /// <summary>
    /// A check for any role abilites of the player which voted, when the vote hasn't been canceled by any other means.
    /// </summary>
    public virtual void OnVote(PlayerControl votePlayer, PlayerControl voteTarget)
    { }
    /// <summary>
    /// A check for any role abilites of the player that was voted, when the vote hasn't been canceled by any other means.
    /// </summary>
    public virtual void OnVoted(PlayerControl votedPlayer, PlayerControl votedTarget)
    { }


    /// <summary>
    /// When need add visual votes
    /// </summary>
    public virtual void AddVisualVotes(PlayerVoteArea votedPlayer, ref List<MeetingHud.VoterState> statesList)
    { }
    /// <summary>
    /// Add real votes num
    /// </summary>
    public virtual int AddRealVotesNum(PlayerVoteArea PVA) => 0;

    /// <summary>
    /// Set text for Kill/Shapeshift/Report/Vent/Protect button
    /// </summary>
    public virtual void SetAbilityButtonText(HudManager hud, byte playerId)
    { }

    public virtual Sprite GetKillButtonSprite(PlayerControl player, bool shapeshifting) => null;
    public virtual Sprite GetAbilityButtonSprite(PlayerControl player, bool shapeshifting) => null;
    public virtual Sprite ImpostorVentButtonSprite(PlayerControl player) => null;
    public virtual Sprite ReportButtonSprite { get; }

    /// <summary>
    /// Set PlayerName text for the role
    /// </summary>
    public virtual string NotifyPlayerName(PlayerControl seer, PlayerControl target, string TargetPlayerName = "", bool IsForMeeting = false) => string.Empty;
    //
    //
    // Add Mark/LowerText/Suffix for player
    // When using this code remember the seer can also see the target, therefore..
    //
    // return string.empty if "seer != seen" if only seer should have it
    // otherwise make some list or byte or smt of sorts to only get the target.
    // not needed if both should have it.
    public virtual string GetMark(PlayerControl seer, PlayerControl seen, bool isForMeeting = false) => string.Empty;

    public virtual float SetModdedLowerText(out Color32? FaceColor)
    {
        FaceColor = null;
        return 2.8f;
    }
    
    public virtual string GetLowerText(PlayerControl seer, PlayerControl seen = null, bool isForMeeting = false, bool isForHud = false) => string.Empty;
    public virtual string GetSuffix(PlayerControl seer, PlayerControl seen, bool isForMeeting = false) => string.Empty;
    public virtual string GetProgressText(byte playerId, bool comms) => string.Empty;

    // Player know role target, color role target
    public virtual bool KnowRoleTarget(PlayerControl seer, PlayerControl target) => false;
    public virtual string PlayerKnowTargetColor(PlayerControl seer, PlayerControl target) => string.Empty;
    public virtual bool OthersKnowTargetRoleColor(PlayerControl seer, PlayerControl target) => false;


    public void OnReceiveRPC(MessageReader reader) 
    {
        float Limit = reader.ReadSingle();
        AbilityLimit = Limit;
    }
    public void SendSkillRPC()
    {
        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SyncRoleSkill, SendOption.Reliable, -1);
        writer.Write(AbilityLimit);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }
    public virtual void ReceiveRPC(MessageReader reader, PlayerControl pc)
    {
        OnReceiveRPC(reader); // Default implementation
    }
}
