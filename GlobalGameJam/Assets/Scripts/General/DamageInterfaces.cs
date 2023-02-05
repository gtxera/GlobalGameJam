using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageDealer
{
    public int DoDamage();
}

public interface IDamageReceiver
{
    public void ReceiveDamage(int damage);
}
