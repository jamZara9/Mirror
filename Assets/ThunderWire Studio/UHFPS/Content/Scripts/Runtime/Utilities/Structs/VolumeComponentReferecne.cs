using System;
using UnityEngine.Rendering;

namespace UHFPS.Runtime
{
    [Serializable]
    public struct VolumeComponentReferecne
    {
        public Volume Volume;
        public int ComponentIndex;

        public VolumeComponent GetVolumeComponent()
        {
            return Volume.profile.components[ComponentIndex];
        }
    }
}