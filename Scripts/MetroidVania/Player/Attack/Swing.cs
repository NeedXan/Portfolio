using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Swing : MonoBehaviour
{
    void Update()
    {
        if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDamageable>(out IDamageable damageable) && !other.CompareTag("Player"))
        {
            if (gameObject.CompareTag("SideSlash"))
                transform.parent.GetComponent<PlayerAttack>().Hit(damageable, 3);
            if (gameObject.CompareTag("DownSlash"))
                transform.parent.GetComponent<PlayerAttack>().Hit(damageable, 2);
        }
        else if (other.TryGetComponent<IPogoable>(out IPogoable pogoable) && !other.CompareTag("Player"))
        {
            if (gameObject.CompareTag("SideSlash"))
                transform.parent.GetComponent<PlayerAttack>().Hit(pogoable, 3);
            if (gameObject.CompareTag("DownSlash"))
                transform.parent.GetComponent<PlayerAttack>().Hit(pogoable, 2);
        }
        else if (other.TryGetComponent<IHitable>(out IHitable hitable) && !other.CompareTag("Player"))
        {
            if (gameObject.CompareTag("SideSlash"))
                transform.parent.GetComponent<PlayerAttack>().Hit(hitable, 3);
            if (gameObject.CompareTag("DownSlash"))
                transform.parent.GetComponent<PlayerAttack>().Hit(hitable, 2);
        }
    }
}
