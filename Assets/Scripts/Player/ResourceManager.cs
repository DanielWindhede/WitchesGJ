using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fami
{
    public class ResourceHelper
    {
        public static Object GetResource(string folder, string path)
        {
            return Resources.Load(folder + "/" + path);
        }
    }
}