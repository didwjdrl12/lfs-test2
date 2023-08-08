using System;
using System.Collections.Generic;
using J2y;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using J2y.Network;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JRestApi
    //
    //
    //  @Example
    //      [Serializable]
    //      public class repo { public string name; }
    //      
    //      JRestWebApi.Call<List<repo>>(_test_url, (repos) =>
    //      {
    //          foreach (var r in repos)
    //              Console.WriteLine(r.name);
    //      });
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JRestWebApi
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] HttpClient
        public static HttpClient _http_client;
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // RestApi
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [RestApi] Call
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static async void Call<T>(string url, Action<T> fun_complete = null) where T : class
        {
            var result = await GetAsync<T>(url);
            JScheduler.AddMainThreadCommand(() =>
            {
                fun_complete(result);
            });
        }
        #endregion




        #region [RestApi] Call
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static async Task<T> GetAsync<T>(string url) where T : class
        {
            if (null == _http_client)
            {
                _http_client = new HttpClient();
                _http_client.DefaultRequestHeaders.Accept.Clear();
                //_http_client.DefaultRequestHeaders.Accept.Add(
                //    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                _http_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _http_client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            }

            //var serializer = new DataContractJsonSerializer(typeof(T));
            //var streamTask = _http_client.GetStreamAsync(url);
            //var repositories = serializer.ReadObject(await streamTask) as T;
            //return repositories;
            await Task.Delay(0);
            return null;
        }
        #endregion
    }


}

