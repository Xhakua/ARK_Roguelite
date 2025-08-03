using System.Collections.Generic;
using UnityEngine;

public class ReactionsBuffModule : MonoBehaviour, IAble2Edit
{
    [SerializeField] private bool once;
    private List<int> hitedInstanceIDs = new List<int>();
    private Collider2D[] colliders;
    //[SerializeField] private LayerMask layerMaskGround;
    //[SerializeField] private float radius;
    [SerializeField] private int damage;
    [SerializeField] private ReactionsBuff.DamageEnum damageEnum;
    //[SerializeField] private int damageCount;
    [SerializeField] private float timerMax = 0.1f;
    private float timer;
    private bool canHit = false;
    private ReactionsBuff reactionsBuff;

    private EditableBullet editableBullet;

    private void OnEnable()
    {
        timer = 0;
        canHit = true;
        hitedInstanceIDs.Clear();
        colliders = new Collider2D[0];
    }
    public int Edit(GameObject self, InventorySO inventory, int index)
    {
        if (!once)
        {
            if (canHit == true)
            {
                Hit(self);
            }
            if (timer < timerMax)
            {
                timer += Time.deltaTime;
            }
            else
            {
                canHit = true;
                timer = 0;
            }
        }
        else
        {
            Hit(self);
        }
        return index;
    }
    public void Hit(GameObject self)
    {
        if (editableBullet == null)
        {
            editableBullet = self.GetComponent<EditableBullet>();
        }

        if (editableBullet.colliders.Length == 0)
        {
            return;
        }

        colliders = editableBullet.colliders;

        hitedInstanceIDs = editableBullet.penetratedInstanceIDs;
        Debug.Log(hitedInstanceIDs.Count);

        foreach (Collider2D collider in colliders)
        {

            if (once && collider.CompareTag("Enemy") && hitedInstanceIDs.Contains(collider.gameObject.GetInstanceID()))
            {
                reactionsBuff = new ReactionsBuff(damageEnum, damage);
                collider.GetComponent<IHurt>().Hurt(reactionsBuff, gameObject);
                //hitedInstanceIDs.Add(collider.gameObject.GetInstanceID());
                canHit = false;
            }
            else if (!once && collider.CompareTag("Enemy"))
            {
                reactionsBuff = new ReactionsBuff(damageEnum, damage);
                collider.GetComponent<IHurt>().Hurt(reactionsBuff, gameObject);
                canHit = false;
            }

        }




    }

}

