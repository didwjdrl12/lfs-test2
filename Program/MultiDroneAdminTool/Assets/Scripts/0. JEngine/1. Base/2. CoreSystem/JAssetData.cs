using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;
using System.Collections;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JAssetData_base
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    [Serializable]
    public abstract class JAssetData_base : ICloneable
    {
        #region [베이스] Clone
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual object Clone()
        {
            return MemberwiseClone();
        }
        #endregion
    }
}
