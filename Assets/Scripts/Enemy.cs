using UnityEngine;
using System.Collections;

public class Enemy : Character {
    
    public EnemyType type { get; private set; }

    public Enemy init (EnemyType type) {
        this.type = type;
        innerInit(type.strenght(), type.endurance(), type.agility());
        return this;
    }

    public override int damage () { return strength; }

    public override bool isHero () { return false; }
    public override string name () { return type.name(); }
}