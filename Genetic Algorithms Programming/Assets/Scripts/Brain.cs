using UnityEngine;

public class Brain : MonoBehaviour
{
    public DNA dna;
    public GameObject eyes;
    (bool left, bool forward, bool right) seeWall;
    public float eggsFound = 0;
    LayerMask ignore = 6;
    bool canMove = false;

    public void Init()
    {
        dna = new DNA();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("egg"))
        {
            eggsFound++;
            col.gameObject.SetActive(false);
        }
    }
    void Update()
    {
        seeWall = (false, false, false);
        bool left = false;
        bool front = false;
        bool right = false;
        canMove = true;
        RaycastHit hit;
        
        Debug.DrawRay(eyes.transform.position, eyes.transform.forward, Color.red);
       
        if (Physics.SphereCast(eyes.transform.position, 0.1f, eyes.transform.forward, out hit, 1f, ~ignore))
        {
            if (hit.collider.gameObject.CompareTag("wall"))
            {
                front = true;
                canMove = false;
            }
        }

        if (Physics.SphereCast(eyes.transform.position, 0.1f, eyes.transform.right, out hit, 1f, ~ignore))
        {
            if (hit.collider.gameObject.CompareTag("wall"))
            {
                right = true;
            }
        }

        if (Physics.SphereCast(eyes.transform.position, 0.1f, -eyes.transform.right, out hit, 1f, ~ignore))
        {
            if (hit.collider.gameObject.CompareTag("wall"))
            {
                left = true;
            }
        }

        seeWall = (left, front, right);
    }

    void FixedUpdate()
    {
        this.transform.Rotate(0, dna.genes[seeWall], 0);
        if (canMove)
        {
            this.transform.Translate(0, 0, 0.1f);
        }
    }
}
