using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour {

    public Transform locationPrefab, enemyMarkerPrefab;

    public Sprite routineMarker;

    private Dictionary<Location, Town> towns = new Dictionary<Location, Town>();

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

    private Dictionary<Location, Point> worldLocations = new Dictionary<Location, Point>();

    private List<EnemyMarker> enemies = new List<EnemyMarker>();

    private FightScreen fightScreen;

    private EnemyMarker fightEnemy;

    public World init (FightScreen fightScreen) {
        this.fightScreen = fightScreen;
        this.fightScreen.world = this;

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

    private void fillWithEnemies () {
        for (int i = 0; i < 5; i++) {
            EnemyMarker marker = Instantiate<Transform>(enemyMarkerPrefab).GetComponent<EnemyMarker>().init(this, landscape, EnemyType.ROGUE);
            enemies.Add(marker);
        }
    }

    public void addTown (Location location, Town town) {
        addLocation(location);
        towns.Add(location, town);
    }

    public void addLocation (Location location) {
        worldLocations.Add(location, null);
        Transform loc = Instantiate<Transform>(locationPrefab);
        switch (location) {
            case Location.ROUTINE:
                loc.GetComponent<SpriteRenderer>().sprite = routineMarker;
                worldLocations[location] = new Point(5, 3);
                break;
            default: Debug.Log("Unknown location type: " + location); break;
        }
        loc.SetParent(locationsContainer);
        loc.localPosition = new Vector3(worldLocations[location].x * cellSize, worldLocations[location].y * cellSize, 0);
    }

    public void showWorld (Location location) {
        currPoint.setPoint(worldLocations[location]);
        currPoint.y--;
        tempPoint.setPoint(currPoint);
        adjustWorld();
        timeCounter = Time.time;
        gameObject.SetActive(true);
    }

    void Update () {
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
    }

    private bool checkPosition (Point point) {
        if (worldLocations.ContainsValue(point)) {
            foreach (KeyValuePair<Location, Point> pair in worldLocations) {
                if (point.isSame(pair.Value)) {
                    visitLocation(pair.Key);
                    return true;
                }
            }
        }
        timeCounter = Time.time + nextActionTime;
        return worldMap[point.x, point.y];
    }

    public bool isLocationPoint (Point point) {
        return worldLocations.ContainsValue(point);
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
        moveEnemy();
    }

    private void moveEnemy () {
        checkEnemyCollision();
    }

    private void checkEnemyCollision () {
        foreach (EnemyMarker enemy in enemies) {
            if (enemy.alive && enemy.position.isSame(currPoint)) {
                fightEnemy = enemy;
                fightScreen.startFight(enemy.enemyType);
                enabled = false;
                return;
            }
        }
    }

    public void backFromFight (bool winner) {
        fightEnemy.disable();
        enabled = true;
        checkEnemyCollision();
    }

    private void visitLocation (Location location) {
        towns[location].walkInTown();
        gameObject.SetActive(false);
    }

    public enum Location {
        ROUTINE
    }
}