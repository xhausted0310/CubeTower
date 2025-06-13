using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MainMono
{
    [SerializeField] private float changePlaceSpeed = 0.5f;
    [SerializeField] private Transform cubeToPlace;
    [SerializeField] private GameObject cubeToCreate;
    [SerializeField] private GameObject allCubes;
    [SerializeField] private GameObject[] canvasStartPage;
    [SerializeField] private Color[] bgColors;
    
    [SerializeField] private Transform mainCam;
    
    
    private readonly float _camMoveSpeed = 2f;
    private CubePos _nowCube = new CubePos(0, 1, 0);
    private Rigidbody _allCubesRb;
    private bool _isLose;
    private Coroutine _showCubePlace;
    private bool _firstCube;
    private float _camMoveToY;
    private int _prevCountMaxHor;
    private Color _toCameraColor;
    

    private readonly List<Vector3> _allCubesPosition = new List<Vector3>
    {
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1),
        new Vector3(1, 0, 1),
        new Vector3(-1, 0, -1),
        new Vector3(-1, 0, 1),
        new Vector3(1, 0, -1),
    };


    private void Start()
    {
        _toCameraColor = Camera.main.backgroundColor;
        mainCam = Camera.main.transform;
        _camMoveToY = 7.91f + _nowCube.y - 1f;

        _allCubesRb = allCubes.GetComponent<Rigidbody>();
        _showCubePlace = StartCoroutine(ShowCubePlace());
    }


    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && cubeToPlace != null && allCubes != null &&
            !EventSystem.current.IsPointerOverGameObject())
        {
#if !UNITY_EDITOR
            if(Input.GetTouch(0).phase != TouchPhase.Began)
            {
                return;
            }
#endif

            if (!_firstCube)
            {
                _firstCube = true;
                foreach (GameObject a in canvasStartPage)
                {
                    Destroy(a);
                }
            }

            GameObject newCube = Instantiate(cubeToCreate, cubeToPlace.position, Quaternion.identity) as GameObject;
            newCube.transform.SetParent(allCubes.transform);
            _nowCube.SetVector(cubeToPlace.position);
            _allCubesPosition.Add(_nowCube.GetVector());

            _allCubesRb.isKinematic = true;
            _allCubesRb.isKinematic = false;


            SpawnPosition();
            MoveCamera();
        }

        if (!_isLose && _allCubesRb.velocity.magnitude > 0.1f)
        {
            Destroy(cubeToPlace.gameObject);
            _isLose = true;
            StopCoroutine(_showCubePlace);
        }

        mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition,
            new Vector3(mainCam.localPosition.x, _camMoveToY, mainCam.localPosition.z),
            _camMoveSpeed * Time.deltaTime);
        if (Camera.main.backgroundColor != _toCameraColor)
        {
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, _toCameraColor, Time.deltaTime / 2f);
        }
    }

    private IEnumerator ShowCubePlace()
    {
        while (true)
        {
            SpawnPosition();
            yield return new WaitForSeconds(changePlaceSpeed);
        }
    }

    private void SpawnPosition()
    {
        List<Vector3> positions = new List<Vector3>();
        
        if (IsPositionEmpty(new Vector3(_nowCube.x + 1, _nowCube.y, _nowCube.z)) &&
            _nowCube.x + 1 != cubeToPlace.position.x)
        {
            positions.Add(new Vector3(_nowCube.x + 1, _nowCube.y, _nowCube.z));
        }

        if (IsPositionEmpty(new Vector3(_nowCube.x - 1, _nowCube.y, _nowCube.z)) &&
            _nowCube.x - 1 != cubeToPlace.position.x)
        {
            positions.Add(new Vector3(_nowCube.x - 1, _nowCube.y, _nowCube.z));
        }

        if (IsPositionEmpty(new Vector3(_nowCube.x, _nowCube.y + 1, _nowCube.z)) &&
            _nowCube.y + 1 != cubeToPlace.position.y)
        {
            positions.Add(new Vector3(_nowCube.x, _nowCube.y + 1, _nowCube.z));
        }

        if (IsPositionEmpty(new Vector3(_nowCube.x, _nowCube.y - 1, _nowCube.z)) &&
            _nowCube.y - 1 != cubeToPlace.position.y)
        {
            positions.Add(new Vector3(_nowCube.x, _nowCube.y - 1, _nowCube.z));
        }

        if (IsPositionEmpty(new Vector3(_nowCube.x, _nowCube.y, _nowCube.z + 1)) &&
            _nowCube.z + 1 != cubeToPlace.position.z)
        {
            positions.Add(new Vector3(_nowCube.x, _nowCube.y, _nowCube.z + 1));
        }

        if (IsPositionEmpty(new Vector3(_nowCube.x, _nowCube.y, _nowCube.z - 1)) &&
            _nowCube.z - 1 != cubeToPlace.position.z)
        {
            positions.Add(new Vector3(_nowCube.x, _nowCube.y, _nowCube.z - 1));
        }

        if (positions.Count > 0)
        {
            cubeToPlace.transform.position = positions[UnityEngine.Random.Range(0, positions.Count)];
        }
        else if (positions.Count == 0)
        {
            _isLose = true;
        }
        else
        {
            cubeToPlace.position = positions[0];
        }
    }

    private bool IsPositionEmpty(Vector3 targetPos)
    {
        if (targetPos.y == 0)
        {
            return false;
        }

        foreach (Vector3 pos in _allCubesPosition)
        {
            if (pos.x == targetPos.x && pos.y == targetPos.y && pos.z == targetPos.z)
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
        foreach (Vector3 pos in _allCubesPosition)
        {
            if (Mathf.Abs(Convert.ToInt32(pos.x)) > maxX)
                maxX = Convert.ToInt32(pos.x);

            if (Mathf.Abs(Convert.ToInt32(pos.y)) > maxY)
                maxY = Convert.ToInt32(pos.y);

            if (Mathf.Abs(Convert.ToInt32(pos.z)) > maxZ)
                maxZ = Convert.ToInt32(pos.z);
        }

        _camMoveToY = 7.91f + _nowCube.y - 1f;
        maxHor = maxX > maxZ ? maxX : maxZ;
        if (maxHor % 3 == 0 && _prevCountMaxHor != maxHor)
        {
            mainCam.localPosition -= new Vector3(0, 0, 3f);
            _prevCountMaxHor = maxHor;
        }

        if (maxY >= 7)
        {
            _toCameraColor = bgColors[2];
        }
        else if (maxY >= 5)
        {
            _toCameraColor = bgColors[1];
        }
        else if (maxY >= 2)
        {
            _toCameraColor = bgColors[0];
        }
    }
}