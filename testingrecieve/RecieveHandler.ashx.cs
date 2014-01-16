using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Vanso.SXMP;

//using Vanso.SXMP;

namespace testingrecieve
{
    /// <summary>
    /// Summary description for RecieveHandler
    /// </summary>
    public class RecieveHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
            //XmlConfigurator.Configure();
            if (context.Request.RequestType.Equals("POST", StringComparison.CurrentCultureIgnoreCase))
            {
                DeliverResponse rsp;
                using (var s = new StreamReader(System.Web.Request.InputStream))
                {
                    var xml = s.ReadToEnd();
                    try
                    {
                        var request = (DeliverRequest) new SXMPParser().Parse(xml);
                        var messageText = request.Text;
                        var senderNumber = request.SourceAddress.Address;
                        //Parsing successful, return response
                        rsp = new DeliverResponse((int) SXMPErrorCode.OK, "OK");
                    }
                    catch (SXMPParseException sx)
                    {
                        rsp = new DeliverResponse((int) SXMPErrorCode.INVALID_XML, sx.Message);
                        Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    }

                }

                Response.ContentType = "text/xml";
                Response.Output.WriteLine(SXMPWriter.Write(rsp));
            }
            else
            {

                Response.StatusCode = (int) HttpStatusCode.MethodNotAllowed;
                Response.Output.WriteLine("Only HTTP POST Method allowed");
                Response.Output.Flush();

            }
        
}

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}