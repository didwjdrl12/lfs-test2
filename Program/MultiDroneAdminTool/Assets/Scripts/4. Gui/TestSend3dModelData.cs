using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text;

public class TestSend3dModelData : MonoBehaviour
{
    public TMP_InputField _ID;
    public TMP_InputField[] _CPLongitude = new TMP_InputField[4];
    public TMP_InputField[] _CPLatitude  = new TMP_InputField[4];

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickSendTestData()
    {
        int id = 0;
        if(!Int32.TryParse(_ID.text, out id))
        {
            return;
        }

        byte[] buffer = new byte[1024*512]; // 512Kbyte.
        int dstOffset = 0;
        // id.
        Buffer.BlockCopy(BitConverter.GetBytes(id), 0, buffer, dstOffset, sizeof(int));                         dstOffset += sizeof(int);
        int stringLen = 0;
        byte[] stringBytes = null;

        // control points.
        int i = 0;
        for (; i < 4; ++i)
        {
            // 경도.
            stringBytes = Encoding.UTF8.GetBytes(_CPLongitude[i].text);
            stringLen = stringBytes.Length;
            Buffer.BlockCopy(BitConverter.GetBytes(stringLen), 0, buffer, dstOffset, sizeof(int));                 dstOffset += sizeof(int);
            Buffer.BlockCopy(stringBytes, 0, buffer, dstOffset, stringLen);                                      dstOffset += stringLen;

            // 위도.
            stringBytes = Encoding.UTF8.GetBytes(_CPLatitude[i].text);
            stringLen = stringBytes.Length;
            Buffer.BlockCopy(BitConverter.GetBytes(stringLen), 0, buffer, dstOffset, sizeof(int));                 dstOffset += sizeof(int);
            Buffer.BlockCopy(stringBytes, 0, buffer, dstOffset, stringLen);                                      dstOffset += stringLen;
        }
        //test.
        //int offset = 0;
        //string str;
        //id = BitConverter.ToInt32(buffer, offset); offset += sizeof(int);
        //for(i=0; i<4; ++i)
        //{
        //    // 경도.
        //    stringLen = BitConverter.ToInt32(buffer, offset);          offset += sizeof(int);
        //    str = Encoding.UTF8.GetString(buffer, offset, stringLen);   offset += stringLen;
        //    // 위도.
        //    stringLen = BitConverter.ToInt32(buffer, offset);          offset += sizeof(int);
        //    str = Encoding.UTF8.GetString(buffer, offset, stringLen);   offset += stringLen;
        //}

        J2y.MultiDrone.MdaDataCenter.NetConnector.SendMessage((int)J2y.MultiDrone.ePacketProtocol.SC_MODEL_DATA_BEZIER, buffer, dstOffset);
    }
}
