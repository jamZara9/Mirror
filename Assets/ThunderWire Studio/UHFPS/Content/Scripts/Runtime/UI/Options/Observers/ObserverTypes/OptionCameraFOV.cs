using Cinemachine;
using System;

namespace UHFPS.Runtime
{
    [Serializable]
    public class OptionCameraFOV : OptionObserverType
    {
        public CinemachineVirtualCamera VirtualCamera;

        public override string Name => "Camera FOV";

        public override void OptionUpdate(object value)
        {
            if (value == null || VirtualCamera == null)
                return;

            VirtualCamera.m_Lens.FieldOfView = (float)value;
        }
    }
}