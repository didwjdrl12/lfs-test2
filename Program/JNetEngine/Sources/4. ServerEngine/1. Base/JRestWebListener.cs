using System;
using System.Collections.Generic;
using J2y;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using J2y.Network;
using System.Net;
using System.Linq;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JRestWebListener
    //
    //  
    //  @Example
    //      var web_listener = JObject.CreateObject<JRestWebListener>();
    //      web_listener.StartHttpListener(8080);
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JRestWebListener : JObject
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // class
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [NestedClass] Mapping
        public class Mapping : Attribute
        {
            public string Map;
            public Mapping(string s)
            {
                Map = s;
            }
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
        #region [Variable] HttpListener
        protected HttpListener _http_listener;
        #endregion
        


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 0. Base Methods
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
        #region [HttpListener] Start
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void StartHttpListener(int port)
        {
            _http_listener = new HttpListener();
            _http_listener.Prefixes.Add(string.Format("http://localhost:{0}/", port));
            _http_listener.Start();

            Task.Run(() =>
            {
                while (true)
                {
                    HttpListenerContext ctx = _http_listener.GetContext();
                    Task.Run(() =>
                    {
                        string methodName = ctx.Request.Url.Segments[1].Replace("/", "");
                        string[] strParams = ctx.Request.Url
                                                .Segments
                                                .Skip(2)
                                                .Select(s => s.Replace("/", ""))
                                                .ToArray();

                        //Debug.Log(ctx.Request.Url);
                        //foreach (var s in ctx.Request.Url.Segments)
                        //    Debug.Log(s + " ");

                        var method = this.GetType()
                                            .GetMethods()
                                            .Where(mi => mi.GetCustomAttributes(true).Any(attr => attr is Mapping && ((Mapping)attr).Map == methodName))
                                            .FirstOrDefault();
                        if (null == method)
                            return;

                        object[] @params = method.GetParameters()
                                            .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                                            .ToArray();

                        object ret = method.Invoke(this, @params);
                        //string retstr = JsonConvert.SerializeObject(ret);

                        string data = "ok";
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(data);
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentEncoding = System.Text.Encoding.UTF8;
                        ctx.Response.ContentLength64 = buffer.Length;
                        //ctx.Response.Headers.Add("Content-Type", contentType);
                        ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);
                        ctx.Response.OutputStream.Close();
                    });
                }
            });
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // RestAPI Event Handler
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
        #region [RestAPI] Example
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [Mapping("RestExample")]
        public void GetRestExampleHandler(string id)
        {
            JScheduler.AddMainThreadCommand(() =>
            {
                Console.WriteLine("RestExample:" + id);
            });
        }
        #endregion

    }
}

