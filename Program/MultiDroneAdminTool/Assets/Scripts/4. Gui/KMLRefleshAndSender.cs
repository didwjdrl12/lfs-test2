using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;
//using J2y.Network;
using System.Text;

//public enum  EAltitudeMode : byte
//{
//	relativeToGround,
//	clampToGround,
//	absolute,
//	clampToSeaFloor,
//	relativeToSeaFloor
//};
//public struct FPlacemark
//{
//	public double LookLongitude;  // 2
//	public double LookLatitude;   // 2
//	public double LookAltitude;   //2
//	public float LookRange;       //1
//	public float LookTilt;        //1
//	public float LookHeading;     //1
//	public EAltitudeMode AltitudeMode; 

//	public double Longitude;  //2
//	public double Latitude;  //2
//	public double Altitude;  //2

//	public float Heading;    //1
//	public float Tilt;       //1
//	public float Roll;       //1

//	public Vector3 Scale;    //3
//	public string FbxName;

//	public int ToBytes(ref byte[] arr, int StartArrIndex)
//    {
//		int dstOffset = StartArrIndex;
//		Buffer.BlockCopy(BitConverter.GetBytes(LookLongitude), 0, arr, dstOffset, sizeof(double));		dstOffset += sizeof(double);
//		Buffer.BlockCopy(BitConverter.GetBytes(LookLatitude), 0, arr, dstOffset, sizeof(double));			dstOffset += sizeof(double);
//		Buffer.BlockCopy(BitConverter.GetBytes(LookAltitude), 0, arr, dstOffset, sizeof(double));			dstOffset += sizeof(double);
//		Buffer.BlockCopy(BitConverter.GetBytes(LookRange), 0, arr, dstOffset, sizeof(float));			dstOffset += sizeof(float);
//		Buffer.BlockCopy(BitConverter.GetBytes(LookTilt), 0, arr, dstOffset, sizeof(float));				dstOffset += sizeof(float);
//		Buffer.BlockCopy(BitConverter.GetBytes(LookHeading), 0, arr, dstOffset, sizeof(float));			dstOffset += sizeof(float);
//		Buffer.BlockCopy(BitConverter.GetBytes((byte)AltitudeMode), 0, arr, dstOffset, sizeof(byte));		dstOffset += sizeof(byte);

//		Buffer.BlockCopy(BitConverter.GetBytes(Longitude), 0, arr, dstOffset, sizeof(double));	dstOffset += sizeof(double);
//		Buffer.BlockCopy(BitConverter.GetBytes(Latitude), 0, arr, dstOffset, sizeof(double));	dstOffset += sizeof(double);
//		Buffer.BlockCopy(BitConverter.GetBytes(Altitude), 0, arr, dstOffset, sizeof(double));	dstOffset += sizeof(double);

//		Buffer.BlockCopy(BitConverter.GetBytes(Heading), 0, arr, dstOffset, sizeof(float));		dstOffset += sizeof(float);
//		Buffer.BlockCopy(BitConverter.GetBytes(Tilt), 0, arr, dstOffset, sizeof(float));			dstOffset += sizeof(float);
//		Buffer.BlockCopy(BitConverter.GetBytes(Roll), 0, arr, dstOffset, sizeof(float));			dstOffset += sizeof(float);

//		if (Scale == null)
//			Scale = Vector3.one;
//		Buffer.BlockCopy(BitConverter.GetBytes(Scale.x), 0, arr, dstOffset, sizeof(float));		dstOffset += sizeof(float);
//		Buffer.BlockCopy(BitConverter.GetBytes(Scale.y), 0, arr, dstOffset, sizeof(float));		dstOffset += sizeof(float);
//		Buffer.BlockCopy(BitConverter.GetBytes(Scale.z), 0, arr, dstOffset, sizeof(float));		dstOffset += sizeof(float);

//		if (FbxName == null)
//			FbxName = "NM";
//		byte[] strBytes = Encoding.UTF8.GetBytes(FbxName);
//		Buffer.BlockCopy(strBytes, 0, arr, dstOffset, sizeof(byte)*strBytes.Length);			dstOffset += sizeof(byte)*strBytes.Length;

//		return dstOffset-StartArrIndex;
//	}
//	public bool ToStruct(byte[] Src, int StartSrcOffset, int EndSrcOffset)
//    {
//		int srcOffset = StartSrcOffset;

//		LookLongitude = BitConverter.ToDouble(Src, srcOffset);		srcOffset += sizeof(double);
//		if (srcOffset > EndSrcOffset)
//			return false;
//		LookLatitude = BitConverter.ToDouble(Src, srcOffset);			srcOffset += sizeof(double);
//		if (srcOffset > EndSrcOffset)
//			return false;
//		LookAltitude = BitConverter.ToDouble(Src, srcOffset);			srcOffset += sizeof(double);
//		if (srcOffset > EndSrcOffset)
//			return false;

//		LookRange = BitConverter.ToSingle(Src, srcOffset);			srcOffset += sizeof(float);
//		if (srcOffset > EndSrcOffset)
//			return false;
//		LookTilt = BitConverter.ToSingle(Src, srcOffset);				srcOffset += sizeof(float);
//		if (srcOffset > EndSrcOffset)
//			return false;
//		LookHeading = BitConverter.ToSingle(Src, srcOffset);			srcOffset += sizeof(float);
//		if (srcOffset > EndSrcOffset)
//			return false;
//		AltitudeMode = (EAltitudeMode)Src[srcOffset];				srcOffset += sizeof(byte);
//		if (srcOffset > EndSrcOffset)
//			return false;


//		Longitude = BitConverter.ToDouble(Src, srcOffset);				srcOffset += sizeof(double);
//		if (srcOffset > EndSrcOffset)
//			return false;
//		Latitude = BitConverter.ToDouble(Src, srcOffset);				srcOffset += sizeof(double);
//		if (srcOffset > EndSrcOffset)
//			return false;
//		Altitude = BitConverter.ToDouble(Src, srcOffset);				srcOffset += sizeof(double);
//		if (srcOffset > EndSrcOffset)
//			return false;

//		Heading = BitConverter.ToSingle(Src, srcOffset);				srcOffset += sizeof(float);
//		if (srcOffset > EndSrcOffset)
//			return false;
//		Tilt = BitConverter.ToSingle(Src, srcOffset);				srcOffset += sizeof(float);
//		if (srcOffset > EndSrcOffset)
//			return false;
//		Roll = BitConverter.ToSingle(Src, srcOffset);				srcOffset += sizeof(float);
//		if (srcOffset > EndSrcOffset)
//			return false;

//		Scale = Vector3.one;
//		Scale.x = BitConverter.ToSingle(Src, srcOffset);				srcOffset += sizeof(float);
//		if (srcOffset > EndSrcOffset)
//			return false;
//		Scale.y = BitConverter.ToSingle(Src, srcOffset);				srcOffset += sizeof(float);
//		if (srcOffset > EndSrcOffset)
//			return false;
//		Scale.z = BitConverter.ToSingle(Src, srcOffset);				srcOffset += sizeof(float);
//		if (srcOffset > EndSrcOffset)
//			return false;

//		int strlen = EndSrcOffset - srcOffset;
//		if (strlen <= 3)
//			return false;
//		byte[] nameArr = new byte[strlen ];
//		Buffer.BlockCopy(Src, srcOffset, nameArr, 0, strlen);

//		FbxName = Encoding.UTF8.GetString(nameArr);

//		return true;
//    }
//};

public class KMLRefleshAndSender : MonoBehaviour
{
    private Text _text;
	private List<FPlacemark> _placemarkList;


	//public static byte[] ConvertDoubleToByteArray(double d)
	//{
	//	return BitConverter.GetBytes(d);
	//}
	//public static double ConvertByteArrayToDouble(byte[] b)
	//{
	//	return BitConverter.ToDouble(b, 0);
	//}
	//public T ByteArrayToStruct<T>(byte[] buffer) where T : struct
	//{
	//	int size = Marshal.SizeOf(typeof(T));

	//	Debug.LogFormat("kjs : ByteArrayToStruct(), size={0}, bufferLen={1}" , size, buffer.Length);

	//	if (size > buffer.Length)
	//	{
	//		throw new Exception();
	//	}

	//	IntPtr ptr = Marshal.AllocHGlobal(size);
	//	Marshal.Copy(buffer, 0, ptr, size);
	//	T obj = (T)Marshal.PtrToStructure(ptr, typeof(T));
	//	Marshal.FreeHGlobal(ptr);
	//	return obj;
	//}
	//public byte[] StructToByteArray(object obj)
	//{
	//	int size = Marshal.SizeOf(obj);
	//	Debug.Log("kjs : StructToByteArray(), size=" + size);
	//	byte[] arr = new byte[size];
	//	IntPtr ptr = Marshal.AllocHGlobal(size);

	//	Marshal.StructureToPtr(obj, ptr, true);
	//	Marshal.Copy(ptr, arr, 0, size);
	//	Marshal.FreeHGlobal(ptr);
	//	return arr;
	//}
	#region KML Loading
	bool FindLookatElements(string textContent, int StartIndex,
							ref double dLongitude,
							ref double dLatitude,
							ref double dAltitude,
							ref double dRange,
							ref double dTilt,
							ref double dHeading,
							ref EAltitudeMode AltitudeMode)
	{
		int nstart = 0, nend = 0;
		int nLookAt = textContent.IndexOf("<LookAt>", StartIndex, System.StringComparison.OrdinalIgnoreCase);
		if (nLookAt < 0)
		{
			nstart = textContent.IndexOf("<altitudeMode>", nLookAt + 1, System.StringComparison.OrdinalIgnoreCase);
			nend = textContent.IndexOf("</altitudeMode>", nstart + 1, System.StringComparison.OrdinalIgnoreCase);
			if (nend > nstart && nstart >= 0)
			{
				string str = textContent.Substring(nstart + 14, nend - nstart - 14);
				AltitudeMode =
					str == "relativeToGround" ? EAltitudeMode.relativeToGround :
					str == "clampToGround" ? EAltitudeMode.clampToGround :
					str == "absolute" ? EAltitudeMode.absolute :
					str == "clampToSeaFloor" ? EAltitudeMode.clampToSeaFloor :
					str == "relativeToSeaFloor" ? EAltitudeMode.relativeToSeaFloor : EAltitudeMode.relativeToGround;
				return true;
			}
			return false;
		}
		dLongitude = dLatitude = dRange = dTilt = dHeading = 0;
		string value;
		bool reval = false;

		// longitude
		nstart = textContent.IndexOf("<longitude>", nLookAt, System.StringComparison.OrdinalIgnoreCase);
		nend = textContent.IndexOf("</longitude>", nstart + 1, System.StringComparison.OrdinalIgnoreCase);
		if (nend > nstart)
		{
			value = textContent.Substring(nstart + 11, nend - nstart - 11);
			reval = double.TryParse(value, out dLongitude);
			if (!reval)
				Debug.Log("kjs : Longitude TryParse()failed, value=" + value);
		}

		// latitude
		nstart = textContent.IndexOf("<latitude>", nLookAt + 1, System.StringComparison.OrdinalIgnoreCase);
		nend = textContent.IndexOf("</latitude>", nstart + 1, System.StringComparison.OrdinalIgnoreCase);
		if (nend > nstart)
		{
			value = textContent.Substring(nstart + 10, nend - nstart - 10);
			reval = double.TryParse(value, out dLatitude);
			if (!reval)
				Debug.Log("kjs : Latitude TryParse()failed, value=" + value);
		}
		// altitude
		nstart = textContent.IndexOf("<altitude>", nLookAt + 1, System.StringComparison.OrdinalIgnoreCase);
		nend = textContent.IndexOf("</altitude>", nstart + 1, System.StringComparison.OrdinalIgnoreCase);
		if (nend > nstart)
		{
			value = textContent.Substring(nstart + 10, nend - nstart - 10);
			reval = double.TryParse(value, out dAltitude);
			if (!reval)
				Debug.Log("kjs : Altitude TryParse()failed, value=" + value);
		}

		// range
		nstart = textContent.IndexOf("<range>", nLookAt + 1, System.StringComparison.OrdinalIgnoreCase);
		nend = textContent.IndexOf("</range>", nstart + 1, System.StringComparison.OrdinalIgnoreCase);
		if (nend > nstart)
		{
			value = textContent.Substring(nstart + 7, nend - nstart - 7);
			reval = double.TryParse(value, out dRange);
			if (!reval) Debug.Log("kjs : Latitude TryParse()failed, value=" + value);
		}

		// tilt.
		nstart = textContent.IndexOf("<tilt>", nLookAt + 1, System.StringComparison.OrdinalIgnoreCase);
		nend = textContent.IndexOf("</tilt>", nstart + 1, System.StringComparison.OrdinalIgnoreCase);
		if (nend > nstart)
		{
			value = textContent.Substring(nstart + 6, nend - nstart - 6);
			reval = double.TryParse(value, out dTilt);
			if (!reval) Debug.Log("kjs : Tilt TryParse()failed, value=" + value);
		}

		// heading.
		nstart = textContent.IndexOf("<heading>", nLookAt + 1, System.StringComparison.OrdinalIgnoreCase);
		nend = textContent.IndexOf("</heading>", nstart + 1, System.StringComparison.OrdinalIgnoreCase);
		if (nend > nstart)
		{
			value = textContent.Substring(nstart + 9, nend - nstart - 9);
			reval = double.TryParse(value, out dHeading);
			if (!reval)
				Debug.Log("kjs : Heading TryParse()failed, value=" + value);
		}

		//altitude mode
		nstart = textContent.IndexOf("<altitudeMode>", nLookAt + 1, System.StringComparison.OrdinalIgnoreCase);
		nend = textContent.IndexOf("</altitudeMode>", nstart + 1, System.StringComparison.OrdinalIgnoreCase);
		if (nend > nstart)
		{
			string str = textContent.Substring(nstart + 14, nend - nstart - 14);
			AltitudeMode =
				str == "relativeToGround" ? EAltitudeMode.relativeToGround :
				str == "clampToGround" ? EAltitudeMode.clampToGround :
				str == "absolute" ? EAltitudeMode.absolute :
				str == "clampToSeaFloor" ? EAltitudeMode.clampToSeaFloor :
				str == "relativeToSeaFloor" ? EAltitudeMode.relativeToSeaFloor : EAltitudeMode.relativeToGround;
		}

		return reval;
	}
	bool FindLocationElements(string textContent, int StartIndex,
							ref double dLongitude,
							ref double dLatitude,
							ref double dAltitude)
	{
		int nLoc = textContent.IndexOf("<Location>", StartIndex);
		if (nLoc < 0)
			return false;

		dLongitude = dLatitude = dAltitude = 0;
		string value;
		bool reval = false;

		// longitude
		int nstart = textContent.IndexOf("<longitude>", nLoc, System.StringComparison.OrdinalIgnoreCase);
		int nend = textContent.IndexOf("</longitude>", nstart + 1, System.StringComparison.OrdinalIgnoreCase);
		if (nend > nstart)
		{
			value = textContent.Substring(nstart + 11, nend - nstart - 11);
			reval = double.TryParse(value, out dLongitude);
			if (!reval)
				Debug.Log("kjs : Location Longitude TryParse()failed, value=" + value);
		}

		// latitude
		nstart = textContent.IndexOf("<latitude>", nLoc + 1, System.StringComparison.OrdinalIgnoreCase);
		nend = textContent.IndexOf("</latitude>", nstart + 1, System.StringComparison.OrdinalIgnoreCase);
		if (nend > nstart)
		{
			value = textContent.Substring(nstart + 10, nend - nstart - 10);
			reval = double.TryParse(value, out dLatitude);
			if (!reval)
				Debug.Log("kjs : Location Latitude TryParse()failed, value=" + value);
		}

		// altitude
		nstart = textContent.IndexOf("<altitude>", nLoc + 1, System.StringComparison.OrdinalIgnoreCase);
		nend = textContent.IndexOf("</altitude>", nstart + 1, System.StringComparison.OrdinalIgnoreCase);
		if (nend > nstart)
		{
			value = textContent.Substring(nstart + 10, nend - nstart - 10);
			reval = double.TryParse(value, out dAltitude);
			if (!reval)
				Debug.Log("kjs : Location Latitude TryParse()failed, value=" + value);
		}
		return reval;
	}
	bool FindOrientationElements(string textContent, int StartIndex,
								ref double dHeading,
								ref double dTilt,
								ref double dRoll)
	{
		int nLoc = textContent.IndexOf("<Orientation>", StartIndex);
		if (nLoc < 0)
			return false;

		dHeading = dTilt = dRoll = 0;
		string value;
		bool reval = false;

		// heading
		int nstart = textContent.IndexOf("<heading>", nLoc, System.StringComparison.OrdinalIgnoreCase);
		int nend = textContent.IndexOf("</heading>", nstart + 1, System.StringComparison.OrdinalIgnoreCase);
		if (nend > nstart)
		{
			value = textContent.Substring(nstart + 9, nend - nstart - 9);
			reval = double.TryParse(value, out dHeading);
			if (!reval)
				Debug.Log("kjs : Orientation Heading TryParse()failed, value=" + value);
		}

		// tilt
		nstart = textContent.IndexOf("<tilt>", nLoc + 1, System.StringComparison.OrdinalIgnoreCase);
		nend = textContent.IndexOf("</tilt>", nstart + 1, System.StringComparison.OrdinalIgnoreCase);
		if (nend > nstart)
		{
			value = textContent.Substring(nstart + 6, nend - nstart - 6);
			reval = double.TryParse(value, out dTilt);
			if (!reval)
				Debug.Log("kjs : Orientation Tilt TryParse()failed, value=" + value);
		}

		// roll
		nstart = textContent.IndexOf("<roll>", nLoc + 1, System.StringComparison.OrdinalIgnoreCase);
		nend = textContent.IndexOf("</roll>", nstart + 1, System.StringComparison.OrdinalIgnoreCase);
		if (nend > nstart)
		{
			value = textContent.Substring(nstart + 6, nend - nstart - 6);
			reval = double.TryParse(value, out dRoll);
			if (!reval)
				Debug.Log("kjs : Orientation Roll TryParse()failed, value=" + value);
		}
		return reval;
	}
	bool FindScaleElements(string textContent, int StartIndex, ref Vector3 Scale)
	{
		int nLoc = textContent.IndexOf("<Scale>", StartIndex);
		if (nLoc < 0)
			return false;

		Scale = Vector3.zero;
		string value;
		bool reval = false;

		// x
		int nstart = textContent.IndexOf("<x>", nLoc, System.StringComparison.OrdinalIgnoreCase);
		int nend = textContent.IndexOf("</x>", nstart + 1, System.StringComparison.OrdinalIgnoreCase);
		if (nend > nstart)
		{
			value = textContent.Substring(nstart + 3, nend - nstart - 3);
			reval = float.TryParse(value, out Scale.x);
			if (!reval)
				Debug.Log("kjs : Scale X TryParse()failed, value=" + value);
		}

		// y
		nstart = textContent.IndexOf("<y>", nLoc + 1, System.StringComparison.OrdinalIgnoreCase);
		nend = textContent.IndexOf("</y>", nstart + 1, System.StringComparison.OrdinalIgnoreCase);
		if (nend > nstart)
		{
			value = textContent.Substring(nstart + 3, nend - nstart - 3);
			reval = float.TryParse(value, out Scale.y);
			if (!reval) Debug.Log("kjs : Scale y TryParse()failed, value=" + value);
		}

		// z
		nstart = textContent.IndexOf("<z>", nLoc + 1, System.StringComparison.OrdinalIgnoreCase);
		nend = textContent.IndexOf("</z>", nstart + 1, System.StringComparison.OrdinalIgnoreCase);
		if (nend > nstart)
		{
			value = textContent.Substring(nstart + 3, nend - nstart - 3);
			reval = float.TryParse(value, out Scale.z);
			if (!reval)
				Debug.Log("kjs : Scale z TryParse()failed, value=" + value);
		}
		return reval;
	}
	bool FindFbxElements(string textContent, int StartIndex, ref string FbxName)
	{
		int nLoc = textContent.IndexOf("<Link>", StartIndex);
		if (nLoc < 0)
			return false;

		bool reval = false;

		int nstart = textContent.IndexOf("/", nLoc + 6, System.StringComparison.OrdinalIgnoreCase);
		int nend = textContent.IndexOf(".fbx", nstart + 1, System.StringComparison.OrdinalIgnoreCase);
		if (nend > nstart)
		{
			FbxName = textContent.Substring(nstart, nend - nstart + 4);
			FbxName = Path.GetFileName(FbxName);
			reval = FbxName.Length > 3;
		}
		return reval;
	}

	bool LoadPlacemarkList(string textContent, int StartCharIndex, ref List<FPlacemark> Arr)
	{
		double lookLongitude = 0, lookLaditude = 0, lookAltitude = 0, lookRange = 0, lookTilt = 0, lookHeading = 0; EAltitudeMode AltitudeMode = EAltitudeMode.absolute;
		double altitude = 0, latitude = 0, longitude = 0;
		double heading = 0, tilt = 0, roll = 0;
		Vector3 scale = Vector3.one;
		string fbxName = "";

		int nplacemark = textContent.IndexOf("<Placemark>", StartCharIndex);
		if (nplacemark < 0)
			return Arr.Count > 0;

		FindLookatElements(textContent, nplacemark + 1, ref lookLongitude, ref lookLaditude, ref lookAltitude, ref lookRange, ref lookTilt, ref lookHeading, ref AltitudeMode);
		FindLocationElements(textContent, nplacemark + 1, ref longitude, ref latitude, ref altitude);
		FindOrientationElements(textContent, nplacemark + 1, ref heading, ref tilt, ref roll);
		FindScaleElements(textContent, nplacemark + 1, ref scale);
		FindFbxElements(textContent, nplacemark + 1, ref fbxName);
		FPlacemark fpm = new FPlacemark();
		fpm.LookLongitude = lookLongitude;
		fpm.LookLatitude = lookLaditude;
		fpm.LookAltitude = lookAltitude;
		fpm.LookRange = (float)lookRange;
		fpm.LookTilt = (float)lookTilt;
		fpm.LookHeading = (float)lookHeading;
		fpm.AltitudeMode = AltitudeMode;

		fpm.Altitude = altitude;
		fpm.Latitude = latitude;
		fpm.Longitude = longitude;

		fpm.Heading = (float)heading;
		fpm.Tilt = (float)tilt;
		fpm.Roll = (float)roll;
		fpm.Scale = scale;
		fpm.FbxName = fbxName;
		//fpm.FbxName = new char[fbxName.Length];
		//Buffer.BlockCopy(fbxName.ToCharArray(), 0, fpm.FbxName, 0, fbxName.Length);

		Arr.Add(fpm);

		nplacemark = textContent.IndexOf("</Placemark>", nplacemark + 1);
		if (nplacemark < 0)
			return true;
		int n = textContent.IndexOf("<Placemark>", nplacemark);
		if (n >= 0)
			return LoadPlacemarkList(textContent, nplacemark + 1, ref Arr);
		else
			return true;
	}
    #endregion
    void Start()
    {
        Transform tr = transform.Find("TextMask/TextContainer/Text");
        if(tr != null)
        {
            _text = tr.GetComponent<Text>();
            UnityEngine.Assertions.Assert.IsNotNull(_text);
            ReadKMLTextFile();
        }
        else
        {
            Debug.Log("kjs : KMLRefleshAndSender::Text IndexOf Failed!");
        }
		// test.
		//FPlacemark pla = new FPlacemark();
		//pla.FbxName = "rome12121212너구리두루미 학꽁치 121212111ffffffff1111111212111111111111111ocallito";
		//pla.LookAltitude = 1004;
		//pla.Scale = new Vector3(1, 2, 3);

		//int arrSize = 0;
		//byte[] arr = pla.ToBytes(ref arrSize);

		//FPlacemark dst = new FPlacemark();
		//dst.ToStruct(arr, arrSize);

		//Debug.Log("kjs : arrSize=" + arrSize);

		ReadKMLTextFile();
	}

    public void OnClickReflesh()
    {
        ReadKMLTextFile();
    }
    public void OnClickSendData()
    {  
        int maxBufferLen = 256 * (_placemarkList!=null ? _placemarkList.Count : 0);
		if (maxBufferLen < 1)
		{
			int killall = 666666;
			byte[] src = BitConverter.GetBytes(killall);
			J2y.MultiDrone.MdaDataCenter.NetConnector.SendMessage((int)J2y.MultiDrone.ePacketProtocol.AS_KML, src, src.Length);
			return;
		}
		byte[] buffer = new byte[maxBufferLen];
		List<Vector2Int> header = new List<Vector2Int>();

		int offset = _placemarkList.Count*2 * 4 + 4; //  배열갯수 * Vector2Int byte size  + 배열갯수(4byte) 를 위한 메모리공간.
		Buffer.BlockCopy(BitConverter.GetBytes(_placemarkList.Count), 0, buffer, 0, 4); // 리스트 갯수 == FPlacemark 배열갯수 (fbx 갯수.), 첫번째정보.

		Vector2Int startEnd = new Vector2Int(offset, 0); // offset : FPlacemark 정보가 시작되는 위치.

		foreach(FPlacemark sdata in _placemarkList)
        {
			UnityEngine.Assertions.Assert.IsTrue(offset < maxBufferLen , "버퍼 최대 값을 벗어났다.");

			offset += sdata.ToBytes(ref buffer, offset);
			
			startEnd.y = offset;		// FPlacemark 정보끝 위치.			
			header.Add(startEnd);

			startEnd = new Vector2Int(offset, 0);
		}
		int realBufferSize = offset;

		//header.Add(startEnd);

		// write Header.
		offset = 4;
		foreach(Vector2Int h in header)
        {
			Buffer.BlockCopy(BitConverter.GetBytes(h.x), 0, buffer, offset, 4); offset += 4;
			Buffer.BlockCopy(BitConverter.GetBytes(h.y), 0, buffer, offset, 4); offset += 4;
		}

		// test. recover to struct.
		//int numArr = BitConverter.ToInt32(buffer, 0); offset = 4;
		//FPlacemark[] arr = new FPlacemark[numArr];
		//Vector2Int[] head = new Vector2Int[numArr];

		//int startData = numArr * 2 * 4 + 4;
		////Vector2Int se = Vector2Int.zero;
		//int i = 0;
		//for(; i<numArr; ++i)
		//{
		//	head[i].x = BitConverter.ToInt32(buffer, offset);		offset += 4;
		//	head[i].y = BitConverter.ToInt32(buffer, offset);		offset += 4;
		//	//head[i] = se;
		//	UnityEngine.Assertions.Assert.IsTrue(head[i].y > head[i].x);// se.y > se.x);
		//}
		//FPlacemark[] temp = new FPlacemark[numArr];
		//i = 0;
		//foreach(Vector2Int v in head)
		//{
		//	temp[i] = new FPlacemark();
		//	UnityEngine.Assertions.Assert.IsTrue(v.y > v.x);
		//	temp[i].ToStruct(buffer, v.x, v.y);
		//	++i;
		//}

		J2y.MultiDrone.MdaDataCenter.NetConnector.SendMessage((int)J2y.MultiDrone.ePacketProtocol.AS_KML, buffer, realBufferSize);

		DateTime dt = DateTime.Now;
		int ms  = dt.Millisecond;
		Debug.LogFormat("kjs : start Load KML Model Time, Hour:{0}, Minute:{1}, Sec:{2}, MilliSec:{3}", dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
	}
    private void ReadKMLTextFile()
    {
        string path = Application.streamingAssetsPath + "/doc.txt";
        
        StreamReader reader = new StreamReader(path);
        
        _text.text = reader.ReadToEnd();
        reader.Close();

		List<FPlacemark> PlacemarkList = new List<FPlacemark>();
		if(LoadPlacemarkList(_text.text, 0, ref PlacemarkList))
        {
			_placemarkList = PlacemarkList;
        }
		else
        {
			if(_placemarkList!=null && _placemarkList.Count>0)
				_placemarkList.Clear();
			Debug.Log("kjs : Failed to LoadPlacemarkList()");
        }
	}	
}
