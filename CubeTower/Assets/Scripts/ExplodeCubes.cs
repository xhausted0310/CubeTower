using UnityEngine;

public class ExplodeCubes : MonoBehaviour
{
    private bool _collisionSet;
    public GameObject restartButton;
    public GameObject InstaButton;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Cube" && !_collisionSet)
        {
            for(int i = collision.gameObject.transform.childCount-1; i>=0; i--)
            {
                Transform child = collision.transform.GetChild(i);
                child.gameObject.AddComponent <Rigidbody>();
                child.gameObject.GetComponent<Rigidbody>().AddExplosionForce(70f, Vector3.up, 5f);
                child.SetParent(null);

            }
            restartButton.SetActive(true);
            //Camera.main.transform.position = new Vector3(-0.13f, 7.91f, -9.92f);
            Camera.main.transform.position -= new Vector3(0, 0, 3f);
            Destroy(collision.gameObject);
            _collisionSet = true;
        }
    }
}
