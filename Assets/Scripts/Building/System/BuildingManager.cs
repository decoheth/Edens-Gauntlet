using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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


    [Header("Build Region")]
    [SerializeField] private GameObject uiBuildIndicator;
    [SerializeField] private GameObject SelectionGrid;
    public bool canBuild;


    [Header("Building Menu")]
    private List<BuildingSO> unlockedObjects = new List<BuildingSO>();
    private List<int> unlockedObjectsIndex = new List<int>();
    public GameObject BuildItemMenuPrefab;


    // References
    private BuildingSO currentSO; 
    Transform structuresParent;
    private Player player;
    private PlayerInventory playerInventory;
    

    // Managers
    private GameObject uiManager;
    private GameObject enemyManager;
    private WaveManager waveManager;

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerCombat playerCombat;


    void Awake()
    {
        structuresParent = GameObject.Find("BuiltStructures").transform;
        uiManager = GameObject.Find("/Managers/UI Manager");
        enemyManager = GameObject.Find("/Managers/Enemy Manager");
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
        waveManager = enemyManager.GetComponent<WaveManager>();

        canBuild = false;
        isBuilding = false;
        //ToggleCanBuild(false);
        //ToggleBuildingMenu(false);

    }

    #region Update Function, checking inputs
    private void Update()
    {
        // Activate Build Mode
        if(Input.GetKeyDown(KeyCode.B) && canBuild && !buildingMenu.activeInHierarchy && waveManager.waveActive==false)
        {
            isBuilding = !isBuilding;

            player.ToggleCombat(!isBuilding);
        }

        if(canBuild == false)
        {
            //ToggleCanBuild(false);
            //ToggleBuildingMenu(false);
            uiBuildIndicator.SetActive(false);
        }
        
        if(buildingUI.activeInHierarchy)
        {
            // Toggle Destroy Mode
            if(Input.GetKeyDown(KeyCode.F))
                DestoryBuildingToggle();
            // Exit Build Mode on Escape
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                isBuilding = false;
                player.ToggleCombat(true);
            }
                
    
        }

        // Toggle Build Menu
        if(isBuilding || buildingMenu.activeInHierarchy)
        {
            if(Input.GetKeyDown(KeyCode.Tab))
                ToggleBuildingMenu(!buildingMenu.activeInHierarchy);
            if(Input.GetKeyDown(KeyCode.Escape))
                ToggleBuildingMenu(false);
        }

        if(isBuilding && !isDestroying && canBuild)
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

        
        if(!canBuild && isBuilding)
        {
            isBuilding = !isBuilding;
            Destroy(previewBuildGameobject);
            previewBuildGameobject = null;
        }

    }

    #endregion

    #region Previews
    public void previewBuild()
    {
        GameObject currentBuild = getCurrentBuild();
        createPreviewPrefab(currentBuild);

        movePreviewToRaycast();
        checkBuildValidity();
    }

    public void createPreviewPrefab(GameObject currentBuild)
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
    #endregion

    #region Build Connectors

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

    #endregion



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
            if(BuildCost())
            {
                GameObject newBuild = Instantiate(getCurrentBuild(), previewBuildGameobject.transform.position, previewBuildGameobject.transform.rotation);

                Destroy(previewBuildGameobject);
                previewBuildGameobject = null;

                foreach(Connector connector in newBuild.GetComponentsInChildren<Connector>())
                {
                    connector.updateConnectors(true);
                }
            }
            else
            {
                Debug.Log("Not Enough Resources");
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

            //DestoryBuildingToggle(true);
            lastHitDestroyTransform = null;
        }
    }

    #region Menus

    public void ToggleBuildingMenu(bool active)
    {
        isBuilding = !active;

        playerMovement.canMove = !active;
        playerCombat.canAttack = !active;
        playerCombat.canBlock = !active;

        buildingMenu.SetActive(active);
        if(active == true)
            LoadBuildingMenu(0);


        Cursor.visible = active;
        Cursor.lockState = active ? CursorLockMode.Confined : CursorLockMode.Locked;
    }

    public void DestoryBuildingToggle(bool fromScript = false)
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

    public void LoadBuildingMenu (int SelectedBuildMenu)
    {
        foreach (Transform child in SelectionGrid.transform) {
            GameObject.Destroy(child.gameObject);
        }

        unlockedObjects.Clear();
        // Index of unlocked objects in the master list
        unlockedObjectsIndex.Clear();
        int indx = 0;

        switch(SelectedBuildMenu)
        {
            case 0: 
                foreach(BuildingSO obj in floorObjects)
                {
                    if(obj.isUnlocked)
                    {
                        unlockedObjects.Add(obj);
                        unlockedObjectsIndex.Add(indx);

                    }
                    indx++;
                }
                break;
            case 1:  
                foreach(BuildingSO obj in wallObjects)
                {
                    if(obj.isUnlocked)
                    {
                        unlockedObjects.Add(obj);
                        unlockedObjectsIndex.Add(indx);

                    }
                    indx++;
                }
                break;
            case 2:
                foreach(BuildingSO obj in plantObjects)
                {
                    if(obj.isUnlocked)
                    {
                        unlockedObjects.Add(obj);
                        unlockedObjectsIndex.Add(indx);

                    }
                    indx++;
                }
                break;
            case 3:
                foreach(BuildingSO obj in utilityObjects)
                {
                    if(obj.isUnlocked)
                    {
                        unlockedObjects.Add(obj);
                        unlockedObjectsIndex.Add(indx);

                    }
                    indx++;
                }
                break;
            case 4:
                foreach(BuildingSO obj in buildingObjects)
                {
                    if(obj.isUnlocked)
                    {
                        unlockedObjects.Add(obj);
                        unlockedObjectsIndex.Add(indx);

                    }
                    indx++;
                }
                break;
        }



        if(unlockedObjects.Count > 0)
        {
            for (int i = 0; i < unlockedObjects.Count; i++)
            {
                var buildMenuItem = Instantiate (BuildItemMenuPrefab, transform.position , Quaternion.identity, SelectionGrid.transform);
                var buildMenuItemTemplate = buildMenuItem.GetComponent<buildMenuItemTemplate>();
                buildMenuItemTemplate.ItemTitle.text = unlockedObjects[i].title;
                buildMenuItemTemplate.ItemDesc.text = unlockedObjects[i].description;
                buildMenuItemTemplate.ItemImage.sprite = unlockedObjects[i].image;
                buildMenuItemTemplate.ItemCost.text = "Lots of Gold";
                int tmp_i = unlockedObjectsIndex[i];
                SelectedBuildType tmp_type = unlockedObjects[i].buildType;
                buildMenuItem.GetComponent<Button>().onClick.AddListener(() => startBuildingButton(tmp_type, tmp_i));
            }

        }
        else
            Debug.Log("This List is Empty");
    }
    #endregion

    public void startBuildingButton(SelectedBuildType buildType, int buildIndex)
    {
        currentBuildType = buildType;
        currentBuildingIndex = buildIndex;
        ToggleBuildingMenu(false);

        isBuilding = true;
    }


    bool BuildCost()
    {
        switch(currentBuildType)
        {
            case SelectedBuildType.FLOOR:
                currentSO = floorObjects[currentBuildingIndex];
                break;
            case SelectedBuildType.WALL:
                currentSO = wallObjects[currentBuildingIndex];
                break;
            case SelectedBuildType.UTILITY:
                currentSO = utilityObjects[currentBuildingIndex];
                break;
            case SelectedBuildType.BUILDING:
                currentSO = buildingObjects[currentBuildingIndex];
                break;
            case SelectedBuildType.PLANT:
                currentSO = plantObjects[currentBuildingIndex];
                break;
        }

        if(playerInventory.wood >= currentSO.woodCost && playerInventory.stone >= currentSO.stoneCost && playerInventory.metal >= currentSO.metalCost && playerInventory.seeds >= currentSO.seedCost)
        {
            // remove costs
            playerInventory.wood -= currentSO.woodCost;
            playerInventory.stone -= currentSO.stoneCost;
            playerInventory.metal -= currentSO.metalCost;
            playerInventory.seeds -= currentSO.seedCost;

            return true;
        }
        else
            return false;
    }

    // Build Regions
    public void ToggleCanBuild(bool active)
    {
        
        uiBuildIndicator.SetActive(active);
        canBuild = active;
        if(active == false && isBuilding==true)
        {
            isBuilding = false;
            player.ToggleCombat(true);   
        }   


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