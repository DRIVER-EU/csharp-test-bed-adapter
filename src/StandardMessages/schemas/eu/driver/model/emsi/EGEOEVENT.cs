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
	
	public partial class EGEOEVENT : ISpecificRecord
	{
		public static Schema _SCHEMA = Avro.Schema.Parse(@"{""type"":""record"",""name"":""EGEOEVENT"",""namespace"":""eu.driver.model.emsi"",""fields"":[{""name"":""DATIME"",""type"":[""null"",""long""],""source"":""element DATIME""},{""name"":""TYPE"",""type"":""string"",""source"":""element TYPE""},{""name"":""POSITION"",""type"":{""type"":""record"",""name"":""POSITION"",""namespace"":""eu.driver.model.emsi"",""fields"":[{""name"":""LOC_ID"",""type"":[""null"",""string""],""source"":""element LOC_ID""},{""name"":""NAME"",""type"":[""null"",""string""],""source"":""element NAME""},{""name"":""TYPE"",""type"":[""null"",""string""],""source"":""element TYPE""},{""name"":""COORDSYS"",""type"":[""null"",""string""],""source"":""element COORDSYS""},{""name"":""COORD"",""type"":{""type"":""array"",""items"":{""type"":""record"",""name"":""COORDType"",""namespace"":""eu.driver.model.emsi"",""fields"":[{""name"":""LAT"",""type"":""double"",""source"":""element LAT""},{""name"":""LON"",""type"":""double"",""source"":""element LON""},{""name"":""HEIGHT"",""type"":[""null"",""double""],""source"":""element HEIGHT""}]}},""source"":""element COORD""},{""name"":""HEIGHT_ROLE"",""type"":[""null"",""string""],""source"":""element HEIGHT_ROLE""},{""name"":""ADDRESS"",""type"":{""type"":""array"",""items"":""string""},""source"":""element ADDRESS""}]},""source"":""element POSITION""},{""name"":""WEATHER"",""type"":{""type"":""array"",""items"":""string""},""source"":""element WEATHER""},{""name"":""FREETEXT"",""type"":[""null"",""string""],""source"":""element FREETEXT""},{""name"":""ID"",""type"":[""null"",""string""],""source"":""element ID""},{""name"":""STATUS"",""type"":[""null"",""string""],""source"":""element STATUS""}]}");
		private System.Nullable<long> _DATIME;
		private string _TYPE;
		private eu.driver.model.emsi.POSITION _POSITION;
		private IList<System.String> _WEATHER;
		private string _FREETEXT;
		private string _ID;
		private string _STATUS;
		public virtual Schema Schema
		{
			get
			{
				return EGEOEVENT._SCHEMA;
			}
		}
		public System.Nullable<long> DATIME
		{
			get
			{
				return this._DATIME;
			}
			set
			{
				this._DATIME = value;
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
		public eu.driver.model.emsi.POSITION POSITION
		{
			get
			{
				return this._POSITION;
			}
			set
			{
				this._POSITION = value;
			}
		}
		public IList<System.String> WEATHER
		{
			get
			{
				return this._WEATHER;
			}
			set
			{
				this._WEATHER = value;
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
		public string ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				this._ID = value;
			}
		}
		public string STATUS
		{
			get
			{
				return this._STATUS;
			}
			set
			{
				this._STATUS = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.DATIME;
			case 1: return this.TYPE;
			case 2: return this.POSITION;
			case 3: return this.WEATHER;
			case 4: return this.FREETEXT;
			case 5: return this.ID;
			case 6: return this.STATUS;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.DATIME = (System.Nullable<long>)fieldValue; break;
			case 1: this.TYPE = (System.String)fieldValue; break;
			case 2: this.POSITION = (eu.driver.model.emsi.POSITION)fieldValue; break;
			case 3: this.WEATHER = (IList<System.String>)fieldValue; break;
			case 4: this.FREETEXT = (System.String)fieldValue; break;
			case 5: this.ID = (System.String)fieldValue; break;
			case 6: this.STATUS = (System.String)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}