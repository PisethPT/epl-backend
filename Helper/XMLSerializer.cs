using System.Xml.Serialization;

namespace epl_backend.Helper
{
	public static class XMLSerializer
	{
		public static string GetXmlSerializer<T>(List<T> model, List<T> list)
		{
			string xml;
			try
            {
				list = new List<T>();
				list.AddRange(model);
				List<List<T>> tf = new List<List<T>>();
				tf.Add(list);
				using (StringWriter sw = new StringWriter())
				{
					XmlSerializer xs = new XmlSerializer(typeof(List<List<T>>));
					xs.Serialize(sw, tf);
					xml = sw.ToString();
				}
			}
            catch (System.Exception ex)
            {

				throw new System.Exception(ex.Message);
            }
		
			return xml;
		}
	}
}
