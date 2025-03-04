using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 destination;
    private float speed;
    private float travelDuration;
    private float elapsedTime;

    [SerializeField] private float projectileSpeed = 40;
    [SerializeField] private GameObject impactFx;
    
    private void SetProjectile(Vector3 target)
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
            AudioPlayer.PlaySound3D(Sound.impact, transform.position);

            if (impactFx)
            {
                // place the impact a little backwards so explosion isn't inside the model.
                Instantiate(impactFx, transform.position + -transform.forward * 0.5f, transform.rotation);
            }
            
            Destroy(gameObject);
        }
    }

    public static void CreateProjectile(Transform owner, Tile targetTile, string projectileName = "Projectile")
    {
        GameObject projectile = Resources.Load<GameObject>(projectileName);
        
        Vector3 spawnPos = owner.position + Vector3.up * 2;
        Projectile projectileInst = Instantiate(projectile, spawnPos, owner.rotation)
            .GetComponent<Projectile>();
        projectileInst.SetProjectile(targetTile.transform.position + Vector3.up);
    }
}