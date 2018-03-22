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
	
	public partial class ETYPEEVENT : ISpecificRecord
	{
		public static Schema _SCHEMA = Avro.Schema.Parse(@"{""type"":""record"",""name"":""ETYPEEVENT"",""namespace"":""eu.driver.model.emsi"",""fields"":[{""name"":""CATEGORY"",""type"":{""type"":""array"",""items"":""string""},""source"":""element CATEGORY""},{""name"":""ACTOR"",""type"":{""type"":""array"",""items"":""string""},""source"":""element ACTOR""},{""name"":""LOCTYPE"",""type"":{""type"":""array"",""items"":""string""},""source"":""element LOCTYPE""},{""name"":""ENV"",""type"":{""type"":""array"",""items"":""string""},""source"":""element ENV""}]}");
		private IList<System.String> _CATEGORY;
		private IList<System.String> _ACTOR;
		private IList<System.String> _LOCTYPE;
		private IList<System.String> _ENV;
		public virtual Schema Schema
		{
			get
			{
				return ETYPEEVENT._SCHEMA;
			}
		}
		public IList<System.String> CATEGORY
		{
			get
			{
				return this._CATEGORY;
			}
			set
			{
				this._CATEGORY = value;
			}
		}
		public IList<System.String> ACTOR
		{
			get
			{
				return this._ACTOR;
			}
			set
			{
				this._ACTOR = value;
			}
		}
		public IList<System.String> LOCTYPE
		{
			get
			{
				return this._LOCTYPE;
			}
			set
			{
				this._LOCTYPE = value;
			}
		}
		public IList<System.String> ENV
		{
			get
			{
				return this._ENV;
			}
			set
			{
				this._ENV = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.CATEGORY;
			case 1: return this.ACTOR;
			case 2: return this.LOCTYPE;
			case 3: return this.ENV;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.CATEGORY = (IList<System.String>)fieldValue; break;
			case 1: this.ACTOR = (IList<System.String>)fieldValue; break;
			case 2: this.LOCTYPE = (IList<System.String>)fieldValue; break;
			case 3: this.ENV = (IList<System.String>)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}
