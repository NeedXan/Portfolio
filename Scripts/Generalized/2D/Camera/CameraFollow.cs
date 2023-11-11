using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraFollow : MonoBehaviour
{
    [Space(3), Header("Customization")]
    [SerializeField] private Vector3 _offset = new Vector3 (0, 0, -10);
    [Range(0f, 1f)]
    [SerializeField] private float _smoothTime = 0.25f;
    [SerializeField] private bool _useBounds;

    // Hidden Variables
    private Vector3 _velocity = Vector3.zero;
    private Transform _target;
    private Bounds _cameraBounds;
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        GetBounds();
    }

    public void GetBounds()
    {
        var height = _camera.orthographicSize;
        var width = height * _camera.aspect;

        var minX = Globals.WorldBounds.min.x + width;
        var maxX = Globals.WorldBounds.max.x - width;

        var minY = Globals.WorldBounds.min.y + height;
        var maxY = Globals.WorldBounds.max.y - height;

        _cameraBounds = new Bounds();
        _cameraBounds.SetMinMax(
            new Vector3(minX, minY, 0),
            new Vector3(maxX, maxY, 0)
            );
    }

    Vector3 targetPosition;

    // Update is called once per frame
    void Update()
    {
        targetPosition = _target.position + _offset;
        if(_useBounds)
            targetPosition = GetCameraBounds();


        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothTime);
    }

    private Vector3 GetCameraBounds()
    {
        return new Vector3(
            Mathf.Clamp(targetPosition.x, _cameraBounds.min.x, _cameraBounds.max.x),
            Mathf.Clamp(targetPosition.y, _cameraBounds.min.y, _cameraBounds.max.y),
            transform.position.z
            ); ;
    }
}
