using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour {

	public static World instance { get; private set; }

    public Transform locationPrefab, enemyMarkerPrefab;

    private const float TILE_SIDE = 64;

    private int rowTiles, columnTiles;

    private Transform hero, landscape, worldContainer, locationsContainer;

    private Vector3 landscapePosition = Vector3.zero, worldPosition = Vector3.zero;

    public bool[,] worldMap { get; private set; }

    private Point currPoint = new Point(), tempPoint = new Point();

    public float cellSize { get; private set; }

    private bool upPress, downPress, leftPress, rightPress;

    private float timeCounter, nextActionTime = .2f;

    private Vector2 screenSize;

    private Dictionary<LocationType, Location> worldLocations = new Dictionary<LocationType, Location>();

    private Dictionary<Point, LocationType> locationPositions = new Dictionary<Point, LocationType>();

    private List<EnemyMarker> enemies = new List<EnemyMarker>();

    private EnemyMarker fightEnemy;

    private Location fightLocation;

    [HideInInspector]
    public Town town;

    public World init () {
		instance = this;
		FightScreen.instance.world = this;

        worldContainer = transform.Find("World Container");
        hero = worldContainer.Find("Hero");
        landscape = worldContainer.Find("Landscape");

        Sprite landSprite = landscape.Find("Land").GetComponent<SpriteRenderer>().sprite;

        Vector2 size = landSprite.rect.size;
        Vector2 worldSize = landSprite.bounds.size;

        cellSize = worldSize.x / (size.x / TILE_SIDE);

        screenSize = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        screenSize.x /= cellSize;
        screenSize.y /= cellSize;

        worldMap = new bool[Mathf.RoundToInt(size.x / TILE_SIDE), Mathf.RoundToInt(size.y / TILE_SIDE)];

        fillObstacles(landscape.Find("Obstacles").GetComponent<SpriteRenderer>().sprite.texture);

        locationsContainer = landscape.Find("Locations");

        worldContainer.gameObject.SetActive(true);

        landscapePosition = landscape.localPosition;

        foreach(LocationType locType in Enum.GetValues(typeof(LocationType))) {
            addLocation(locType);
        }

        fillWithEnemies();

        gameObject.SetActive(false);
		return this;
	}

    private void fillObstacles (Texture2D obstacles) {
        int tileSide = (int)TILE_SIDE;
        int halfTile = Mathf.RoundToInt((float)TILE_SIDE * .5f);
        for (int i = 0; i < worldMap.GetLength(0); i++) {
            for (int j = 0; j < worldMap.GetLength(1); j++) {
                worldMap[i,j] = obstacles.GetPixel(halfTile + (i * tileSide), halfTile + (j * tileSide)).a < .01f;
            }
        }
    }

    public void addLocation (LocationType type) {
        Location location = Instantiate<Transform>(locationPrefab).GetComponent<Location>().init(type, locationsContainer, cellSize);
        worldLocations.Add(type, location);
        locationPositions.Add(location.position, type);
    }

    private void fillWithEnemies () {
//		foreach (Location location in worldLocations.Values) {
//			if (location.type.isEnemyCamp()) {
//				enemies.AddRange(location.spawn(landscape, enemyMarkerPrefab));
//			}
//		}
        enemies.Add(Instantiate<Transform>(enemyMarkerPrefab).GetComponent<EnemyMarker>().init(landscape, new List<EnemyType>(new EnemyType[]{EnemyType.ROGUE, EnemyType.ROGUE, EnemyType.ROGUE}), worldLocations[LocationType.ROUTINE]));
    }

    public void showWorld (LocationType type) {
        currPoint.setPoint(worldLocations[type].position);
        currPoint.y--;
        tempPoint.setPoint(currPoint);
        adjustWorld();
        timeCounter = Time.time;
        gameObject.SetActive(true);
    }

    void Update () {
        if (fightEnemy != null) {
            FightScreen.instance.startFight(fightEnemy.enemyTypes);
            enabled = false;
        }
        if (timeCounter < Time.time && checkBtnPress()) {
            takeStep();
        }
    }

    private bool checkBtnPress () {
        upPress = Input.GetKey(KeyCode.UpArrow);
        downPress = Input.GetKey(KeyCode.DownArrow);
        leftPress = Input.GetKey(KeyCode.LeftArrow);
        rightPress = Input.GetKey(KeyCode.RightArrow);

        return upPress || downPress || leftPress || rightPress;
    }

    private void takeStep () {
        tempPoint.x += rightPress? 1: leftPress? -1: 0;
        tempPoint.y += upPress? 1: downPress? -1: 0;
        if (checkPosition(tempPoint)) {
            currPoint.setPoint(tempPoint);
        }
        tempPoint.setPoint(currPoint);
        adjustWorld();
        moveEnemy();
    }

    private bool checkPosition (Point point) {
        if (locationPositions.ContainsKey(point)) {
            if (!worldLocations[locationPositions[point]].isRuined) {
                visitLocation(locationPositions[point]);
                return true;
            }
        }
        timeCounter = Time.time + nextActionTime;
        return worldMap[point.x, point.y];
    }

    public bool isLocationPoint (Point point) {
        return locationPositions.ContainsKey(point);
    }

    private void adjustWorld () {
        landscapePosition.x = -currPoint.x * cellSize;
        landscapePosition.y = -currPoint.y * cellSize;
        if (currPoint.x < screenSize.x) {
            worldPosition.x = (currPoint.x - screenSize.x) * cellSize;
        } else if (currPoint.x > (worldMap.GetLength(0) - screenSize.x)) {
            worldPosition.x = (currPoint.x - (worldMap.GetLength(0) - screenSize.x)) * cellSize;
        } else {
            worldPosition.x = 0;
        }
        if (currPoint.y < screenSize.y) {
            worldPosition.y = (currPoint.y - screenSize.y) * cellSize;
        } else if (currPoint.y > (worldMap.GetLength(1) - screenSize.y)) {
            worldPosition.y = (currPoint.y - (worldMap.GetLength(1) - screenSize.y)) * cellSize;
        } else {
            worldPosition.y = 0;
        }
        landscape.localPosition = landscapePosition;
        worldContainer.localPosition = worldPosition;
    }

    private void moveEnemy () {
        checkEnemyCollision();
    }

    private void checkEnemyCollision () {
        foreach (EnemyMarker enemy in enemies) {
            if (enemy.alive && enemy.position.isSame(currPoint)) {
                fightEnemy = enemy;
                return;
            }
        }
    }

    public void backFromFight (bool playerWin) {
        if (playerWin) {
            if (fightEnemy != null) {
                fightEnemy.disable();
                fightEnemy = null;
            }
            if (fightLocation != null) {
                fightLocation.ruin();
                fightLocation = null;
            }
        } else {
            foreach (KeyValuePair<HeroType, Hero> pair in Vars.heroes) {
                pair.Value.setHealthToMax();
            }
            visitLocation(LocationType.ROUTINE);
        }
        enabled = true;
        checkEnemyCollision();
    }

    private void visitLocation (LocationType type) {
        if (type.isTown()) {
            town.walkInTown(type);
            gameObject.SetActive(false);
        } else if (type.isEnemyCamp()) {
            fightLocation = worldLocations[type];
			FightScreen.instance.startFight(type.boss());
            enabled = false;
        }
    }
}