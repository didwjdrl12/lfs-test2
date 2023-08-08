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
        // mathematics
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [수학]

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // 이런 함수 없나??	
        public static void Swap<T>(ref T x, ref T y)
        {
            T temp;
            temp = x; x = y; y = temp;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static float Distance2d(Vector3 a, Vector3 b)
        {
            return Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.z - b.z) * (a.z - b.z));
        }



        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // [0, 1] -> [0, 1](Smooth)
        public static float SmoothLerp2(float t)
        {
            float percentage = 0.5f + Mathf.Sin(t * Mathf.PI - (Mathf.PI * 0.5f)) * 0.5f;
            //percentage = Mathf.Clamp01(percentage);
            return percentage;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static float SmoothLerp(float t)
        {
            return Mathf.Clamp01(Mathf.Sin(t * 90f));
        }
        public static float InverseLerp(float a, float b, float value)
        {
            if (a != b)
                return ((value - a) / (b - a));
            else
                return 0.0f;
        }
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // sin[-1, 1] -> sin[0, 1]
        public static float Sin01(float t)
        {
            return Mathf.Sin(t) * 0.5f + 0.5f;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // sphere1과 sphere2의 충돌 검사를 한다.
        // 리턴값이 트루일 경우 t초에 충돌한다.
        public static bool SphereCollisionDetect(Vector3 v1, Vector3 p1, float r1,
            Vector3 v2, Vector3 p2, float r2, ref float t)
        {
            Vector3 s = p1 - p2;        // vector between the centers of each sphere
            Vector3 v = v1 - v2;        // relative velocity between spheres
            float r = r1 + r2;

            float c1 = Vector3.Dot(s, s) - r * r; // if negative, they overlap
            if (c1 < 0.0) // if true, they already overlap
            {
                // This is bad ... we need to correct this by moving them a tiny fraction from each other
                //a->pos +=
                t = 0.0f;
                return true;
            }

            float a1 = Vector3.Dot(v, v);
            if (a1 < 0.00001f)
                return false; // does not move towards each other

            float b1 = Vector3.Dot(v, s);
            if (b1 >= 0.0)
                return false; // does not move towards each other

            float d1 = b1 * b1 - a1 * c1;
            if (d1 < 0.0)
                return false; // no real roots ... no collision

            t = (-b1 - Mathf.Sqrt(d1)) / a1;

            return true;
        }
        
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static int GetMax(int[] array, ref int index, int cheak = 99)
        {
            int max = 0;
            var tmp = index;
            index = 0;

            for (int i = 0; i < array.Length; i++)
            {
                if (max < array[i] && i != cheak)
                {
                    max = array[i];
                    index = i;
                }
            }
            return max;
        }
        #endregion

        #region [수학] Vector3
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vector3 MakeDirection(Vector3 p1, Vector3 p2)
        {
            return (p2 - p1).normalized;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vector3 MakeDirectionXZ(Vector3 p1, Vector3 p2)
        {
            var dir = new Vector3(p2.x - p1.x, 0f, p2.z - p1.z);
            return dir.normalized;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vector3 MakeDirectionXZ(Vector3 p, bool normalize = false)
        {
            p = new Vector3(p.x, 0f, p.z);
            if (normalize)
                p.Normalize();
            return p;
        }
        #endregion

        #region [유틸] Frame <-> Time
        public static float FrameToTime(int frame)
        {
            return (float)frame / 30f;
        }
        public static int TimeToFrame(float time)
        {
            return (int)(time * 30f);
        }
        #endregion

        #region [유틸] Color <-> Int
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Color IntToColor(int c)
        {
            var r = (float)((c >> 16) & 0xff);
            var g = (float)((c >> 8) & 0xff);
            var b = (float)(c & 0xff);
            return new Color(r / 255f, g / 255f, b / 255f);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Color32 IntToColor32(int c)
        {
            var r = (byte)((c >> 16) & 0xff);
            var g = (byte)((c >> 8) & 0xff);
            var b = (byte)(c & 0xff);
            return new Color32(r, g, b, 255);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static int ColorToInt(Color32 c)
        {
            var ci = (c.r >> 16) | (c.g >> 16) | (c.b);
            return ci;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static int ColorToInt(Color c)
        {
            return ColorToInt((Color32)c);
        }
        #endregion

        #region [메모리] Memset
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Memset(int[] target, int value)
        {
            Memset(target, value, target.Length);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Memset(int[] target, int value, int count)
        {
            int i;
            int blockSize = Math.Min(1024, count);

            for (i = 0; i < blockSize; i++)
                target[i] = value;

            if (i == count)
                return;

            int bytesCount = blockSize * sizeof(int);

            while (i + blockSize < count)
            {
                Buffer.BlockCopy(target, 0, target, i, bytesCount);
                i += blockSize;
            }

            bytesCount = (count - i) * sizeof(int);
            Buffer.BlockCopy(target, 0, target, i, bytesCount);
        }
        #endregion

        #region [KeyValuePair] Make
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static KeyValuePair<K, U> MakePair<K, U>()
        {
            return new KeyValuePair<K, U>();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static KeyValuePair<K, U> MakePair<K, U>(K key, U value)
        {
            return new KeyValuePair<K, U>(key, value);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static KeyValuePair<string, U> MakePair<U>(string key) where U : new()
        {
            return new KeyValuePair<string, U>(key, new U());
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 확률
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        static System.Random random = new System.Random();

        #region [랜덤] Values (Unity3D)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void RandomValues(int[] recordTime, int min, int max)
        {
            for (var i = 0; i < recordTime.Length; ++i)
                recordTime[i] = UnityEngine.Random.Range(min, max);
        }
        public static Vector3 RandomRange(Vector3 center, Vector3 size)
        {
            return center + RandomVector3(size);
        }
        public static Vector3 RandomRange(Vector3 center, float size_x, float size_z)
        {
            return center + RandomVector3(size_x, size_z);
        }
        public static Vector3 RandomVector3(Vector3 size)
        {
            return new Vector3(
               ((float)random.NextDouble() - 0.5f) * size.x,
               ((float)random.NextDouble() - 0.5f) * size.y,
               ((float)random.NextDouble() - 0.5f) * size.z
            );
        }
        public static Vector3 RandomVector3(float size_x, float size_z)
        {
            var random = new System.Random();
            var v3 = new Vector3(
               ((float)random.NextDouble() - 0.5f) * size_x,
               0f,
               ((float)random.NextDouble() - 0.5f) * size_z
            );
            return v3;
        }
        public static Vector3 RandomRange(BoxCollider box)
        {
            return RandomRange(box.center, box.size);
        }
        #endregion

        #region [랜덤] Values (System)
        static System.Random _random = new System.Random((int)DateTime.Now.Ticks);

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static float RandomRange(float min, float max)
        {
            if (min == max)
                return min;
            var r = min + (float)_random.NextDouble() * (max - min);
            return r;
        }
        public static int RandomRange(int min, int max)
        {
            if (min == max)
                return min;
            int r = min + _random.Next() % (max - min);
            return r;
        }

       
        #endregion

        #region [랜덤] Shuffle
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Shuffle<T>(T[] array)
        {
            Shuffle(array, array.Length * 2);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Shuffle<T>(T[] array, int count)
        {
            int arrayLen = array.Length;
            Shuffle(array, arrayLen, count);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Shuffle<T>(T[] array, int arrayLen, int count)
        {
            while (count > 1)
            {
                int k1 = RandomRange(0, arrayLen); // 0 <= k < n.
                int k2 = RandomRange(0, arrayLen); // 0 <= k < n.
                count--;
                T temp = array[k1];
                array[k1] = array[k2];
                array[k2] = temp;
            }
        }
        #endregion

    }

//}
