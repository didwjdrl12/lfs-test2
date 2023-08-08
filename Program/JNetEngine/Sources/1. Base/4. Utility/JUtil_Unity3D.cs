using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using UnityEngine;
//using Random = UnityEngine.Random;
using System.Xml.Serialization;
using System.Reflection;
using System.Collections;
using System.Runtime.CompilerServices;

//namespace J2y
//{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JUtil
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public partial class JUtil : MonoBehaviour
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // GameObject / Transform
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [GameObject] Create
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static GameObject CreateGameObject(string name, Transform parentTM = null)
        {
            //var prefab = (GameObject)Resources.Load(filename);
            //var go = (GameObject)GameObject.Instantiate(prefab);
            var go = new GameObject(name);
            if (parentTM != null)
                go.transform.parent = parentTM;

            //Resources.UnloadAsset(prefab); // 테스트 필요
            return go;
        }
        #endregion

        #region [Transform] Reset/Hierarchy
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ResetTransform(Transform tm, bool scale = true)
        {
            tm.localPosition = Vector3.zero;
            tm.localRotation = Quaternion.identity;
            if (scale)
                tm.localScale = Vector3.one;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void SetParent(GameObject parentGO, GameObject childGO)
        {
            childGO.transform.parent = parentGO.transform;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void SetParent(Transform parentTM, Transform childTM)
        {
            childTM.parent = parentTM;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void DestroyAllChildren(GameObject parentGO)
        {
            DestroyAllChildren(parentGO.transform);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void DestroyAllChildren(Transform parentTM)
        {
            if (null == parentTM)
                return;

            foreach (Transform child in parentTM)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        public static GameObject CreateGameObject(string name, Transform parentTM = null, bool resetTM = true)
        {
            //var prefab = (GameObject)Resources.Load(filename);
            //var go = (GameObject)GameObject.Instantiate(prefab);
            var go = new GameObject(name);
            if (parentTM != null)
                go.transform.parent = parentTM;

            if (resetTM)
                ResetTransform(go.transform, true);
            //Resources.UnloadAsset(prefab); // 테스트 필요
            return go;
        }
        
        #endregion

        #region [Transform] Find
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Transform FindTransform(Transform parentTM, string objName)
        {
            if (parentTM == null) return null;

            foreach (Transform trans in parentTM)
            {
                if (trans.name == objName)
                {
                    return trans;
                }

                Transform foundTransform = FindTransform(trans, objName);
                if (foundTransform != null)
                {
                    return foundTransform;
                }
            }

            return null;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Transform FindTransform(GameObject parentObj, string objName)
        {
            return FindTransform(parentObj.transform, objName);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Transform[] CollectTransforms(Transform parentTM, string objName)
        {
            var trans = new List<Transform>();
            CollectTransforms(parentTM, objName, ref trans);
            return trans.ToArray();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void CollectTransforms(Transform parentTM, string objName, ref List<Transform> transforms)
        {
            if (parentTM == null) return;

            foreach (Transform trans in parentTM)
            {
                if (trans.name == objName)
                    transforms.Add(trans);

                CollectTransforms(trans, objName, ref transforms);
            }
        }

        public static T FindTransformComponent<T>(Transform parentTM, string objName) where T : Component
        {
            var tm = FindTransform(parentTM, objName);
            if (tm != null)
                return tm.GetComponent<T>();
            return null;
        }
        public static T[] CollectTransformComponent<T>(Transform parentTM, string objName) where T : Component
        {
            var trans = new List<Transform>();
            var coms = new List<T>();
            CollectTransforms(parentTM, objName, ref trans);
            foreach (var t in trans)
            {
                var c = t.GetComponent<T>();
                if (c != null)
                    coms.Add(c);
            }
            return coms.ToArray();
        }

    #endregion


    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Unity3D
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region [Unity3D] Layer 
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    public static void MoveToLayer(Transform root, string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            MoveToLayer(root, layer);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void MoveToLayer(Transform root, int layer)
        {
            root.gameObject.layer = layer;
            foreach (Transform child in root)
                MoveToLayer(child, layer);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void SetLayer(GameObject root, string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            root.layer = layer;
            foreach (Transform child in root.transform)
                MoveToLayer(child, layer);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void SetLayer(Transform root, string layerName)
        {
            SetLayer(root.gameObject, layerName);
        }
        #endregion
        
        #region [Unity3D] 셰이더 리셋
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static void Reset_shaders(Renderer[] renderers)
        //{		
        //	var dict = new Dictionary<Material, Material>();

        //	foreach (var rs in renderers)
        //	{
        //		foreach (var mtrl in rs.sharedMaterials)
        //			if ((mtrl != null) && !dict.ContainsKey(mtrl))
        //				dict[mtrl] = mtrl;
        //	}

        //	foreach (var mtrl in dict.Values)
        //		mtrl.shader = Shader.Find(mtrl.shader.name);		
        //}

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Reset_shaders(Renderer[] renderers)
        {
            // 중국 버전 보류
            //foreach (var rs in renderers)
            //{
            //    //var thisMaterial = rs.sharedMaterials;
            //    //var shaders = new string[thisMaterial.Length];

            //    //for (int i = 0; i < thisMaterial.Length; i++)
            //    //{
            //    //	if(thisMaterial[i] != null)
            //    //		shaders[i] = thisMaterial[i].shader.name;
            //    //}

            //    //for (int i = 0; i < thisMaterial.Length; i++)
            //    //{
            //    //	if(shaders[i] != null)
            //    //		thisMaterial[i].shader = Shader.Find(shaders[i]);
            //    //}       

            //    foreach (var mtrl in rs.sharedMaterials)
            //        if (mtrl != null && mtrl.shader != null && mtrl.shader.name != null)
            //            mtrl.shader = Shader.Find(mtrl.shader.name);
            //}
        }
        #endregion

        #region [Unity3D] 물리/충돌
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool Raycast_screenPoint(Vector3 scnPos, out RaycastHit hit, string layerName)
        {
            int layer = 0;
            if (layerName == "")
                layer = ~0;
            else
                layer = 1 << LayerMask.NameToLayer(layerName);

            if (Physics.Raycast(Camera.main.ScreenPointToRay(scnPos), out hit, 9999f, layer))
                return true;

            return false;
        }
        #endregion


    }

       

//}
