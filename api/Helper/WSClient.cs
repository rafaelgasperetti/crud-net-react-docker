using api.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static api.Helper.Helper;

namespace api.Helper
{
    public class WSClient
    {
        public enum Metodo
        {
            GET,
            POST
        }

        private static Resposta ObterResposta(WebRequest request, Encoding encoding)
        {
            var response = (HttpWebResponse)request.GetResponse();
            string resp = null;

            if (response.Headers["Content-Encoding"] == "gzip")
            {
                using (GZipStream strResp = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (MemoryStream strReader = new MemoryStream())
                    {
                        strResp.CopyTo(strReader);
                        resp = encoding.GetString(strReader.ToArray());
                    }
                }
            }
            else
            {
                using (StreamReader strResp = new StreamReader(response.GetResponseStream()))
                {
                    resp = strResp.ReadToEnd();
                }
            }

            return new Resposta()
            {
                Codigo = (int)HttpStatusCode.OK,
                CodigoInterno = (int)CodigoInterno.Sucesso,
                Data = resp,
                Status = "OK"
            };
        }

        public static Resposta WSCall(string link, Metodo method, string contentType, string body, string authorization, Encoding encoding = null)
        {
            try
            {
                if (encoding == null)
                {
                    encoding = new UTF8Encoding();
                }

                var req = WebRequest.Create(link);
                req.Method = Enum.GetName(typeof(Metodo), method);

                if (!string.IsNullOrEmpty(contentType))
                {
                    req.ContentType = contentType;
                }

                if (!string.IsNullOrEmpty(authorization))
                {
                    req.Headers.Add("Authorization", authorization);
                }

                req.Headers.Add("Accept-Encoding", "gzip, deflate, br");

                req.Timeout = 30000;

                if (method == Metodo.POST && !string.IsNullOrEmpty(body))
                {
                    using (Stream strReq = req.GetRequestStream())
                    {
                        byte[] bodyStr = encoding.GetBytes(body);
                        strReq.Write(bodyStr, 0, bodyStr.Length);

                        return ObterResposta(req, encoding);
                    }
                }
                else
                {
                    return ObterResposta(req, encoding);
                }
            }
            catch (WebException w)
            {
                if (w.Response != null && w.Response is HttpWebResponse resp)
                {
                    string respStr = null;

                    using (StreamReader streamResp = new StreamReader(w.Response.GetResponseStream()))
                    {
                        respStr = streamResp.ReadToEnd();
                    }

                    return new Resposta()
                    {
                        Codigo = (int)HttpStatusCode.InternalServerError,
                        CodigoInterno = (int)CodigoInterno.ErroWebClient,
                        Data = (respStr ?? "") + Environment.NewLine + w.Message,
                        Status = "Não foi possível executar a solicitação (WebException OK). Link: " + (link ?? "null")
                    };
                }
                else
                {
                    return new Resposta()
                    {
                        Codigo = (int)HttpStatusCode.InternalServerError,
                        CodigoInterno = (int)CodigoInterno.ErroWebClientNoResponse,
                        Data = w,
                        Status = "Não foi possível executar a solicitação (WebException NOK). Link: " + (link ?? "null")
                    };
                }
            }
            catch (Exception e)
            {
                return new Resposta()
                {
                    Codigo = (int)HttpStatusCode.InternalServerError,
                    CodigoInterno = (int)CodigoInterno.ErroWebCall,
                    Data = e,
                    Status = "Não foi possível executar a solicitação (Exception). Link: " + (link ?? "null")
                };
            }
        }
    }
}
