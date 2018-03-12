// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by avrogen.exe, version 0.9.0.0
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace eu.driver.model.mlp
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using Avro;
	using Avro.Specific;
	
	public partial class Msid : ISpecificRecord
	{
		public static Schema _SCHEMA = Avro.Schema.Parse(@"{""type"":""record"",""name"":""Msid"",""namespace"":""eu.driver.model.mlp"",""fields"":[{""name"":""msid"",""type"":""string""},{""name"":""attr_type"",""type"":{""type"":""enum"",""name"":""AttrType"",""namespace"":""eu.driver.model.mlp"",""symbols"":[""MSISDN"",""IMSI"",""IMEI"",""MIN"",""MDN"",""EME_MSID"",""ASID"",""OPE_ID"",""IPV4"",""IPV6"",""SESSID""]}},{""name"":""attr_enc"",""type"":{""type"":""enum"",""name"":""AttrEnc"",""namespace"":""eu.driver.model.mlp"",""symbols"":[""ASC"",""CRP""]}}]}");
		private string _msid;
		private eu.driver.model.mlp.AttrType _attr_type;
		private eu.driver.model.mlp.AttrEnc _attr_enc;
		public virtual Schema Schema
		{
			get
			{
				return Msid._SCHEMA;
			}
		}
		public string msid
		{
			get
			{
				return this._msid;
			}
			set
			{
				this._msid = value;
			}
		}
		public eu.driver.model.mlp.AttrType attr_type
		{
			get
			{
				return this._attr_type;
			}
			set
			{
				this._attr_type = value;
			}
		}
		public eu.driver.model.mlp.AttrEnc attr_enc
		{
			get
			{
				return this._attr_enc;
			}
			set
			{
				this._attr_enc = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.msid;
			case 1: return this.attr_type;
			case 2: return this.attr_enc;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.msid = (System.String)fieldValue; break;
			case 1: this.attr_type = (eu.driver.model.mlp.AttrType)fieldValue; break;
			case 2: this.attr_enc = (eu.driver.model.mlp.AttrEnc)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}