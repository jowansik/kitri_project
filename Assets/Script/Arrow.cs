﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField]
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
            other.SendMessage("onDamaged", power);

        // other.GetComponent<Actor>().onDamaged(power);

        Destroy(this.gameObject);   // todo : 오브젝트 풀 만들기
    }

    public void SetArrow(Vector3 _dir, float _speed, int _power, Transform _playerTR, Vector3 _arrowOffset, bool _skill)
    {
        dir = _dir;
        speed = _speed;
        power = _power;

        if (_skill)
            power *= 2;

        transform.LookAt(_playerTR.position + _arrowOffset);

        transform.Rotate(new Vector3(-90, 0, 0));
    }
}
