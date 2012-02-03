using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Svt.Caspar
{
	public class CGManager
	{
		Channel channel_ = null;
		internal CGManager(Channel channel)
		{
			channel_ = channel;
		}

		internal Channel Channel
		{
			get { return channel_; }
		}

		[Obsolete("use another overload", false)]
		public void Add(CasparCGItem item)
		{
			Add(item, false);
		}
		[Obsolete("use another overload", false)]
		public void Add(int layer, CasparCGItem item)
		{
			item.Layer = layer;
			Add(item, false);
		}
		[Obsolete("use another overload", false)]
		public void Add(int layer, CasparCGItem item, bool bPlayOnLoad)
		{
			item.Layer = layer;
			Add(item, bPlayOnLoad);
		}
		[Obsolete("use another overload", false)]
		public void Add(CasparCGItem item, bool bPlayOnLoad)
		{
			bool bAutoPlay = item.AutoPlay || bPlayOnLoad;
			Channel.Device.Server.SendString("CG " + Channel.ID + " ADD " + item.Layer + " \"" + item.TemplateIdentifier + "\" " + (bAutoPlay ? "1" : "0") + " \"" + item.XMLData + "\"");
		}

		public void Add(int layer, string template)
		{
			Add(layer, template, false, string.Empty);
		}
		public void Add(int layer, string template, bool bPlayOnLoad)
		{
			Add(layer, template, bPlayOnLoad, string.Empty);
		}
		public void Add(int layer, string template, string data)
		{
			Add(layer, template, false, data);
		}
		public void Add(int layer, string template, bool bPlayOnLoad, string data)
		{
			Channel.Device.Server.SendString("CG " + Channel.ID + " ADD " + layer + " \"" + template + "\" " + (bPlayOnLoad ? "1" : "0") + " \"" + (!string.IsNullOrEmpty(data) ? data: string.Empty) + "\"");
		}

		public void Add(int layer, string template, ICGDataContainer data)
		{
			Add(layer, template, false, data);
		}
		public void Add(int layer, string template, bool bPlayOnLoad, ICGDataContainer data)
		{
			Channel.Device.Server.SendString("CG " + Channel.ID + " ADD " + layer + " \"" + template + "\" " + (bPlayOnLoad ? "1" : "0") + " \"" + ((data!=null) ? data.ToAMCPEscapedXml() : string.Empty) + "\"");
		}

		public void Remove(int layer)
		{
			Channel.Device.Server.SendString("CG " + Channel.ID + " REMOVE " + layer);
		}
		
		public void Clear()
		{
			Channel.Device.Server.SendString("CG " + Channel.ID + " CLEAR");
		}

		public void Play(int layer)
		{
			Channel.Device.Server.SendString("CG " + Channel.ID + " PLAY " + layer);
		}

		public void Stop(int layer)
		{
			Channel.Device.Server.SendString("CG " + Channel.ID + " STOP " + layer);
		}

		public void Next(int layer)
		{
			Channel.Device.Server.SendString("CG " + Channel.ID + " NEXT " + layer);
		}

		[Obsolete("this command is no longer supported", true)]
		public void Prev(int layer)
		{
			Channel.Device.Server.SendString("CG " + Channel.ID + " PREV " + layer);
		}

		[Obsolete("this command is no longer supported", true)]
		public void Goto(int layer, string label)
		{
			Channel.Device.Server.SendString("CG " + Channel.ID + " GOTO " + layer + " " + label);
		}

		[Obsolete("use ICGDataContainer instead", false)]
		public void Update(CasparCGItem item)
		{
			Channel.Device.Server.SendString("CG " + Channel.ID + " UPDATE " + item.Layer + " " + " \"" + item.XMLData + "\"");
		}
		public void Update(int layer, ICGDataContainer data)
		{
			Channel.Device.Server.SendString("CG " + Channel.ID + " UPDATE " + layer + " " + " \"" + data.ToAMCPEscapedXml() + "\"");
		}

		public void Invoke(int layer, string method)
		{
			Channel.Device.Server.SendString("CG " + Channel.ID + " INVOKE " + layer + " " + method);
		}

		public void Info()
		{
			Channel.Device.Server.SendString("CG " + Channel.ID + " INFO");
		}
	}
    	
	public interface ICGDataContainer
	{
		string ToXml();
		string ToAMCPEscapedXml();
	}

	public class CasparCGDataCollection : ICGDataContainer
	{
		private Dictionary<string, ICGComponentData> data_ = new Dictionary<string, ICGComponentData>();

		public void SetData(string name, string value)
		{
			data_[name] = new CGTextFieldData(value);
		}
		public void SetData(string name, ICGComponentData data)
		{
			data_[name] = data;
		}
		public ICGComponentData GetData(string name)
		{
			if (!string.IsNullOrEmpty(name) && data_.ContainsKey(name))
				return data_[name];

			return null;
		}
		public void Clear()
		{
			data_.Clear();
		}
		public void RemoveData(string name)
		{
			if(!string.IsNullOrEmpty(name) && data_.ContainsKey(name))
				data_.Remove(name);
		}

        public List<CGDataPair> DataPairs
        {
            get
            {
                List<CGDataPair> dataPairs = new List<CGDataPair>();
                data_.ToList().ForEach(d => dataPairs.Add(new CGDataPair(d.Key, d.Value)));

                return dataPairs;
            }
        }

		public string ToXml()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<templateData>");
			foreach (string key in data_.Keys)
			{
				sb.Append("<componentData id=\"" + key + "\">");
				data_[key].ToXml(sb);
				sb.Append("</componentData>");
			}
			sb.Append("</templateData>");
			return sb.ToString();
		}

		public string ToAMCPEscapedXml()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<templateData>");
			foreach(string key in data_.Keys)
			{
				sb.Append("<componentData id=\\\"" + key + "\\\">");
				data_[key].ToAMCPEscapedXml(sb);
				sb.Append("</componentData>");
			}
			sb.Append("</templateData>");

			sb.Replace(Environment.NewLine, "\\n");
			return sb.ToString();
		}
	}
}