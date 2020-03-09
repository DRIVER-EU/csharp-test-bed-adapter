// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by avrogen.exe, version 0.9.0.0
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace eu.driver.model.geojson.photo
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using Avro;
	using Avro.Specific;
	
	public partial class meta : ISpecificRecord
	{
		public static Schema _SCHEMA = Avro.Schema.Parse(@"{""type"":""record"",""name"":""meta"",""namespace"":""eu.driver.model.geojson.photo"",""fields"":[{""name"":""Source"",""type"":{""type"":""record"",""name"":""Source"",""namespace"":""eu.driver.model.geojson.photo"",""fields"":[{""name"":""filename"",""type"":""string""},{""name"":""camera_make"",""type"":[""null"",""string""]},{""name"":""width"",""type"":""long""},{""name"":""size"",""type"":""long""},{""name"":""height"",""type"":""long""},{""name"":""direction"",""type"":[""null"",""double""]},{""name"":""camera_model"",""type"":[""null"",""string""]},{""name"":""field_of_view"",""type"":[""null"",""double""]}]}}]}");
		private eu.driver.model.geojson.photo.Source _Source;
		public virtual Schema Schema
		{
			get
			{
				return meta._SCHEMA;
			}
		}
		public eu.driver.model.geojson.photo.Source Source
		{
			get
			{
				return this._Source;
			}
			set
			{
				this._Source = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.Source;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.Source = (eu.driver.model.geojson.photo.Source)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}