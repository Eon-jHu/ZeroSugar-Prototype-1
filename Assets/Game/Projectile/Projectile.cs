using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 destination;
    private float speed;
    private float travelDuration;
    private float elapsedTime;

    [SerializeField] private AudioClip impactClip;

    public void SetProjectile(Vector3 target, float projectileSpeed)
    {
        destination = target;
        target.y = transform.position.y;
        speed = projectileSpeed;
        travelDuration = Vector3.Distance(transform.position, destination) / speed;
        elapsedTime = 0f;
    }

    private void Update()
    {
        if (elapsedTime < travelDuration)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
        }
        else
        {
            if (impactClip)
            {
                AudioSource.PlayClipAtPoint(impactClip, transform.position, 1f);
            }
            Destroy(gameObject);
        }
    }

    public static void CreateProjectile(Transform owner, Tile targetTile)
    {
        GameObject projectile = Resources.Load<GameObject>("Projectile");
        
        Vector3 spawnPos = owner.position + Vector3.up * 2;
        Projectile projectileInst = Instantiate(projectile, spawnPos, owner.rotation)
            .GetComponent<Projectile>();
        projectileInst.SetProjectile(targetTile.transform.position + Vector3.up, 40);
    }
}