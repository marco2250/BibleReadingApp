// Taken from http://stackoverflow.com/questions/1124597/why-isnt-there-an-xml-serializable-dictionary-in-net/5941122#5941122

using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using System.Text;

namespace BibleReading.Common45.Root.Xml.Serialization
{
    public static class SerializationExtensions
    {
        public static string Serialize<T>(this T obj)
        {
            var serializer = new DataContractSerializer(obj.GetType());
            using (var writer = new StringWriter())
            using (var stm = new XmlTextWriter(writer))
            {
                serializer.WriteObject(stm, obj);
                return writer.ToString();
            }
        }

        public static T Deserialize<T>(this string serialized)
        {
            T ret;

            var serializer = new DataContractSerializer(typeof(T));
            using (var reader = new StringReader(serialized))
            {
                using (var stm = new XmlTextReader(reader))
                {
                    ret = (T)serializer.ReadObject(stm);
                }

                reader.Close();
            }

            return ret;
        }

        public static T DeserializeFromFile<T>(string filePath)
        {
            string dicionarySerialized;
            using (var sr = new StreamReader(filePath, Encoding.UTF8))
            {
                dicionarySerialized = sr.ReadToEnd();

                sr.Close();
            }

            return dicionarySerialized.Deserialize<T>();
        }
    }
}
