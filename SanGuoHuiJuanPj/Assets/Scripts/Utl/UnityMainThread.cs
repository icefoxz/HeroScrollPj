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
        internal static UnityMainThread thread;
        public ExceptionHandlerUi ExceptionPanel;
        private Queue<UnityAction> jobs = new Queue<UnityAction>();
        void Awake()
        {
            thread = this;
            ExceptionPanel.Init();
        }

        void Update()
        {
            while (jobs.Count > 0) jobs.Dequeue().Invoke();
        }

        internal void RunNextFrame(UnityAction newJob) => jobs.Enqueue(newJob);
    }
}
