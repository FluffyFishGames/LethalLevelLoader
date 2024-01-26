﻿using DunGen;
using DunGen.Graph;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;

namespace LethalLevelLoader
{
    public class ContentExtractor
    {
        //[HarmonyPatch(typeof(RoundManager), "Awake")]
        //[HarmonyPrefix]
        //[HarmonyPriority(350)]
        internal static void TryScrapeVanillaContent(RoundManager roundManager)
        {
            if (LethalLevelLoaderPlugin.hasVanillaBeenPatched == false)
            {
                StartOfRound startOfRound = StartOfRound.Instance;
                if (startOfRound != null)
                {
                    foreach (DungeonFlow dungeonFlow in roundManager.dungeonFlowTypes)
                        TryAddReference(OriginalContent.DungeonFlows, dungeonFlow);

                    foreach (Item item in startOfRound.allItemsList.itemsList)
                        TryAddReference(OriginalContent.Items, item);

                    foreach (SelectableLevel selectableLevel in startOfRound.levels)
                        ExtractSelectableLevelReferences(selectableLevel);

                    foreach (DungeonFlow dungeonFlow in roundManager.dungeonFlowTypes)
                        ExtractDungeonFlowReferences(dungeonFlow);
                }

                foreach (TerminalNode terminalNode in Terminal_Patch.Terminal.terminalNodes.terminalNodes)
                    TryAddReference(OriginalContent.TerminalNodes, terminalNode);


                foreach (TerminalNode terminalNode in Terminal_Patch.Terminal.terminalNodes.specialNodes)
                    TryAddReference(OriginalContent.TerminalNodes, terminalNode);

                foreach (TerminalKeyword terminalKeyword in Terminal_Patch.Terminal.terminalNodes.allKeywords)
                {
                    TryAddReference(OriginalContent.TerminalKeywords, terminalKeyword);
                    foreach (CompatibleNoun compatibleNoun in terminalKeyword.compatibleNouns)
                        if (compatibleNoun.result != null)
                            TryAddReference(OriginalContent.TerminalNodes, compatibleNoun.result);
                }

                foreach (TerminalNode terminalNode in new List<TerminalNode>(OriginalContent.TerminalNodes))
                    foreach (CompatibleNoun compatibleNoun in terminalNode.terminalOptions)
                        if (compatibleNoun.result != null)
                            TryAddReference(OriginalContent.TerminalNodes, compatibleNoun.result);

                foreach (AudioMixerGroup audioMixerGroup in Resources.FindObjectsOfTypeAll(typeof(AudioMixerGroup)))
                {
                    TryAddReference(OriginalContent.AudioMixers, audioMixerGroup.audioMixer);
                    TryAddReference(OriginalContent.AudioMixerGroups, audioMixerGroup);
                }

                foreach (ReverbPreset reverbPreset in Resources.FindObjectsOfTypeAll<ReverbPreset>())
                    TryAddReference(OriginalContent.ReverbPresets, reverbPreset);

                OriginalContent.SelectableLevels = new List<SelectableLevel>(StartOfRound.Instance.levels.ToList());
                OriginalContent.MoonsCatalogue = new List<SelectableLevel>(Terminal_Patch.Terminal.moonsCatalogueList.ToList());
            }
            DebugHelper.DebugScrapedVanillaContent();
        }

        internal static void ExtractSelectableLevelReferences(SelectableLevel selectableLevel)
        {
            foreach (SpawnableEnemyWithRarity enemyWithRarity in selectableLevel.Enemies)
                TryAddReference(OriginalContent.Enemies, enemyWithRarity.enemyType);

            foreach (SpawnableEnemyWithRarity enemyWithRarity in selectableLevel.OutsideEnemies)
                TryAddReference(OriginalContent.Enemies, enemyWithRarity.enemyType);

            foreach (SpawnableEnemyWithRarity enemyWithRarity in selectableLevel.DaytimeEnemies)
                TryAddReference(OriginalContent.Enemies, enemyWithRarity.enemyType);

            foreach (SpawnableMapObject spawnableMapObject in selectableLevel.spawnableMapObjects)
                TryAddReference(OriginalContent.SpawnableMapObjects, spawnableMapObject.prefabToSpawn);

            foreach (SpawnableOutsideObjectWithRarity spawnableOutsideObject in selectableLevel.spawnableOutsideObjects)
                TryAddReference(OriginalContent.SpawnableOutsideObjects, spawnableOutsideObject.spawnableObject);

            TryAddReference(OriginalContent.LevelAmbienceLibraries, selectableLevel.levelAmbienceClips);
        }

        internal static void ExtractDungeonFlowReferences(DungeonFlow dungeonFlow)
        {
            foreach (Tile tile in dungeonFlow.GetTiles())
                foreach (RandomScrapSpawn randomScrapSpawn in tile.gameObject.GetComponentsInChildren<RandomScrapSpawn>())
                    TryAddReference(OriginalContent.ItemGroups, randomScrapSpawn.spawnableItems);
        }

        internal static void TryAddReference<T>(List<T> referenceList, T reference) where T : UnityEngine.Object
        {
            if (!referenceList.Contains(reference))
                referenceList.Add(reference);
        }
    }
}
