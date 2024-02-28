﻿using GameNetcodeStuff;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ContentType { Vanilla, Custom, Any } //Any & All included for built in checks.

namespace LethalLevelLoader
{
    [CreateAssetMenu(menuName = "LethalLevelLoader/ExtendedLevel")]
    public class ExtendedLevel : ScriptableObject
    {
        [Header("Extended Level Settings")]
        [Space(5)] public string contentSourceName = string.Empty; //Levels from AssetBundles will have this as their Assembly Name.
        [Space(5)] public SelectableLevel selectableLevel;
        [Space(5)] [SerializeField] private int routePrice = 0;
        [Space(5)] public bool isHidden = false;
        [Space(5)] public bool isLocked = false;
        [Space(5)] public string lockedNodeText = string.Empty;

        [Space(10)] public List<StoryLogData> storyLogs = new List<StoryLogData>();

        public int RoutePrice
        {
            get
            {
                if (routeNode != null)
                {
                    return AdvancedCompany.Lib.Moons.GetMoonPrice(routeConfirmNode.buyRerouteToMoon);
//                    routePrice = routeNode.itemCost;
//                    routeConfirmNode.itemCost = routePrice;
//                    return (routeNode.itemCost);
                }
                else
                {
                    DebugHelper.LogWarning("routeNode Is Missing! Using internal value!");
                    return (routePrice);
                }
            }
            set
            {
                routeNode.itemCost = value;
                routeConfirmNode.itemCost = value;
                routePrice = value;
            }
        }

        [Space(10)]
        [Header("Dynamic DungeonFlow Injections Settings")]
        [Space(5)] public ContentType allowedDungeonContentTypes = ContentType.Any;
        [Space(5)] public List<string> levelTags = new List<string>();

        [HideInInspector] public ContentType levelType;
        [HideInInspector] public string NumberlessPlanetName => GetNumberlessPlanetName(selectableLevel);

        [SerializeField][TextArea] public string infoNodeDescripton = string.Empty;
        [HideInInspector] public TerminalNode routeNode;
        [HideInInspector] public TerminalNode routeConfirmNode;
        [HideInInspector] public TerminalNode infoNode;

        [Space(10)]
        [Header("Misc. Settings")]
        [Space(5)] public bool generateAutomaticConfigurationOptions = true;

        public bool IsLoaded => SceneManager.GetSceneByName(selectableLevel.sceneName).isLoaded;

        [HideInInspector] public LevelEvents levelEvents = new LevelEvents();

        internal bool isLethalExpansion = false;

        internal static ExtendedLevel Create(SelectableLevel newSelectableLevel, ContentType newContentType)
        {
            ExtendedLevel newExtendedLevel = ScriptableObject.CreateInstance<ExtendedLevel>();

            newExtendedLevel.levelType = newContentType;
            newExtendedLevel.selectableLevel = newSelectableLevel;

            return (newExtendedLevel);
        }
        internal void Initialize(string newContentSourceName, bool generateTerminalAssets)
        {
            if (levelType == ContentType.Vanilla && selectableLevel.levelID > 8)
            {
                DebugHelper.LogWarning("LethalExpansion SelectableLevel " + NumberlessPlanetName + " Found, Setting To LevelType: Custom.");
                levelType = ContentType.Custom;
                //generateTerminalAssets = true;
                contentSourceName = "Lethal Expansion";
                levelTags.Clear();
                isLethalExpansion = true;
            }

            if (contentSourceName == string.Empty)
                contentSourceName = newContentSourceName;

            if (levelType == ContentType.Custom)
                levelTags.Add("Custom");

            if (generateTerminalAssets == true)
            {
                //DebugHelper.Log("Generating Terminal Assets For: " + NumberlessPlanetName);
                TerminalManager.GatherOrCreateLevelTerminalData(this);
            }

            if (levelType == ContentType.Custom)
            {
                name = NumberlessPlanetName.StripSpecialCharacters() + "ExtendedLevel";
                selectableLevel.name = NumberlessPlanetName.StripSpecialCharacters() + "Level";
            }
        }

        internal static Regex NonLettersRegex = new Regex("[^a-zA-Z]");
        /// <summary>
        /// Returns the name of the planet without the number
        /// </summary>
        /// <param name="selectableLevel">The level to return the name for</param>
        /// <returns>Name without number</returns>
        internal static string GetNumberlessPlanetName(SelectableLevel selectableLevel)
        {
            if (selectableLevel != null)
                return NonLettersRegex.Replace(selectableLevel.PlanetName, "");
            else
                return string.Empty;
        }

        /// <summary>
        /// Changes the ID of the level and reflect that change to terminal keywords as well
        /// </summary>
        /// <param name="id">New ID for the level</param>
        internal void SetLevelID(int id)
        {
            selectableLevel.levelID = id;
            if (routeNode != null)
                routeNode.displayPlanetInfo = selectableLevel.levelID;
            if (routeConfirmNode != null)
                routeConfirmNode.buyRerouteToMoon = selectableLevel.levelID;
        }
    }
        

    [System.Serializable]
    public class LevelEvents
    {
        public ExtendedEvent onLevelLoaded = new ExtendedEvent();
        public ExtendedEvent onNighttime = new ExtendedEvent();
        public ExtendedEvent<EnemyAI> onDaytimeEnemySpawn = new ExtendedEvent<EnemyAI>();
        public ExtendedEvent<EnemyAI> onNighttimeEnemySpawn = new ExtendedEvent<EnemyAI>();
        public ExtendedEvent<StoryLog> onStoryLogCollected = new ExtendedEvent<StoryLog>();
        public ExtendedEvent<LungProp> onApparatusTaken = new ExtendedEvent<LungProp>();
        public ExtendedEvent<(EntranceTeleport, PlayerControllerB)> onPlayerEnterDungeon = new ExtendedEvent<(EntranceTeleport, PlayerControllerB)>();
        public ExtendedEvent<(EntranceTeleport, PlayerControllerB)> onPlayerExitDungeon = new ExtendedEvent<(EntranceTeleport, PlayerControllerB)>();
        public ExtendedEvent<bool> onPowerSwitchToggle = new ExtendedEvent<bool>();

    }

    [System.Serializable]
    public class StoryLogData
    {
        public int storyLogID;
        public string terminalWord = string.Empty;
        public string storyLogTitle = string.Empty;
        [TextArea] public string storyLogDescription = string.Empty;

        [HideInInspector] internal int newStoryLogID;
    }
}
