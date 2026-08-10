using UnityEngine;
using UnityEngine.Events;

namespace SlimesToRiches.Arena.Core
{
    public class ArenaSimulationLoop : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent PreSimulation = new();

        [SerializeField]
        private UnityEvent<float> CalculateMovement = new();

        [SerializeField]
        private UnityEvent CalculateCollision = new();

        [SerializeField]
        private UnityEvent PostSimulation = new();

        [SerializeField]
        private UnityEvent UpdatePresentation = new();

        private void Update()
        {
            PreSimulation?.Invoke();
            CalculateMovement?.Invoke(Time.deltaTime);
            CalculateCollision?.Invoke();
            PostSimulation?.Invoke();
            UpdatePresentation?.Invoke();
        }
    }
}
