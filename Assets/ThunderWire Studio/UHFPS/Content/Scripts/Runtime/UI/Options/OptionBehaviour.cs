using Newtonsoft.Json.Linq;
using UnityEngine;

namespace UHFPS.Runtime
{
    public abstract class OptionBehaviour : MonoBehaviour
    {
        public bool IsChanged;

        public abstract object GetOptionValue();
        public abstract void SetOptionValue(object value);

        public virtual void SetOptionData(string[] data) { }
    }
}