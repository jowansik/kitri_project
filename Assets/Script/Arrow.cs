using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Vector3 dir;
    private float speed;
    private int power;

    private void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;

        other.GetComponent<Actor>().onDamaged(power);

        Destroy(this.gameObject);   // todo : 오브젝트 풀 만들기
    }

    public void SetArrow(Vector3 _dir, float _speed, int _power)
    {
        dir = _dir;
        speed = _speed;
        power = _power;
    }
}
