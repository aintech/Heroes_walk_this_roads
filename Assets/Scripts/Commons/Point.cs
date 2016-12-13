using UnityEngine;
using System.Collections;

public class Point {
    public int x;
    public int y;

    public Point () {}

    public Point (int x, int y) {
        this.x = x;
        this.y = y;
    }

    public void setXY (int x, int y) {
        this.x = x;
        this.y = y;
    }

    public void setPoint (Point point) {
        this.x = point.x;
        this.y = point.y;
    }

    public bool isSame (Point point) {
        return x == point.x && y == point.y;
    }

    public override string ToString () {
        return "(" + x.ToString() + ", " + y.ToString() + ")";
    }

    public override bool Equals(object obj) {
        if (obj == null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != typeof(Point))
            return false;
        Point other = (Point)obj;
        return x == other.x && y == other.y;
    }
    

    public override int GetHashCode() {
        unchecked {
            return x.GetHashCode() ^ y.GetHashCode();
        }
    }
}