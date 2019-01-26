using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShootingController : MonoBehaviour {
    public float ShootDuration = 0.5f;
    public GameObject projectile;
    public float RandomRangeFactor = 2f;

    private float _time;
    private GameObject _player;

	void Start () {
        _time = 0f;
        _player = GameObject.FindGameObjectWithTag("Player");
    }
	
	void Update () {
        transform.LookAt(_player.transform.position, Vector3.left);
        _time += Time.deltaTime;
        if (_time > ShootDuration)
        {
            _time = 0;
            GameObject bullet = Instantiate(projectile, transform.position, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            float random = Random.Range(-RandomRangeFactor, RandomRangeFactor);
            Vector3 position = _player.transform.position - transform.position - new Vector3(random, random, random);
            rb.AddForce(position * 100);
            bullet.transform.LookAt(_player.transform.position);
            print("shooting");
        }
	}
}
