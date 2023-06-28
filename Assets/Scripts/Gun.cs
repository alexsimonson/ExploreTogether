using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExploreTogether {
    [CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
    public class Gun : Weapon {
        [Header("Events")]
        public GameEvent onWeaponUIChanged;
        public Gun(){
            type = Type.Weapon;
            style = Style.Gun;
        }

        public AudioClip reloadSound;
        public int magazineRounds = 10;
        public int magazineSizeMax = 10;
        public int bulletCount = 30;
        public string name = "Gun";


        void OnEnable(){
            attackSound = Resources.Load("Audio/gunshot", typeof(AudioClip)) as AudioClip;
            reloadSound = Resources.Load("Audio/reload", typeof(AudioClip)) as AudioClip;
        }

        public override void Attack(GameObject owner){
            if(magazineRounds <= 0){
                Debug.Log("Out of ammo");
                return;
            }
            Shoot(owner);
            magazineRounds -= 1;
            UpdateWeaponUI();
        }

        public void Reload(GameObject owner){
            int bulletsToFill = magazineSizeMax - magazineRounds;
            if(bulletCount == 0){
                Debug.Log("There's no ammo left");
                return;
            }
            if(bulletCount < bulletsToFill){
                // we can only use the amount left
                magazineRounds += bulletCount;
                bulletCount -= bulletCount;
            }else{
                magazineRounds = magazineSizeMax;
                bulletCount -= bulletsToFill;
            }
            UpdateWeaponUI();
            owner.GetComponent<PlayerCombat>().audioSource.GetComponent<AudioSource>().PlayOneShot(reloadSound);
        }

        public void UpdateWeaponUI(){
            onWeaponUIChanged.Raise(null, this);
        }
    }
}
