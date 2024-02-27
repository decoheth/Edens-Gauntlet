using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using TMPro;
public class BuildingManager : MonoBehaviour
{


    [Header("Build Objects SO")]
    [SerializeField] private List<BuildingSO> floorObjects = new List<BuildingSO>();
    [SerializeField] private List<BuildingSO> wallObjects = new List<BuildingSO>();
    [SerializeField] private List<BuildingSO> utilityObjects = new List<BuildingSO>();
    [SerializeField] private List<BuildingSO> buildingObjects = new List<BuildingSO>();
    [SerializeField] private List<BuildingSO> plantObjects = new List<BuildingSO>();



    [Header("Build Settings")]
    [SerializeField] private SelectedBuildType currentBuildType;
    [SerializeField] private LayerMask connectorLayer;


    [Header("Destroy Settings")]
    [SerializeField] private bool isDestroying = false;
    private Transform lastHitDestroyTransform;
    private List<Material> LastHitMaterials = new List<Material>();

    [Header("Preview Objects")]
    [SerializeField] private Material previewMaterialValid;
    [SerializeField] private Material previewMaterialInvalid;
    [SerializeField] private float connectorOverlapRadius = 1;
    [SerializeField] private float maxGroundAngle = 45f;

    [Header("Internal State")]
    [SerializeField] public bool isBuilding = false;
    [SerializeField] private int currentBuildingIndex;
    private GameObject previewBuildGameobject;
    private bool isPreviewInValidPosition = false;
    private Transform ModelParent = null;

    [Header("UI")]
    
    [SerializeField] private GameObject buildingMenu;
    [SerializeField] private GameObject buildingUI;
    [SerializeField] private TMP_Text destroyText;



    Transform structuresParent;
    private Player player;
    private GameObject uiManager;
    private GameObject enemyManager;


    void Awake()
    {
        structuresParent = GameObject.Find("Structures").transform;
        uiManager = GameObject.Find("/Managers/UI Manager");
        enemyManager = GameObject.Find("/Managers/Enemy Manager");
        player = GameObject.FindWithTag("Player").GetComponent<Player>();

        

    }
    private void Update()
    {
        // Activate Build Mode
        if(Input.GetKeyDown(KeyCode.B))
        {
            isBuilding = !isBuilding;

            if(isBuilding == true)
            {
                //Disable combat
                player.ToggleCombat(false);
            }

            if(isBuilding == false)
            {
                player.ToggleCombat(true);
            }

        }

        // Toggle Destroy Mode
        if(buildingUI.activeInHierarchy)
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                destoryBuildingToggle();
            }
        }

        // Toggle Build Menu
        if(isBuilding || buildingMenu.activeInHierarchy)
        {
            if(Input.GetKeyDown(KeyCode.Tab))
                toggleBuildingMenu(!buildingMenu.activeInHierarchy);
        }

        if(isBuilding && !isDestroying)
        {
            previewBuild();

            if(Input.GetMouseButtonDown(0))
                placeBuild();

        }
        else if (previewBuildGameobject)
        {
            Destroy(previewBuildGameobject);
            previewBuildGameobject = null;
        }

        if(isDestroying)
        {
            previewDestroy();
            if(Input.GetMouseButtonDown(0))
                destroyBuild();
        }
    }

    private void previewBuild()
    {
        GameObject currentBuild = getCurrentBuild();
        createPreviewPrefab(currentBuild);

        movePreviewToRaycast();
        checkBuildValidity();
    }

    private void createPreviewPrefab(GameObject currentBuild)
    {
        if(previewBuildGameobject == null)
        {
            previewBuildGameobject = Instantiate(currentBuild);

            ModelParent = previewBuildGameobject.transform.GetChild(0);

            previewifyModel(ModelParent, previewMaterialValid);
            previewifyModel(previewBuildGameobject.transform);
        }
    }

    private void movePreviewToRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            previewBuildGameobject.transform.position = hit.point;
        }
    }

    private void checkBuildValidity()
    {
        Collider[] colliders = Physics.OverlapSphere(previewBuildGameobject.transform.position, connectorOverlapRadius, connectorLayer);
        if(colliders.Length > 0)
        {
            previewConnectBuild(colliders);
        }
        else
        {
            previewSeperateBuild();

            if(isPreviewInValidPosition)
            {
                Collider[] overlapColliders = Physics.OverlapBox(previewBuildGameobject.transform.position, new Vector3(2f,2f,2f), previewBuildGameobject.transform.rotation);
                foreach(Collider overlapCollider in overlapColliders)
                {
                    if(overlapCollider.gameObject != previewBuildGameobject && overlapCollider.transform.root.CompareTag("Buildables"))
                    {
                        previewifyModel(ModelParent, previewMaterialInvalid);
                        isPreviewInValidPosition = false;
                        return;
                    }
                }
            }
        }
    }

    private void previewConnectBuild(Collider[] colliders)
    {
        Connector bestConnector = null;

        foreach(Collider collider in colliders)
        {
            Connector connector = collider.GetComponent<Connector>();

            if (connector.canConnectTo)
            {
                bestConnector = connector;
                break;
            }
        }

        if(bestConnector == null || currentBuildType == SelectedBuildType.FLOOR && bestConnector.isConnectedToFloor || currentBuildType == SelectedBuildType.WALL && bestConnector.isConnectedToWall)
        {
            previewifyModel(ModelParent, previewMaterialInvalid);
            isPreviewInValidPosition = false;
            return;
        }

        snapPreviewPrefabToConnector(bestConnector);     
    }

    private void snapPreviewPrefabToConnector(Connector connector)
    {
        Transform previewConnector = findSnapConnector(connector.transform, previewBuildGameobject.transform.GetChild(1));
        previewBuildGameobject.transform.position = connector.transform.position - (previewConnector.position - previewBuildGameobject.transform.position);

        if(currentBuildType == SelectedBuildType.WALL)
        {
            Quaternion newRotation = previewBuildGameobject.transform.rotation;
            newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, connector.transform.rotation.eulerAngles.y, newRotation.eulerAngles.z);
            previewBuildGameobject.transform.rotation = newRotation;
        }

        previewifyModel(ModelParent, previewMaterialValid);
        isPreviewInValidPosition = true;
    }

    private void previewSeperateBuild()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            if(currentBuildType == SelectedBuildType.WALL)
            {
                previewifyModel(ModelParent, previewMaterialInvalid);
                isPreviewInValidPosition = false;
                return;
            }


            if(Vector3.Angle(hit.normal, Vector3.up) < maxGroundAngle)
            {
                previewifyModel(ModelParent, previewMaterialValid);
                isPreviewInValidPosition = true;
            }

            else
            {
                previewifyModel(ModelParent, previewMaterialInvalid);
                isPreviewInValidPosition = false;
            }
        }
    }

    private Transform findSnapConnector (Transform snapConnector, Transform previewConnectorParent)
    {
        ConnectorPosition OppositeConnectorTag = getOppositePosition(snapConnector.GetComponent<Connector>());

        foreach (Connector connector in previewConnectorParent.GetComponentsInChildren<Connector>())
        {
            if(connector.connectorPosition == OppositeConnectorTag)
                return connector.transform;
        }

        return null;
    }

    private ConnectorPosition getOppositePosition(Connector connector)
    {
        ConnectorPosition position = connector.connectorPosition;

        if(currentBuildType == SelectedBuildType.WALL && connector.connectorParentType == SelectedBuildType.FLOOR)
            return ConnectorPosition.BOTTOM;

        if(currentBuildType == SelectedBuildType.FLOOR && connector.connectorParentType == SelectedBuildType.WALL && connector.connectorPosition == ConnectorPosition.TOP)
        {
            if(connector.transform.root.rotation.y == 0)
            {
                return getConnectorClosestToPlayer(true);
            }
            else
            {
                return getConnectorClosestToPlayer(false);
            }
        }
        
        switch(position)
        {
            case ConnectorPosition.LEFT:
                return ConnectorPosition.RIGHT;
            case ConnectorPosition.RIGHT:
                return ConnectorPosition.LEFT;
            case ConnectorPosition.BOTTOM:
                return ConnectorPosition.TOP;
            case ConnectorPosition.TOP:
                return ConnectorPosition.BOTTOM;
            default:
                return ConnectorPosition.BOTTOM;
        }
    }   

    private ConnectorPosition getConnectorClosestToPlayer(bool topBottom)
    {
        Transform cameraTransform = Camera.main.transform;

        if(topBottom)
            return cameraTransform.position.z >= previewBuildGameobject.transform.position.z ? ConnectorPosition.BOTTOM : ConnectorPosition.TOP;
        else
            return cameraTransform.position.x >= previewBuildGameobject.transform.position.x ? ConnectorPosition.LEFT : ConnectorPosition.RIGHT;

    }

    private void previewifyModel(Transform modelParent, Material previewMaterial = null)
    {
        if(previewMaterial != null)
        {
            foreach(MeshRenderer meshRenderer in modelParent.GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.material = previewMaterial;
            }
        }
        else
        {
            foreach(Collider modelColliders in modelParent.GetComponentsInChildren<Collider>())
            {
                modelColliders.enabled = false;
            }
        }
    }

    private GameObject getCurrentBuild()
    {
        switch(currentBuildType)
        {
            case SelectedBuildType.FLOOR:
                return floorObjects[currentBuildingIndex].prefab;
            case SelectedBuildType.WALL:
                return wallObjects[currentBuildingIndex].prefab;
            case SelectedBuildType.UTILITY:
                return utilityObjects[currentBuildingIndex].prefab;
            case SelectedBuildType.BUILDING:
                return buildingObjects[currentBuildingIndex].prefab;
            case SelectedBuildType.PLANT:
                return plantObjects[currentBuildingIndex].prefab;
        }

        return null;
    }

    private void placeBuild ()
    {
        if(previewBuildGameobject != null && isPreviewInValidPosition)
        {
            GameObject newBuild = Instantiate(getCurrentBuild(), previewBuildGameobject.transform.position, previewBuildGameobject.transform.rotation);

            Destroy(previewBuildGameobject);
            previewBuildGameobject = null;

            //isBuilding = false;

            foreach(Connector connector in newBuild.GetComponentsInChildren<Connector>())
            {
                connector.updateConnectors(true);
            }

        }
    }

    private void previewDestroy()
    {
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            if (hit.transform.root.CompareTag("Buildables"))
            {
                if(!lastHitDestroyTransform)
                {
                    lastHitDestroyTransform = hit.transform.root;

                    LastHitMaterials.Clear();
                    foreach(MeshRenderer lastHitMeshRenderers in lastHitDestroyTransform.GetComponentsInChildren<MeshRenderer>())
                    {
                        LastHitMaterials.Add(lastHitMeshRenderers.material);
                    }

                    previewifyModel(lastHitDestroyTransform.GetChild(0), previewMaterialInvalid);
                }

                else if (hit.transform.root != lastHitDestroyTransform)
                {
                    resetLastHitDestroyTransform();
                }
            }

            else if (lastHitDestroyTransform)
            {
                resetLastHitDestroyTransform();
            }
            
        }
    }

    private void resetLastHitDestroyTransform()
    {
        int counter = 0;
        foreach(MeshRenderer lastHitMeshRenderers in lastHitDestroyTransform.GetComponentsInChildren<MeshRenderer>())
        {
            lastHitMeshRenderers.material = LastHitMaterials[counter];
            counter++;

        }
        lastHitDestroyTransform = null;
    }


    private void destroyBuild()
    {
        if(lastHitDestroyTransform)
        {
            foreach(Connector connector in lastHitDestroyTransform.GetComponentsInChildren<Connector>())
            {
                connector.gameObject.SetActive(false);
                connector.updateConnectors(true);
            }
            Destroy(lastHitDestroyTransform.gameObject);
            // TODO: Refund build cost to player

            //destoryBuildingToggle(true);
            lastHitDestroyTransform = null;
        }
    }


    public void toggleBuildingMenu(bool active)
    {
        isBuilding = !active;

        buildingMenu.SetActive(active);

        // Disable camera sensitivity
        Camera.main.GetComponent<MouseLook>().enabled = !active;

        Cursor.visible = active;
        Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void destoryBuildingToggle(bool fromScript = false)
    {
        if (fromScript)
        {
            isDestroying = false;
            destroyText.text = "F: Destroy Off";
            destroyText.color = Color.green;
        }
        else
        {
            isDestroying = !isDestroying;
            destroyText.text = isDestroying ? "F: Destroy On" : "F: Destroy Off";
            destroyText.color = isDestroying ? Color.red : Color.black;
        }
    }
    
    public void changeBuildTypeButton(string selectedBuildType)
    {
        if(System.Enum.TryParse(selectedBuildType, out SelectedBuildType result))
        {
            currentBuildType = result;
        }
        else
        {
            Debug.Log("Build Type Doesn't Exist");
        }
    }

    public void startBuildingButton(int buildIndex)
    {
        currentBuildingIndex = buildIndex;
        toggleBuildingMenu(false);

        isBuilding = true;
    }




}

[System.Serializable]
public enum SelectedBuildType
{
    FLOOR,
    WALL,
    UTILITY,
    BUILDING,
    PLANT,
}