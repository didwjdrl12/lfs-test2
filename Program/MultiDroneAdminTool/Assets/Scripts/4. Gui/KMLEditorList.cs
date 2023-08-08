
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;
using System;

public class KMLEditorList : MonoBehaviour
{
    public GameObject _kmlEditPrefab;
    public Transform _ItemParent;

    private List<InputField> _inputFieldList = new List<InputField>();
    private int _arrInputFieldIndex = 0;

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

	// Start is called before the first frame update
	void Start()
    {
        if (_kmlEditPrefab == null)
            return;

		List<FPlacemark> listPla = new List<FPlacemark>();
        ReadKMLTextFile(ref listPla);
		Reflesh(listPla);

        //GameObject go;
        //for (int i = 0; i < 1; ++i)
        //{
        //    go = Instantiate<GameObject>(_kmlEditPrefab, _ItemParent);
        //    KMLData kd = go.GetComponent<KMLData>();
        //    _listKmlData.AddRange(kd._arrInputField);     
        //}
        //
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (++_arrInputFieldIndex >= _inputFieldList.Count)
            {
                _arrInputFieldIndex = 0;
            }
			_inputFieldList[_arrInputFieldIndex].Select();
        }
    }
    public void OnClickLoadKML()
    {
		List<FPlacemark> listPla = new List<FPlacemark>();
		ReadKMLTextFile(ref listPla);
		if (listPla.Count < 1)
			return;
		Reflesh(listPla);

		UpdateInputfieldList();
		_arrInputFieldIndex = 0;
		_inputFieldList[_arrInputFieldIndex].Select();
	}
	void OnClickInputField(InputField CurInputfield)
    {
		_arrInputFieldIndex = _inputFieldList.FindIndex(elem => elem == CurInputfield);
		if (_arrInputFieldIndex < 0)
			_arrInputFieldIndex = 0;

		//Debug.Log("kjs : OnClickInputField(), index=" + _arrInputFieldIndex);
    }
	void OnClickDelete(GameObject Go)
    {
		int idx = 0;
		foreach(Transform tr in _ItemParent)
        {
			if(tr.gameObject==Go)
            {
				break;
            }
			else
            {
				++idx;
            }
        }
		_arrInputFieldIndex = idx * Go.GetComponent<KMLData>()._arrInputField.Length;
		OnClickRemove();
    }
	private void UpdateInputfieldList()
    {
		_inputFieldList.Clear();
		KMLData kd;
		foreach(Transform tr in _ItemParent)
        {	
			if(!tr.gameObject.activeSelf)
            {
				//Debug.Log("kjs : UpdateInputfieldList(), Deactived obj");
				continue;				
            }
			kd = tr.GetComponent<KMLData>();
			
			_inputFieldList.AddRange(kd._arrInputField);
        }
		//Debug.Log("kjs : UpdateInputfieldList(), numInputFiled=" + _inputFieldList.Count);
    }
	private void Reflesh(List<FPlacemark> ListPlacemark)
    {
		int nPla = ListPlacemark != null ? ListPlacemark.Count : 0;
		if (nPla<1)
			return;

		GameObject go;
		KMLData kd;
		Transform tr;
		int cnt = 0;
		_arrInputFieldIndex = 0;
		//_inputFieldList.Clear();
		int nChild = _ItemParent.childCount;
		for (; cnt<nPla; ++cnt)
		{
			if(cnt>=nChild)
            {
				go = Instantiate<GameObject>(_kmlEditPrefab, _ItemParent);
				if (go == null)
					continue;
				kd = go.GetComponent<KMLData>();
				kd._onInputFieldClicked = OnClickInputField;
				kd._onClickDelete = OnClickDelete;
			}
			else
			{
				tr = _ItemParent.GetChild(cnt);
				kd = tr.gameObject.GetComponent<KMLData>();
			}
			UnityEngine.Assertions.Assert.IsNotNull(kd);
			kd.Data = ListPlacemark[cnt];			
			//_inputFieldList.AddRange(kd._arrInputField);
		}
		UpdateInputfieldList();
		_inputFieldList[_arrInputFieldIndex].Select();

		// 나머지 제거.
		for(; nChild>cnt; ++cnt)
        {
			Destroy(_ItemParent.GetChild(cnt).gameObject);
        }
	}
    public void OnClickAdd()
    {
		GameObject go = Instantiate<GameObject>(_kmlEditPrefab, _ItemParent);
		if (go == null)
			return;
		KMLData kd = go.GetComponent<KMLData>();
		
		kd._onInputFieldClicked = OnClickInputField;
		kd._onClickDelete = OnClickDelete;

		UpdateInputfieldList();
	}
    public void OnClickRemove()
    {
		int numInput = _kmlEditPrefab.GetComponent<KMLData>()._arrInputField.Length;
		int index = _arrInputFieldIndex / numInput;
		if(index>= _ItemParent.childCount)
        {
			Debug.Log("kjs : OnClickRemove(), Invalid Index, _arrInputfieldIndex=" + _arrInputFieldIndex);
			return;
        }
		Transform ch = _ItemParent.GetChild(index);
		if (ch == null)
			return;
		ch.gameObject.SetActive(false);
		Destroy(ch.gameObject);
				
		UpdateInputfieldList();
		_arrInputFieldIndex = 0;
		if (_inputFieldList.Count > 0)
		{			
			_inputFieldList[_arrInputFieldIndex].Select();
		}
	}
    public void OnClickRemoveAll()
    {
		foreach (Transform ch in _ItemParent)
		{
			Destroy(ch.gameObject);
		}
		_inputFieldList.Clear();
		_arrInputFieldIndex = 0;
	}
    public void OnClickSendData()
    {
        // test.
        //byte[] jsonBytes = File.ReadAllBytes("output_object_json_v1.3.json");
        //J2y.MultiDrone.MdaDataCenter.NetConnector.SendMessage(114, jsonBytes, jsonBytes.Length);
        //return;

        List<FPlacemark> listPla = new List<FPlacemark>();
		KMLData kd;
		foreach(Transform tr in _ItemParent)
        {
			kd = tr.gameObject.GetComponent<KMLData>();
			if (kd.Data.FbxName==null || kd.Data.FbxName.Length < 4 || kd.Data.Scale==null || !kd.Data.FbxName.StartsWith("kml_", StringComparison.Ordinal))
				continue;
			listPla.Add(kd.Data);
        }
        int maxBufferLen = 256 * (listPla != null ? listPla.Count : 0);
        if (maxBufferLen < 1)
        {
            int killall = 666666;
            byte[] src = BitConverter.GetBytes(killall);
            J2y.MultiDrone.MdaDataCenter.NetConnector.SendMessage((int)J2y.MultiDrone.ePacketProtocol.AS_KML, src, src.Length);
            return;
        }
        byte[] buffer = new byte[maxBufferLen];
        List<Vector2Int> header = new List<Vector2Int>();

        int offset = listPla.Count * 2 * 4 + 4; //  배열갯수 * Vector2Int byte size  + 배열갯수(4byte) 를 위한 메모리공간.
        Buffer.BlockCopy(BitConverter.GetBytes(listPla.Count), 0, buffer, 0, 4); // 리스트 갯수 == FPlacemark 배열갯수 (fbx 갯수.), 첫번째정보.

        Vector2Int startEnd = new Vector2Int(offset, 0); // offset : FPlacemark 정보가 시작되는 위치.

        foreach (FPlacemark sdata in listPla)
        {
            UnityEngine.Assertions.Assert.IsTrue(offset < maxBufferLen, "버퍼 최대 값을 벗어났다.");

            offset += sdata.ToBytes(ref buffer, offset);

            startEnd.y = offset;        // FPlacemark 정보끝 위치.			
            header.Add(startEnd);

            startEnd = new Vector2Int(offset, 0);
        }
        int realBufferSize = offset;

        //header.Add(startEnd);

        // write Header.
        offset = 4;
        foreach (Vector2Int h in header)
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
    }

    private void ReadKMLTextFile(ref List<FPlacemark> ResultData)
    {
        string path = Application.streamingAssetsPath + "/doc.txt";

        StreamReader reader = new StreamReader(path);

        string text = reader.ReadToEnd();
        reader.Close();

        if (!LoadPlacemarkList(text, 0, ref ResultData))
        {
            Debug.Log("kjs : Failed to LoadPlacemarkList()");
        }
    }
}
