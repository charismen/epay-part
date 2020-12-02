using System;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using LOGIC.Repo;
using Plexform.Base.Accounting.Map;

namespace Plexform.Base.Accounting.Payment.Repo
{
    #region SUNSystemConnection
    public class SUNSystemConnection
    {
        public string APIUrl;

        public SUNSystemConnection()
        {
            var appsettingsjson = JObject.Parse(System.IO.File.ReadAllText("appsettings.json"));
            var SUNSystem = (JObject)appsettingsjson["SUNSystem"];
            APIUrl = SUNSystem.Property("WebServiceURL").Value.ToString();
        }

        public async Task<Envelope> XMLSender(string urlFunctionPath, Envelope envelope, string functionName, string aliasName = "")
        {
            Envelope response = envelope;
            POSTXMLResponse xmlres = new POSTXMLResponse();
            string actionURL = string.Empty;
            string strRq = string.Empty;
            string strRs = string.Empty;

            try
            {
                actionURL = ConcatURL(APIUrl, urlFunctionPath);

                strRq = ToXML(envelope, functionName);
                if (string.IsNullOrEmpty(strRq))
                {
                    return response;
                }

                xmlres = postXMLData(APIUrl, actionURL, strRq);
                if (xmlres == null)
                {
                    return response;
                }

                strRs = xmlres.XMLResult;
                if (string.IsNullOrEmpty(strRs))
                {
                    return response;
                }

                response = LoadFromXMLString(strRs, envelope, xmlres.IsSuccess, aliasName);
            }
            catch (Exception ex)
            {
                string exMsg = ex.Message.ToString();
            }
            return response;
        }

        string ConcatURL(string strA, string strB)
        {
            if (strA.Substring(strA.Length - 1) != "/")
                strA += "/";

            return string.Concat(strA, strB);
        }

        public POSTXMLResponse postXMLData(string url, string destinationUrl, string requestXml)
        {
            POSTXMLResponse result = new POSTXMLResponse
            {
                IsSuccess = false,
                XMLResult = string.Empty
            };

            try
            {
                byte[] bytes;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Headers.Add("SOAPAction", destinationUrl);
                bytes = System.Text.Encoding.ASCII.GetBytes(requestXml);
                request.ContentType = "text/xml; encoding='utf-8'";
                request.ContentLength = bytes.Length;
                request.Method = "POST";
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                HttpWebResponse response;
                response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    string responseStr = new StreamReader(responseStream).ReadToEnd();
                    result.IsSuccess = true;
                    result.XMLResult = responseStr;

                    return result;
                }
                return result;
            }
            catch (WebException webEx)
            {
                result.IsSuccess = false;
                using (WebResponse response = webEx.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    {
                        result.XMLResult = new StreamReader(data).ReadToEnd();
                    }
                }
                return result;
            }
        }

        public string ToXMLAny<T>(T any)
        {
            var res = string.Empty;
            try
            {
                if (any != null)
                {
                    using (var stringwriter = new System.IO.StringWriter())
                    {
                        var serializer = new XmlSerializer(any.GetType());
                        serializer.Serialize(stringwriter, any);
                        res = stringwriter.ToString();
                    };
                }
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
            }
            return res;
        }

        public string ToXML(Envelope request, string functionName)
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                settings.NewLineHandling = NewLineHandling.None;
                settings.Indent = false;
                StringWriter StringWriter = new StringWriter();
                StringWriter.NewLine = "";
                XmlWriter writer = XmlWriter.Create(StringWriter, settings);
                XmlSerializer MySerializer = new XmlSerializer(typeof(Envelope));

                MySerializer.Serialize(writer, request);
                string strXML = StringWriter.ToString();

                if (functionName.ToUpper() == ("getAllOutStandingBillByCustomerCode").ToUpper())
                {
                    strXML = strXML.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://schemas.xmlsoap.org/soap/envelope/\"", "")
                        .Replace(" xmlns=\"urn:ifxforum-org:XSD:1\"", "")
                        .Replace("xmlns=\"http://soap.ebilling.lgm.com/xsd\"", "")
                        .Replace("<Body", "<soapenv:Body")
                        .Replace("</Body", "</soapenv:Body")
                        .Replace("</Envelope", "</soapenv:Envelope");

                    strXML = strXML.Replace("<Envelope", "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:soap=\"http://soap.ebilling.lgm.com\"")
                        .Replace("getAllOutStandingBillByCustomerCode", "soap:getAllOutStandingBillByCustomerCode")
                        .Replace("customerCode", "soap:customerCode")
                        .Replace("authenticationDTO", "soap:authenticationDTO")
                        .Replace("config", "soap:config")
                        .Replace("limit", "soap:limit")
                        .Replace("username", "soap:username")
                        .Replace("offset", "soap:offset");
                }
                else if (functionName.ToUpper() == ("getProformaBillById").ToUpper())
                {
                    strXML = strXML.Replace("<Envelope", @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:soap=""http://soap.ebilling.lgm.com"" xmlns:xsd=""http://soap.ebilling.lgm.com/xsd\""")
                        .Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://schemas.xmlsoap.org/soap/envelope/\"", "")
                        .Replace(" xmlns=\"urn:ifxforum-org:XSD:1\"", "")
                        .Replace("xmlns=\"http://soap.ebilling.lgm.com/xsd\"", "")
                        .Replace("<Body", "<soapenv:Body")
                        .Replace("</Body", "</soapenv:Body")
                        .Replace("</Envelope", "</soapenv:Envelope");

                    strXML = strXML.Replace("getProformaBillById", "soap:getProformaBillById")
                        .Replace("authenticationDTO", "soap:authenticationDTO")
                        .Replace("password", "xsd:password")
                        .Replace("username", "xsd:username")
                        .Replace("billId", "soap:billId");
                }
                else if (functionName.ToUpper() == ("getCreditBillById").ToUpper())
                {
                    strXML = strXML.Replace("<Envelope", @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:soap=""http://soap.ebilling.lgm.com"" xmlns:xsd=""http://soap.ebilling.lgm.com/xsd\""")
                        .Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://schemas.xmlsoap.org/soap/envelope/\"", "")
                        .Replace(" xmlns=\"urn:ifxforum-org:XSD:1\"", "")
                        .Replace("xmlns=\"http://soap.ebilling.lgm.com/xsd\"", "")
                        .Replace("<Body", "<soapenv:Body")
                        .Replace("</Body", "</soapenv:Body")
                        .Replace("</Envelope", "</soapenv:Envelope");

                    strXML = strXML.Replace("getCreditBillById", "soap:getCreditBillById")
                       .Replace("authenticationDTO", "soap:authenticationDTO")
                       .Replace("password", "xsd:password")
                       .Replace("username", "xsd:username")
                       .Replace("billId", "soap:billId");
                }
                else if (functionName.ToUpper() == ("updatePayment").ToUpper())
                {
                    strXML = strXML.Replace("<Envelope", @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:soap=""http://soap.ebilling.lgm.com"" xmlns:xsd=""http://soap.ebilling.lgm.com/xsd\""")
                        .Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://schemas.xmlsoap.org/soap/envelope/\"", "")
                        .Replace(" xmlns=\"urn:ifxforum-org:XSD:1\"", "")
                        .Replace("xmlns=\"http://soap.ebilling.lgm.com/xsd\"", "")
                        .Replace("<Body", "<soapenv:Body")
                        .Replace("</Body", "</soapenv:Body")
                        .Replace("</Envelope", "</soapenv:Envelope");

                    strXML = strXML.Replace("updatePayment", "soap:updatePayment")
                       .Replace("authenticationDTO", "soap:authenticationDTO")
                       .Replace("password", "xsd:password")
                       .Replace("username", "xsd:username")
                       .Replace("billId", "soap:billId")
                       .Replace("amount", "soap:amount")
                       .Replace("reference", "soap:reference")
                       .Replace("email", "soap:email")
                       .Replace("paymentDTO", "soap:paymentDTO");
                }
                return strXML;
            }
            catch (Exception ex)
            {
                string exMsg = ex.Message.ToString();
                return string.Empty;
            }
        }

        public string GenerateFormattedNo(string Prefix, string SpCode, string Postfix, int Runno, Int16 NoPOS, int NoLength, DateTime CurDate, string Branch = null, int TermID = -1)
        {
            string strFront;
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            string strBack;
            string strMid;
            string strCode;
            const string KeyBranch = "#B";
            const string KeyTerm = "#T";
            const string KeyYR = "#YR";
            const string KeyMT = "#MT";
            const string KeyDY = "#DY";
            try
            {
                strFront = Prefix.Trim();
                if ((strFront.Length > 0))
                {
                    if (((Branch == null)
                                == false))
                    {
                        if ((TermID != -1))
                        {
                            strFront = CheckingSysKey(strFront, KeyTerm, TermID.ToString());
                        }
                        else
                        {
                            strFront = CheckingSysKey(strFront, KeyTerm, String.Empty);
                        }

                        strFront = CheckingSysKey(strFront, KeyBranch, Branch);
                        strFront = CheckingSysKey(strFront, KeyYR, CurDate.Year.ToString().Substring(2));
                        strFront = CheckingSysKey(strFront, KeyMT, Format(Convert.ToInt64(CurDate.Month), "00"));
                        strFront = CheckingSysKey(strFront, KeyDY, Format(Convert.ToInt64(CurDate.Day), "00"));
                    }

                }

                strBack = Postfix.Trim();
                if ((strBack.Length > 0))
                {
                    if (((Branch == null)
                                == false))
                    {
                        if ((TermID != -1))
                        {
                            strBack = CheckingSysKey(strBack, KeyTerm, TermID.ToString());
                        }
                        else
                        {
                            strBack = CheckingSysKey(strBack, KeyTerm, String.Empty);
                        }

                        strBack = CheckingSysKey(strBack, KeyBranch, Branch);
                        strBack = CheckingSysKey(strBack, KeyYR, CurDate.Year.ToString().Substring(2));
                        strBack = CheckingSysKey(strBack, KeyMT, Format(Convert.ToInt64(CurDate.Month), "00"));
                        strBack = CheckingSysKey(strBack, KeyDY, Format(Convert.ToInt64(CurDate.Day), "00"));
                    }

                }

                SpCode = SpCode.Trim();
                if ((SpCode.Length > 0))
                {
                    if ((TermID != -1))
                    {
                        SpCode = CheckingSysKey(SpCode, KeyTerm, TermID.ToString());
                    }
                    else
                    {
                        SpCode = CheckingSysKey(SpCode, KeyTerm, String.Empty);
                    }

                    SpCode = CheckingSysKey(SpCode, KeyBranch, Branch);
                    SpCode = CheckingSysKey(SpCode, KeyYR, CurDate.Year.ToString().Substring(2));
                    SpCode = CheckingSysKey(SpCode, KeyMT, Format(Convert.ToInt64(CurDate.Month), "00"));
                    SpCode = CheckingSysKey(SpCode, KeyDY, Format(Convert.ToInt64(CurDate.Day), "00"));
                }

                if ((NoLength >= Runno.ToString().Length))
                {
                    strCode = Runno.ToString();
                    strCode = strCode.PadLeft(NoLength, Convert.ToChar("0"));
                }
                else
                {
                    return String.Empty;
                }

                if ((NoPOS == 1))
                {
                    strMid = string.Concat(strCode, SpCode);
                }
                else
                {
                    strMid = string.Concat(SpCode, strCode);
                }

                return string.Concat(strFront, strMid, strBack);
            }
            catch (Exception errGen)
            {
                return String.Empty;
            }
        }

        public Envelope LoadFromXMLString(string xmlText, Envelope request, bool isSuccess, string returnAlias = "")
        {
            Envelope result = request;

            try
            {
                xmlText = xmlText.Replace("<?xml version='1.0' encoding='UTF-8'?>", "");
                xmlText = xmlText.Replace("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\">", "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:urn=\"urn:ifxforum-org:XSD:1\">");

                if (isSuccess)
                {
                    if (!string.IsNullOrEmpty(returnAlias))
                    {
                        returnAlias = "return" + returnAlias + "Response";
                    }

                    #region getCreditBillById
                    xmlText = xmlText.Replace("<ns:getCreditBillByIdResponse xmlns:ns=\"http://soap.ebilling.lgm.com\">", "<urn:getCreditBillByIdResponse xmlns:urn=\"urn:ifxforum-org:XSD:1\" coreSchemaVersion=\"1.0\">");
                    xmlText = xmlText.Replace("<return xmlax21 =\"http://soap.ebilling.lgm.com/xsd\" xmlax23=\"http://shared.ebilling.lgm.com/xsd\" xmlxsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:type=\"urn:CreditBillDTOWrapper\">", "<urn:return>");
                    xmlText = xmlText.Replace("<ns:return xmlns:ax21=\"http://soap.ebilling.lgm.com/xsd\" xmlns:ax23=\"http://shared.ebilling.lgm.com/xsd\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:type=\"ax21:CreditBillDTOWrapper\">", "<urn:return>");
                    #endregion getCreditBillById

                    #region getProformaBillById
                    xmlText = xmlText.Replace("<ns:getProformaBillByIdResponse xmlns:ns=\"http://soap.ebilling.lgm.com\">", "<urn:getProformaBillByIdResponse xmlns:urn=\"urn:ifxforum-org:XSD:1\" coreSchemaVersion=\"1.0\">");
                    xmlText = xmlText.Replace("<return xmlax21 =\"http://soap.ebilling.lgm.com/xsd\" xmlax23=\"http://shared.ebilling.lgm.com/xsd\" xmlxsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:type=\"urn:ProformaBillDTOWrapper\">", "<urn:return>");
                    xmlText = xmlText.Replace("<ns:return xmlns:ax21=\"http://soap.ebilling.lgm.com/xsd\" xmlns:ax23=\"http://shared.ebilling.lgm.com/xsd\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:type=\"ax21:ProformaBillDTOWrapper\">", "<urn:return>");
                    #endregion getProformaBillById

                    xmlText = xmlText.Replace(" xsi:type=\"ax23:CreditVoteDTO\"", "");
                    xmlText = xmlText.Replace(" xsi:type=\"ax23:TaxCategoryDTO\"", "");
                    xmlText = xmlText.Replace(" xsi:type=\"ax23:ExchangeRateDTO\"", "");
                    xmlText = xmlText.Replace(" xsi:type=\"ax23:SunCustomerProformaDTO\"", "");

                    xmlText = xmlText.Replace(" xsi:type=\"ax23:ProformaCountDTO\"", "");
                    xmlText = xmlText.Replace(" xsi:type=\"ax23:DatabaseDTO\"", "");
                    xmlText = xmlText.Replace(" xsi:type=\"ax23:FileDTO\"", "");
                    xmlText = xmlText.Replace(" xsi:type=\"ax23:ItemDTO\"", "");
                    xmlText = xmlText.Replace(" xsi:type=\"ax23:OfficerCmbDTO\"", "");
                    xmlText = xmlText.Replace(" xsi:type=\"ax23:ProjectDTO\"", "");
                    xmlText = xmlText.Replace(" xsi:type=\"ax23:UnitCmbDTO\"", "");

                    #region getAllOutStandingBillByCustomerCode
                    xmlText = xmlText.Replace("<ns:getAllOutStandingBillByCustomerCodeResponse xmlns:ns=\"http://soap.ebilling.lgm.com\">", "<urn:getAllOutStandingBillByCustomerCodeResponse xmlns:urn=\"urn:ifxforum-org:XSD:1\" coreSchemaVersion=\"1.0\">");
                    xmlText = xmlText.Replace("<ns:return xmlns:ax21=\"http://soap.ebilling.lgm.com/xsd\" xmlns:ax23=\"http://shared.ebilling.lgm.com/xsd\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:type=\"ax21:PagingLoadResultSalesWrapper\">", "<urn:return>");
                    xmlText = xmlText.Replace(" xsi:type=\"ax21:SalesListModelWrapper\"", "");
                    xmlText = xmlText.Replace(" xsi:type=\"ax21:SalesListModelWrapper\"", "");
                    #endregion getAllOutStandingBillByCustomerCode

                    xmlText = xmlText.Replace("ax21:", "urn:");
                    xmlText = xmlText.Replace("ax22:", "urn:");
                    xmlText = xmlText.Replace("ax23:", "urn:");
                    xmlText = xmlText.Replace("ns:", "urn:");
                    xmlText = xmlText.Replace("xmlurn:", "xmlns:");
                    xmlText = xmlText.Replace(" xsi:nil=\"true\" ", "");

                    if (!string.IsNullOrEmpty(returnAlias))
                    {
                        xmlText = xmlText.Replace("return", returnAlias);
                    }

                    xmlText = xmlText.Replace(" xsi:type=\"urn:SunCustomerDTO\"", "");
                }
                else
                {
                    xmlText = xmlText.Replace("<soapenv:Fault>", "<urn:Fault xmlns:urn=\"urn:ifxforum-org:XSD:1\" coreSchemaVersion=\"1.0\">");
                    xmlText = xmlText.Replace("</soapenv:Fault>", "</urn:Fault>");
                    xmlText = xmlText.Replace("faultcode", "urn:faultcode");
                    xmlText = xmlText.Replace("faultstring", "urn:faultstring");
                    xmlText = xmlText.Replace("detail", "urn:detail");
                }

                XmlSerializer serializer = new XmlSerializer(typeof(Envelope));
                using (StringReader reader = new StringReader(xmlText))
                {
                    result = (Envelope)(serializer.Deserialize(reader));
                }
            }
            catch (Exception ex)
            {
                string exMsg = ex.Message.ToString();
            }
            return result;
        }

        public string Format(Int64 a, string b)
        {
            return string.Concat(a.ToString(), b);
        }

        public string POSTWebRequest(string url, string posting_data, byte[] _byteVersion)
        {
            string result = string.Empty;

            try
            {
                SslProtocols _Tls12 = (SslProtocols)0x00000C00;
                SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
                ServicePointManager.SecurityProtocol = Tls12;

                System.Net.ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;
                //System.Net.ServicePointManager.ServerCertificateValidationCallback = (object se, System.Security.Cryptography.X509Certificates.X509Certificate cert, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslerror) => true;

                System.Net.HttpWebRequest objRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                //objRequest.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequired;
                objRequest.Method = "POST";
                //objRequest.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
                objRequest.ContentType = "application/x-www-form-urlencoded";
                //objRequest.Accept = "application/json";
                objRequest.AllowAutoRedirect = true;
                objRequest.KeepAlive = false;
                objRequest.Timeout = 300000;
                objRequest.ContentLength = _byteVersion.Length;

                Stream stream = objRequest.GetRequestStream();
                stream.Write(_byteVersion, 0, _byteVersion.Length);
                stream.Close();

                using (System.Net.HttpWebResponse objResponse = (System.Net.HttpWebResponse)objRequest.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(objResponse.GetResponseStream()))
                    {
                        result = (reader.ReadToEnd()).Trim();
                    }
                }
            }
            catch (System.Net.WebException webEx)
            {
                result = "ERROR " + webEx.Message.ToString();
            }
            catch (Exception ex)
            {
                result = "ERROR " + ex.Message.ToString();
            }
            return result;
        }

        public string CheckingSysKey(string _values, string _keys, string _keyReplaces)
        {
            string result = string.Empty;
            if ((_values.IndexOf(_keys) >= 0))
            {
                result = _values.Replace(_keys, _keyReplaces);
            }
            else
            {
                result = _values;
            }
            return result;
        }
    }
    #endregion SUNSystemConnection

    #region RSASign
    public class RSASign
    {
        static string ErrorCode = "XX";

        public static string nvl_VerifiMsg(string plainText, string encryptedstring, string PhysicalApplicationPath)
        {
            string[] list = new string[2];
            list[0] = (PhysicalApplicationPath + "fpxuat_current.cer");      //Old Certificate 
            list[1] = (PhysicalApplicationPath + "fpxuat.cer");              //New Certificate       
            string returnMsg = "Error";
            try
            {
                ArrayList certs = getCerts(list);
                RSACryptoServiceProvider rsaEncryptor;
                Boolean checkCert = false;
                byte[] plainData = System.Text.Encoding.Default.GetBytes(plainText);
                byte[] signatureData = HexToBytes(encryptedstring);
                Console.WriteLine("certs.Count : " + certs.Count);
                if (certs.Count == 1)
                {
                    rsaEncryptor = (RSACryptoServiceProvider)(((X509Certificate2)certs[0]).PublicKey.Key);
                    checkCert = rsaEncryptor.VerifyData(plainData, "SHA1", signatureData);
                }
                else if (certs.Count == 2) //Checks either any Cert should be valid on same date of expiration 
                {
                    rsaEncryptor = (RSACryptoServiceProvider)(((X509Certificate2)certs[0]).PublicKey.Key);
                    checkCert = rsaEncryptor.VerifyData(plainData, "SHA1", signatureData);
                    if (!checkCert)
                    {
                        rsaEncryptor = (RSACryptoServiceProvider)(((X509Certificate2)certs[1]).PublicKey.Key);
                        checkCert = rsaEncryptor.VerifyData(plainData, "SHA1", signatureData);
                    }
                }
                else
                {
                    returnMsg = "Invalid Certificates. " + "Code : [" + ErrorCode + "]";  //No Certificate (or) All Certificate are Expired 
                    return returnMsg;
                }

                if (checkCert)
                {
                    ErrorCode = "00";
                    returnMsg = "[" + ErrorCode + "]" + " Your signature has been verified successfully. ";
                }
                else
                {
                    ErrorCode = "09";
                    returnMsg = "[" + ErrorCode + "]" + " Your Data cannot be verified against the Signature. ";
                }
            }
            catch (Exception e)
            {
                ErrorCode = "03";
                returnMsg = "[" + ErrorCode + "] ERROR :: " + e.Message;
            }
            return returnMsg;
        }

        public static ArrayList getCerts(string[] list)
        {
            int cert_exists = 0;
            ArrayList Certs = new ArrayList();
            X509Certificate2 x509_2;
            for (int i = 0; i < 2; i++)
            {
                Console.WriteLine("LOOP [" + i + "] : [" + list[i] + "]");
                if (!File.Exists(list[i]))
                {
                    cert_exists++;
                    continue;
                }
                x509_2 = new X509Certificate2(list[i]);
                String[] date = x509_2.GetExpirationDateString().Split(' ');
                DateTime CertDate = DateTime.ParseExact(date[0], "M/dd/yyyy", null);
                CertDate = CertDate.AddDays(-1);
                Console.WriteLine("\t" + CertDate.Date + " - " + DateTime.Today.Date);

                if (CertDate.Date > DateTime.Today.Date)
                {
                    Console.WriteLine("Passed " + i + " : " + list[i]);
                    if (i > 0)  //Roll Over the FPX.cer  Current Certificate to FPX_CURRENT.cer (IF FPX_CURRENT.cer ALREADY EXISTS AND EXPIRED WILL BE RENAMED TO FPX_CURRENT_<CURRENT DATE>.cer )
                    {
                        if (Certrollover(list[i], list[i - 1]))
                            Console.WriteLine("Roll Over Completed at Level 1");
                    }
                    Certs.Add((X509Certificate2)x509_2);
                    return Certs;
                }
                else if (CertDate.Date == DateTime.Today.Date)
                {

                    if (i > 0 && (!File.Exists(list[i - 1])))  //Roll Over the FPX.cer  Current Certificate to FPX_CURRENT.cer (IF FPX_CURRENT.cer ALREADY EXISTS AND EXPIRED WILL BE RENAMED TO FPX_CURRENT_<CURRENT DATE>.cer )
                    {
                        if (Certrollover(list[i], list[i - 1]))
                            Console.WriteLine("Roll Over Completed at Level 2");
                        Certs.Add((X509Certificate2)x509_2);
                        return Certs;
                    }


                    i++;
                    if (i < 2)
                    {
                        if (!File.Exists(list[i]))
                        {
                            Console.WriteLine("Failed to read Second Certificate  " + list[i]);
                            Certs.Add((X509Certificate2)x509_2);
                            return Certs;
                        }
                        Certs.Add(new X509Certificate2(list[i]));
                    }

                    Certs.Add((X509Certificate2)x509_2);


                    return Certs;
                }
            }
            if (cert_exists == 2)
                ErrorCode = "06";
            else if (Certs.Count == 0 && cert_exists == 1)
                ErrorCode = "07";
            else if (Certs.Count == 0 && cert_exists == 0)
                ErrorCode = "08";
            return Certs;
        }

        public static bool Certrollover(string old_cert, string new_cert)
        {
            Console.WriteLine("Roll Over the Current Certificate old_cert[" + old_cert + "]   new_cert[" + new_cert + "]");
            if (File.Exists(new_cert))
            {
                String current_time_stamp = "_Old_" + (DateTime.Now).ToString("yyyyMMddHHmmssffff");
                Console.WriteLine("File.Exists : " + new_cert);
                System.IO.File.Move(new_cert, new_cert + current_time_stamp);              //FPX_CURRENT.cer to FPX_CURRENT.cer_<CURRENT TIMESTAMP>
                Console.WriteLine("Moved  " + new_cert + " to " + new_cert + current_time_stamp);
            }

            if ((!File.Exists(new_cert)) && File.Exists(old_cert))
            {
                System.IO.File.Move(old_cert, new_cert);                                    //FPX.cer to FPX_CURRENT.cer
                Console.WriteLine("Moved  " + old_cert + " to " + new_cert);
            }

            return true;
        }

        public static string RSASignValue(string data, string PhysicalApplicationPath)
        {
            RSACryptoServiceProvider rsaCsp = LoadCertificateFile(PhysicalApplicationPath);
            byte[] dataBytes = System.Text.Encoding.Default.GetBytes(data);
            byte[] signatureBytes = rsaCsp.SignData(dataBytes, "SHA1");
            return BitConverter.ToString(signatureBytes).Replace("-", null);
        }

        public static RSACryptoServiceProvider LoadCertificateFile(string filename)
        {
            using (System.IO.FileStream fs = System.IO.File.OpenRead(filename))
            {
                byte[] data = new byte[fs.Length];
                byte[] res = null;
                fs.Read(data, 0, data.Length);
                if (data[0] != 0x30)
                {
                    res = GetPem("RSA PRIVATE KEY", data);
                }
                try
                {
                    RSACryptoServiceProvider rsa = DecodeRSAPrivateKey(res);
                    return rsa;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ex :" + ex);
                }
                return null;
            }
        }

        public static string GetSignature(string data)
        {
            byte[] dataBytes = System.Text.Encoding.Default.GetBytes(data);
            byte[] result;
            SHA512Managed shaM = new SHA512Managed();
            result = shaM.ComputeHash(dataBytes);

            if (result != null)
                return BitConverter.ToString(result).Replace("-", "");
            else
                return "";
        }

        public static byte[] GetPem(string type, byte[] data)
        {
            string pem = Encoding.UTF8.GetString(data);
            string header = String.Format("-----BEGIN {0}-----\\n", type);
            string footer = String.Format("-----END {0}-----", type);
            int start = pem.IndexOf(header) + header.Length;
            int end = pem.IndexOf(footer, start);
            string base64 = pem.Substring(start, (end - start));
            return Convert.FromBase64String(base64);
        }

        public static byte[] HexToBytes(string hex)
        {
            hex = hex.Trim();
            byte[] bytes = new byte[hex.Length / 2];
            for (int index = 0; index < bytes.Length; index++)
            {
                bytes[index] = byte.Parse(hex.Substring(index * 2, 2), NumberStyles.HexNumber);
            }
            return bytes;
        }

        public static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            // --------- Set up stream to decode the asn.1 encoded RSA private key ------
            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem);  //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();    //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102) //version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;


                //------ all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);

                Console.WriteLine("showing components ..");
                if (verbose)
                {
                    showBytes("\nModulus", MODULUS);
                    showBytes("\nExponent", E);
                    showBytes("\nD", D);
                    showBytes("\nP", P);
                    showBytes("\nQ", Q);
                    showBytes("\nDP", DP);
                    showBytes("\nDQ", DQ);
                    showBytes("\nIQ", IQ);
                }

                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                CspParameters CspParameters = new CspParameters();
                CspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(1024, CspParameters);
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                binr.Close();
            }
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)     //expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();    // data size in next byte
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte(); // data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;     // we already have the data size
            }

            while (binr.ReadByte() == 0x00)
            {   //remove high order zeros in data
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);       //last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }

        private static bool verbose = false;

        private static void showBytes(String info, byte[] data)
        {
            Console.WriteLine("{0} [{1} bytes]", info, data.Length);
            for (int i = 1; i <= data.Length; i++)
            {
                Console.Write("{0:X2} ", data[i - 1]);
                if (i % 16 == 0)
                    Console.WriteLine();
            }
            Console.WriteLine("\n\n");
        }
    }
    #endregion RSASign

    #region PAYMENTHDR
    public class PAYMENTHDR : BaseRepository<DTO.Payment.PAYMENTHDR>
    {
        public PAYMENTHDR(IWebHostEnvironment env, string connectionString) : base(env, connectionString) { }
    }
    #endregion

    #region PAYMENTDTL
    public class PAYMENTDTL : BaseRepository<DTO.Payment.PAYMENTDTL>
    {
        public PAYMENTDTL(IWebHostEnvironment env, string connectionString) : base(env, connectionString) { }
    }
    #endregion

    #region PAYMENTTENDER
    public class PAYMENTTENDER : BaseRepository<DTO.Payment.PAYMENTTENDER>
    {
        public PAYMENTTENDER(IWebHostEnvironment env, string connectionString) : base(env, connectionString) { }
    }
    #endregion

    #region PAYMENTLOG
    public class PAYMENTLOG : BaseRepository<DTO.Payment.PAYMENTLOG>
    {
        public PAYMENTLOG(IWebHostEnvironment env, string connectionString) : base(env, connectionString) { }
    }
    #endregion

    #region TENDERTYPE
    public class TENDERTYPE : BaseRepository<DTO.Payment.TENDERTYPE>
    {
        public TENDERTYPE(IWebHostEnvironment env, string connectionString) : base(env, connectionString) { }
    }
    #endregion
}
