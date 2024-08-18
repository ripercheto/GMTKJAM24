//Author: Small Hedge Games
//Updated: 13/06/2024

using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SmallHedge.SoundManager
{
    [CreateAssetMenu(menuName = "Small Hedge/Sounds SO", fileName = "Sounds SO")]
    public class SoundsSO : ScriptableObject
    {
        [OnInspectorInit(nameof(UpdateTypes))]
        public SoundList[] sounds;

        private void UpdateTypes()
        {
            var values = Enum.GetValues(typeof(SoundType));
            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i].Type = (SoundType)values.GetValue(i);
            }
        }
    }
}