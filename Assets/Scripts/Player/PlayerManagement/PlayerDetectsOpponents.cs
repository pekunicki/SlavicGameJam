﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Player.PlayerManagement
{
    public class PlayerDetectsOpponents : MonoBehaviour {

        public bool OpponentInRange;
        public List<GameObject> ListOfOpponentsInRange;

        void Update()
        {
            if (ListOfOpponentsInRange.Count > 0)
                ListOfOpponentsInRange = ListOfOpponentsInRange.Where(item => item != null).ToList();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("npc") && !ListOfOpponentsInRange.Contains(other.gameObject)) { 
                ListOfOpponentsInRange.Add(other.gameObject);
                OpponentInRange = true;
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("npc"))
            {
                if (ListOfOpponentsInRange.Contains(other.gameObject))
                    ListOfOpponentsInRange.Remove(other.gameObject);
                OpponentInRange = false;
            }
        }


    }
}
