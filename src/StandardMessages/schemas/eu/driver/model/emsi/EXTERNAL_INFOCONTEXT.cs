// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by avrogen.exe, version 0.9.0.0
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace eu.driver.model.emsi
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using Avro;
	using Avro.Specific;
	
	public partial class EXTERNAL_INFOCONTEXT : ISpecificRecord
	{
		public static Schema _SCHEMA = Avro.Schema.Parse(@"{""type"":""record"",""name"":""EXTERNAL_INFOCONTEXT"",""namespace"":""eu.driver.model.emsi"",""fields"":[{""name"":""FREETEXT"",""type"":[""null"",""string""],""source"":""element FREETEXT""},{""name"":""URI"",""type"":""string"",""source"":""element URI""},{""name"":""TYPE"",""type"":[""null"",""string""],""source"":""element TYPE""}]}");
		private string _FREETEXT;
		private string _URI;
		private string _TYPE;
		public virtual Schema Schema
		{
			get
			{
				return EXTERNAL_INFOCONTEXT._SCHEMA;
			}
		}
		public string FREETEXT
		{
			get
			{
				return this._FREETEXT;
			}
			set
			{
				this._FREETEXT = value;
			}
		}
		public string URI
		{
			get
			{
				return this._URI;
			}
			set
			{
				this._URI = value;
			}
		}
		public string TYPE
		{
			get
			{
				return this._TYPE;
			}
			set
			{
				this._TYPE = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.FREETEXT;
			case 1: return this.URI;
			case 2: return this.TYPE;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.FREETEXT = (System.String)fieldValue; break;
			case 1: this.URI = (System.String)fieldValue; break;
			case 2: this.TYPE = (System.String)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}
