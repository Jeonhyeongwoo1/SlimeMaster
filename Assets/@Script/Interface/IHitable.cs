using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitable
{
    void TakeDamage(float damage, CreatureController attacker);
    Action<int, int> onHitReceived { get; set; }
}
