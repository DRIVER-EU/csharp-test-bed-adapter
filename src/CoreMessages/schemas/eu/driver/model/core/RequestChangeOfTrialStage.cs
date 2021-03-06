// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by avrogen.exe, version 0.9.0.0
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace eu.driver.model.core
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using Avro;
	using Avro.Specific;
	
	public partial class RequestChangeOfTrialStage : ISpecificRecord
	{
		public static Schema _SCHEMA = Avro.Schema.Parse(@"{""type"":""record"",""name"":""RequestChangeOfTrialStage"",""namespace"":""eu.driver.model.core"",""fields"":[{""name"":""ostTrialId"",""doc"":""The unique identifier of the running Trial."",""default"":null,""type"":[""null"",""int""]},{""name"":""ostTrialSessionId"",""doc"":""The sessionId for the running Trial."",""type"":""int""},{""name"":""ostTrialStageId"",""doc"":""The stageId of the stage that should be activated."",""type"":""int""}]}");
		/// <summary>
		/// The unique identifier of the running Trial.
		/// </summary>
		private System.Nullable<int> _ostTrialId;
		/// <summary>
		/// The sessionId for the running Trial.
		/// </summary>
		private int _ostTrialSessionId;
		/// <summary>
		/// The stageId of the stage that should be activated.
		/// </summary>
		private int _ostTrialStageId;
		public virtual Schema Schema
		{
			get
			{
				return RequestChangeOfTrialStage._SCHEMA;
			}
		}
		/// <summary>
		/// The unique identifier of the running Trial.
		/// </summary>
		public System.Nullable<int> ostTrialId
		{
			get
			{
				return this._ostTrialId;
			}
			set
			{
				this._ostTrialId = value;
			}
		}
		/// <summary>
		/// The sessionId for the running Trial.
		/// </summary>
		public int ostTrialSessionId
		{
			get
			{
				return this._ostTrialSessionId;
			}
			set
			{
				this._ostTrialSessionId = value;
			}
		}
		/// <summary>
		/// The stageId of the stage that should be activated.
		/// </summary>
		public int ostTrialStageId
		{
			get
			{
				return this._ostTrialStageId;
			}
			set
			{
				this._ostTrialStageId = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.ostTrialId;
			case 1: return this.ostTrialSessionId;
			case 2: return this.ostTrialStageId;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.ostTrialId = (System.Nullable<int>)fieldValue; break;
			case 1: this.ostTrialSessionId = (System.Int32)fieldValue; break;
			case 2: this.ostTrialStageId = (System.Int32)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}
