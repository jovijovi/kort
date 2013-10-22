using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using log4net.Appender;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using KORT.Data;
using KORT.Network;
using KORT.Helper;
using KORT.Util;
using KORT.Language;

namespace KORT.Server
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single)]
    public partial class KORTService : IService
    {
        private static Dictionary<Functions, Method> MethodDictionary
            = new Dictionary<Functions, Method>();
        private static SessionHolder<Session> _session = new SessionHolder<Session>();
        private static log4net.ILog _tracer;
        static KORTService()
        {
            //setup logger
            var repository = log4net.LogManager.CreateRepository("KORTServiceRepository");

            var appender = new FileAppender(new log4net.Layout.PatternLayout("%r %d [%t] %c %-5p - %m%n"),
                                            System.IO.Path.Combine(
                                                AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                "Log/Server_" + DateTime.Now.ToShortDateString() + ".log"),
                                            true);

            appender.Encoding = Encoding.UTF8;
            //Off\Fatal\CommonError\Warn\Info\Debug\All
            appender.Threshold = log4net.Core.Level.Info;
            
            log4net.Config.BasicConfigurator.Configure(repository, appender);

            repository.Configured = true;

            _tracer = log4net.LogManager.GetLogger("KORTServiceRepository", "KORTService");

            BuildMIME();
            RegistMethod();
        }
       
        static void RegistMethod(Method method)
        {
            _tracer.Debug(method.MethodName);
            MethodDictionary.Add(method.MethodName, method);
        }

        private static void AddSuccessInfo(ref JObject result, string resultType, object resultObject, string detail)
        {
            result.Add(FieldKeyword.Success, true);
            if (!string.IsNullOrEmpty(resultType)) result.Add(FieldKeyword.ResultType, resultType);
            if (resultObject != null) result.Add(FieldKeyword.Result, JObject.FromObject(resultObject));
            if (!string.IsNullOrEmpty(detail)) result.Add(FieldKeyword.ErrorDetail, detail);
        }

        private static void AddSuccessInfo(ref JObject result, string resultType, bool resultObject, string detail)
        {
            result.Add(FieldKeyword.Success, true);
            if (!string.IsNullOrEmpty(resultType)) result.Add(FieldKeyword.ResultType, resultType);
            result.Add(FieldKeyword.Result, resultObject);
            if (!string.IsNullOrEmpty(detail)) result.Add(FieldKeyword.ErrorDetail, detail);
        }

        private static void AddFailInfo(ref JObject result, string commonError, string errorDetail)
        {
            result.Add(FieldKeyword.Success, false);
            result.Add(FieldKeyword.ResultType, ResultType.Null);
            if (!string.IsNullOrEmpty(commonError)) result.Add(FieldKeyword.CommonError, commonError);
            if (!string.IsNullOrEmpty(errorDetail)) result.Add(FieldKeyword.ErrorDetail, errorDetail);
        }

        private static void AddBadParameterInfo(ref JObject result, Functions function, string language)
        {
            result.Add(FieldKeyword.Success, false);
            result.Add(FieldKeyword.ResultType, ResultType.Null);
            result.Add(FieldKeyword.CommonError, ErrorNumber.SeeDetail.ToString());
            result.Add(FieldKeyword.ErrorDetail, MessageHelper.GetMessage(ErrorNumber.BadParameter, language,
                                                     new[] { FunctionKeyword.GetFunctionsName(function, language) }));
        }

        private Session currentSession;
        public Stream Method(string funcName, Stream stream)
        {
            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            if (_tracer.IsInfoEnabled) _tracer.InfoFormat("Call function {0}.", funcName);
            var result = new JObject();

            WebOperationContext context = WebOperationContext.Current;
            #region check context
            if (context==null)
            {
                if(_tracer.IsErrorEnabled)_tracer.Error(ErrorNumber.CommonBadContext.ToString());
                AddFailInfo(ref result, ErrorNumber.CommonBadContext.ToString(), null);
                return new MemoryStream(KORT.Util.Tools.GZipCompress(Encoding.UTF8.GetBytes(result.ToString())));
            }
            else
            {
                context.OutgoingResponse.ContentType = "application/json";
            }
            #endregion

            if (!DBHelper.IsWorking())
            {
                AddFailInfo(ref result, ErrorNumber.ServerInMaintenance.ToString(), null);
                return new MemoryStream(KORT.Util.Tools.GZipCompress(Encoding.UTF8.GetBytes(result.ToString())));
            }

            Functions func;
            
            #region check function
            try
            {
                 func = (Functions)Enum.Parse(typeof(Functions), funcName);
            }
            catch (Exception)
            {
                if (_tracer.IsErrorEnabled) _tracer.Error(ErrorNumber.CommonFunctionNotExist.ToString());
                AddFailInfo(ref result, ErrorNumber.CommonFunctionNotExist.ToString(), null);
                return new MemoryStream(KORT.Util.Tools.GZipCompress(Encoding.UTF8.GetBytes(result.ToString())));
            }
            
            if(!MethodDictionary.ContainsKey(func))
            {
                if (_tracer.IsErrorEnabled) _tracer.Error(ErrorNumber.CommonFunctionNotExist.ToString());
                AddFailInfo(ref result, ErrorNumber.CommonFunctionNotExist.ToString(), null);
                return new MemoryStream(KORT.Util.Tools.GZipCompress(Encoding.UTF8.GetBytes(result.ToString())));
            }
            #endregion

            JObject request;
            #region parse request
            try
            {
                using (var decompressStream = new MemoryStream())
                {
                    KORT.Util.Tools.GZipDecompress(stream, decompressStream);
                    decompressStream.Position = 0;
                    StreamReader reader = new StreamReader(decompressStream, Encoding.UTF8);
                    string text = reader.ReadToEnd();
                    request = JObject.Parse(text);
                }
            }
            catch (Exception e)
            {
                if (_tracer.IsErrorEnabled) _tracer.Error(ErrorNumber.CommonBadRequest.ToString(), e);
                AddFailInfo(ref result, ErrorNumber.CommonBadRequest.ToString(), funcName);
                return new MemoryStream(KORT.Util.Tools.GZipCompress(Encoding.UTF8.GetBytes(result.ToString())));
            }
            #endregion
            if (_tracer.IsDebugEnabled) _tracer.Debug(request.ToString(Formatting.Indented));

            string token = request[FieldKeyword.Token] != null ? request[FieldKeyword.Token].ToString() : null;
            #region check token
            if (func != Functions.Login && (string.IsNullOrEmpty(token) || !_session.Hit(token)))
            {
                //no token or expires token
                if (_tracer.IsInfoEnabled) _tracer.Info(ErrorNumber.CommonExpireToken.ToString());
                AddFailInfo(ref result, ErrorNumber.CommonExpireToken.ToString(), null);
                return new MemoryStream(KORT.Util.Tools.GZipCompress(Encoding.UTF8.GetBytes(result.ToString())));
            }
            #endregion

            #region call functions
            currentSession = _session.GetSession(token);

            if (func != Functions.Login
                && (currentSession == null || !FunctionKeyword.CheckFunction(currentSession.UserType, func)))
            {
                if (_tracer.IsInfoEnabled) _tracer.Info(ErrorNumber.CommonWrongAuthority.ToString());
                AddFailInfo(ref result, ErrorNumber.CommonWrongAuthority.ToString(), funcName);
            }
            else if (MethodDictionary[func].CheckRequest != null
                && !MethodDictionary[func].CheckRequest(request))
            {
                if (_tracer.IsInfoEnabled) _tracer.Info(ErrorNumber.CommonBadParameter.ToString());
                AddFailInfo(ref result, ErrorNumber.CommonBadParameter.ToString(), funcName);
            }
            else
            {
                try
                {
                    var currentLanguage = request[FieldKeyword.Language] != null ? request[FieldKeyword.Language].ToString().ToLower() : null;
                    if (currentSession == null && !MessageHelper.IsSupport(currentLanguage)) currentLanguage = SupportLanguage.Default;
                    if (currentSession != null)
                    {
                        if (MessageHelper.IsSupport(currentLanguage)) currentSession.Language = currentLanguage;
                        else currentLanguage = currentSession.Language;
                    }
                    System.Diagnostics.Debug.WriteLine("use language:" + currentLanguage);
                    MethodDictionary[func].Logic(request, ref result, currentLanguage, ref currentSession);
                }
                catch (Exception e)
                {
                    if (_tracer.IsErrorEnabled) _tracer.Error(ErrorNumber.Other.ToString(), e);
                    AddFailInfo(ref result, ErrorNumber.Other.ToString(), e.Message);
                }
            }
            #endregion

            if(currentSession!=null)
                try
                {
                    timer.Stop();
                    LogHelper.SaveLog(currentSession.UserName, func.ToString(), request.ToString(), result.ToString(), timer.ElapsedMilliseconds);
                }
                catch (Exception e)
                {
                    if (_tracer.IsErrorEnabled) _tracer.Error(ErrorNumber.Other.ToString(), e);
                    AddFailInfo(ref result, ErrorNumber.Other.ToString(), e.Message);
                }

            if (_tracer.IsInfoEnabled) _tracer.InfoFormat("End function {0}.", funcName);
            return new MemoryStream(KORT.Util.Tools.GZipCompress(Encoding.UTF8.GetBytes(result.ToString())));
        }

        public Stream GetFile(string filePath)
        {
            System.Diagnostics.Debug.WriteLine("Request file:" + filePath);
            //for test right now
            WebOperationContext context = WebOperationContext.Current;
            var path = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Web"), filePath);
            System.Diagnostics.Debug.WriteLine(path);
            if (context==null) return null;
            if(File.Exists(path))
            {
                var fi = new FileInfo(path);
                if (MIME.ContainsKey(fi.Extension)) context.OutgoingResponse.ContentType = MIME[fi.Extension];
                else context.OutgoingResponse.ContentType = "application/octet-stream";
                try
                {
                    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    context.OutgoingResponse.ContentLength = fs.Length;
                    return fs;

                }
                catch (Exception e)
                {
                    context.OutgoingResponse.StatusCode = HttpStatusCode.ServiceUnavailable;
                    context.OutgoingResponse.ContentType = "application/plain";
                    return new MemoryStream(Encoding.UTF8.GetBytes(e.ToString()));
                }
                
            }
            context.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
            context.OutgoingResponse.ContentType = "application/plain";
            return new MemoryStream(Encoding.UTF8.GetBytes("Not Found"));
        }
        private static Dictionary<string,string> MIME = new Dictionary<string, string>();
        private static void BuildMIME()
        {
            try
            {
                MIME.Add(".001", "application/x-001");
                MIME.Add(".301", "application/x-301");
                MIME.Add(".323", "text/h323");
                MIME.Add(".906", "application/x-906");
                MIME.Add(".907", "drawing/907");
                MIME.Add(".a11", "application/x-a11");
                MIME.Add(".acp", "audio/x-mei-aac");
                MIME.Add(".ai", "application/postscript");
                MIME.Add(".aif", "audio/aiff");
                MIME.Add(".aifc", "audio/aiff");
                MIME.Add(".aiff", "audio/aiff");
                MIME.Add(".anv", "application/x-anv");
                MIME.Add(".asa", "text/asa");
                MIME.Add(".asf", "video/x-ms-asf");
                MIME.Add(".asp", "text/asp");
                MIME.Add(".asx", "video/x-ms-asf");
                MIME.Add(".au", "audio/basic");
                MIME.Add(".avi", "video/avi");
                MIME.Add(".awf", "application/vnd.adobe.workflow");
                MIME.Add(".biz", "text/xml");
                MIME.Add(".bmp", "application/x-bmp");
                MIME.Add(".bot", "application/x-bot");
                MIME.Add(".c4t", "application/x-c4t");
                MIME.Add(".c90", "application/x-c90");
                MIME.Add(".cal", "application/x-cals");
                MIME.Add(".cat", "application/vnd.ms-pki.seccat");
                MIME.Add(".cdf", "application/x-netcdf");
                MIME.Add(".cdr", "application/x-cdr");
                MIME.Add(".cel", "application/x-cel");
                MIME.Add(".cer", "application/x-x509-ca-cert");
                MIME.Add(".cg4", "application/x-g4");
                MIME.Add(".cgm", "application/x-cgm");
                MIME.Add(".cit", "application/x-cit");
                MIME.Add(".class", "java/*");
                MIME.Add(".cml", "text/xml");
                MIME.Add(".cmp", "application/x-cmp");
                MIME.Add(".cmx", "application/x-cmx");
                MIME.Add(".cot", "application/x-cot");
                MIME.Add(".crl", "application/pkix-crl");
                MIME.Add(".crt", "application/x-x509-ca-cert");
                MIME.Add(".csi", "application/x-csi");
                MIME.Add(".css", "text/css");
                MIME.Add(".cut", "application/x-cut");
                MIME.Add(".dbf", "application/x-dbf");
                MIME.Add(".dbm", "application/x-dbm");
                MIME.Add(".dbx", "application/x-dbx");
                MIME.Add(".dcd", "text/xml");
                MIME.Add(".dcx", "application/x-dcx");
                MIME.Add(".der", "application/x-x509-ca-cert");
                MIME.Add(".dgn", "application/x-dgn");
                MIME.Add(".dib", "application/x-dib");
                MIME.Add(".dll", "application/x-msdownload");
                MIME.Add(".doc", "application/msword");
                MIME.Add(".dot", "application/msword");
                MIME.Add(".drw", "application/x-drw");
                MIME.Add(".dtd", "text/xml");
                MIME.Add(".dwf", "Model/vnd.dwf");
                //MIME.Add(".dwf", "application/x-dwf");
                MIME.Add(".dwg", "application/x-dwg");
                MIME.Add(".dxb", "application/x-dxb");
                MIME.Add(".dxf", "application/x-dxf");
                MIME.Add(".edn", "application/vnd.adobe.edn");
                MIME.Add(".emf", "application/x-emf");
                MIME.Add(".eml", "message/rfc822");
                MIME.Add(".ent", "text/xml");
                MIME.Add(".epi", "application/x-epi");
                //MIME.Add(".eps", "application/x-ps");
                MIME.Add(".eps", "application/postscript");
                MIME.Add(".etd", "application/x-ebx");
                MIME.Add(".exe", "application/x-msdownload");
                MIME.Add(".fax", "image/fax");
                MIME.Add(".fdf", "application/vnd.fdf");
                MIME.Add(".fif", "application/fractals");
                MIME.Add(".fo", "text/xml");
                MIME.Add(".frm", "application/x-frm");
                MIME.Add(".g4", "application/x-g4");
                MIME.Add(".gbr", "application/x-gbr");
                MIME.Add(".gcd", "application/x-gcd");
                MIME.Add(".gif", "image/gif");
                MIME.Add(".gl2", "application/x-gl2");
                MIME.Add(".gp4", "application/x-gp4");
                MIME.Add(".hgl", "application/x-hgl");
                MIME.Add(".hmr", "application/x-hmr");
                MIME.Add(".hpg", "application/x-hpgl");
                MIME.Add(".hpl", "application/x-hpl");
                MIME.Add(".hqx", "application/mac-binhex40");
                MIME.Add(".hrf", "application/x-hrf");
                MIME.Add(".hta", "application/hta");
                MIME.Add(".htc", "text/x-component");
                MIME.Add(".htm", "text/html");
                MIME.Add(".html", "text/html");
                MIME.Add(".htt", "text/webviewhtml");
                MIME.Add(".htx", "text/html");
                MIME.Add(".icb", "application/x-icb");
                MIME.Add(".ico", "image/x-icon");
                //MIME.Add(".ico", "application/x-ico");
                MIME.Add(".iff", "application/x-iff");
                MIME.Add(".ig4", "application/x-g4");
                MIME.Add(".igs", "application/x-igs");
                MIME.Add(".iii", "application/x-iphone");
                MIME.Add(".img", "application/x-img");
                MIME.Add(".ins", "application/x-internet-signup");
                MIME.Add(".isp", "application/x-internet-signup");
                MIME.Add(".IVF", "video/x-ivf");
                MIME.Add(".java", "java/*");
                MIME.Add(".jfif", "image/jpeg");
                MIME.Add(".jpe", "image/jpeg");
                //MIME.Add(".jpe", "application/x-jpe");
                MIME.Add(".jpeg", "image/jpeg");
                MIME.Add(".jpg", "image/jpeg");
                //MIME.Add(".jpg", "application/x-jpg");
                MIME.Add(".js", "application/x-javascript");
                MIME.Add(".jsp", "text/html");
                MIME.Add(".la1", "audio/x-liquid-file");
                MIME.Add(".lar", "application/x-laplayer-reg");
                MIME.Add(".latex", "application/x-latex");
                MIME.Add(".lavs", "audio/x-liquid-secure");
                MIME.Add(".lbm", "application/x-lbm");
                MIME.Add(".lmsff", "audio/x-la-lms");
                MIME.Add(".ls", "application/x-javascript");
                MIME.Add(".ltr", "application/x-ltr");
                MIME.Add(".m1v", "video/x-mpeg");
                MIME.Add(".m2v", "video/x-mpeg");
                MIME.Add(".m3u", "audio/mpegurl");
                MIME.Add(".m4e", "video/mpeg4");
                MIME.Add(".mac", "application/x-mac");
                MIME.Add(".man", "application/x-troff-man");
                MIME.Add(".math", "text/xml");
                //MIME.Add(".mdb", "application/msaccess");
                MIME.Add(".mdb", "application/x-mdb");
                MIME.Add(".mfp", "application/x-shockwave-flash");
                MIME.Add(".mht", "message/rfc822");
                MIME.Add(".mhtml", "message/rfc822");
                MIME.Add(".mi", "application/x-mi");
                MIME.Add(".mid", "audio/mid");
                MIME.Add(".midi", "audio/mid");
                MIME.Add(".mil", "application/x-mil");
                MIME.Add(".mml", "text/xml");
                MIME.Add(".mnd", "audio/x-musicnet-download");
                MIME.Add(".mns", "audio/x-musicnet-stream");
                MIME.Add(".mocha", "application/x-javascript");
                MIME.Add(".movie", "video/x-sgi-movie");
                MIME.Add(".mp1", "audio/mp1");
                MIME.Add(".mp2", "audio/mp2");
                MIME.Add(".mp2v", "video/mpeg");
                MIME.Add(".mp3", "audio/mp3");
                MIME.Add(".mp4", "video/mpeg4");
                MIME.Add(".mpa", "video/x-mpg");
                MIME.Add(".mpd", "application/vnd.ms-project");
                MIME.Add(".mpe", "video/x-mpeg");
                MIME.Add(".mpeg", "video/mpg");
                MIME.Add(".mpg", "video/mpg");
                MIME.Add(".mpga", "audio/rn-mpeg");
                MIME.Add(".mpp", "application/vnd.ms-project");
                MIME.Add(".mps", "video/x-mpeg");
                MIME.Add(".mpt", "application/vnd.ms-project");
                MIME.Add(".mpv", "video/mpg");
                MIME.Add(".mpv2", "video/mpeg");
                MIME.Add(".mpw", "application/vnd.ms-project");
                MIME.Add(".mpx", "application/vnd.ms-project");
                MIME.Add(".mtx", "text/xml");
                MIME.Add(".mxp", "application/x-mmxp");
                MIME.Add(".net", "image/pnetvue");
                MIME.Add(".nrf", "application/x-nrf");
                MIME.Add(".nws", "message/rfc822");
                MIME.Add(".odc", "text/x-ms-odc");
                MIME.Add(".out", "application/x-out");
                MIME.Add(".p10", "application/pkcs10");
                MIME.Add(".p12", "application/x-pkcs12");
                MIME.Add(".p7b", "application/x-pkcs7-certificates");
                MIME.Add(".p7c", "application/pkcs7-mime");
                MIME.Add(".p7m", "application/pkcs7-mime");
                MIME.Add(".p7r", "application/x-pkcs7-certreqresp");
                MIME.Add(".p7s", "application/pkcs7-signature");
                MIME.Add(".pc5", "application/x-pc5");
                MIME.Add(".pci", "application/x-pci");
                MIME.Add(".pcl", "application/x-pcl");
                MIME.Add(".pcx", "application/x-pcx");
                MIME.Add(".pdf", "application/pdf");
                MIME.Add(".pdx", "application/vnd.adobe.pdx");
                MIME.Add(".pfx", "application/x-pkcs12");
                MIME.Add(".pgl", "application/x-pgl");
                MIME.Add(".pic", "application/x-pic");
                MIME.Add(".pko", "application/vnd.ms-pki.pko");
                MIME.Add(".pl", "application/x-perl");
                MIME.Add(".plg", "text/html");
                MIME.Add(".pls", "audio/scpls");
                MIME.Add(".plt", "application/x-plt");
                MIME.Add(".png", "image/png");
                //MIME.Add(".png", "application/x-png");
                MIME.Add(".pot", "application/vnd.ms-powerpoint");
                MIME.Add(".ppa", "application/vnd.ms-powerpoint");
                MIME.Add(".ppm", "application/x-ppm");
                MIME.Add(".pps", "application/vnd.ms-powerpoint");
                MIME.Add(".ppt", "application/vnd.ms-powerpoint");
                //MIME.Add(".ppt", "application/x-ppt");
                MIME.Add(".pr", "application/x-pr");
                MIME.Add(".prf", "application/pics-rules");
                MIME.Add(".prn", "application/x-prn");
                MIME.Add(".prt", "application/x-prt");
                //MIME.Add(".ps", "application/x-ps");
                MIME.Add(".ps", "application/postscript");
                MIME.Add(".ptn", "application/x-ptn");
                MIME.Add(".pwz", "application/vnd.ms-powerpoint");
                MIME.Add(".r3t", "text/vnd.rn-realtext3d");
                MIME.Add(".ra", "audio/vnd.rn-realaudio");
                MIME.Add(".ram", "audio/x-pn-realaudio");
                MIME.Add(".ras", "application/x-ras");
                MIME.Add(".rat", "application/rat-file");
                MIME.Add(".rdf", "text/xml");
                MIME.Add(".rec", "application/vnd.rn-recording");
                MIME.Add(".red", "application/x-red");
                MIME.Add(".rgb", "application/x-rgb");
                MIME.Add(".rjs", "application/vnd.rn-realsystem-rjs");
                MIME.Add(".rjt", "application/vnd.rn-realsystem-rjt");
                MIME.Add(".rlc", "application/x-rlc");
                MIME.Add(".rle", "application/x-rle");
                MIME.Add(".rm", "application/vnd.rn-realmedia");
                MIME.Add(".rmf", "application/vnd.adobe.rmf");
                MIME.Add(".rmi", "audio/mid");
                MIME.Add(".rmj", "application/vnd.rn-realsystem-rmj");
                MIME.Add(".rmm", "audio/x-pn-realaudio");
                MIME.Add(".rmp", "application/vnd.rn-rn_music_package");
                MIME.Add(".rms", "application/vnd.rn-realmedia-secure");
                MIME.Add(".rmvb", "application/vnd.rn-realmedia-vbr");
                MIME.Add(".rmx", "application/vnd.rn-realsystem-rmx");
                MIME.Add(".rnx", "application/vnd.rn-realplayer");
                MIME.Add(".rp", "image/vnd.rn-realpix");
                MIME.Add(".rpm", "audio/x-pn-realaudio-plugin");
                MIME.Add(".rsml", "application/vnd.rn-rsml");
                MIME.Add(".rt", "text/vnd.rn-realtext");
                MIME.Add(".rtf", "application/msword");
                //MIME.Add(".rtf", "application/x-rtf");
                MIME.Add(".rv", "video/vnd.rn-realvideo");
                MIME.Add(".sam", "application/x-sam");
                MIME.Add(".sat", "application/x-sat");
                MIME.Add(".sdp", "application/sdp");
                MIME.Add(".sdw", "application/x-sdw");
                MIME.Add(".sit", "application/x-stuffit");
                MIME.Add(".slb", "application/x-slb");
                MIME.Add(".sld", "application/x-sld");
                MIME.Add(".slk", "drawing/x-slk");
                MIME.Add(".smi", "application/smil");
                MIME.Add(".smil", "application/smil");
                MIME.Add(".smk", "application/x-smk");
                MIME.Add(".snd", "audio/basic");
                MIME.Add(".sol", "text/plain");
                MIME.Add(".sor", "text/plain");
                MIME.Add(".spc", "application/x-pkcs7-certificates");
                MIME.Add(".spl", "application/futuresplash");
                MIME.Add(".spp", "text/xml");
                MIME.Add(".ssm", "application/streamingmedia");
                MIME.Add(".sst", "application/vnd.ms-pki.certstore");
                MIME.Add(".stl", "application/vnd.ms-pki.stl");
                MIME.Add(".stm", "text/html");
                MIME.Add(".sty", "application/x-sty");
                MIME.Add(".svg", "text/xml");
                MIME.Add(".swf", "application/x-shockwave-flash");
                MIME.Add(".tdf", "application/x-tdf");
                MIME.Add(".tg4", "application/x-tg4");
                MIME.Add(".tga", "application/x-tga");
                MIME.Add(".tif", "image/tiff");
                //MIME.Add(".tif", "application/x-tif");
                MIME.Add(".tiff", "image/tiff");
                MIME.Add(".tld", "text/xml");
                MIME.Add(".top", "drawing/x-top");
                MIME.Add(".torrent", "application/x-bittorrent");
                MIME.Add(".tsd", "text/xml");
                MIME.Add(".txt", "text/plain");
                MIME.Add(".uin", "application/x-icq");
                MIME.Add(".uls", "text/iuls");
                MIME.Add(".vcf", "text/x-vcard");
                MIME.Add(".vda", "application/x-vda");
                MIME.Add(".vdx", "application/vnd.visio");
                MIME.Add(".vml", "text/xml");
                MIME.Add(".vpg", "application/x-vpeg005");
                MIME.Add(".vsd", "application/vnd.visio");
                //MIME.Add(".vsd", "application/x-vsd");
                MIME.Add(".vss", "application/vnd.visio");
                MIME.Add(".vst", "application/vnd.visio");
                //MIME.Add(".vst", "application/x-vst");
                MIME.Add(".vsw", "application/vnd.visio");
                MIME.Add(".vsx", "application/vnd.visio");
                MIME.Add(".vtx", "application/vnd.visio");
                MIME.Add(".vxml", "text/xml");
                MIME.Add(".wav", "audio/wav");
                MIME.Add(".wax", "audio/x-ms-wax");
                MIME.Add(".wb1", "application/x-wb1");
                MIME.Add(".wb2", "application/x-wb2");
                MIME.Add(".wb3", "application/x-wb3");
                MIME.Add(".wbmp", "image/vnd.wap.wbmp");
                MIME.Add(".wiz", "application/msword");
                MIME.Add(".wk3", "application/x-wk3");
                MIME.Add(".wk4", "application/x-wk4");
                MIME.Add(".wkq", "application/x-wkq");
                MIME.Add(".wks", "application/x-wks");
                MIME.Add(".wm", "video/x-ms-wm");
                MIME.Add(".wma", "audio/x-ms-wma");
                MIME.Add(".wmd", "application/x-ms-wmd");
                MIME.Add(".wmf", "application/x-wmf");
                MIME.Add(".wml", "text/vnd.wap.wml");
                MIME.Add(".wmv", "video/x-ms-wmv");
                MIME.Add(".wmx", "video/x-ms-wmx");
                MIME.Add(".wmz", "application/x-ms-wmz");
                MIME.Add(".wp6", "application/x-wp6");
                MIME.Add(".wpd", "application/x-wpd");
                MIME.Add(".wpg", "application/x-wpg");
                MIME.Add(".wpl", "application/vnd.ms-wpl");
                MIME.Add(".wq1", "application/x-wq1");
                MIME.Add(".wr1", "application/x-wr1");
                MIME.Add(".wri", "application/x-wri");
                MIME.Add(".wrk", "application/x-wrk");
                MIME.Add(".ws", "application/x-ws");
                MIME.Add(".ws2", "application/x-ws");
                MIME.Add(".wsc", "text/scriptlet");
                MIME.Add(".wsdl", "text/xml");
                MIME.Add(".wvx", "video/x-ms-wvx");
                MIME.Add(".xdp", "application/vnd.adobe.xdp");
                MIME.Add(".xdr", "text/xml");
                MIME.Add(".xfd", "application/vnd.adobe.xfd");
                MIME.Add(".xfdf", "application/vnd.adobe.xfdf");
                MIME.Add(".xhtml", "text/html");
                MIME.Add(".xls", "application/vnd.ms-excel");
                //MIME.Add(".xls", "application/x-xls");
                MIME.Add(".xlw", "application/x-xlw");
                MIME.Add(".xml", "text/xml");
                MIME.Add(".xpl", "audio/scpls");
                MIME.Add(".xq", "text/xml");
                MIME.Add(".xql", "text/xml");
                MIME.Add(".xquery", "text/xml");
                MIME.Add(".xsd", "text/xml");
                MIME.Add(".xsl", "text/xml");
                MIME.Add(".xslt", "text/xml");
                MIME.Add(".xwd", "application/x-xwd");
                MIME.Add(".x_b", "application/x-x_b");
                MIME.Add(".x_t", "application/x-x_t");
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }
    }
}