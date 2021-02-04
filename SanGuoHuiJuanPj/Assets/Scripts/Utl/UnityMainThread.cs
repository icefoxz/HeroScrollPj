using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Utl
{
    internal class UnityMainThread : MonoBehaviour
    {
        internal static UnityMainThread run;
        private Queue<UnityAction> jobs = new Queue<UnityAction>();
        void Awake() => run = this;

        void Update()
        {
            while (jobs.Count > 0) jobs.Dequeue().Invoke();
        }

        internal void AddJob(UnityAction newJob) => jobs.Enqueue(newJob);
    }
}
