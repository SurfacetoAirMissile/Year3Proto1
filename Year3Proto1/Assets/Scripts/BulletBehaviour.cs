using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : ProjectileShared
{

    private void Update()
    {
        PSUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Transform masterParent = StaticFunc.GetParentRecursive(collision.transform);
        // if the object we hit belongs to a craft
        if (masterParent.name.Contains("Player") || masterParent.name.Contains("AI"))
        {
            HovercraftShared script = masterParent.GetComponent<HovercraftShared>();
            // If the victim is not the owner of this bullet
            if (script.gameObject != owner)
            {
                // this is a different craft
                // deal damage to it
                masterParent.GetComponent<HovercraftShared>().healthComponent.DealDamage(damage, owner.name);
                GameObject hitSound = Instantiate(hitSoundEffectPrefab, transform, false);
                hitSound.transform.SetParent(null);
                GameObject hitImpact = Instantiate(bulletImpact, transform, false);
                hitImpact.transform.SetParent(null);
                timeElapsed = lifetime * .97f;
                if (script.controller == HovercraftShared.ControllerType.AIController)
                {
                    if (masterParent.name.Contains("Scimitar"))
                    {
                        // Tells the AI to chase the chassis of the owner of the bullet.
                        masterParent.GetComponent<ScimitarAIController>().ChangeState(ScimitarAIController.HovercraftAIState.Chase, owner.transform.GetChild(0).gameObject);
                    }
                }
                
            }
        }
    }
}
