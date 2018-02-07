using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private float lifeTime = 10f;
    private Vector3 dir;
    private float speed;
    private int power;

    private void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
        lifeTime -= Time.deltaTime;

        if (lifeTime < 0f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
            return;

        if (other.tag == "Player")
            other.GetComponent<Actor>().onDamaged(power);

        Destroy(this.gameObject);   // todo : 오브젝트 풀 만들기
    }

    public void SetArrow(Vector3 _dir, float _speed, int _power, Transform _playerTR)
    {
        dir = _dir;
        speed = _speed;
        power = _power;

        transform.LookAt(_playerTR);
        transform.Rotate(new Vector3(-90, 0, 0));
    }
}
