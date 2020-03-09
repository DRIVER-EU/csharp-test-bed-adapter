// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by avrogen.exe, version 0.9.0.0
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace eu.driver.model.geojson.photo.files
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using Avro;
	using Avro.Specific;
	
	public partial class Region : ISpecificRecord
	{
		public static Schema _SCHEMA = Avro.Schema.Parse(@"{""type"":""record"",""name"":""Region"",""namespace"":""eu.driver.model.geojson.photo.files"",""fields"":[{""name"":""x"",""type"":""long""},{""name"":""y"",""type"":""long""},{""name"":""width"",""type"":""long""},{""name"":""height"",""type"":""long""},{""name"":""scale"",""type"":""double""},{""name"":""quality"",""type"":""int""}]}");
		private long _x;
		private long _y;
		private long _width;
		private long _height;
		private double _scale;
		private int _quality;
		public virtual Schema Schema
		{
			get
			{
				return Region._SCHEMA;
			}
		}
		public long x
		{
			get
			{
				return this._x;
			}
			set
			{
				this._x = value;
			}
		}
		public long y
		{
			get
			{
				return this._y;
			}
			set
			{
				this._y = value;
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
		public double scale
		{
			get
			{
				return this._scale;
			}
			set
			{
				this._scale = value;
			}
		}
		public int quality
		{
			get
			{
				return this._quality;
			}
			set
			{
				this._quality = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.x;
			case 1: return this.y;
			case 2: return this.width;
			case 3: return this.height;
			case 4: return this.scale;
			case 5: return this.quality;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.x = (System.Int64)fieldValue; break;
			case 1: this.y = (System.Int64)fieldValue; break;
			case 2: this.width = (System.Int64)fieldValue; break;
			case 3: this.height = (System.Int64)fieldValue; break;
			case 4: this.scale = (System.Double)fieldValue; break;
			case 5: this.quality = (System.Int32)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}