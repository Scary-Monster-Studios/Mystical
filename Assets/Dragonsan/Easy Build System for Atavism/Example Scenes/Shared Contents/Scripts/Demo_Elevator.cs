using System.Collections;
using UnityEngine;

public class Demo_Elevator : MonoBehaviour
{
    public float speed = 10f;
    public Vector3 localPointA;
    public Vector3 localPointB;

    private IEnumerator Start()
    {
        while (true)
        {
            do yield return null; while (MoveTowards(localPointA));
            yield return new WaitForSeconds(1f);
            do yield return null; while (MoveTowards(localPointB));
            yield return new WaitForSeconds(1f);
        }
    }

    private bool MoveTowards(Vector3 position)
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, position, speed * Time.deltaTime);
        return transform.localPosition != position;
    }
}
