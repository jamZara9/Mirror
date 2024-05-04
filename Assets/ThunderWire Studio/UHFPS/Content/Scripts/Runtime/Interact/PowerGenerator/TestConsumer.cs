using System.Reactive.Subjects;
using UHFPS.Runtime;
using UnityEngine;

public class TestConsumer : MonoBehaviour, IPowerConsumer
{
    [field: SerializeField]
    public float ConsumeWattage { get; set; }

    public BehaviorSubject<bool> IsTurnedOn { get; set; } = new(true);

    public void OnPowerState(bool state)
    {
        
    }

    private void Start()
    {
        IsTurnedOn.OnNext(true);
    }
}
