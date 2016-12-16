﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LocationType {
    ROUTINE, BANDIT_FORTRESS
}

public static class LocationDescriptor {

    private static Dictionary<LocationType, Point> positions;

    private static Dictionary<LocationType, EnemyType> bosses;

    private static Dictionary<LocationType, EnemyType[]> spawns;

    public static Point position (this LocationType type) {
        if (positions == null) {
            positions = new Dictionary<LocationType, Point>();
            positions.Add(LocationType.ROUTINE, new Point(5, 3));
            positions.Add(LocationType.BANDIT_FORTRESS, new Point(15, 15));
        }
        return positions[type];
    }

    public static bool isTown (this LocationType type) {
        return type == LocationType.ROUTINE;
    }

    public static bool isEnemyCamp (this LocationType type) {
        return type == LocationType.BANDIT_FORTRESS;
    }

    public static EnemyType boss (this LocationType type) {
        if (bosses == null) {
            bosses = new Dictionary<LocationType, EnemyType>();
            bosses.Add(LocationType.BANDIT_FORTRESS, EnemyType.ROGUE);
        }
        return bosses[type];
    }

    public static EnemyType[] spawn (this LocationType type) {
        if (spawns == null) {
            spawns = new Dictionary<LocationType, EnemyType[]>();
            spawns.Add(LocationType.BANDIT_FORTRESS, new EnemyType[]{EnemyType.ROGUE});
        }
        return spawns[type];
    }
}