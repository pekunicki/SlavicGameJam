﻿using System.Collections;
using System.Diagnostics;
using Assets.Scripts.Gameplay;
using Assets.Scripts.Player.PlayerManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Assets.Scripts.NPC
{
    public class NPCMovement : MonoBehaviour
    {

        [SerializeField] public float Speed = 0.0005f;
        [SerializeField] public bool FreeTraversing = true;
        [SerializeField] public GameObject TargetToAttack;
        [SerializeField] public bool Targeting;

        public bool SelfAnimating;

        public NPCInfo npcInfo;

        //Is all about random trversing.
        private int _directionX;
        private int _directionY = 1;
        private bool _isChangingDirection;
        public bool IsFightingMelee;

        //Is all about travelling to the target.
        private float _journeyTime;
        private float _journeyLength;

        //Cooldown related with fighting
        private Stopwatch _cooldownTime;
        public float CoolDownLimitAttact = 2;

        void Awake()
        {
            _cooldownTime = new Stopwatch();
            _cooldownTime.Reset();
            npcInfo = gameObject.GetComponent<NPCInfo>();
            TargetToAttack = GameObject.FindWithTag("Player");
            StartCoroutine(CheckDirectionCoroutine());
            StartCoroutine(Ranged());
            ResetTarget();
        }

        private IEnumerator Ranged()
        {
            while (true)
            {
                AttactRanged(TargetToAttack);
                yield return new WaitForSeconds(5);
            }
        }

        void Update()
        {
            if(!_isChangingDirection && !Targeting)
                Move();
            if (!FreeTraversing)
            {
                CheckDirectionInTargeting();
                MoveToTheTarget(TargetToAttack);
                if (IsFightingMelee)
                {
                    if (_cooldownTime.Elapsed.Milliseconds<=0f)
                    {
                        _cooldownTime.Reset();
                        _cooldownTime.Start();
                        AttactMelee(TargetToAttack);
                    }
                    else if(_cooldownTime.Elapsed.Seconds>=CoolDownLimitAttact)
                        _cooldownTime.Reset();
                }
            }

            if (npcInfo.HealthPoints <= 0)
                Dying();
        }

        //APPLY MUSIC OF DYING
        private void Dying()
        {
            Debug.Log("Object destroyed" + this);
            Destroy(gameObject,1);
        }

        //APPLY MUSIC OF MOVING NPC
        private void Move()
        {
            var newX = transform.position.x + _directionX*Speed;
            var newY = transform.position.y + _directionY*Speed;
            transform.position = new Vector2(newX, newY);
        }

        //APPLY MUSIC OF MOVING NPC
        private void MoveToTheTarget(GameObject target)
        {
            if (!Targeting)
            {
                Targeting = true;
                _journeyTime = Time.time;
                _journeyLength = Vector3.Distance(transform.position, target.transform.position);
            }
            else
            {
                float distCovered = (Time.time - _journeyTime)*Speed;
                float fracJourney = distCovered/_journeyLength;
                transform.position = Vector3.LerpUnclamped(transform.position, target.transform.position, fracJourney);
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (Targeting && other.CompareTag(GameplayServices.Tags.Player))
            {
                IsFightingMelee = true;
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (Targeting && other.CompareTag(GameplayServices.Tags.Player))
            {
                IsFightingMelee = false;
            }
        }

        //APPLY MUSIC ATTACT OF NPC
        void AttactMelee(GameObject target)
        {
            PlayerInfo player = target.GetComponent<PlayerInfo>();
            player.GetDamage(npcInfo.Damage);
        }

        void AttactRanged(GameObject target)
        {
            PlayerInfo player = target.GetComponent<PlayerInfo>();
            FireBullet(target,0.1f);
        }

        private void FireBullet(GameObject target, float projectileSpeed)
        {
           GameObject projectile = Instantiate(Resources.Load("bellsprout_ball") as GameObject, this.transform.position, this.transform.rotation) as GameObject;
           projectile.GetComponent<Bullet>().InitalizeBullet(target, projectileSpeed,npcInfo.Damage);
        }

        private void ResetTarget()
        {
            FreeTraversing = true;
            Targeting = false;
            _journeyTime = 0;
            _journeyLength = 0;
        }

        IEnumerator CheckDirectionCoroutine()
        {
            var time = Random.Range(3, 6);
            while (!Targeting)
            {
                ChangeDirection();
                yield return new WaitForSeconds(time);
            }
        }

        public void ChangeDirection()
        {
            int newDirectX;
            int newDirectY;

            do
            {
                newDirectX = Random.Range(-1, 2);
                newDirectY = Random.Range(-1, 2);
            } while (newDirectY == _directionY || newDirectX == _directionX);

            _directionX = newDirectX;
            _directionY = newDirectY;
            _isChangingDirection = false;

            if (SelfAnimating)
            {

                return;
            }
                
            if(_directionY>0)
                FlipUp();
            if(_directionX>0)
                FlipRight();
            if (_directionX < 0)
                FlipLeft();
            if(_directionY<0)
                FlipDown();
        }

        private void CheckDirectionInTargeting()
        {
            var direct = transform.position - TargetToAttack.transform.position;
            if (Mathf.Abs(direct.x) - Mathf.Abs(direct.y) > 0)
            {
                if (direct.x > 0)
                    FlipLeft();
                else
                    FlipRight();
            }
            else if (direct.y < 0)
                FlipUp();
            else
                FlipDown();


        }

        private void FlipRight()
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        private void FlipLeft()
        {
            transform.eulerAngles = new Vector3(0, 0, 180);
        }

        private void FlipUp()
        {
            transform.eulerAngles = new Vector3(0, 0, 90);
        }

        private void FlipDown()
        {
            transform.eulerAngles = new Vector3(0, 0, 270);
        }

    }
}
