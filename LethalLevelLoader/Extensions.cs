﻿using DunGen;
using DunGen.Graph;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LethalLevelLoader
{
    public static class Extensions
    {
        public static List<Tile> GetTiles(this DungeonFlow dungeonFlow)
        {
            List<Tile> tilesList = new List<Tile>();

            foreach (GraphNode dungeonNode in dungeonFlow.Nodes)
                foreach (TileSet dungeonTileSet in dungeonNode.TileSets)
                    foreach (GameObjectChance dungeonTileWeight in dungeonTileSet.TileWeights.Weights)
                        foreach (Tile dungeonTile in dungeonTileWeight.Value.GetComponentsInChildren<Tile>())
                            tilesList.Add(dungeonTile);

            foreach (GraphLine dungeonLine in dungeonFlow.Lines)
                foreach (DungeonArchetype dungeonArchetype in dungeonLine.DungeonArchetypes)
                {
                    foreach (TileSet dungeonTileSet in dungeonArchetype.BranchCapTileSets)
                        foreach (GameObjectChance dungeonTileWeight in dungeonTileSet.TileWeights.Weights)
                            foreach (Tile dungeonTile in dungeonTileWeight.Value.GetComponentsInChildren<Tile>())
                                tilesList.Add(dungeonTile);

                    foreach (TileSet dungeonTileSet in dungeonArchetype.TileSets)
                        foreach (GameObjectChance dungeonTileWeight in dungeonTileSet.TileWeights.Weights)
                            foreach (Tile dungeonTile in dungeonTileWeight.Value.GetComponentsInChildren<Tile>())
                                tilesList.Add(dungeonTile);
                }

            return (tilesList);
        }

        public static List<RandomMapObject> GetRandomMapObjects(this DungeonFlow dungeonFlow)
        {
            List<RandomMapObject> returnList = new List<RandomMapObject>();

            foreach (Tile dungeonTile in dungeonFlow.GetTiles())
                foreach (RandomMapObject randomMapObject in dungeonTile.gameObject.GetComponentsInChildren<RandomMapObject>())
                    returnList.Add(randomMapObject);

            return (returnList);
        }

        public static List<SpawnSyncedObject> GetSpawnSyncedObjects(this DungeonFlow dungeonFlow)
        {
            List<SpawnSyncedObject> returnList = new List<SpawnSyncedObject>();

            foreach (Tile dungeonTile in dungeonFlow.GetTiles())
            {
                foreach (Doorway dungeonDoorway in dungeonTile.gameObject.GetComponentsInChildren<Doorway>())
                {
                    foreach (GameObjectWeight doorwayTileWeight in dungeonDoorway.ConnectorPrefabWeights)
                        foreach (SpawnSyncedObject spawnSyncedObject in doorwayTileWeight.GameObject.GetComponentsInChildren<SpawnSyncedObject>())
                            returnList.Add(spawnSyncedObject);

                    foreach (GameObjectWeight doorwayTileWeight in dungeonDoorway.BlockerPrefabWeights)
                        foreach (SpawnSyncedObject spawnSyncedObject in doorwayTileWeight.GameObject.GetComponentsInChildren<SpawnSyncedObject>())
                            returnList.Add(spawnSyncedObject);
                }

                foreach (SpawnSyncedObject spawnSyncedObject in dungeonTile.gameObject.GetComponentsInChildren<SpawnSyncedObject>())
                    returnList.Add(spawnSyncedObject);
            }
            return (returnList);
        }

        public static void AddReferences(this CompatibleNoun compatibleNoun, TerminalKeyword firstNoun, TerminalNode firstResult)
        {
            compatibleNoun.noun = firstNoun;
            compatibleNoun.result = firstResult;
        }

        public static void AddCompatibleNoun(this TerminalKeyword terminalKeyword, TerminalKeyword newNoun,  TerminalNode newResult)
        {
            if (terminalKeyword.compatibleNouns == null)
                terminalKeyword.compatibleNouns = new CompatibleNoun[0];
            CompatibleNoun newCompataibleNoun = new CompatibleNoun();
            newCompataibleNoun.noun = newNoun;
            newCompataibleNoun.result = newResult;
            terminalKeyword.compatibleNouns = terminalKeyword.compatibleNouns.AddItem(newCompataibleNoun).ToArray();
        }

        public static void AddCompatibleNoun(this TerminalNode terminalNode, TerminalKeyword newNoun, TerminalNode newResult)
        {
            if (terminalNode.terminalOptions == null)
                terminalNode.terminalOptions = new CompatibleNoun[0];
            CompatibleNoun newCompataibleNoun = new CompatibleNoun();
            newCompataibleNoun.noun = newNoun;
            newCompataibleNoun.result = newResult;
            terminalNode.terminalOptions = terminalNode.terminalOptions.AddItem(newCompataibleNoun).ToArray();
        }
    }
}
