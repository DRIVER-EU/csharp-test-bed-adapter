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
	
	public partial class Source : ISpecificRecord
	{
		public static Schema _SCHEMA = Avro.Schema.Parse(@"{""type"":""record"",""name"":""Source"",""namespace"":""eu.driver.model.geojson.photo"",""fields"":[{""name"":""filename"",""type"":""string""},{""name"":""camera_make"",""type"":[""null"",""string""]},{""name"":""width"",""type"":""long""},{""name"":""size"",""type"":""long""},{""name"":""height"",""type"":""long""},{""name"":""direction"",""type"":[""null"",""double""]},{""name"":""camera_model"",""type"":[""null"",""string""]},{""name"":""field_of_view"",""type"":[""null"",""double""]}]}");
		private string _filename;
		private string _camera_make;
		private long _width;
		private long _size;
		private long _height;
		private System.Nullable<double> _direction;
		private string _camera_model;
		private System.Nullable<double> _field_of_view;
		public virtual Schema Schema
		{
			get
			{
				return Source._SCHEMA;
			}
		}
		public string filename
		{
			get
			{
				return this._filename;
			}
			set
			{
				this._filename = value;
			}
		}
		public string camera_make
		{
			get
			{
				return this._camera_make;
			}
			set
			{
				this._camera_make = value;
			}
		}
		public long width
		{
			get
			{
				return this._width;
			}
			set
			{
				this._width = value;
			}
		}
		public long size
		{
			get
			{
				return this._size;
			}
			set
			{
				this._size = value;
			}
		}
		public long height
		{
			get
			{
				return this._height;
			}
			set
			{
				this._height = value;
			}
		}
		public System.Nullable<double> direction
		{
			get
			{
				return this._direction;
			}
			set
			{
				this._direction = value;
			}
		}
		public string camera_model
		{
			get
			{
				return this._camera_model;
			}
			set
			{
				this._camera_model = value;
			}
		}
		public System.Nullable<double> field_of_view
		{
			get
			{
				return this._field_of_view;
			}
			set
			{
				this._field_of_view = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.filename;
			case 1: return this.camera_make;
			case 2: return this.width;
			case 3: return this.size;
			case 4: return this.height;
			case 5: return this.direction;
			case 6: return this.camera_model;
			case 7: return this.field_of_view;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.filename = (System.String)fieldValue; break;
			case 1: this.camera_make = (System.String)fieldValue; break;
			case 2: this.width = (System.Int64)fieldValue; break;
			case 3: this.size = (System.Int64)fieldValue; break;
			case 4: this.height = (System.Int64)fieldValue; break;
			case 5: this.direction = (System.Nullable<double>)fieldValue; break;
			case 6: this.camera_model = (System.String)fieldValue; break;
			case 7: this.field_of_view = (System.Nullable<double>)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}