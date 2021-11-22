using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    private CubePos nowCube = new CubePos(0,1,0);
    public float changePlaceSpeed = 0.5f;
    private float camMoveSpeed = 2f;
    public Transform cubeToPlace;
    public GameObject cubeToCreate, allCubes;
    private Rigidbody allCubesRB;
    private bool isLose;
    private Coroutine showCubePlace;
    private bool firstCube;
    public GameObject[] canvasStartPage;
    private float camMoveToY;
    private int prevCountMaxHor;
    public Color[] bgColors;
    private Color toCameraColor;
   //private Vector3 MaxElem;

    private List<Vector3> allCubesPosition = new List<Vector3> {
        new Vector3 (0,0,0),  
        new Vector3 (1,0,0),
        new Vector3 (-1,0,0),
        new Vector3 (0,1,0),
        new Vector3 (0,0,1),
        new Vector3 (0,0,-1),
        new Vector3 (1,0,1),
        new Vector3 (-1,0,-1),
        new Vector3 (-1,0,1),
        new Vector3 (1,0,-1),
    };
    private Transform mainCam;
    private void Start()
    {
        toCameraColor = Camera.main.backgroundColor;
        mainCam = Camera.main.transform;
        camMoveToY = 7.91f + nowCube.y - 1f;

        allCubesRB = allCubes.GetComponent<Rigidbody>();
        showCubePlace = StartCoroutine(ShowCubePlace());
    }


    private void Update()
    {
        if((Input.GetMouseButtonDown(0) || Input.touchCount>0) && cubeToPlace != null && allCubes!= null && !EventSystem.current.IsPointerOverGameObject()) 
        {
#if !UNITY_EDITOR
            if(Input.GetTouch(0).phase != TouchPhase.Began)
            {
                return;
            }
#endif
            
            if(!firstCube)
            {
                firstCube = true;
                foreach(GameObject a in canvasStartPage)
                {
                    Destroy(a);
                }
            }
           GameObject newCube = Instantiate(cubeToCreate, cubeToPlace.position, Quaternion.identity) as GameObject;
            newCube.transform.SetParent(allCubes.transform);
            nowCube.SetVector(cubeToPlace.position);
            allCubesPosition.Add(nowCube.GetVector());

            allCubesRB.isKinematic = true;
            allCubesRB.isKinematic = false;

            
            SpawnPosition();
            MoveCamera();

        }

        if(!isLose && allCubesRB.velocity.magnitude >0.1f)
        {
            Destroy(cubeToPlace.gameObject);
            isLose = true;
            StopCoroutine(showCubePlace);
        }
        mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition, 
            new Vector3(mainCam.localPosition.x, camMoveToY, mainCam.localPosition.z),
            camMoveSpeed * Time.deltaTime);
        if(Camera.main.backgroundColor !=toCameraColor)
        {
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, toCameraColor, Time.deltaTime / 2f);
        }
    }
    private IEnumerator ShowCubePlace()
    {
        while(true)
        {
            SpawnPosition();
            yield return new WaitForSeconds(changePlaceSpeed);
        }
    }

    private void SpawnPosition()
    {
        List<Vector3> positions = new List<Vector3>();
        if(IsPositionEmpty(new Vector3(nowCube.x+1 , nowCube.y, nowCube.z)) && nowCube.x+1 != cubeToPlace.position.x)
        {
            positions.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z));
        }
         if (IsPositionEmpty(new Vector3(nowCube.x-1, nowCube.y, nowCube.z)) && nowCube.x-1 != cubeToPlace.position.x)
        {
            positions.Add(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));
        }
         if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y+1, nowCube.z)) && nowCube.y+1 !=cubeToPlace.position.y)
        {
            positions.Add(new Vector3(nowCube.x, nowCube.y+1, nowCube.z));
        }
         if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y-1, nowCube.z)) && nowCube.y - 1 != cubeToPlace.position.y)
        {
            positions.Add(new Vector3(nowCube.x, nowCube.y-1, nowCube.z));
        }
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z+1))&& nowCube.z+1 != cubeToPlace.position.z)
        {
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z+1));
        }
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1)) && nowCube.z - 1 != cubeToPlace.position.z)
        {
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1));
        }
        if(positions.Count>0)
        {
            cubeToPlace.transform.position = positions[UnityEngine.Random.Range(0, positions.Count)];
        }
        else if(positions.Count == 0)
        {
            isLose = true;
        }
        else
        {
            cubeToPlace.position = positions[0];

        }
    }

    private bool IsPositionEmpty(Vector3 targetPos)
    {
        if(targetPos.y == 0)
        {
            return false;
        }
        foreach(Vector3 pos in allCubesPosition)
        {
            if(pos.x == targetPos.x && pos.y == targetPos.y && pos.z == targetPos.z)
            {
                return false;
            }
        }
        return true;
    }
     private void MoveCamera()
    {
        int maxX = 0;
        int maxY = 0;
        int maxZ = 0;
        int maxHor;
        foreach(Vector3 pos in allCubesPosition)
        {
            if (Mathf.Abs(Convert.ToInt32(pos.x)) > maxX)
                maxX = Convert.ToInt32(pos.x);

            if (Mathf.Abs(Convert.ToInt32(pos.y)) > maxY)
                maxY = Convert.ToInt32(pos.y);

            if (Mathf.Abs(Convert.ToInt32(pos.z)) > maxZ)
                maxZ = Convert.ToInt32(pos.z);

        }
        camMoveToY = 7.91f + nowCube.y - 1f;
        maxHor = maxX > maxZ ? maxX : maxZ;
        if(maxHor%3 == 0 && prevCountMaxHor!=maxHor)
        {
            mainCam.localPosition -= new Vector3(0, 0, 3f);
            prevCountMaxHor = maxHor;
        }
        if(maxY>=7)
        {
            toCameraColor = bgColors[2];
        }
        else if(maxY>=5)
        {
            toCameraColor = bgColors[1];
        }
        else if (maxY >= 2)
        {
            toCameraColor = bgColors[0];
        }
    }
}
struct CubePos
{
    public int x;
    public int y;
    public int z;
    public CubePos(int X, int Y, int Z)
    {
        x = X;
        y = Y;
        z = Z;
    }
    public Vector3 GetVector()
    {
        return new Vector3(x,y,z);
    }
    public void SetVector(Vector3 pos)
    {
        x = Convert.ToInt32(pos.x);
        y = Convert.ToInt32(pos.y);
        z = Convert.ToInt32(pos.z);
    }
}