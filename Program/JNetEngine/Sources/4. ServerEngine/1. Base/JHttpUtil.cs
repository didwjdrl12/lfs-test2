using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Security.Cryptography;
using System.Net;

#if NET_SERVER
public class JHttpUtil
{
	#region getMD5Str
	public static string getMD5Str(string str)
	{
		StringBuilder sb = new StringBuilder();
		foreach (byte b in System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str)))
		{
			sb.Append(b.ToString("X2"));
		}
		return sb.ToString().ToLower();
	}

	private static String[] hexDigits = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };

	public static String byteArrayToHexString(byte[] b)
	{
		StringBuilder resultSb = new StringBuilder();
		for (int i = 0; i < b.Length; i++)
		{
			resultSb.Append(byteToHexString(b[i]));
		}
		return resultSb.ToString();
	}

	private static String byteToHexString(byte b)
	{
		int n = b;
		if (n < 0)
		{
			n = 256 + n;
		}
		int d1 = n / 16;
		int d2 = n % 16;
		return hexDigits[d1] + hexDigits[d2];
	}

	public static String MD5Encode(String origin)
	{
		String resultString = null;
		try
		{
			resultString = origin;

			resultString = byteArrayToHexString(System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(resultString)));
		}
		catch (Exception)
		{
		}
		return resultString;
	}

	#endregion

	#region json 디코더
	public static T decodeJson<T>(string json)
	{
		try
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
		}
		catch (Exception)
		{
			return default(T);
		}
	}
	#endregion
    
	#region json 인코더
	public static string encodeJson<T>(T obj)
	{
		try
		{
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
		}
		catch (Exception e)
		{
			throw e;
		}
	}
	#endregion

	#region doPost
	//------------------------------------------------------------------------------------------------------------------------------------------------------
	public static string doPost(string url, string body)
	{
		string encodingName = "utf-8";
		string str = "";
		if ((url == null) || (url == ""))
		{
			return null;
		}
		WebRequest request = WebRequest.Create(url);
		request.Method = "POST";
		request.ContentType = "application/x-www-form-urlencoded";

		byte[] bytes = null;
		if (body == null)
		{
			request.ContentLength = 0L;
		}
		else
		{
			try
			{
				bytes = Encoding.Default.GetBytes(body);
				request.ContentLength = bytes.Length;
				Stream requestStream = request.GetRequestStream();
				requestStream.Write(bytes, 0, bytes.Length);
				requestStream.Close();
			}
			catch (Exception e)
			{
				return e.Message;
			}
		}

		try
		{
			Stream responseStream = request.GetResponse().GetResponseStream();
			byte[] buffer = new byte[0x200];
			for (int j = responseStream.Read(buffer, 0, 0x200); j > 0; j = responseStream.Read(buffer, 0, 0x200))
			{
				Encoding encoding = Encoding.GetEncoding(encodingName);
				str = str + encoding.GetString(buffer, 0, j);
			}
			return str;
		}
		catch (Exception exception2)
		{
			return exception2.Message;
		}
	}
	#endregion

	#region doGet
	//------------------------------------------------------------------------------------------------------------------------------------------------------
	public static string doGet(string url)
	{
		string encodingName = "utf-8";
		string str = "";
		if ((url == null) || (url == ""))
		{
			return null;
		}
		WebRequest request = WebRequest.Create(url);
		request.Method = "GET";
		request.ContentType = "application/x-www-form-urlencoded";
		//request.ContentType = "application/json; charset=UTF-8";
		//request.Accept = "application/json";

		try
		{
			WebResponse response = request.GetResponse();
			//Console.WriteLine(((HttpWebResponse)response).StatusDescription);
			// Get the stream containing content returned by the server.
			Stream responseStream = response.GetResponseStream();

			byte[] buffer = new byte[0x200];
			for (int j = responseStream.Read(buffer, 0, 0x200); j > 0; j = responseStream.Read(buffer, 0, 0x200))
			{
				Encoding encoding = Encoding.GetEncoding(encodingName);
				str = str + encoding.GetString(buffer, 0, j);
			}
			return str;
		}
		catch (Exception exception2)
		{
			return exception2.Message;
		}
	}

	#endregion


	#region doGetAsync
	//------------------------------------------------------------------------------------------------------------------------------------------------------
	public static void doGetAsync(string url, Action<int, string> fun_complete)
	{
		string encodingName = "utf-8";
		if (string.IsNullOrEmpty(url))
		{
			fun_complete(-1, "");
			return;
		}

		var request = (HttpWebRequest)WebRequest.Create(url);
		request.Method = "GET";
		request.ContentType = "application/x-www-form-urlencoded";
		//request.ContentType = "application/json; charset=UTF-8";
		//request.Accept = "application/json";

		try
		{
			DoWithResponse(request, (response) =>
			{
				try
				{
					var encoding = Encoding.GetEncoding(encodingName);

					var body = new StreamReader(response.GetResponseStream(), encoding).ReadToEnd();
					fun_complete(1, body);
				}				
				catch (Exception e)
				{
					fun_complete(-3, e.Message);
				}
			});
		}
		catch (Exception e)
		{
			fun_complete(-2, e.Message);
		}
	}

	private static void DoWithResponse(HttpWebRequest request, Action<HttpWebResponse> responseAction)
	{
		request.BeginGetResponse(new AsyncCallback((iar) =>
		{
			var response = (HttpWebResponse)((HttpWebRequest)iar.AsyncState).EndGetResponse(iar);
			responseAction(response);
		}), request);


		//Action wrapperAction = () =>
		//{
		//	request.BeginGetResponse(new AsyncCallback((iar) =>
		//	{
		//		var response = (HttpWebResponse)((HttpWebRequest)iar.AsyncState).EndGetResponse(iar);
		//		responseAction(response);
		//	}), request);
		//};
		//wrapperAction.BeginInvoke(new AsyncCallback((iar) =>
		//{
		//	var action = (Action)iar.AsyncState;
		//	action.EndInvoke(iar);
		//}), wrapperAction);
	}

	#endregion




	#region 스트림 디코딩

	public static string doEncoding(Stream stream)
	{
		try
		{
			string str = string.Empty;
			byte[] buffer = new byte[0x200];
			for (int j = stream.Read(buffer, 0, 0x200); j > 0; j = stream.Read(buffer, 0, 0x200))
			{
				Encoding encoding = Encoding.GetEncoding("UTF-8");
				str = str + encoding.GetString(buffer, 0, j);
			}
			return str;
		}
		catch (Exception exception2)
		{
			return exception2.Message;
		}
	}

	#endregion
}

#endif
