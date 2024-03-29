using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : GunComponent
{
    [SerializeField] private int shotgunBullets;

    [SerializeField] private int minSpreadShot;
    [SerializeField] private int maxSpreadShot;

    public override IEnumerator Shoot(Transform bulletSpawnLocation)
    {
        if (currentClip <= 0)
            yield break;

        for (int i = 0; i < shotgunBullets; i++)
        {
            bulletSpawnLocation.transform.eulerAngles += new Vector3(0, 0, SpreadBullets(minSpreadShot, maxSpreadShot));
            GameObject Bullet = Instantiate(BulletPrefab, bulletSpawnLocation.transform.position, bulletSpawnLocation.transform.rotation);
            Bullet.transform.parent = MainGameManager.instance.BulletsLocation.transform;
            Bullet.GetComponent<Projectiles>().Init(dmg);
        }

        currentClip -= 1;

        yield return null;
    }
}
