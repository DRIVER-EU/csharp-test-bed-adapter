// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by avrogen.exe, version 0.9.0.0
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace eu.driver.model.sim.entity.item
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using Avro;
	using Avro.Specific;
	
	public partial class RescueLabel : ISpecificRecord
	{
		public static Schema _SCHEMA = Avro.Schema.Parse(@"{""type"":""record"",""name"":""RescueLabel"",""namespace"":""eu.driver.model.sim.entity.item"",""fields"":[{""name"":""subLabel"",""doc"":""Sub label of rescue that this item has"",""type"":{""type"":""enum"",""name"":""RescueSubLabel"",""namespace"":""eu.driver.model.sim.entity.item"",""symbols"":[""POLICE"",""MEDICAL"",""FIRE"",""SECURITY"",""MILITARY""]}}]}");
		/// <summary>
		/// Sub label of rescue that this item has
		/// </summary>
		private eu.driver.model.sim.entity.item.RescueSubLabel _subLabel;
		public virtual Schema Schema
		{
			get
			{
				return RescueLabel._SCHEMA;
			}
		}
		/// <summary>
		/// Sub label of rescue that this item has
		/// </summary>
		public eu.driver.model.sim.entity.item.RescueSubLabel subLabel
		{
			get
			{
				return this._subLabel;
			}
			set
			{
				this._subLabel = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.subLabel;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.subLabel = (eu.driver.model.sim.entity.item.RescueSubLabel)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}
