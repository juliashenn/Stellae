using Unity.Android.Gradle.Manifest;
using Unity.Multiplayer.PlayMode;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public Planet currentPlanet;
    private Vector3 planetDir;
    public Planet[] planets;

    public void HandleObjGravity(Transform obj)
    {
        Vector3[] directions = { -obj.up, obj.forward, -obj.forward, obj.right, -obj.right };
        RaycastHit[] hits = new RaycastHit[0];
        for (int i = 0; hits.Length == 0 && i < directions.Length; i++)
        {
            hits = Physics.RaycastAll(obj.position, directions[i], 3f);
        }

        if (hits.Length == 0)
        {
            planetDir = currentPlanet.transform.position - obj.position;
            hits = Physics.RaycastAll(obj.position, planetDir, 3f);
        }

        Vector3 normalDir = (obj.position - currentPlanet.transform.position).normalized;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform == currentPlanet)
            {
                normalDir = hits[i].normal.normalized;
                break;
            }
        }

        Rigidbody rb = obj.GetComponent<Rigidbody>();  
        if (rb != null)
        {
            Vector3 velocity = rb.linearVelocity + normalDir.normalized * -10f * Time.fixedDeltaTime;

            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
            //obj.GetComponent<Rigidbody>().AddForce(normalDir.normalized * -10f, ForceMode.Acceleration);
            
            Quaternion targetRot = Quaternion.FromToRotation(obj.transform.up, normalDir) * obj.transform.rotation;
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, 15f * Time.fixedDeltaTime));
            //obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, targetRot, 15f * Time.fixedDeltaTime);
        }
        hits = new RaycastHit[0];
    }
}
