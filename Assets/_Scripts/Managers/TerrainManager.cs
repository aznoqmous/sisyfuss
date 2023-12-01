using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TerrainManager : MonoBehaviour
{
    public static TerrainManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    EdgeCollider2D _edgeCollider;
    LineRenderer _lineRenderer;

    float _currentPoint = 0;
    float _totalPoint = 0;

    [SerializeField] ScriptableWorld _pastWorld;
    [SerializeField] ScriptableWorld _currentWorld;

    [SerializeField] Transform _foliageContainer;
    [SerializeField] Transform _treeContainer;
    [SerializeField] Transform _landPartContainer;

    [SerializeField] Transform _clouds;
    [SerializeField] float _cloudsSpeed;
    [SerializeField] SpriteRenderer _cloudFrontSprite;
    [SerializeField] SpriteRenderer _cloudMiddleSprite;
    [SerializeField] SpriteRenderer _cloudBackSprite;

    Foliage _foliagePrefab { get { return _currentWorld.FoliagePrefab; } }
    Foliage _treePrefab { get { return _currentWorld.TreePrefab; } }

    Color _meshColor;

    [Header("LandPart")]
    [SerializeField] StartingRoom _startingRoom;
    [SerializeField] StartingRoom _startingRoomPrefab;
    [SerializeField] Altar _altarPrefab;
    List<GameObject> _landParts { get { return _currentWorld.LandParts; } }
    Boss _boss { get { return _currentWorld.Boss; } }
    bool _isBossFight = false;
    bool _isStarting = true;
    bool _isWorldTransition = false;
    public bool IsBossFight { get { return _isBossFight; } }
    public bool IsStarting { get { return _isStarting; } }

    List<GameObject> _shuffledLandParts = new List<GameObject>();
    float _landPartCountBeforeBoss {get {return _currentWorld.LandPartsBeforeBoss; }}
    float _landPartCount = 0f;
    float _totalLandPartCount = 0f;
    float _nextLandPartDistance { get { return _currentWorld.LandPartsDistance * (_landPartCount+1); } }
    float _lineLength = 2f;
    float _altarCount = 0f;
    [SerializeField] float _landPartSmoothSteps;
    public float LandPartCount { get { return _landPartCount;  } }
    public float TotalLandPartCount { get { return _totalLandPartCount;  } }

    float _upgradeChestCount = 0f;
    public float UpgradeChestCount { get { return _upgradeChestCount; } set { _upgradeChestCount = value;  } } 
    float _nextUpgradeChestDistance { get { return GameManager.Instance.Seed % 30f + 100f + 100f * (_upgradeChestCount); } }
    public bool CanSpawnUpgradeChest { get {
            return _totalPoint > _nextUpgradeChestDistance; 
        } }

    private void Start()
    {
        _mesh = new Mesh();
        _meshFilter.mesh = _mesh;
        _edgeCollider = GetComponent<EdgeCollider2D>();
        _lineRenderer = GetComponent<LineRenderer>();

        for (int i = 0; i < _lineRenderer.positionCount; i++)
        {
            Vector3 linePoint = _lineRenderer.GetPosition(i);
            _points.Add(linePoint);
        }

        SetEdgeCollider(_lineRenderer);

        _startingRoom.gameObject.SetActive(false);
        _pastWorld = GameManager.Instance.CurrentWorld;
        SetWorld(GameManager.Instance.CurrentWorld);
        LoadLandPart(_startingRoomPrefab.gameObject);
    }

    void SetEdgeCollider(LineRenderer line)
    {
        _vertices.Clear();
        _triangles.Clear();
        List<Vector2> edges = new List<Vector2>();
        foreach(Vector3 point in _points)
        {
            edges.Add((Vector2)point);
            AddVertice(point);
            _lastPoint = point;
        }
        _edgeCollider.SetPoints(edges);

        //AddVertice(_lastPoint + Vector3.down * 40f);
    }

    float _worldRatio = 0f;
    private void Update()
    {
        /* Build mesh */
        if (Mathf.Abs(_lastPoint.x - Player.Instance.transform.position.x) < 60f) GenerateNextPoint();
        if (Mathf.Abs(_points[0].x - Player.Instance.transform.position.x) > 70f) RemoveFirstPoint();

        _worldRatio = GetTransitionRatioAtPosition(Player.Instance.transform.position);
        _meshColor = Color.Lerp(_pastWorld.GroundColor, _currentWorld.GroundColor, _worldRatio);
        _meshRenderer.material.color= _meshColor;

        _lineRenderer.material.color = Color.Lerp(_pastWorld.TreeColor, _currentWorld.TreeColor, _worldRatio);

        _cloudFrontSprite.color = Color.Lerp(_pastWorld.CloudFrontColor, _currentWorld.CloudFrontColor, _worldRatio);
        _cloudMiddleSprite.color = Color.Lerp(_pastWorld.CloudMiddleColor, _currentWorld.CloudMiddleColor, _worldRatio);
        _cloudBackSprite.color = Color.Lerp(_pastWorld.CloudBackColor, _currentWorld.CloudBackColor, _worldRatio);
        Camera.main.backgroundColor = Color.Lerp(_pastWorld.SkyColor, _currentWorld.SkyColor, _worldRatio);

        /* Move cloud */
        float targetPositionX = Player.Instance.transform.position.x - 60f;
        Vector3 position = _clouds.position;
        if(position.x < targetPositionX) position.x = targetPositionX;
        position.y = Mathf.Lerp(position.y, Player.Instance.transform.position.y, Time.deltaTime * 10f);
        _clouds.position = position;
        /*_clouds.position = Vector3.Lerp(
            _clouds.position, 
            new Vector3(_clouds.position.x + 1f, Player.Instance.transform.position.y, 0),
            Time.deltaTime * _cloudsSpeed
        );*/
    }

    Vector3 _lastPoint = Vector3.zero;
    List<Vector3> _points = new List<Vector3>();
    
    void GenerateNextPoint()
    {
        _currentPoint++;
        Vector3 npoint = GetNextPoint();
        AddPoint(npoint);

        if (_isWorldTransition)
        {
            if(npoint.x >= _lastWorldPoint.x + _worldTransitionLength)
            {
                _isWorldTransition = false;
                LoadLandPart(_startingRoomPrefab.gameObject);
            }
            return;
        }

        if (_isBossFight) {
            
            if(EntityManager.Instance.IsBossAlive) return;
            _isBossFight = false;
            NextWorld();
            return;
        }

        if (_totalPoint > _nextLandPartDistance)
        {
            if (_landPartCount >= _landPartCountBeforeBoss && !_isBossFight)
            {
                GameObject landPart = LoadLandPart(_currentWorld.BossLandPart.gameObject);
                Boss boss = landPart.GetComponent<BossLandPart>().Boss;
                EntityManager.Instance.SetBoss(boss);
                _isBossFight = true;
                return;
            }

            if (_currentWorld.LandPartsBeforeAltar == 0 || Mathf.FloorToInt(_landPartCount / _currentWorld.LandPartsBeforeAltar) > _altarCount)
            {
                LoadLandPart(_altarPrefab.gameObject);
                _altarCount++;
            }
            else
            {
                SpawnLandPart();
                _landPartCount++;
                _totalLandPartCount++;
            }
        }
    }

    Vector3 GetNextPoint()
    {
        float rand = RandomHelper.RandomValue((_currentPoint + 1) / _currentWorld.Frequence);
        _lastPoint = _points[_points.Count - 1];
        Vector3 nextPoint = _lastPoint + new Vector3(_currentWorld.LineLength, Mathf.Lerp(_currentWorld.MinElevation, _currentWorld.MaxElevation, rand), 0);
        return nextPoint;
    }

    float GetAngle(Vector3 firstPoint, Vector3 lastPoint)
    {
        Vector2 diff = firstPoint - lastPoint;
        float angle = Mathf.Atan2(diff.y, diff.x);
        angle += angle > 0f ? -Mathf.PI : +Mathf.PI;
        return angle;
    }

    void AddPoint(Vector3 point)
    {
        _totalPoint++;
        point.z = Random.Range(-8f, 0f);

        _lastPoint = _points[_points.Count - 1];
        float ratio = GetTransitionRatioAtPosition(point);

        float angle = GetAngle(_lastPoint, point);
        if (Mathf.Abs(angle) < 0.3f * Mathf.PI)
        {
            Foliage grass = Instantiate(_foliagePrefab, Vector3.Lerp(_lastPoint, point, Random.Range(0.2f, 0.4f)), Quaternion.identity, _foliageContainer);
            grass.SetColor(Color.Lerp(_pastWorld.FoliageColor, _currentWorld.FoliageColor, ratio));
        }
        if (Mathf.Abs(angle) < 0.2f * Mathf.PI)
        {
            Foliage grass = Instantiate(_foliagePrefab, Vector3.Lerp(_lastPoint, point, Random.Range(0.6f, 0.8f)), Quaternion.identity, _foliageContainer);
            grass.SetColor(Color.Lerp(_pastWorld.FoliageColor, _currentWorld.FoliageColor, ratio));
        }

        if (Mathf.Abs(angle) < 0.1f * Mathf.PI && Random.value > 0.5f)
        {
            Foliage tree = Instantiate(_treePrefab, _lastPoint + Vector3.down * 0.5f, Quaternion.identity, _treeContainer);
            //tree.transform.localScale = Vector3.one * Random.Range(0.8f, 1.5f);
            tree.transform.localEulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);
            tree.SetColor(ratio > 0.5f ? _currentWorld.TreeColor : _pastWorld.TreeColor);
            //if (Random.value > 0.5f) tree.transform.localEulerAngles = new Vector3(0, 180f, 0);
        }

        _points.Add(point);
        _lineRenderer.positionCount = _points.Count - 1;
        _lineRenderer.SetPositions(_points.ToArray());
        SetEdgeCollider(_lineRenderer);
        _lastPoint = point;
    }

    void RemoveFirstPoint()
    {
        _points.RemoveAt(1);
        _points[0] = _points[1] + Vector3.down * 40f;

        _lineRenderer.positionCount = _points.Count - 1;
        _lineRenderer.SetPositions(_points.ToArray());

        SetEdgeCollider(_lineRenderer);
    }

    void SpawnLandPart()
    {
        if(_shuffledLandParts.Count <= 0) {
            _shuffledLandParts = RandomHelper.Shuffle(_landPartCount, _landParts);
        }
        

        GameObject landPart = _shuffledLandParts[0];
        _shuffledLandParts.Remove(landPart);

        LoadLandPart(landPart);
    }

    GameObject LoadLandPart(GameObject landPart)
    {
        LineRenderer lineRenderer = landPart.GetComponent<LineRenderer>();
        GameObject newLandPart;
        if (lineRenderer != null)
        {
            Vector3 offset = lineRenderer.GetPosition(0);

            // smooth step before landPart lineRenderer
            if (lineRenderer.positionCount >= 2)
            {
                float previousAngle = GetAngle(_points[_points.Count - 2], _points[_points.Count - 1]);
                float firstLandPartAngle = GetAngle(_lastPoint + lineRenderer.GetPosition(0) - offset, _lastPoint + lineRenderer.GetPosition(1) - offset);
                float diff = firstLandPartAngle - previousAngle;
                for (int i = 0; i < _landPartSmoothSteps; i++)
                {
                    Vector3 nextPoint = new Vector3(_lineLength, Mathf.Tan(previousAngle + diff * i / _landPartSmoothSteps) * _lineLength, -5f);
                    AddPoint(_lastPoint + nextPoint);
                }
            }

            newLandPart = Instantiate(landPart, _lastPoint - offset, Quaternion.identity, _landPartContainer);
            newLandPart.GetComponent<LineRenderer>().enabled = false;

            // add landPart lineRenderer to Terrain line renderer
            Vector3 origin = _lastPoint;
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                Vector3 point = lineRenderer.GetPosition(i);
                AddPoint(origin + point - offset);
            }

            // smooth step after landPart lineRenderer
            if (lineRenderer.positionCount >= 2)
            {
                int smoothSteps = 5;
                float previousAngle = GetAngle(_points[_points.Count - 2], _points[_points.Count - 1]);
                float firstLandPartAngle = GetAngle(_lastPoint, GetNextPoint());
                float diff = firstLandPartAngle - previousAngle;
                for (int i = 0; i < smoothSteps; i++)
                {
                    Vector3 nextPoint = new Vector3(_lineLength, Mathf.Tan(previousAngle + diff * i / smoothSteps) * _lineLength, -5f);
                    AddPoint(_lastPoint + nextPoint);
                }
            }
        }
        else newLandPart = Instantiate(landPart, _lastPoint, Quaternion.identity, _landPartContainer);

        StartingRoom startingRoom = newLandPart.GetComponent<StartingRoom>();
        if (startingRoom != null) startingRoom.HideLineRenderer();

        return newLandPart;
    }

    public Vector2 GetTerrainPositionAtX(float x)
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(x, Player.Instance.transform.position.y + 1000f), Vector2.down);
        return  hit.point;
    }

    [Header("Terrain Mesh")]
    List<Vector3> _vertices = new List<Vector3>();
    List<int> _triangles = new List<int>();
    Mesh _mesh;
    [SerializeField] MeshFilter _meshFilter;
    [SerializeField] MeshRenderer _meshRenderer;

    void AddVertice(Vector3 point)
    {
        float y = -40f;
        Vector3 bottom = new Vector3(point.x, y, -11f);
        point.z = -11f;
        _vertices.Add(point);
        _vertices.Add(bottom);
        if(_lastPoint != null)
        {
            if (_vertices.Count < 4) return;
            _triangles.Add(_vertices.Count - 4 + 0);
            _triangles.Add(_vertices.Count - 4 + 2);
            _triangles.Add(_vertices.Count - 4 + 1);

            _triangles.Add(_vertices.Count - 4 + 1);
            _triangles.Add(_vertices.Count - 4 + 2);
            _triangles.Add(_vertices.Count - 4 + 3);

            _mesh.Clear();
            _mesh.SetVertices(_vertices);
            _mesh.SetTriangles(_triangles, 0);
            _mesh.RecalculateNormals();
        }
    }

    [SerializeField] float _worldTransitionLength = 40f;
    Vector3 _lastWorldPoint;
    public float GetTransitionRatioAtPosition(Vector3 position)
    {
        if (_lastWorldPoint == null) return 0;
        return Mathf.Clamp01((position.x - _lastWorldPoint.x) / _worldTransitionLength);
    }
    public Color GetDustColorAtPosition(Vector3 position)
    {
        return Color.Lerp(_pastWorld.DustColor, _currentWorld.DustColor, GetTransitionRatioAtPosition(position));
    }
    void NextWorld()
    {
        Debug.Log("NEXT WORLD");
        _lastWorldPoint = _lastPoint;
        GameManager.Instance.NextWorld();
        SetWorld(GameManager.Instance.CurrentWorld);
        _isWorldTransition = true;
    }

    void SetWorld(ScriptableWorld world)
    {
        if (_currentWorld != null) _pastWorld = _currentWorld;
        _currentWorld = world;
        EntityManager.Instance.SetFoes(_currentWorld.Foes);
        _landPartCount = 0f;
        _altarCount = 0f;
    }
}
