using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class ZombiesBoids : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Zombie _zombiePrefab;
    private List<Zombie> _zombies;
    public BoidSettings Settings;
    private Task[] _tasks;
    void Start()
    {
        _zombies = new List<Zombie>();
        _zombies = FindObjectsOfType<Zombie>().ToList();
        _tasks = new Task[_zombies.Count];
        foreach (var zombie in _zombies) {
            zombie.Init(Settings, _target, zombie.Position, Vector2.right, 1);
        }
    }

     void FixedUpdate()
    {
        if (_zombies == null)
            return;
        for (var index = 0; index < _zombies.Count; index++)
        {
            UpdateZombie(_zombies[index]);
        }
    }
    private void UpdateZombie(Zombie z)
    {
        z.NumPerceivedFlockmates = 0;
        z.AvgFlockHeading = Vector2.zero;
        z.CentreOfFlockmates = Vector2.zero;
        z.AvgAvoidanceHeading = Vector2.zero;
        foreach (var z2 in _zombies.Where(z2 => z2 != z))
        {
            UpdateMates(z2, z);
        }
        z.UpdateBoid();
    }
    private void UpdateMates(Zombie z2, Zombie z)
    {
        var offset = z2.Position - z.Position;
        float sqrDst = offset.x * offset.x + offset.y * offset.y;

        if (!(sqrDst < Settings.perceptionRadius * Settings.perceptionRadius))
            return;
        z.NumPerceivedFlockmates += 1;
        z.AvgFlockHeading += z2.Direction;
        z.CentreOfFlockmates += (Vector2)z2.Position;
        if (sqrDst < Settings.avoidanceRadius * Settings.avoidanceRadius)
            z.AvgAvoidanceHeading -= (Vector2)offset / sqrDst;
    }
}
