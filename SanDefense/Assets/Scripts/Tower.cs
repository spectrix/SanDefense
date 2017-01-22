﻿using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class Tower : MonoBehaviour
{
    public enum AttackStyle
    {
        AttackFirstEnemy,
        AttackFurthest,
        AttackLowest,
    };

    public enum ShootStyle
    {
        Straight,
        Lob,
    };

    public GameObject bullet; //prefab of bullet to
    public GameObject turretHead; //this turns and shoot, if none use the game object this is attached to to turn
    public GameObject barrelTip;
    public AttackStyle attackStyle = AttackStyle.AttackLowest; //algorithm to determine the target
    public ShootStyle shootStyle = ShootStyle.Straight;
	int roundConstructed = -1;

    [Range(0, 500)]
    public float damage = 20;

    [Range(0, 3)]
    public float attackCooldown = 1;

    //[Range(0.5f,10)]
    public float bulletSpeed = 2;

    [Range(3, 20)]
    public float radius = 5;
    private float radiusSqr = 25;

    private BoxCollider boxCollider;
    private Transform head;
 
    private GameObject target;
    private float timer = 0;
    private Vector3 targetForward;

	Renderer[] renderers;
	Color highlightColor = Color.red;
	Color regularColor;
    // Use this for initialization
    void Start()
    {
        //initializing variables
        boxCollider = GetComponent<BoxCollider>();
        if (turretHead)
            head = turretHead.transform;
        else head = transform;

        radiusSqr = Mathf.Pow(radius, 2);
		renderers = GetComponentsInChildren<Renderer> ();
		regularColor = renderers [0].material.color;
		roundConstructed = GameManager.Instance.CurWave;
    }

    // Update is called once per frame
    void Update()
    {
		if (!GameManager.Instance.IsPaused) {
			DetermineTarget ();

			targetForward = head.forward;

			if (target) {
				DetermineDirection ();
				head.forward = Vector3.Lerp (head.forward, -targetForward, .6f);
				Shoot ();
			}


			timer += Time.deltaTime;
		}
    }

    void DetermineTarget()
    {
        switch (attackStyle)
        {
            case AttackStyle.AttackLowest:

                //refreshes target
                target = null;
                float lowestHealth = float.MaxValue;

                //searches for target with lowest health
				foreach (GameObject enemy in EnemyManager.Instance.Enemies)
                {
                    if (enemy)
                    {
                        Vector3 dist = enemy.transform.position - transform.position;
                        dist.y = 0;
						float enemyHealth = enemy.GetComponent<Health> ().CurHealth;
                        if (dist.sqrMagnitude < radiusSqr && enemyHealth < lowestHealth)
                        {
                            target = enemy;
                            lowestHealth = enemyHealth;
                        }
                    }
                }
                break;

            case AttackStyle.AttackFurthest:

                //refreshes target
                target = null;
                float furthestDist = 0;

                //loops through all enemies
				foreach (GameObject enemy in EnemyManager.Instance.Enemies)
                {
                    //check if enemy is dead
                    if (enemy)
                    {
                        Vector3 dist = enemy.transform.position - transform.position;
                        dist.y = 0;

                        //print(dist.magnitude + " " + radius);
                        //print(dist.magnitude > furthestDist);

                        //checks if in radius and if further than current max
                        if (dist.sqrMagnitude < radiusSqr && dist.sqrMagnitude > furthestDist)
                        {
                            //print("Targeting: " + enemy.gameObject.name + " " + furthestDist);
                            target = enemy;
                            furthestDist = dist.sqrMagnitude;
                        }
                    }
                }

                break;

            case AttackStyle.AttackFirstEnemy:
                if (target)
                {
                    //checks if target is still in range
                    Vector3 dist = target.transform.position - transform.position;
                    dist.y = 0;

                    //if in range break out of switch
                    if (dist.sqrMagnitude < radiusSqr)
                        break;
                    else target = null; //resets target and search for new one
                }

                //search for a new target
				foreach (GameObject enemy in EnemyManager.Instance.Enemies)
                {
                    //check if enemy is dead
                    if (enemy)
                    {
                        Vector3 dist = enemy.transform.position - transform.position;
                        dist.y = 0;

                        if (dist.sqrMagnitude < radiusSqr)
                        {
                            target = enemy;
                            break;
                        }
                        else continue;
                    }
                }

                break;

        }
    }

    /// <summary>
    /// Determines direction based on where the enemy is
    /// </summary>
    void DetermineDirection()
    {
        targetForward = (target.transform.position - transform.position).normalized;
        if (shootStyle == ShootStyle.Lob)
        {
            targetForward = (targetForward + Vector3.up).normalized;
        }
    }

    /// <summary>
    /// Checks the cooldown and shoots at the enemy
    /// </summary>
    void Shoot()
    {
        if (timer > attackCooldown)
        {
            Bullet bul = (Instantiate(bullet, barrelTip.transform.position, head.transform.rotation, transform)).GetComponent<Bullet>();
            bul.Initialize(target, bulletSpeed, damage);
            timer = 0;
        }
    }

    //for visualizing radius
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

	public bool Highlighted {
		set {
			Color c = value ? highlightColor : regularColor;
			foreach (Renderer r in renderers) {
				r.material.color = c;
			}
		}
	}

	public int RoundConstructed {
		get {
			return roundConstructed;
		}
	}
}
