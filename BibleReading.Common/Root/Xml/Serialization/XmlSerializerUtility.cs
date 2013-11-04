using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using BibleReading.Common45.Properties;
using System.Diagnostics;

namespace BibleReading.Common45.Root.Xml.Serialization
{
    public class XmlSerializerUtility
    {
        public static string Serialize(object obj)
        {
            return Serialize(obj, null);
        }

        public static string Serialize(object obj, string rootElement)
        {
            XmlSerializer srReq;

            if (string.IsNullOrEmpty(rootElement))
            {
                srReq = new XmlSerializer(obj.GetType());
            }
            else
            {
                var root = new XmlRootAttribute(rootElement);
                srReq = new XmlSerializer(obj.GetType(), root);
            }

            var wr = new StringWriterWithEncoding(Encoding.UTF8);
            srReq.Serialize(wr, obj);
            string retorno = wr.ToString();
            wr.Close();

            return retorno;
        }

        public static string SerializeToFile(object obj)
        {
            return SerializeToFile(obj, null);
        }

        public static string SerializeToFile(object obj, string rootElement)
        {
            return SerializeToFile(obj, rootElement, Settings.Default.SerializationPath + obj + "_" + DateTime.Now.Ticks.ToString() + ".xml");
        }

        public static string SerializeToFile(object obj, string rootElement, string filePath)
        {
            string serializedObject = string.Empty;

            try
            {
                //if (Settings.Default.SerializationEnabled)
                //{
                    serializedObject = Serialize(obj, rootElement);
                    var sw = new StreamWriter(filePath);
                    sw.Write(serializedObject);
                    sw.Flush();
                    sw.Close();
                //}
            }
            catch(Exception ex)
            {
                string exceptionStr = ex.ToString();

                Debug.WriteLine(exceptionStr);
            }

            return serializedObject;
        }

        public static object Read(string str, Type componente)
        {
            return Read(str, componente, null);
        }

        public static object Read(string str, Type componente, string rootElement)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            XmlSerializer serializer;

            if (!string.IsNullOrEmpty(rootElement))
            {
                var root = new XmlRootAttribute(rootElement);
                serializer = new XmlSerializer(componente, root);
            }
            else
            {
                serializer = new XmlSerializer(componente);
            }

            var sr = new StreamReader(WriteStringToStream(str), Encoding.UTF8);
            XmlReader reader = new XmlTextReader(sr);

            return serializer.Deserialize(reader);

        }

        public static Stream WriteStringToStream(string valor)
        {
            byte[] byt = Encoding.UTF8.GetBytes(valor);
            var resposta = new MemoryStream();
            resposta.Write(byt, 0, byt.Length);

            if (resposta.CanSeek)
                resposta.Position = 0;

            return resposta;
        }

        public static object ReadFromFile(string filePath, Type type)
        {
            var sr = new StreamReader(filePath, Encoding.UTF8);

            var retorno = Read(sr.ReadToEnd(), type);
            //object retorno = srReq.Deserialize(sr);
            sr.Close();
            sr.Dispose();

            return retorno;
        }

        /// <summary>
        /// http://devproj20.blogspot.com/2008/02/writing-xml-with-utf-8-encoding-using.html
        /// </summary>
        public class StringWriterWithEncoding : StringWriter
        {
            readonly Encoding _encoding;

            public StringWriterWithEncoding(Encoding encoding)
            {
                _encoding = encoding;
            }

            public override Encoding Encoding
            {
                get { return _encoding; }
            }
        }
    }
}
