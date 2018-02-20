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
	
	public partial class LogSettings : ISpecificRecord
	{
		public static Schema _SCHEMA = Avro.Schema.Parse(@"{""type"":""record"",""name"":""LogSettings"",""namespace"":""eu.driver.model.core"",""fields"":[{""name"":""logToFile"",""doc"":""If set [0..5], log to file as specified in logFile"",""default"":null,""type"":[""null"",""int""]},{""name"":""logFile"",""doc"":""Name of the log file"",""default"":null,""type"":[""null"",""string""]},{""name"":""logToConsole"",""doc"":""If set [0..5], log to console. Number indicates logging level"",""default"":null,""type"":[""null"",""int""]},{""name"":""logToKafka"",""doc"":""If set [0..5], log to Kafka"",""default"":null,""type"":[""null"",""int""]}]}");
		/// <summary>
		/// If set [0..5], log to file as specified in logFile
		/// </summary>
		private System.Nullable<int> _logToFile;
		/// <summary>
		/// Name of the log file
		/// </summary>
		private string _logFile;
		/// <summary>
		/// If set [0..5], log to console. Number indicates logging level
		/// </summary>
		private System.Nullable<int> _logToConsole;
		/// <summary>
		/// If set [0..5], log to Kafka
		/// </summary>
		private System.Nullable<int> _logToKafka;
		public virtual Schema Schema
		{
			get
			{
				return LogSettings._SCHEMA;
			}
		}
		/// <summary>
		/// If set [0..5], log to file as specified in logFile
		/// </summary>
		public System.Nullable<int> logToFile
		{
			get
			{
				return this._logToFile;
			}
			set
			{
				this._logToFile = value;
			}
		}
		/// <summary>
		/// Name of the log file
		/// </summary>
		public string logFile
		{
			get
			{
				return this._logFile;
			}
			set
			{
				this._logFile = value;
			}
		}
		/// <summary>
		/// If set [0..5], log to console. Number indicates logging level
		/// </summary>
		public System.Nullable<int> logToConsole
		{
			get
			{
				return this._logToConsole;
			}
			set
			{
				this._logToConsole = value;
			}
		}
		/// <summary>
		/// If set [0..5], log to Kafka
		/// </summary>
		public System.Nullable<int> logToKafka
		{
			get
			{
				return this._logToKafka;
			}
			set
			{
				this._logToKafka = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.logToFile;
			case 1: return this.logFile;
			case 2: return this.logToConsole;
			case 3: return this.logToKafka;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.logToFile = (System.Nullable<int>)fieldValue; break;
			case 1: this.logFile = (System.String)fieldValue; break;
			case 2: this.logToConsole = (System.Nullable<int>)fieldValue; break;
			case 3: this.logToKafka = (System.Nullable<int>)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}
