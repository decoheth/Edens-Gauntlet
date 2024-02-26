using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class AIManager : MonoBehaviour
{

    [Header("NavMesh")]

    public NavMeshSurface navSurface;
    public GameObject navArea;
    public NavMeshData navData;
    public Vector3 navMeshSize;
    List<NavMeshBuildSource> navSources = new List<NavMeshBuildSource>();

    void Awake()
    {   
        navMeshSize = navArea.GetComponent<Collider>().bounds.size;

        navData = new NavMeshData();
        NavMesh.AddNavMeshData(navData);
    }

    public void Start()
    {
        BakeNavMesh(false);
    }

    public void BakeNavMesh(bool Async)
    {
        Bounds navBounds = new Bounds((new Vector3(0,0,-40)), navMeshSize);

        List<NavMeshBuildMarkup> markups = new List<NavMeshBuildMarkup>();

        List<NavMeshModifier> modifiers;
        if (navSurface.collectObjects == CollectObjects.Children)
        {
            modifiers = new List<NavMeshModifier>(GetComponentsInChildren<NavMeshModifier>());
        }
        else
        {
            modifiers = NavMeshModifier.activeModifiers;
        }

        for (int i = 0; i < modifiers.Count; i++)
        {
            if (((navSurface.layerMask & (1 << modifiers[i].gameObject.layer)) == 1)
                && modifiers[i].AffectsAgentType(navSurface.agentTypeID))
            {
                markups.Add(new NavMeshBuildMarkup()
                {
                    root = modifiers[i].transform,
                    overrideArea = modifiers[i].overrideArea,
                    area = modifiers[i].area,
                    ignoreFromBuild = modifiers[i].ignoreFromBuild
                });
            }
        }


        if (navSurface.collectObjects == CollectObjects.Children)
        {
            NavMeshBuilder.CollectSources(transform, navSurface.layerMask, navSurface.useGeometry, navSurface.defaultArea, markups, navSources);
        }
        else
        {
            NavMeshBuilder.CollectSources(navBounds, navSurface.layerMask, navSurface.useGeometry, navSurface.defaultArea, markups, navSources);
        }

        navSources.RemoveAll(source => source.component != null && source.component.gameObject.GetComponent<NavMeshAgent>() != null);

        if (Async)
        {
            NavMeshBuilder.UpdateNavMeshDataAsync(navData, navSurface.GetBuildSettings(), navSources, navBounds);
        }
        else
        {
            NavMeshBuilder.UpdateNavMeshData(navData, navSurface.GetBuildSettings(), navSources, navBounds);
        }
        Debug.Log("Navmesh Updated");
    }



}
