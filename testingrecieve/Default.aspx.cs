using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanso.SXMP;

namespace testingrecieve
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.RequestType.Equals("POST", StringComparison.CurrentCultureIgnoreCase))
            {
                DeliverResponse rsp;
                using (var s = new StreamReader(Request.InputStream))
                {
                    var xml = s.ReadToEnd();
                    try
                    {
                        var request = (DeliverRequest)new SXMPParser().Parse(xml);
                        var messageText = request.Text;
                        var senderNumber = request.SourceAddress.Address;
                        //Parsing successful, return response
                        rsp = new DeliverResponse((int)SXMPErrorCode.OK, "OK");
                    }
                    catch (Exception sx)//catch (SXMPParseException sx)
                    {
                        rsp = new DeliverResponse((int)SXMPErrorCode.INVALID_XML, sx.Message);
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }

                }

                Response.ContentType = "text/xml";
                Response.Output.WriteLine(SXMPWriter.Write(rsp));
            }
            else
            {

                Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                Response.Output.WriteLine("Only HTTP POST Method allowed");
                Response.Output.Flush();

            }

        }
    }
}
