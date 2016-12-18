using UnityEngine;
using System.Collections;

public class EnemyMarker : MonoBehaviour {

	private SpriteRenderer image;

	public Transform trans { get; private set; }

	public EnemyType enemyType { get; private set; }

	private float waitTime, minWaitTime = 1, maxWaitTime = 7;

	private bool inMotion;

    public Point position { get; private set; }

    private Vector3 newPos = Vector3.zero;

    private World world;

    public bool alive { get; private set; }

    private Location location;

    public EnemyMarker init (World world, Transform landscape, EnemyType enemyType, Location location) {
        this.world = world;
		trans = transform;
        image = GetComponent<SpriteRenderer>();
        trans.SetParent(landscape);
        resetMarker(enemyType, location);

		return this;
	}

	private void findNewTargetPosition () {
//		dist = Random.value * ScanningScreen.FIELD_RADIUS;
//		angle = Random.value * 360f;
//		targetPosition.x = dist * Mathf.Cos(angle);
//		targetPosition.y = dist * Mathf.Sin(angle);
	}

    public void resetMarker (EnemyType enemyType, Location location) {
		this.enemyType = enemyType;
        this.location = location;
        alive = true;
        image.sprite = ImagesProvider.getEnemyMarker(enemyType);
        initPos();
	}

    private void initPos () {
        Point initPoint = location.position;
        do {
            position = new Point(Random.Range(initPoint.x - 10, initPoint.x + 10), Random.Range(initPoint.y - 10, initPoint.y + 10));
        } while (!world.worldMap[position.x, position.y] && !world.isLocationPoint(position));
        moveToPos();
    }

    public void moveToPos () {
        newPos.x = position.x * world.cellSize;
        newPos.y = position.y * world.cellSize;
        trans.localPosition = newPos;
    }

    public void disable () {
        alive = false;
        gameObject.SetActive(false);
    }
}