using System.Collections.Generic;
using UnityEngine;

namespace TransportScript {
    public class BusStop : MonoBehaviour
    {

        List<Passenger> passengerList = new List<Passenger>();
        [SerializeField]
        public string color = "black";
        [SerializeField]
        private Transform busTransform;
        public float moveSpeed = 2f;
        float duration = 10f;
        public int zajetosc = 0;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Passenger" && tag == "BusStop")
            {
                Passenger passenger = other.gameObject.GetComponent<Passenger>();
                passengerList.Add(passenger);
            }
            if (other.tag == "Bus")
            {
                MovePassengersTowardsBus();
                //passengerList.Clear();
            }
        }

        private void MovePassengersTowardsBus()
        {
            foreach (Passenger passenger in passengerList)
            {
                passenger.transform.position += new Vector3(0f, -1f ,0f);
            }
            passengerList.Clear();
        }
    }
}
