﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoMod;
using RWCustom;
using UnityEngine;
using Menu;

class patch_ProcessManager : ProcessManager
{
    [MonoModIgnore]
    public patch_ProcessManager(RainWorld rainWorld) : base(rainWorld)
    {
    }

    public enum ProcessID
    {

        MainMenu,

        Game,

        SleepScreen,

        DeathScreen,

        StarveScreen,

        RegionSelect,

        OptionsMenu,

        MusicPlayer,

        GhostScreen,

        KarmaToMaxScreen,

        SlideShow,

        MenuMic,

        PauseMenu,

        FastTravelScreen,

        RegionsOverviewScreen,

        CustomEndGameScreen,

        InputSelect,

        TutorialControlsPage,

        SlugcatSelect,

        IntroRoll,

        Credits,

        ConsoleOptionsMenu,

        Dream,

        RainWorldSteamManager,

        MultiplayerMenu,

        MultiplayerResults,

        InputOptions,

        Statistics,

        MessageScreen
    }

    private void SwitchMainProcess(ProcessManager.ProcessID ID)
    {
        this.shadersTime = 0f;
        if (ID == ProcessManager.ProcessID.Game && this.menuMic != null)
        {
            this.menuMic = null;
            this.sideProcesses.Remove(this.menuMic);
        }
        else if (ID != ProcessManager.ProcessID.Game && this.menuMic == null)
        {
            this.menuMic = new MenuMicrophone(this, this.soundLoader);
            this.sideProcesses.Add(this.menuMic);
        }
        MainLoopProcess mainLoopProcess = this.currentMainLoop;
        if (this.currentMainLoop != null)
        {
            this.currentMainLoop.ShutDownProcess();
            this.currentMainLoop.processActive = false;
            this.currentMainLoop = null;
            this.soundLoader.ReleaseAllUnityAudio();
            HeavyTexturesCache.ClearRegisteredFutileAtlases();
            System.GC.Collect();
            Resources.UnloadUnusedAssets();
        }
        if (ID != ProcessManager.ProcessID.SleepScreen && ID != ProcessManager.ProcessID.GhostScreen && ID != ProcessManager.ProcessID.DeathScreen && ID != ProcessManager.ProcessID.KarmaToMaxScreen)
        {
            this.rainWorld.progression.Revert();
        }
        switch (ID)
        {
            case ProcessManager.ProcessID.MainMenu:
                this.currentMainLoop = new MainMenu(this, mainLoopProcess != null && mainLoopProcess.ID == ProcessManager.ProcessID.IntroRoll);
                break;
            case ProcessManager.ProcessID.Game:
                this.currentMainLoop = new RainWorldGame(this);
                break;
            case ProcessManager.ProcessID.SleepScreen:
            case ProcessManager.ProcessID.DeathScreen:
            case ProcessManager.ProcessID.StarveScreen:
                this.currentMainLoop = new SleepAndDeathScreen(this, ID);
                break;
            case ProcessManager.ProcessID.RegionSelect:
                this.currentMainLoop = new Menu.RegionSelectMenu(this);
                break;
            case ProcessManager.ProcessID.OptionsMenu:
                this.currentMainLoop = new OptionsMenu(this);
                break;
            case ProcessManager.ProcessID.GhostScreen:
            case ProcessManager.ProcessID.KarmaToMaxScreen:
                this.currentMainLoop = new GhostEncounterScreen(this, ID);
                break;
            case ProcessManager.ProcessID.SlideShow:
                this.currentMainLoop = new SlideShow(this, this.nextSlideshow);
                break;
            case ProcessManager.ProcessID.FastTravelScreen:
            case ProcessManager.ProcessID.RegionsOverviewScreen:
                this.currentMainLoop = new FastTravelScreen(this, ID);
                break;
            case ProcessManager.ProcessID.CustomEndGameScreen:
                this.currentMainLoop = new CustomEndGameScreen(this);
                break;
            case ProcessManager.ProcessID.InputSelect:
                this.currentMainLoop = new InputSelectScreen(this);
                break;
            case ProcessManager.ProcessID.SlugcatSelect:
                this.currentMainLoop = new SlugcatSelectMenu(this);
                break;
            case ProcessManager.ProcessID.IntroRoll:
                this.currentMainLoop = new IntroRoll(this);
                break;
            case ProcessManager.ProcessID.Credits:
                this.currentMainLoop = new EndCredits(this);
                break;
            case ProcessManager.ProcessID.ConsoleOptionsMenu:
                this.currentMainLoop = new ConsoleOptionsMenu(this);
                break;
            case ProcessManager.ProcessID.Dream:
                this.currentMainLoop = new DreamScreen(this);
                break;
            case (ProcessManager.ProcessID)patch_ProcessManager.ProcessID.MessageScreen:
                this.currentMainLoop = new MessageScreen(this);
                break;
            case ProcessManager.ProcessID.MultiplayerMenu:
                this.currentMainLoop = new MultiplayerMenu(this);
                break;
            case ProcessManager.ProcessID.MultiplayerResults:
                this.currentMainLoop = new MultiplayerResults(this);
                break;
            case ProcessManager.ProcessID.InputOptions:
                this.currentMainLoop = new InputOptionsMenu(this);
                break;
            case ProcessManager.ProcessID.Statistics:
                this.currentMainLoop = new StoryGameStatisticsScreen(this);
                break;
        }
        if (mainLoopProcess != null)
        {
            mainLoopProcess.CommunicateWithUpcomingProcess(this.currentMainLoop);
        }
        this.blackFadeTime = this.currentMainLoop.FadeInTime;
        this.blackDelay = this.currentMainLoop.InitialBlackSeconds;
        if (this.fadeSprite != null)
        {
            this.fadeSprite.RemoveFromContainer();
            Futile.stage.AddChild(this.fadeSprite);
        }
        if (this.loadingLabel != null)
        {
            this.loadingLabel.RemoveFromContainer();
            Futile.stage.AddChild(this.loadingLabel);
        }
        if (this.musicPlayer != null)
        {
            this.musicPlayer.UpdateMusicContext(this.currentMainLoop);
        }
        this.pauseFadeUpdate = true;
    }

}

