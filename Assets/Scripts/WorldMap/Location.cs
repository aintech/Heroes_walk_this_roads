using UnityEngine;
using System.Collections;

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

    public void ruin () {
        isRuined = true;
        GetComponent<SpriteRenderer>().sprite = ruinedSprite;
    }
}