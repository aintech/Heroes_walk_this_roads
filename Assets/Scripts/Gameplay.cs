using UnityEngine;
using System;
using System.Collections;

public class Gameplay : MonoBehaviour {
    
    public Transform itemPrefab;

    public static Hideable topHideable;

    public Story story { get; private set; }

    public Town town { get; private set; }

    public StatusScreen statusScreen { get; private set; }

    public ItemDescriptor itemDescriptor { get; private set; }

    public FightScreen fightScreen { get; private set; }

    public QuantityPopup quantityPopup { get; private set; }

    public World world { get; private set; }

    void Awake () {
        Vars.gameplay = this;
        ItemFactory.itemPrefab = itemPrefab;

        Imager.init();
		foreach (HeroType hType in Enum.GetValues(typeof(HeroType))) {
			Vars.heroes.Add(hType, new Hero().init(hType));
		}

        Player.init();

        Camera.main.GetComponent<Utils>().init();

        Transform commons = GameObject.Find("Commons").transform;
        quantityPopup = commons.Find("Quantity Popup").GetComponent<QuantityPopup>().init();
        commons.Find("Images Provider").GetComponent<ImagesProvider>().init();
        itemDescriptor = commons.Find("Item Descriptor").GetComponent<ItemDescriptor>().init();

        statusScreen = GameObject.Find("Status Screen").GetComponent<StatusScreen>().init(itemDescriptor);
        Vars.userInterface = GameObject.FindGameObjectWithTag("UserInterface").GetComponent<UserInterface>().init();

        fightScreen = GameObject.Find("Fight Screen").GetComponent<FightScreen>().init();

        world = GameObject.Find("World").GetComponent<World>().init(fightScreen);

        town = GameObject.Find("Town").GetComponent<Town>().init(world);

        story = GameObject.Find("Story Teller").GetComponent<Story>().init(town);

        startGame();
    }

    public void startGame () {
        statusScreen.inventory.fillWithRandomItems(50, "Player Item");
        statusScreen.inventory.calculateFreeVolume();
        town.walkInTown(LocationType.ROUTINE);
    }
}