using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;
    private BoidSettings _settings;
    private float _speed;
    public Vector2 Direction;
    public Vector3 Position {
        get
        {
            return transform.localPosition;
        }
        set
        {
            transform.localPosition = value; 
          //  Debug.Log(value);
        }
    }
    public float Speed { get => _speed; set => _speed = value; }

    public Transform Target { get => _target; set => _target = value; }

    public Vector2 CentreOfFlockmates { get => _centreOfFlockmates; set => _centreOfFlockmates = value; }

    public Vector2 AvgFlockHeading { get => _avgFlockHeading; set => _avgFlockHeading = value; }

    public Vector2 AvgAvoidanceHeading { get => _avgAvoidanceHeading; set => _avgAvoidanceHeading = value; }

    public int NumPerceivedFlockmates { get => _numPerceivedFlockmates; set => _numPerceivedFlockmates = value; }

    private Vector2 _velocity;
    private Vector2 _centreOfFlockmates;
    private Vector2 _avgFlockHeading;
    private Vector2 _avgAvoidanceHeading;
    private Transform _target;
    private Transform _cachedTransform;
    private int _numPerceivedFlockmates;

    private bool IsHeadingForCollision => Physics2D.CircleCast(Position,_settings.boundsRadius,Direction,_settings.collisionAvoidDst, _settings.obstacleMask);
    private bool IsDirCollision(Vector2 dir) => Physics2D.CircleCast(Position,_settings.boundsRadius,dir,_settings.collisionAvoidDst, _settings.obstacleMask);

    public void Init(BoidSettings settings, Transform target, Vector2 position, Vector2 direction, float speed)
    {
        Position = position;
        Direction = direction;
        Speed = speed;
        _velocity = direction * speed;
        _settings = settings;
        Target = target;
        AvgAvoidanceHeading = Vector2.zero;
        _cachedTransform = transform;
    }
    
    public void UpdateBoid () {
        Vector2 acceleration = Vector2.zero;

        acceleration = MoveToTarget(acceleration);
        acceleration = MoveToMates(acceleration);
        acceleration = MoveToCenter(acceleration);

        if (IsHeadingForCollision) {
            Vector2 collisionAvoidDir = ObstacleRays();
            Vector2 collisionAvoidForce = SteerTowards (collisionAvoidDir) * _settings.avoidCollisionWeight;
            if(collisionAvoidDir != Vector2.zero) acceleration = collisionAvoidForce;
        }

        _velocity += acceleration * Time.deltaTime;
        //float speed = _velocity.magnitude;
        Vector2 dir = _velocity / _speed;
        //_speed = Mathf.Clamp(speed, _settings.minSpeed, _settings.maxSpeed);
        _velocity = dir * _speed;
        var pos = (Vector3)_velocity * Time.deltaTime;
        Position += pos;
        Direction = dir;
    }
    private Vector2 MoveToMates(Vector2 acceleration)
    {
        if (NumPerceivedFlockmates == 0) return acceleration;
        CentreOfFlockmates /= NumPerceivedFlockmates;
        var offsetToFlockmatesCenter = ((Vector3)CentreOfFlockmates - Position);
        var alignmentForce = SteerTowards(AvgFlockHeading) * _settings.alignWeight;
        var cohesionForce = SteerTowards(offsetToFlockmatesCenter) * _settings.cohesionWeight;
        var separationForce = SteerTowards(AvgAvoidanceHeading) * _settings.seperateWeight;

        acceleration += alignmentForce;
        acceleration += cohesionForce;
        acceleration += separationForce;
        return acceleration;
    }
    private Vector2 MoveToCenter(Vector2 acceleration)
    {
        if (Position.x > 45) acceleration += Vector2.left * _speed;
        if (Position.x < -45) acceleration += Vector2.right * _speed;
        if (Position.y > 45) acceleration += Vector2.down * _speed;
        if (Position.y < -45) acceleration += Vector2.up * _speed;

        return acceleration;
    }
    private Vector2 MoveToTarget(Vector2 acceleration)
    {
        if (Target == null) return acceleration;
        Vector2 offsetToTarget = (Target.localPosition - Position);
        var results = new RaycastHit2D[10];
        var direction = Target.position - Position;
        if (Physics2D.Raycast(Position, direction, _settings.ContactFilter2D, results, _settings.perceptionRadius) <= 0) return acceleration;
        foreach (var obj in results)
        {
            if (obj.collider == null) break;
            if (obj.collider.gameObject.layer == 6) break;
            if (obj.collider.gameObject.layer == 8)
            {
                acceleration = SteerTowards(offsetToTarget) * _settings.targetWeight;
                Speed = _settings.maxSpeed;
            }
            else
            {
                Speed = _settings.minSpeed;
            }
        }
        return acceleration;
    }

    Vector2 ObstacleRays () {
        Vector2[] rayDirections1 = BoidHelper.directions1;
        Vector2[] rayDirections2 = BoidHelper.directions2;
        int[] angles = BoidHelper.angles;
        Vector2 dir = Direction;
        for (int i = 0; i < angles.Length; i++)
        {
            dir = RotateTowards(angles[i]) * _speed;
            if (!IsDirCollision(dir)) return dir;
        }
        Debug.Log("not Found " + dir); 
        return Vector2.zero;
    }
    Vector2 SteerTowards (Vector2 vector) {
        Vector2 v = vector.normalized * _speed - _velocity;
        return Vector2.ClampMagnitude (v, _settings.maxSteerForce);
    }
    private Vector2 RotateTowards (int angle) => new Vector2((float)(Direction.x * Math.Cos(angle) - Direction.y * Math.Sin(angle)), (float)(Direction.x * Math.Cos(angle) + Direction.y * Math.Sin(angle)));
    
}
