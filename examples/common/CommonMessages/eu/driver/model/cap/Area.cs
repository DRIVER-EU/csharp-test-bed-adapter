// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by avrogen.exe, version 0.9.0.0
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace eu.driver.model.cap
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using Avro;
	using Avro.Specific;
	
	public partial class Area : ISpecificRecord
	{
		public static Schema _SCHEMA = Avro.Schema.Parse(@"{""type"":""record"",""name"":""Area"",""namespace"":""eu.driver.model.cap"",""fields"":[{""name"":""areaDesc"",""type"":""string""},{""name"":""polygon"",""default"":null,""type"":[""null"",""string"",{""type"":""array"",""items"":""string""}]},{""name"":""circle"",""default"":null,""type"":[""null"",""string"",{""type"":""array"",""items"":""string""}]},{""name"":""geocode"",""default"":null,""type"":[""null"",{""type"":""record"",""name"":""ValueNamePair"",""namespace"":""eu.driver.model.cap"",""fields"":[{""name"":""valueName"",""type"":""string""},{""name"":""value"",""type"":""string""}]},{""type"":""array"",""items"":""ValueNamePair""}]},{""name"":""altitude"",""default"":null,""type"":[""null"",""double""]},{""name"":""ceiling"",""default"":null,""type"":[""null"",""double""]}]}");
		private string _areaDesc;
		private object _polygon;
		private object _circle;
		private object _geocode;
		private System.Nullable<double> _altitude;
		private System.Nullable<double> _ceiling;
		public virtual Schema Schema
		{
			get
			{
				return Area._SCHEMA;
			}
		}
		public string areaDesc
		{
			get
			{
				return this._areaDesc;
			}
			set
			{
				this._areaDesc = value;
			}
		}
		public object polygon
		{
			get
			{
				return this._polygon;
			}
			set
			{
				this._polygon = value;
			}
		}
		public object circle
		{
			get
			{
				return this._circle;
			}
			set
			{
				this._circle = value;
			}
		}
		public object geocode
		{
			get
			{
				return this._geocode;
			}
			set
			{
				this._geocode = value;
			}
		}
		public System.Nullable<double> altitude
		{
			get
			{
				return this._altitude;
			}
			set
			{
				this._altitude = value;
			}
		}
		public System.Nullable<double> ceiling
		{
			get
			{
				return this._ceiling;
			}
			set
			{
				this._ceiling = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.areaDesc;
			case 1: return this.polygon;
			case 2: return this.circle;
			case 3: return this.geocode;
			case 4: return this.altitude;
			case 5: return this.ceiling;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.areaDesc = (System.String)fieldValue; break;
			case 1: this.polygon = (System.Object)fieldValue; break;
			case 2: this.circle = (System.Object)fieldValue; break;
			case 3: this.geocode = (System.Object)fieldValue; break;
			case 4: this.altitude = (System.Nullable<double>)fieldValue; break;
			case 5: this.ceiling = (System.Nullable<double>)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}
