using System.Collections.Generic;
using UnityEngine;

namespace TransportScript {
    public class Bus : MonoBehaviour
    {
        List<Passenger> passengerList = new List<Passenger>();
        Vector3 newPosition = new Vector3(-10f, -10f, 0f);
        public Manager manager;
        public float moveDistance = 5f;
        public float moveSpeed = 2f;
        private Rigidbody2D _righbody;
        public Statistics statistics;
        private void Awake()
        {
            _righbody = GetComponent<Rigidbody2D>();

        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_righbody != null)
            {
                _righbody.freezeRotation = true;
            }
            if (other.tag == "BusStop" && tag == "Bus")
            {
                Transform busStopTransform = other.transform;
                RemovePassager(other.gameObject.GetComponent<BusStop>(),busStopTransform);
            }
            if (other.tag == "Passenger" && tag == "Bus")
            {
                Passenger passenger = other.gameObject.GetComponent<Passenger>();
                if (AddPassager(passenger))
                {
                    passengerList.Add(passenger);
                }
                newPosition = new Vector3(manager.getx(), manager.gety(), 0f);
                other.transform.position = newPosition;
                manager.add();
            }
        }
        private bool AddPassager(Passenger passenger)
        {
            foreach (Passenger passengers in passengerList)
            {
                if (passengers.getId() == passenger.getId())
                {
                    return false;
                }
            }
            Debug.Log(passenger.getId());
            //Debug.Log("dodanie pasażera do listy");
            return true;
        }
        private void RemovePassager(BusStop busStop, Transform busStopTransform)
        {
            float x = 0.7f * busStop.zajetosc ;
            for (int i = passengerList.Count - 1; i >= 0; i--)
            {
                Passenger passenger = passengerList[i];
                if (passenger.getColor() == busStop.color)
                {
                    Debug.Log("usuwanie pasazera");
                    manager.removed();
                    passenger.changeWait();
                    MovePassengerTowardsBusStop(passenger, busStopTransform, x);
                    passengerList.RemoveAt(i);
                    statistics.UpdateScore();
                    busStop.zajetosc --;
                    x = 0.7f * busStop.zajetosc;
                }
            }
        }
        private void MovePassengerTowardsBusStop(Passenger passenger, Transform busStopTransform, float x)
        {
            passenger.transform.position = new Vector3(busStopTransform.position.x - x, busStopTransform.position.y, -1f);
            //passenger.transform.position += new Vector3(0f, 1f, 0f);
        }
    }
}
