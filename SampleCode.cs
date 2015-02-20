using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;
using System.Linq;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//end of library includes
public class GameManager : MonoBehaviour
{
    #region singleton
//Singleton
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType(typeof(GameManager)) as GameManager;

            return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion

    #region Variables and Accessors
    //Variables and Accessors
    private static bool cellHolderCreated;
    private static GameObject cellHolder;
    private static List<GameObject> listOfLoadCubes;

    private bool b = false;
    private int _score;

    private int numberOfCurrentCell;
   
    public int NumberOfCurrentCells
    {
        get { return numberOfCurrentCell; }
    }

    private Vector3 sizeOfGenSphere;
    private Color lastColor;
    private Color newColor;

    private Shader tessilationShader;
 
    private List<GameObject> listOfObjectsInTheScenes;
    private int listFind = 0;
    public GameObject test;
    public Vector3 v3test;

//Designers need to see text labels to edit UI on the fly.
    [SerializeField] private Text label;
    [SerializeField] private Text discLabel;

    private Camera mainCamera;
    public Camera MainCamera
    {
        get { return mainCamera; }
    }

    private Texture2D[] arrayOftextures;
    public Texture2D[] ArrayOfTextures
    {
        get { return arrayOftextures; }
    }

    private List<GameObject> listOfDuplicatedCells;
    //Options param
    private bool inOptions;
    private bool tweenCompleate = true;
    private Vector3 cameraPos;
    //Loading scene params 
    private int numberOfCube;
    private int numberOfBars = 20;
    private float widthOfBarGap = 1.3f;

    public bool genCells = false;
    public bool genScienceView = false;
    public bool genEbola = false;
    public bool genBlood = false;
    public bool genParticals = false;

    public struct CellStruct
    {
        public float cellHealth;
        public Color cellColor;
        public float cellTessilationAmount;

    };

    #endregion

    #region generators
    public int RandomInt(int firstInt, int secondInt)
    {
        return  UnityEngine.Random.Range(firstInt, secondInt);
    }


    public bool RandomBool(int firstNumber, int secondNumber)
    {
        int i = UnityEngine.Random.Range(firstNumber, secondNumber);
 //ternary operator
        return x = 0 ? true : false;
    }

    public float RandomFloat(float firstFloatToRand, float secondFloatToRand)
    {
        return UnityEngine.Random.Range(firstFloatToRand, secondFloatToRand);
    }

    public Vector3 RandomPos(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
    {
        return  new Vector3(UnityEngine.Random.Range(minX, maxX),
                                      UnityEngine.Random.Range(minY, maxY),
                                      UnityEngine.Random.Range(minZ, maxZ));
    }
    #endregion

    #region	start
    void Start()
    {
//Init Lists
        listOfObjectsInTheScene = new List<GameObject>();
        listOfDuplicatedCells = new List<GameObject>();
        listOfLoadCubes = new List<GameObject>();
//Check level you are in for Designers
        Debug.Log(Application.loadedLevelName);
        sizeOfGenSphere = new Vector3(5, 5, 5);
        tessilationShader = Shader.Find("TessellationHighRimGloss");

        mainCamera = GameObject.Find("Main Camera").camera;

        if (label == null)
        {
            label = GameObject.Find("Label1").GetComponent<Text>();
            discLabel = GameObject.Find("DiscLabel").GetComponent<Text>();
        }

        if (genCells)
        {
            for (int i = 0; i < 1; i++)
            {
                // CellGenerator("Ebola", Random.insideUnitSphere * 60, 1, 10, 0.2f);
                CellGenerator("Ebola", new Vector3(0, 0, 25), 1, 10, 0.2f);
            }
        }

        if (genEbola)
        {
            for (int i = 0; i < 1; i++)
            {
                // CellGenerator("Ebola", Random.insideUnitSphere * 20, 100, 1, 0.01f);
                CellGenerator("Ebola", new Vector3(i * 3, 0, 5), 500, 1, 0.2f);

            }
        }

        if (genBlood)
        {
            for (int i = 0; i < 100; i++)
            {
                BloodCellGen();
            }
        }

        if (Application.loadedLevelName == "TitleScreen")
        {
            LoadBars();
        }
//Collect all game Objects in list 
        GatherAllObjectsInScene();
    }
    #endregion

    #region Update
    void Update()
    {
        if (Input.GetButtonUp("Cancel"))
        {
            QuitGame();
        }
       
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                label.text = hit.collider.gameObject.name;
            }
        }

        if (Input.GetKeyUp(KeyCode.P))
        {
            loadLevel("Main");
        }

        if (Input.GetKeyUp(KeyCode.O))
        {
            loadLevel("TitleScreen");
        }
    }
    #endregion

    #region BloodCellGen
    private void BloodCellGen()
    {
        if (!cellHolderCreated)
        {
            cellHolder = new GameObject();
            cellHolder.name = "Cell Holder";
            cellHolderCreated = !cellHolderCreated;
        }
        GameObject headOfCell = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        headOfCell.transform.parent = cellHolder.transform;
        headOfCell.name = name + " " + numberOfCurrentCell;
        numberOfCurrentCell++;
        headOfCell.transform.position = UnityEngine.Random.insideUnitSphere * 25;
        headOfCell.transform.localScale = new Vector3(3, 1, 3);
        headOfCell.AddComponent<Rigidbody>();
        headOfCell.rigidbody.useGravity = false;
        headOfCell.transform.rotation = UnityEngine.Random.rotation;
        headOfCell.renderer.material.color = new Color(1, 0, 0);
    }
    #endregion

    #region CellGen
    private void CellButtonGen()
    {
        CellGenerator("Button", UnityEngine.Random.insideUnitSphere, 1, 5, 0.2f);
    }

    private void CellEbolaGen()
    {
        CellGenerator("Ebola", new Vector3(3, 0, 5), 500, 1f, 0.2f);
    }

    private GameObject CellGenerator(string name, Vector3 cellPos, int numberToGen, float size, float frequencey)
    {
//Super secret cell Gen code, please ask me if you would like to see it. 
//Using object pooling and generating primative game obeccts.
//Future of the game will be mesh generated geo spheres. 
    }
    #endregion

    #region GatherObjects
    private void GatherAllObjectsInScene()
    {
        //Puts list of all gameobjects so you can render by the distance to camera for optimization.
        GameObject[] listOfAllObj = UnityEngine.Object.FindObjectsOfType<GameObject>();

        foreach (object go in listOfAllObj)
        {
            if (listOfAllObj[listFind].GetComponent<Renderer>() != null)
            {
                listOfObjectsInTheScene.Add(listOfAllObj[listFind]);
            }
            listFind++;
        }
    }
    #endregion

    #region ColorGen
    public Color RandomColor(GameObject objectToColor,
                             float minLastR, float maxLastR, float minLastG, float maxLastG, float minLastB, float maxLastB,
                             float minR, float maxR, float minG, float maxG, float minB, float maxB)
    {
        if ((minLastB > 0) || (maxLastR > 0) || (minLastG > 0) || (maxLastG > 0) || (minLastB > 0) || (maxLastB > 0))
        {
            if (RandomBool(0, 2))
            {
                float randomR = UnityEngine.Random.Range(minLastR, maxLastR);
                float randomG = UnityEngine.Random.Range(minLastG, maxLastG);
                float randomB = UnityEngine.Random.Range(minLastB, maxLastB);

                if (lastColor.r + randomR > 1)
                {
                    newColor.r = lastColor.r;
                }
                else
                {
                    newColor.r += randomR;
                }

                if (lastColor.g + randomG > 1)
                {
                    newColor.g = lastColor.r;
                }
                else
                {
                    newColor.g += randomG;
                }

                if (lastColor.b + randomB > 1)
                {
                    newColor.b = lastColor.b;
                }
                else
                {
                    newColor.b += randomB;
                }
            }
            else
            {
                float randomR = UnityEngine.Random.Range(minLastR, maxLastR);
                float randomG = UnityEngine.Random.Range(minLastG, maxLastG);
                float randomB = UnityEngine.Random.Range(minLastB, maxLastB);

                if (lastColor.r - randomR < 0)
                {
                    newColor.r = lastColor.r;
                }
                else
                {
                    newColor.r -= randomR;
                }

                if (lastColor.g - randomG < 0)
                {
                    newColor.g = lastColor.r;
                }
                else
                {
                    newColor.g -= randomG;
                }

                if (lastColor.b - randomB < 0)
                {
                    newColor.b = lastColor.b;
                }
                else
                {
                    newColor.b -= randomB;
                }
            }
        }
        else
        {
            newColor = new Color(UnityEngine.Random.Range(minR, maxR),
                                  UnityEngine.Random.Range(minG, maxG),
                                  UnityEngine.Random.Range(minB, maxB));

        }
        lastColor = newColor;
        objectToColor.renderer.material.SetColor("_Color", newColor);

        return newColor;
    }
    #endregion

    #region Shader
    public void AssignRandomShader(GameObject objToAssignShader)
    {
        objToAssignShader.renderer.material.SetTexture("_MainTex", arrayOftextures[RandomInt(0, arrayOftextures.Length)]);
        objToAssignShader.renderer.material.SetTexture("_DispTex", arrayOftextures[RandomInt(0, arrayOftextures.Length)]);

        objToAssignShader.renderer.material.SetTexture("_SecondDispTex", arrayOftextures[RandomInt(0, arrayOftextures.Length)]);
        objToAssignShader.renderer.material.SetFloat("_SecondDispTexAmount", RandomFloat(0.01f, 1f));
        objToAssignShader.renderer.material.SetColor("_SecondDispTexColor", new Color(RandomFloat(0.01f, 0.1f), RandomFloat(0.01f, 1f), RandomFloat(0.01f, 0.1f), 1));
      
        objToAssignShader.renderer.material.SetColor("_Color", new Color(RandomFloat(.01f, .4f), RandomFloat(0.01f, .4f), RandomFloat(0.01f, .4f), 1));
       
        if (RandomBool(0, 2))
        {
            objToAssignShader.renderer.material.SetColor("_RimColor", new Color(RandomFloat(.01f, 1f), RandomFloat(0.01f, 1f), RandomFloat(0.01f, 1f), 0));
            objToAssignShader.renderer.material.SetFloat("_RimPower", RandomFloat(0.5f, 3f));
        }
    }
    public void AssignShader(GameObject objToAssignShader, Vector3 size, Color color, int texture, Color colorOfRim, float rimPower)
    {
        objToAssignShader.renderer.material.SetTexture("_MainTex", arrayOftextures[texture]);
        objToAssignShader.renderer.material.SetTexture("_DispTex", arrayOftextures[texture]);

        objToAssignShader.renderer.material.SetColor("_Color", color);
     
        if (RandomBool(0, 2))
        {
            objToAssignShader.renderer.material.SetColor("_RimColor", new Color(RandomFloat(.01f, 1f), RandomFloat(0.01f, 1f), RandomFloat(0.01f, 1f), 0));
            objToAssignShader.renderer.material.SetFloat("_RimPower", RandomFloat(0.5f, 3f));
        }
    }
    #endregion

    #region CellDivision
    public void DivideCells(GameObject cellToDivide, Shader currentShader, float sizeOfCell)
    {
        for (int i = 0; i < 1; i++)
        {
            cellToDivide.collider.enabled = false;
            GameObject cellToDivide2 = Instantiate(cellToDivide, cellToDivide.transform.position, Quaternion.identity) as GameObject;
            cellToDivide2.renderer.material.shader = currentShader;
            if (cellHolder != null)
                cellToDivide2.transform.parent = cellHolder.transform;

            cellToDivide2.name = name + " " + numberOfCurrentCell;
            numberOfCurrentCell++;

            HOTween.To(cellToDivide.transform, RandomFloat(1, 2), "position", cellToDivide.transform.position + UnityEngine.Random.insideUnitSphere * (sizeOfCell * 2f));
            HOTween.To(cellToDivide2.transform, RandomFloat(1, 2), "position", cellToDivide2.transform.position + UnityEngine.Random.insideUnitSphere * (sizeOfCell * 2f));
            label.text = numberOfCurrentCell.ToString();
        }
    }
    #endregion

    #region GameStates
    private void QuitGame()
    {
        Application.Quit();
    }

    private void LoadScene()
    {
        cellHolderCreated = !cellHolderCreated;
        Application.LoadLevel(0);
    }

    private static void loadLevel(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }

    private void ToggleComponent(string componentToToggle)
    {
        switch (componentToToggle)
        {
            case "SSAO":
                mainCamera.GetComponent<SSAOEffect>().enabled = !mainCamera.GetComponent<SSAOEffect>().enabled;
                break;

            case "DOF":
                mainCamera.GetComponent<DepthOfField34>().enabled = !mainCamera.GetComponent<DepthOfField34>().enabled;
                break;

            case "Particle":
                GameObject.Find("Particle System").active = !GameObject.Find("Particle System").active;
                break;
            default:
                break;
        }
    }
    #endregion

    #region Options
    private void Options()
    {
        if (!inOptions)
        {
            if (tweenCompleate)
            {
                tweenCompleate = false;
                inOptions = true;
                cameraPos = mainCamera.transform.position;
             
                HOTween.To(mainCamera.transform, 1, new TweenParms().Prop("position", mainCamera.transform.position + new Vector3(0, 0, 30)).OnComplete(ToggleBool));
            }
        }
        else
        {
            if (tweenCompleate)
            {
                tweenCompleate = false;
                inOptions = false;
                cameraPos = mainCamera.transform.position;
                HOTween.To(mainCamera.transform, 1, new TweenParms().Prop("position", mainCamera.transform.position + new Vector3(0, 0, -30)).OnComplete(ToggleBool));
            }
        }
    }
    private void ToggleBool()
    {
        tweenCompleate = true;
    }
    #endregion

    private IEnumerator MoveObject(GameObject objectToMove, Vector3 target, float overTime)
    {
        float startTime = 0;
        while (startTime <= 1)
        {
            startTime += Time.deltaTime / overTime;
            objectToMove.transform.position = Vector3.Lerp(objectToMove.transform.position, target, Time.deltaTime);
            yield return null;
        }
        objectToMove.transform.position = target;
    }

    IEnumerator Wait(float amountToWait)
    {
        yield return new WaitForSeconds(amountToWait);
    }

    #region LoadScene

    private void LoadBars()
    {
        numberOfCube = 0;
        listOfLoadCubes.Clear();
        GameObject loadSceneObjectManager = new GameObject("LoadScene object manager");
        LoadSceneCubes tempCubeTempComponent;

        for (int i = 0; i < numberOfBars; i++)
        {
            for (int j = 0; j < numberOfBars; j++)
            {
                GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tempCube.AddComponent<LoadSceneCubes>();
                tempCube.transform.position = new Vector3((i * widthOfBarGap) - (numberOfBars * 0.5f), 0, (j * widthOfBarGap) - (numberOfBars * 0.5f));
                tempCube.transform.parent = loadSceneObjectManager.transform;

                tempCubeTempComponent = tempCube.GetComponent<LoadSceneCubes>();
                tempCubeTempComponent.loadCubeNumeber = numberOfCube;
                tempCube.name = "Load cube " + numberOfCube;
                listOfLoadCubes.Add(tempCube);

                numberOfCube++;
                RevertListShader(null);

            }
        }
        LoadSavedCubes();
    }

    public void DisableBars()
    {
        for (int i = 0; i < listOfLoadCubes.Count; i++)
        {
            listOfLoadCubes[i].gameObject.SetActive(false);
        }
    }

    public void EnableBars()
    {
        for (int i = 0; i < listOfLoadCubes.Count; i++)
        {
            listOfLoadCubes[i].gameObject.SetActive(true);
        }
    }

    public void ChangeListShader(GameObject clickedGameObject)
    {
        for (int i = 0; i < listOfLoadCubes.Count; i++)
        {
            if (listOfLoadCubes[i].gameObject == clickedGameObject)
            {
                RevertListShader(clickedGameObject);
            }
            else
            {
                listOfLoadCubes[i].renderer.material.shader = Shader.Find("HardSurface/Hardsurface Free/Transparent Specular");

            }
        }
    }

    public void RevertListShader(GameObject clickedGameObject)
    {
        for (int i = 0; i < listOfLoadCubes.Count; i++)
        {
            listOfLoadCubes[i].renderer.material.shader = Shader.Find("HardSurface/Hardsurface Free/Opaque Specular");
        }
    }
    #endregion

    static void SaveObject(string testNumber = null)
    {
        //float[] cellScore = new float[listOfBars.Count];
        float[] loadCubeValue = new float[listOfLoadCubes.Count];
        if (testNumber == null)
        {

        }
        for (int i = 0; i < listOfLoadCubes.Count; i++)
        {
            LoadSceneCubes LoadSceneComponent = listOfLoadCubes[i].GetComponent<LoadSceneCubes>();

            loadCubeValue[i] = LoadSceneComponent.loadCubeValue;

        }
        Save(loadCubeValue);
    }

    static void Save(float[] loadCubeValue)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/cellInfo.dat");

        CellData data = new CellData();
        data.loadCubeValue = loadCubeValue;

        bf.Serialize(file, data);
        file.Close();
    }

    static float[] LoadSavedCubes()
    {
        if (File.Exists(Application.persistentDataPath + "/cellInfo.dat"))
        {

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/cellInfo.dat", FileMode.Open);

            CellData data = (CellData)bf.Deserialize(file);

            for (int i = 0; i < 100; i++)
            {
                LoadSceneCubes tempLoadCubeComponent = listOfLoadCubes[i].GetComponent<LoadSceneCubes>();
                tempLoadCubeComponent.loadCubeValue = data.loadCubeValue[i];
            }
            file.Close();
        }
        return null;
    }
}
[Serializable]
class CellData
{
    public float[] loadCubeValue = new float[100];
}
[Serializable]
class ModuleData
{
    public string[] sequenceName = new string[100];
    public float[] firstRangeOfModuleEffect = new float[100];
    public float[] secondRangeOfModuleEffect = new float[100];
}
