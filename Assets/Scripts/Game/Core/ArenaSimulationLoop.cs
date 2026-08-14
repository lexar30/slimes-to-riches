using UnityEngine;
using UnityEngine.Events;

namespace SlimesToRiches.Arena.Core
{
    public class ArenaSimulationLoop : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent PreSimulation = new();

        [SerializeField]
        private UnityEvent<float> ProcessPhysics = new();

        [SerializeField]
        private UnityEvent UpdatePresentation = new();

        private void Update()
        {
            PreSimulation.Invoke();
            UpdatePresentation.Invoke();
        }

        private void FixedUpdate()
        {
            ProcessPhysics.Invoke(Time.fixedDeltaTime);
        }
    }
}
