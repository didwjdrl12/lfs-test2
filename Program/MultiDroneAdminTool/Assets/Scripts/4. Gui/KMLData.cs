using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using UnityEngine.EventSystems;

public enum EAltitudeMode : byte
{
	relativeToGround,
	clampToGround,
	absolute,
	clampToSeaFloor,
	relativeToSeaFloor
};
public struct FPlacemark
{
	public double LookLongitude;  // 2
	public double LookLatitude;   // 2
	public double LookAltitude;   //2
	public float LookRange;       //1
	public float LookTilt;        //1
	public float LookHeading;     //1
	public EAltitudeMode AltitudeMode;

	public double Longitude;  //2
	public double Latitude;  //2
	public double Altitude;  //2

	public float Heading;    //1
	public float Tilt;       //1
	public float Roll;       //1

	public Vector3 Scale;    //3
	public string FbxName;

	public int ToBytes(ref byte[] arr, int StartArrIndex)
	{
		int dstOffset = StartArrIndex;
		Buffer.BlockCopy(BitConverter.GetBytes(LookLongitude), 0, arr, dstOffset, sizeof(double)); dstOffset += sizeof(double);
		Buffer.BlockCopy(BitConverter.GetBytes(LookLatitude), 0, arr, dstOffset, sizeof(double)); dstOffset += sizeof(double);
		Buffer.BlockCopy(BitConverter.GetBytes(LookAltitude), 0, arr, dstOffset, sizeof(double)); dstOffset += sizeof(double);
		Buffer.BlockCopy(BitConverter.GetBytes(LookRange), 0, arr, dstOffset, sizeof(float)); dstOffset += sizeof(float);
		Buffer.BlockCopy(BitConverter.GetBytes(LookTilt), 0, arr, dstOffset, sizeof(float)); dstOffset += sizeof(float);
		Buffer.BlockCopy(BitConverter.GetBytes(LookHeading), 0, arr, dstOffset, sizeof(float)); dstOffset += sizeof(float);
		Buffer.BlockCopy(BitConverter.GetBytes((byte)AltitudeMode), 0, arr, dstOffset, sizeof(byte)); dstOffset += sizeof(byte);

		Buffer.BlockCopy(BitConverter.GetBytes(Longitude), 0, arr, dstOffset, sizeof(double)); dstOffset += sizeof(double);
		Buffer.BlockCopy(BitConverter.GetBytes(Latitude), 0, arr, dstOffset, sizeof(double)); dstOffset += sizeof(double);
		Buffer.BlockCopy(BitConverter.GetBytes(Altitude), 0, arr, dstOffset, sizeof(double)); dstOffset += sizeof(double);

		Buffer.BlockCopy(BitConverter.GetBytes(Heading), 0, arr, dstOffset, sizeof(float)); dstOffset += sizeof(float);
		Buffer.BlockCopy(BitConverter.GetBytes(Tilt), 0, arr, dstOffset, sizeof(float)); dstOffset += sizeof(float);
		Buffer.BlockCopy(BitConverter.GetBytes(Roll), 0, arr, dstOffset, sizeof(float)); dstOffset += sizeof(float);

		if (Scale == null)
			Scale = Vector3.one;
		Buffer.BlockCopy(BitConverter.GetBytes(Scale.x), 0, arr, dstOffset, sizeof(float)); dstOffset += sizeof(float);
		Buffer.BlockCopy(BitConverter.GetBytes(Scale.y), 0, arr, dstOffset, sizeof(float)); dstOffset += sizeof(float);
		Buffer.BlockCopy(BitConverter.GetBytes(Scale.z), 0, arr, dstOffset, sizeof(float)); dstOffset += sizeof(float);

		if (FbxName == null)
			FbxName = "NM";
		byte[] strBytes = Encoding.UTF8.GetBytes(FbxName);
		Buffer.BlockCopy(strBytes, 0, arr, dstOffset, sizeof(byte) * strBytes.Length); dstOffset += sizeof(byte) * strBytes.Length;

		return dstOffset - StartArrIndex;
	}
	public bool ToStruct(byte[] Src, int StartSrcOffset, int EndSrcOffset)
	{
		int srcOffset = StartSrcOffset;

		LookLongitude = BitConverter.ToDouble(Src, srcOffset); srcOffset += sizeof(double);
		if (srcOffset > EndSrcOffset)
			return false;
		LookLatitude = BitConverter.ToDouble(Src, srcOffset); srcOffset += sizeof(double);
		if (srcOffset > EndSrcOffset)
			return false;
		LookAltitude = BitConverter.ToDouble(Src, srcOffset); srcOffset += sizeof(double);
		if (srcOffset > EndSrcOffset)
			return false;

		LookRange = BitConverter.ToSingle(Src, srcOffset); srcOffset += sizeof(float);
		if (srcOffset > EndSrcOffset)
			return false;
		LookTilt = BitConverter.ToSingle(Src, srcOffset); srcOffset += sizeof(float);
		if (srcOffset > EndSrcOffset)
			return false;
		LookHeading = BitConverter.ToSingle(Src, srcOffset); srcOffset += sizeof(float);
		if (srcOffset > EndSrcOffset)
			return false;
		AltitudeMode = (EAltitudeMode)Src[srcOffset]; srcOffset += sizeof(byte);
		if (srcOffset > EndSrcOffset)
			return false;

		Longitude = BitConverter.ToDouble(Src, srcOffset); srcOffset += sizeof(double);
		if (srcOffset > EndSrcOffset)
			return false;
		Latitude = BitConverter.ToDouble(Src, srcOffset); srcOffset += sizeof(double);
		if (srcOffset > EndSrcOffset)
			return false;
		Altitude = BitConverter.ToDouble(Src, srcOffset); srcOffset += sizeof(double);
		if (srcOffset > EndSrcOffset)
			return false;

		Heading = BitConverter.ToSingle(Src, srcOffset); srcOffset += sizeof(float);
		if (srcOffset > EndSrcOffset)
			return false;
		Tilt = BitConverter.ToSingle(Src, srcOffset); srcOffset += sizeof(float);
		if (srcOffset > EndSrcOffset)
			return false;
		Roll = BitConverter.ToSingle(Src, srcOffset); srcOffset += sizeof(float);
		if (srcOffset > EndSrcOffset)
			return false;

		Scale = Vector3.one;
		Scale.x = BitConverter.ToSingle(Src, srcOffset); srcOffset += sizeof(float);
		if (srcOffset > EndSrcOffset)
			return false;
		Scale.y = BitConverter.ToSingle(Src, srcOffset); srcOffset += sizeof(float);
		if (srcOffset > EndSrcOffset)
			return false;
		Scale.z = BitConverter.ToSingle(Src, srcOffset); srcOffset += sizeof(float);
		if (srcOffset > EndSrcOffset)
			return false;

		int strlen = EndSrcOffset - srcOffset;
		if (strlen <= 3)
			return false;
		byte[] nameArr = new byte[strlen];
		Buffer.BlockCopy(Src, srcOffset, nameArr, 0, strlen);

		FbxName = Encoding.UTF8.GetString(nameArr);

		return true;
	}
};

public class KMLData : MonoBehaviour//, IPointerClickHandler
{
	public delegate void OnInputFieldClicked(InputField CurInputField);
	public OnInputFieldClicked _onInputFieldClicked;
	public delegate void OnClickDel(GameObject Go);
	public OnClickDel _onClickDelete;

    public InputField[] _arrInputField;
    public FPlacemark Data { get { return _data; } set
        {
            _data = value;
            _arrInputField[0].text = _data.Longitude.ToString();
            _arrInputField[1].text = _data.Latitude.ToString();
            _arrInputField[2].text = _data.Altitude.ToString();

            _arrInputField[3].text = _data.Heading.ToString();
            _arrInputField[4].text = _data.Tilt.ToString();
            _arrInputField[5].text = _data.Roll.ToString();

            _arrInputField[6].text = _data.Scale.x.ToString();
            _arrInputField[7].text = _data.Scale.y.ToString();
            _arrInputField[8].text = _data.Scale.z.ToString();

            _arrInputField[9].text = _data.FbxName;
        } }

    private FPlacemark _data;

	public void OnValueChanged(string Str)
    {
		EventSystem esys = EventSystem.current;
		GameObject curobj = esys.currentSelectedGameObject;
		if (curobj == null)
			return;

		double d;
		if (!double.TryParse(Str, out d))
			d = 0;

        int index = Array.FindIndex(_arrInputField, element => element.gameObject == curobj);
        if (index >= 0)
        {
            switch (index)
            {
                case 0:
                    _data.Longitude = d;
                    break;
                case 1:
                    _data.Latitude = d;
                    break;
                case 2:
                    _data.Altitude = d;
                    break;
				case 3:
					_data.Heading = (float)d;
					break;
				case 4:
					_data.Tilt = (float)d;
					break;
				case 5:
					_data.Roll = (float)d;
					break;
				case 6:
					_data.Scale.x = (float)d;
					break;
				case 7:
					_data.Scale.y = (float)d;
					break;
				case 8:
					_data.Scale.z = (float)d;
					break;
				case 9:
					_data.FbxName = Str;
					break;
            }
        }
    }
	public void OnEndEdit(string Str)
    {
		OnValueChanged(Str);
	}
	public void OnPressed()
    {		
		if(_onInputFieldClicked!=null)
        {
			EventSystem esys = EventSystem.current;
			GameObject curobj = esys.currentSelectedGameObject;
			if (curobj != null)
			{
				int index = Array.FindIndex(_arrInputField, element => element.gameObject == curobj);
				if (index >= 0)
				{
					_onInputFieldClicked(curobj.GetComponent<InputField>());
				}
			}			
        }
	}
    //private int _arrInputFieldIndex = 0;

    // Start is called before the first frame update
    //void Start()
    //{
    //    _arrInputField[_arrInputFieldIndex].Select();
    //}

    // Update is called once per frame
    //void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.Tab))
    //    {
    //        if(++_arrInputFieldIndex>=_arrInputField.Length)
    //        {
    //            _arrInputFieldIndex = 0;                
    //        }
    //        _arrInputField[_arrInputFieldIndex].Select();
    //        Debug.Log("kjs : Tab : " + _arrInputFieldIndex);
    //    }

    //}
	public void OnClickDelete()
    {
		if (_onClickDelete != null)
			_onClickDelete(gameObject);
    }
}
