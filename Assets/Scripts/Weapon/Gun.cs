using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunType
{
    RIFLE = 1,
    SHOTGUN,
    BAZOOKA
}

public class Gun : MonoBehaviour
{
    public StateMachine gunState = new StateMachine();
    public ShotMethod shotMethod;

    private int Guntype;
    private int shotCount = 0;
    private float shotCooltime = 0f;

    private Transform arm;
    public Transform ShotPosition { get; private set; }
    public Vector2 Direction { get { return (Vector2.right * transform.localScale).normalized; } }

    private Animator playerAnimator;

    public GameObject bulletPrefab;
    public GameObject bazookaPrefab;
    Rigidbody2D rb;

    [SerializeField]
    private AudioClip shotSFX = null;
    [SerializeField]
    private Light gunFireLight;

    public Action OnShotBullet;

    private void Awake()
    {
        SwitchGun(GunType.RIFLE);
        playerAnimator = GetComponent<Animator>();
        arm = transform.Find("Arm");
        ShotPosition = arm.Find("ShotPosition");
        InitGunStateMachine();
    }

    private void Update()
    {
        gunState.UpdateStateMachine();
        gunFireLight.intensity -= 100 * Time.deltaTime;
        gunFireLight.enabled = gunFireLight.intensity >= 0;
        if (shotCooltime > 0)
            shotCooltime -= Time.deltaTime;
    }

    private void InitGunStateMachine()
    {
        State idle = new State();
        idle.Enter += delegate
        {
            playerAnimator.SetBool("isShooting", false);
        };

        idle.StateUpdate += delegate
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (Guntype == 0)
                    SwitchGun(GunType.SHOTGUN);
                else if (Guntype == 1)
                    SwitchGun(GunType.BAZOOKA);
                else if (Guntype == 2)
                    SwitchGun(GunType.RIFLE);
            }
            if (Input.GetButton("Fire"))
            {
                Shot();
            }
        };

        State fire = new State();

        fire.Enter += delegate
        {
            shotCooltime = 0;
            shotCount = shotMethod.bulletsPerShot;
            playerAnimator.SetBool("isShooting", true);
        };

        fire.StateUpdate += delegate
        {
            shotCooltime -= Time.deltaTime;
            if (shotCooltime <= 0 && shotCount > 0)
            {
                gunFireLight.intensity = 10;
                shotMethod.Shot(arm.rotation * Direction);
                shotCount--;
                shotCooltime = shotMethod.shotCooltime;
                SoundManager.inst.PlaySFX(gameObject, shotSFX);
                OnShotBullet?.Invoke();
            }
            if (shotCount <= 0)
            {
                if (shotMethod.GetType() == typeof(RifleShotMethod) || Input.GetButtonUp("Fire"))
                {
                    Debug.Log((shotMethod.GetType() == typeof(RifleShotMethod)) + " " + Input.GetButtonUp("Fire"));
                    gunState.Transition("idle");
                }
            }
        };

        gunState.AddNewState("idle", idle);
        gunState.AddNewState("fire", fire);
        gunState.Transition("idle");
    }

    public void Shot()
    {
        if (gunState.IsState("fire"))
        {
            shotCount = shotMethod.bulletsPerShot;
        }
        else if (shotCooltime <= 0)
        {
            OnShotBullet?.Invoke();
            gunState.Transition("fire");
        }
    }

    public void SwitchGun(GunType type)
    {
        switch (type)
        {
            case GunType.RIFLE:
                Guntype = 0;
                shotMethod = new RifleShotMethod(this, 3, 0.05f); //초당 60발
                break;
            case GunType.SHOTGUN:
                Guntype = 1;
                shotMethod = new ShotgunShotMethod(this, 1, 0.6f); //초당 60발
                break;
            case GunType.BAZOOKA:
                Guntype = 2;
                shotMethod = new BazookaShotMethod(this, 1, 0.6f); //바주카 상태에서는 대쉬 봉인, 뒤로 반동 주고 싶음
                break;
        }
    }
}

#region ShotMethod

public abstract class ShotMethod
{
    protected Gun gun;
    public int bulletsPerShot;
    public float shotCooltime;
    public abstract void Shot(Vector2 direction);
}

public class RifleShotMethod : ShotMethod
{
    public RifleShotMethod(Gun gun, int bps, float cooltime)
    {
        this.gun = gun;
        bulletsPerShot = bps;
        shotCooltime = cooltime;
    }

    public override void Shot(Vector2 direction)
    {
        GameObject bullet = GameObject.Instantiate(gun.bulletPrefab);
        bullet.transform.position = gun.ShotPosition.position + new Vector3(0, UnityEngine.Random.Range(-0.01f, 0.01f));
        bullet.GetComponent<Rigidbody2D>().velocity = direction * 25f;
        CameraController.Shake(0.02f, 0.05f);
    }
}

public class ShotgunShotMethod : ShotMethod
{
    public ShotgunShotMethod(Gun gun, int bps, float cooltime)
    {
        this.gun = gun;
        bulletsPerShot = bps;
        shotCooltime = cooltime;
    }

    public override void Shot(Vector2 direction)
    {
        Vector2 Original = direction;
        for(int i = 0; i < 6; i++)
        {
            GameObject bullet = GameObject.Instantiate(gun.bulletPrefab);
            bullet.transform.position = gun.ShotPosition.position + new Vector3(0, UnityEngine.Random.Range(-0.01f, 0.01f));
            direction = Quaternion.Euler(0,0,20f - (UnityEngine.Random.Range(5f,9f)*i)) * Original;
            bullet.GetComponent<Rigidbody2D>().velocity = direction * 20f;
        }
    }
}

public class BazookaShotMethod : ShotMethod
{
    public BazookaShotMethod(Gun gun, int bps, float cooltime)
    {
        this.gun = gun;
        bulletsPerShot = bps;
        shotCooltime = cooltime;
    }

    public override void Shot(Vector2 direction)
    {
        GameObject bullet = GameObject.Instantiate(gun.bazookaPrefab);
        bullet.transform.position = gun.ShotPosition.position + new Vector3(0, UnityEngine.Random.Range(-0.01f, 0.01f));
        bullet.GetComponent<Rigidbody2D>().velocity = direction * 20f;
        
    }
}

#endregion

/*
#region FireMode
public abstract class FireMode
{

}

public class FireModeOnce : FireMode
{

}

public class FireModeContinuous : FireMode
{

}
#endregion
*/