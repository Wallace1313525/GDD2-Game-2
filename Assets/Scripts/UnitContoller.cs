﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct UnitData
{
    [SerializeField]
    public Mesh mesh;
    [SerializeField]
    public int maxCount;
    [SerializeField]
    public GameObject prefab;
    public int activeCount;
    public Material material;
    public GameObject activeInstances;
    public GameObject inactiveInstances;
    //public Unit[] instances;
    public Matrix4x4[] transforms;
}
public class UnitContoller : MonoBehaviour
{
    public Player player;
    // selected units
    // all player units - array of arraylistsn
    // player landmarks
    // enemy units - array of arraylists
    public UnitData[] enemyUnitData;
    // enemy landmarks

    public EnemyNav enemyNav;
    public GameObject navControllerPrefab;
    public GameObject enemyNavController;


    void Start()
    {

        enemyNavController = Instantiate(navControllerPrefab, player.transform);
        enemyNav = enemyNavController.GetComponent<EnemyNav>();
        enemyUnitData = SetupUnitData(enemyUnitData);

        // TODO: temp fix for testing REMOVE THIS
        for (int a = 0; a < enemyUnitData.Length; a++)
        {
            for (int i = 0; i < enemyUnitData[a].activeInstances.transform.childCount; i++)
            {
                // TODO fix this but not really because we should just remove it anyway
                enemyUnitData[a].activeInstances.transform.GetChild(i).transform.position = new Vector3(UnityEngine.Random.Range(-10, 10), 0, UnityEngine.Random.Range(-10, 10));
                enemyUnitData[a].activeCount += 1;
            }

        }

    }

    private UnitData[] SetupUnitData(UnitData[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i].activeInstances = new GameObject();
            data[i].activeInstances.name = data[i].prefab.name + " active instances";
            data[i].inactiveInstances = new GameObject();
            data[i].inactiveInstances.name = data[i].prefab.name + " inactive instances";

            data[i].transforms = new Matrix4x4[data[i].maxCount];
            for (int j = 0; j < data[i].maxCount; j++)
            {
                Unit instance = Instantiate(data[i].prefab).GetComponent<Unit>();
                

                instance.gameObject.SetActive(false);

                instance.transform.parent = enemyUnitData[i].inactiveInstances.transform;
                //instance.transform.parent = enemyUnitData[i].activeInstances.transform;

                data[i].transforms[j] = Matrix4x4.TRS(instance.transform.position, instance.transform.rotation, instance.transform.localScale);
            }
        }
        return data;
    }

    void Update()
    {
        //TODO: instance render each type of unit and landmark
        UpdateUnitData(enemyUnitData);
        for (int i = 0; i < enemyUnitData.Length; i++)
        {
            InstanceRenderer.Render(enemyUnitData[i].mesh, enemyUnitData[i].material, enemyUnitData[i].transforms, enemyUnitData[i].activeCount);
        }
    }

    private void UpdateUnitData(UnitData[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            for (int j = 0; j < data[i].activeInstances.transform.childCount; j++)
            {
                Transform instanceTransform = data[i].activeInstances.transform.GetChild(j);
                data[i].transforms[j] = Matrix4x4.TRS(instanceTransform.position, instanceTransform.rotation, instanceTransform.localScale);
            }
        }
    }

    public void UnitTest(Vector3 target)
    {

        for (int i = 0; i < enemyUnitData.Length; i++)
        {
            for (int j = 0; j < enemyUnitData[i].activeInstances.transform.childCount; j++)
            {
                Unit instance = enemyUnitData[i].activeInstances.transform.GetChild(j).GetComponent<Unit>();
                //instance.PathTo(target);
            }
        }
    }

    public void FindPath(Vector3 hit)
    {
        enemyNav.FindPath(hit);
    }

    void DeselectUnits()
    {

    }

    void ToggleUnitSelection(ISelectable unit)
    {
    
    }

    public void SpawnUnits(/*TODO: input values*/)
    {
        Unit instance = enemyUnitData[0].inactiveInstances.transform.GetChild(0).GetComponent<Unit>();
        if (!instance) Debug.LogError("No inactive instances");

        instance.gameObject.SetActive(true);
        instance.transform.parent = enemyUnitData[0].activeInstances.transform;
        instance.transform.position = new Vector3(UnityEngine.Random.Range(-100,100), 0, UnityEngine.Random.Range(-100, 100));
        enemyUnitData[0].activeCount++;
        enemyNav.Units.Add(instance);
    }
}
