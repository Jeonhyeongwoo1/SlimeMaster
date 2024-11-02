using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitable
{
    void TakeDamage(float damage);
    Action<int, int> onHitReceived { get; set; }
}
