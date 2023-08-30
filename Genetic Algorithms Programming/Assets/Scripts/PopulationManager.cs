﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopulationManager : MonoBehaviour {

	public GameObject botPrefab;
	public GameObject[] startingPos;
	public int populationSize = 50;
	List<GameObject> population = new List<GameObject>();
	public static float elapsed = 0;
	public float trialTime = 10;
	public float timeScale = 2;
	int generation = 1;
	public GenerateMaze maze;

	GUIStyle guiStyle = new GUIStyle();
	void OnGUI()
	{
		guiStyle.fontSize = 25;
		guiStyle.normal.textColor = Color.white;
		GUI.BeginGroup (new Rect (10, 10, 250, 150));
		GUI.Box (new Rect (0,0,140,140), "Stats", guiStyle);
		GUI.Label(new Rect (10,25,200,30), "Gen: " + generation, guiStyle);
		GUI.Label(new Rect (10,50,200,30), string.Format("Time: {0:0.00}",elapsed), guiStyle);
		GUI.Label(new Rect (10,75,200,30), "Population: " + population.Count, guiStyle);
		GUI.EndGroup ();
	}


	// Use this for initialization
	void Start ()
	{
		for(int i = 0; i < populationSize; i++)
		{
			int starti = Random.Range(0, startingPos.Length);
			GameObject b = Instantiate(botPrefab, startingPos[starti].transform.position, this.transform.rotation);
			b.transform.Rotate(0, Mathf.Round(Random.Range(-90, 91) / 90) * 90, 0);
			b.GetComponent<Brain>().Init();
			population.Add(b);
		}

		Time.timeScale = timeScale;
	}

	GameObject Breed(GameObject parent1, GameObject parent2)
	{
		int starti = Random.Range(0, startingPos.Length);
		GameObject offspring = Instantiate(botPrefab, startingPos[starti].transform.position, this.transform.rotation);
        offspring.transform.Rotate(0, Mathf.Round(Random.Range(-90, 91) / 90) * 90, 0);
		Brain b = offspring.GetComponent<Brain>();
		
		if (Random.Range(0, 100) == 1)
		{
			b.Init();
		}
		else
		{
			b.Init();
			b.dna.Combine(parent1.GetComponent<Brain>().dna, parent2.GetComponent<Brain>().dna);
		}
       
		return offspring;
	}

	void BreedNewPopulation()
	{
		List<GameObject> sortedList = population.OrderByDescending(o => o.GetComponent<Brain>().eggsFound).ToList();
		string eggsCollected = "Generation: " + generation;
	
		foreach (GameObject g in sortedList)
		{
			eggsCollected += ", " + g.GetComponent<Brain>().eggsFound;
		}
	
		Debug.Log("Eggs: " + eggsCollected);
		population.Clear();

		while (population.Count < populationSize)
		{
			int bestParentCutoff = sortedList.Count / 4;

			for (int i = 0; i < bestParentCutoff - 1; i++)
			{
				for (int j = 1; j < bestParentCutoff; j++)
				{
					population.Add(Breed(sortedList[i], sortedList[j]));
					if (population.Count == populationSize) break;
                    population.Add(Breed(sortedList[j], sortedList[i]));
                    if (population.Count == populationSize) break;
                }
                if (population.Count == populationSize) break;
            }
        }

		for (int i = 0; i < sortedList.Count; i++)
		{
			Destroy(sortedList[i]);
		}

		generation++;
	}
	
	void Update ()
	{
		elapsed += Time.deltaTime;
		if (elapsed >= trialTime)
		{
			maze.Reset();
			BreedNewPopulation();
			elapsed = 0;
		}
	}
}
