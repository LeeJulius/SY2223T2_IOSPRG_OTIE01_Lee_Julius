using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [SerializeField] private GameObject WeaponPrefab;
    [SerializeField] private WeaponTypes weapon;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.name.StartsWith("Player"))
        {
            PlayerInventory platerInventory = other.gameObject.GetComponent<PlayerInventory>();

            if (platerInventory.IsPickUpValid(weapon))
            {
                // Pickup Gun Pickup
                PlayerInventory playerInventory = other.gameObject.GetComponent<PlayerInventory>();
                playerInventory.PickUpGun(weapon, WeaponPrefab);

                // Despawn Gun Pickup
                Destroy(gameObject);
            }
        }
    }
}
