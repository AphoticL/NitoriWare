﻿using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class TouhouSortSorter : MonoBehaviour
{
	// Primary class for TouhouSort game handling
	// Handles objects and win/loss

	// Spacing between starting sortables
	static float GAP = 2.2f;

	// Max number of sortable touhous
	public int slotCount;

	public Transform stagingArea;
    public TouhouSortZoneManager zoneManager;

    public TouhouSortSortable touhouTemplate;

	//public TouhouSortTouhouBucket touhouBucket;
    public GameObject victoryDisplay;

	TouhouSortSortable[] touhous;
	Vector3[] slots;

	bool sorted;
    
    [System.Serializable]
    public struct Category
    {
        public string name;
        public Sprite leftIcon, rightIcon;
        public Sprite[] leftPool, rightPool;
    }
	//public Category[] categories;
    
    public struct Style
    {
        public string name;
        public Sprite sprite;
    }

    void Start() {
		Category category = (MicrogameController.instance.getTraits() as TouhouSortTraits).category;

        zoneManager.SetZoneAttributes(0, category.leftIcon, category.name, false);
        zoneManager.SetZoneAttributes(1, category.rightIcon, category.name, true);

        // Scoop up as many touhous as we can
        touhous = LoadTouhous (category, slotCount);
        
        slotCount = touhous.Length;

		sorted = false;

		// Fill starting slots with touhous
		CreateSlots ();
		FillSlots ();

		// Check the sort at the start, just in case
		CheckSort ();
	}

    TouhouSortSortable[] LoadTouhous(Category category, int amount)
    {
        List<Style> leftStyles = new List<Style>();
        List<Style> rightStyles = new List<Style>();

        foreach (Sprite sprite in category.leftPool)
        {
            Style style = new Style();
            style.name = category.name;
            style.sprite = sprite;

            leftStyles.Add(style);
        }
        foreach (Sprite sprite in category.rightPool)
        {
            Style style = new Style();
            style.sprite = sprite;

            rightStyles.Add(style);
        }
        
        MouseGrabbableGroup grabGroup = stagingArea.GetComponent<MouseGrabbableGroup>();
        TouhouSortSortable[] randomTouhous = new TouhouSortSortable[amount];

        UnityEvent dudEvent = new UnityEvent();
        UnityEvent sortEvent = new UnityEvent();
        sortEvent.AddListener(CheckSort);

        for (int i = 0; i < amount; i++)
        {
            Style style;
            int coin = Random.Range(0, 2);
            if (coin == 0)
                style = leftStyles[Random.Range(0, leftStyles.Count)];
            else
                style = rightStyles[Random.Range(0, rightStyles.Count)];

            // Build a new touhou instance
            TouhouSortSortable touhou = Instantiate(touhouTemplate, transform.position, transform.rotation);
            touhou.GetComponent<SpriteRenderer>().sprite = style.sprite;
            touhou.gameObject.AddComponent<PolygonCollider2D>();

            touhou.SetStyle(style.name);

            MouseGrabbable grab = touhou.gameObject.AddComponent<MouseGrabbable>();
            grab.onGrab = dudEvent;
            grab.onRelease = sortEvent;
            grab.disableOnVictory = true;
            
            touhou.transform.parent = stagingArea;
            
            grabGroup.addGrabbable(grab, true);
            randomTouhous[i] = touhou;
        }

        return randomTouhous;
    }

    void CreateSlots()
    {
		// Instantiate a list of Vector3 objects
		// which will be the starting spots of
		// our touhous
		slots = new Vector3[slotCount];
		Vector3 origin = stagingArea.position;

		if (slotCount % 2 == 0) {
			origin.x = origin.x + (GAP / 2);
		}

		for (int i = 0; i < slotCount; i++) {
			Vector3 slot = origin;
			int multiplier = (i + 1) / 2;

			if (i % 2 == 0) {
				slot.x = origin.x + (multiplier * GAP);
			}
			else {
				slot.x = origin.x - (multiplier * GAP);
			}

			slots [i] = slot;
		}
	}

	void FillSlots()
    {
		// Fill instantiated slots with sortable touhous
		for (int i = 0; i < slotCount; i++)
        {
			touhous [i].transform.position = slots [i];
		}
	}

	public void CheckSort()
    {
		// Check the current state of the sort
		// End the game if everything is sorted
		bool allSorted = true;

		foreach (TouhouSortSortable sortable in touhous)
        {
			bool thisSorted = false;

			// Get the touhou's current zone, if any
			TouhouSortDropZone currentZone = sortable.GetCurrentZone();
			
			if (currentZone)
            {
				thisSorted = currentZone.Belongs (sortable);
			}

			if (thisSorted != true)
            {
				allSorted = false;
				break;
			}
		}

		if (allSorted)
        {
			// Sorted
			//Debug.Log("Sorted");
			sorted = true;

            victoryDisplay.SetActive(true);

			MicrogameController.instance.setVictory(true, true);
		}
		else if (sorted)
        {
			// Unsorted (wont ever happen)
			Debug.Log("Unsorted");
			sorted = false;

            victoryDisplay.SetActive(false);

            MicrogameController.instance.setVictory(false, true);
		}
	}

}
