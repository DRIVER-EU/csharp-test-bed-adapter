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
	
	public partial class Question : ISpecificRecord
	{
		public static Schema _SCHEMA = Avro.Schema.Parse(@"{""type"":""record"",""name"":""Question"",""namespace"":""eu.driver.model.core"",""fields"":[{""name"":""id"",""doc"":""The id of the question in OST database."",""type"":""int""},{""name"":""name"",""doc"":""The question."",""type"":""string""},{""name"":""description"",""doc"":""The additional clarifications shown below the question."",""type"":""string""},{""name"":""answer"",""doc"":""The answer marked by the user (names of radio buttons, names of checkboxes, value of slider or text)."",""type"":""string""},{""name"":""comment"",""doc"":""The comment provided by the user below the question."",""type"":""string""},{""name"":""typeOfQuestion"",""type"":{""type"":""enum"",""name"":""TypeOfQuestion"",""namespace"":""eu.driver.model.core"",""symbols"":[""slider"",""checkbox"",""radiobutton"",""text""]}}]}");
		/// <summary>
		/// The id of the question in OST database.
		/// </summary>
		private int _id;
		/// <summary>
		/// The question.
		/// </summary>
		private string _name;
		/// <summary>
		/// The additional clarifications shown below the question.
		/// </summary>
		private string _description;
		/// <summary>
		/// The answer marked by the user (names of radio buttons, names of checkboxes, value of slider or text).
		/// </summary>
		private string _answer;
		/// <summary>
		/// The comment provided by the user below the question.
		/// </summary>
		private string _comment;
		private eu.driver.model.core.TypeOfQuestion _typeOfQuestion;
		public virtual Schema Schema
		{
			get
			{
				return Question._SCHEMA;
			}
		}
		/// <summary>
		/// The id of the question in OST database.
		/// </summary>
		public int id
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
			}
		}
		/// <summary>
		/// The question.
		/// </summary>
		public string name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}
		/// <summary>
		/// The additional clarifications shown below the question.
		/// </summary>
		public string description
		{
			get
			{
				return this._description;
			}
			set
			{
				this._description = value;
			}
		}
		/// <summary>
		/// The answer marked by the user (names of radio buttons, names of checkboxes, value of slider or text).
		/// </summary>
		public string answer
		{
			get
			{
				return this._answer;
			}
			set
			{
				this._answer = value;
			}
		}
		/// <summary>
		/// The comment provided by the user below the question.
		/// </summary>
		public string comment
		{
			get
			{
				return this._comment;
			}
			set
			{
				this._comment = value;
			}
		}
		public eu.driver.model.core.TypeOfQuestion typeOfQuestion
		{
			get
			{
				return this._typeOfQuestion;
			}
			set
			{
				this._typeOfQuestion = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.id;
			case 1: return this.name;
			case 2: return this.description;
			case 3: return this.answer;
			case 4: return this.comment;
			case 5: return this.typeOfQuestion;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.id = (System.Int32)fieldValue; break;
			case 1: this.name = (System.String)fieldValue; break;
			case 2: this.description = (System.String)fieldValue; break;
			case 3: this.answer = (System.String)fieldValue; break;
			case 4: this.comment = (System.String)fieldValue; break;
			case 5: this.typeOfQuestion = (eu.driver.model.core.TypeOfQuestion)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}
