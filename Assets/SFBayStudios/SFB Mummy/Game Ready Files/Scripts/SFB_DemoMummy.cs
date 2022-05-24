using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFB_DemoMummy : MonoBehaviour
{

    public GameObject castParticle;
    public Transform castPoint;

    public GameObject tauntParticle;
    public Transform tauntPoint;

    public void CastSpell()
    {
        GameObject newCastParticle = Instantiate(castParticle, castPoint.position, Quaternion.identity);
        Destroy(newCastParticle, 10.0f);
    }

    public void CastTaunt()
    {
        GameObject newCastParticle = Instantiate(tauntParticle, tauntPoint.position, Quaternion.identity);
        newCastParticle.transform.parent = tauntPoint;
        Destroy(newCastParticle, 10.0f);
    }
}