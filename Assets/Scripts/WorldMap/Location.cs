using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Location : MonoBehaviour{

    public Sprite ruinedSprite;

    public LocationType type { get; private set; }

    public Point position { get; private set; }

    public bool isRuined { get; private set; }

    public Location init (LocationType type, Transform container, float cellSize) {
        this.type = type;
        isRuined = false;
        position = type.position();
        GetComponent<SpriteRenderer>().sprite = ImagesProvider.getLocationMarker(type);
        transform.SetParent(container);
        transform.localPosition = new Vector3(position.x * cellSize, position.y * cellSize, 0);
        return this;
    }

	public List<EnemyMarker> spawn (Transform landscape, Transform enemyMarkerPrefab) {
		List<EnemyMarker> markers = new List<EnemyMarker>();
		EnemyType[] enemyTypes = type.spawn();
		List<EnemyType> types = new List<EnemyType>();
		int enemiesInPack = UnityEngine.Random.Range(1, 3);
		for (int i = 0; i < enemiesInPack; i++) {
			types.Add(enemyTypes[UnityEngine.Random.Range(0, enemyTypes.Length)]);
		}
		for (int i = 0; i < 5; i++) {
			EnemyMarker marker = Instantiate<Transform>(enemyMarkerPrefab).GetComponent<EnemyMarker>().init(landscape, types, this);
			markers.Add(marker);
		}
		return markers;
	}

    public void ruin () {
        isRuined = true;
        GetComponent<SpriteRenderer>().sprite = ruinedSprite;
    }
}