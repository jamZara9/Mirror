using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonster : IDamage
{
    void Idle();
    void Move();
    void Attack();
    void Damage();
    void Die();
}
