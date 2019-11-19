using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Constants {
    public static class enums {
        public enum Colors {
            RED = 0,
            GREEN = 1
        }

        private static Color[] colors = {
            Color.red, // 0
            Color.green // 1
        };

        public static Color GetColor(Colors color) {
            return colors[(int)color];
        }

        [System.Serializable] public enum CourseId
        {
            OteviraniHlasu,
            Didgeridoo,
            TibetskeZpivani
        }
    }
}
