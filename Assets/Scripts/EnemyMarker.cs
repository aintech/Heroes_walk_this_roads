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

    public EnemyMarker init (World world, Transform landscape, EnemyType enemyType) {
        this.world = world;
		trans = transform;
        image = GetComponent<SpriteRenderer>();
        trans.SetParent(landscape);
		resetMarker(enemyType);

		return this;
	}

	private void findNewTargetPosition () {
//		dist = Random.value * ScanningScreen.FIELD_RADIUS;
//		angle = Random.value * 360f;
//		targetPosition.x = dist * Mathf.Cos(angle);
//		targetPosition.y = dist * Mathf.Sin(angle);
	}

	public void resetMarker (EnemyType enemyType) {
		this.enemyType = enemyType;
        alive = true;
        image.sprite = ImagesProvider.getEnemyMarkerSprite(enemyType);
		initPos();
	}

    private void initPos () {
        do {
            position = new Point(Random.Range(10, 30), Random.Range(10, 30));
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