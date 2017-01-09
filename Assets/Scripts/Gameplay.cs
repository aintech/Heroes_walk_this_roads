using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Gameplay : MonoBehaviour {
    
	public static Gameplay instance { get; private set; }

    public Transform itemPrefab;

    public static Hideable topHideable;

    public Story story { get; private set; }

    public Town town { get; private set; }

    public QuantityPopup quantityPopup { get; private set; }

    public World world { get; private set; }

    void Awake () {
		instance = this;

        ItemFactory.itemPrefab = itemPrefab;

        Imager.init();
		foreach (HeroType hType in Enum.GetValues(typeof(HeroType))) {
			Vars.heroes.Add(hType, new Hero().init(hType));
		}

        Camera.main.GetComponent<Utils>().init();

        Transform commons = GameObject.Find("Commons").transform;
        quantityPopup = commons.Find("Quantity Popup").GetComponent<QuantityPopup>().init();
        commons.Find("Images Provider").GetComponent<ImagesProvider>().init();
//        commons.Find("Item Descriptor").GetComponent<ItemDescriptor>().init();
        commons.Find("Item Descriptor 2").GetComponent<ItemDescriptor2>().init();

		GameObject.Find("Status Screen").GetComponent<StatusScreen>().init();
		GameObject.FindGameObjectWithTag("UserInterface").GetComponent<UserInterface>().init();

		GameObject.Find("Fight Screen").GetComponent<FightScreen>().init();

        world = GameObject.Find("World").GetComponent<World>().init();

        town = GameObject.Find("Town").GetComponent<Town>().init(world);

        story = GameObject.Find("Story Teller").GetComponent<Story>().init(town);

        startGame();
    }

    public void startGame () {
		StatusScreen.instance.inventory.fillWithRandomItems(50, "Player Item");
		StatusScreen.instance.inventory.calculateFreeVolume();
        giveItemsToHeroes();
        town.walkInTown(LocationType.ROUTINE);
    }

    private void giveItemsToHeroes () {
        foreach (KeyValuePair<HeroType, Hero> pair in Vars.heroes) {
            Item weapon = Instantiate<Transform>(ItemFactory.itemPrefab).GetComponent<Item>().init(ItemFactory.createWeaponData(WeaponType.NOBLE_SWORD));
            weapon.gameObject.SetActive(false);
            pair.Value.equipWeapon((WeaponData)weapon.itemData);
        }
    }
}