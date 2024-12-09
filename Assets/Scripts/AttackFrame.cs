using System.Collections;
using UnityEngine;

public class AttackFrame : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Enemy>(out Enemy enemy))
        {
            PlayerController controller = GetComponentInParent<PlayerController>();
            enemy.OnDamage(controller.InflictDamage(10));
            enemy.GetComponent<Collider2D>().enabled = false;
            enemy.StartCoroutine(ChangeColor(enemy));

        }
    }

    private IEnumerator ChangeColor(Enemy enemy)
    {
        Color color = enemy.enemySpriteRenderer.color;
        color = new Color(1, 0, 0);
        enemy.enemySpriteRenderer.color = color;
        yield return new WaitForSeconds(0.2f);
        color = new Color(1, 1, 1);
        enemy.enemySpriteRenderer.color = color;
        enemy.GetComponent<Collider2D>().enabled = true;

    }
}
