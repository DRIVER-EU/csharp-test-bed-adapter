// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by avrogen.exe, version 0.9.0.0
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace eu.driver.model.geojson.sim
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using Avro;
	using Avro.Specific;
	
	public partial class Feature : ISpecificRecord
	{
		public static Schema _SCHEMA = Avro.Schema.Parse("{\"type\":\"record\",\"name\":\"Feature\",\"namespace\":\"eu.driver.model.geojson.sim\",\"fiel" +
				"ds\":[{\"name\":\"type\",\"default\":\"Feature\",\"type\":{\"type\":\"enum\",\"name\":\"FeatureTyp" +
				"e\",\"namespace\":\"eu.driver.model.geojson.sim\",\"symbols\":[\"Feature\"]}},{\"name\":\"bb" +
				"ox\",\"default\":null,\"type\":[\"null\",{\"type\":\"array\",\"items\":\"double\"}]},{\"name\":\"g" +
				"eometry\",\"type\":[{\"type\":\"record\",\"name\":\"Point\",\"namespace\":\"eu.driver.model.ge" +
				"ojson.sim\",\"fields\":[{\"name\":\"type\",\"default\":\"Point\",\"type\":{\"type\":\"enum\",\"nam" +
				"e\":\"PointType\",\"namespace\":\"eu.driver.model.geojson.sim\",\"symbols\":[\"Point\"]}},{" +
				"\"name\":\"coordinates\",\"type\":{\"type\":\"array\",\"items\":\"double\"}}]},{\"type\":\"record" +
				"\",\"name\":\"LineString\",\"namespace\":\"eu.driver.model.geojson.sim\",\"fields\":[{\"name" +
				"\":\"type\",\"default\":\"LineString\",\"type\":{\"type\":\"enum\",\"name\":\"LineStringType\",\"n" +
				"amespace\":\"eu.driver.model.geojson.sim\",\"symbols\":[\"LineString\"]}},{\"name\":\"coor" +
				"dinates\",\"type\":{\"type\":\"array\",\"items\":{\"type\":\"array\",\"items\":\"double\"}}}]},{\"" +
				"type\":\"record\",\"name\":\"MultiLineString\",\"namespace\":\"eu.driver.model.geojson.sim" +
				"\",\"fields\":[{\"name\":\"type\",\"default\":\"MultiLineString\",\"type\":{\"type\":\"enum\",\"na" +
				"me\":\"MultiLineStringType\",\"namespace\":\"eu.driver.model.geojson.sim\",\"symbols\":[\"" +
				"MultiLineString\"]}},{\"name\":\"coordinates\",\"type\":{\"type\":\"array\",\"items\":{\"type\"" +
				":\"array\",\"items\":{\"type\":\"array\",\"items\":\"double\"}}}}]},{\"type\":\"record\",\"name\":" +
				"\"Polygon\",\"namespace\":\"eu.driver.model.geojson.sim\",\"fields\":[{\"name\":\"type\",\"de" +
				"fault\":\"Polygon\",\"type\":{\"type\":\"enum\",\"name\":\"PolygonType\",\"namespace\":\"eu.driv" +
				"er.model.geojson.sim\",\"symbols\":[\"Polygon\"]}},{\"name\":\"coordinates\",\"type\":{\"typ" +
				"e\":\"array\",\"items\":{\"type\":\"array\",\"items\":{\"type\":\"array\",\"items\":\"double\"}}}}]" +
				"},{\"type\":\"record\",\"name\":\"MultiPolygon\",\"namespace\":\"eu.driver.model.geojson.si" +
				"m\",\"fields\":[{\"name\":\"type\",\"default\":\"MultiPolygon\",\"type\":{\"type\":\"enum\",\"name" +
				"\":\"MultiPolygonType\",\"namespace\":\"eu.driver.model.geojson.sim\",\"symbols\":[\"Multi" +
				"Polygon\"]}},{\"name\":\"coordinates\",\"type\":{\"type\":\"array\",\"items\":{\"type\":\"array\"" +
				",\"items\":{\"type\":\"array\",\"items\":{\"type\":\"array\",\"items\":\"double\"}}}}}]}]},{\"nam" +
				"e\":\"properties\",\"doc\":\"Properties that provide additional specification of the S" +
				"imulated entity in addition to its geographic information.\",\"type\":{\"type\":\"reco" +
				"rd\",\"name\":\"SimulatedEntityProperties\",\"namespace\":\"eu.driver.model.geojson.sim\"" +
				",\"fields\":[{\"name\":\"guid\",\"doc\":\"globally unique identifier for this entity\",\"ty" +
				"pe\":\"string\"},{\"name\":\"name\",\"doc\":\"name of this entity\",\"type\":\"string\"},{\"name" +
				"\":\"speed\",\"doc\":\"speed of the entity in m/s\",\"default\":null,\"type\":[\"null\",\"doub" +
				"le\"]},{\"name\":\"type\",\"type\":{\"type\":\"enum\",\"name\":\"TypeEnum\",\"namespace\":\"eu.dri" +
				"ver.model.geojson.sim\",\"symbols\":[\"OBJECT\",\"PERSON\",\"CAR\",\"VAN\",\"TRUCK\",\"BOAT\",\"" +
				"PLANE\",\"HELICOPTER\",\"MOTORCYCLE\",\"DRONE\",\"UNIT\",\"STATION\",\"UNITGROUP\",\"UNKNOWN\"]" +
				"}},{\"name\":\"label\",\"doc\":\"Label that describes the domain of the entity. E.g. Po" +
				"lice, Medical, Fire or Military.\",\"type\":\"string\"},{\"name\":\"subEntities\",\"doc\":\"" +
				"Entities contained by this entity. Only used for Units, Stations and Unit Groups" +
				". Array of strings consists of guids.\",\"default\":null,\"type\":[\"null\",{\"type\":\"ar" +
				"ray\",\"items\":\"string\"}]}]}}]}");
		private eu.driver.model.geojson.sim.FeatureType _type;
		private IList<System.Double> _bbox;
		private object _geometry;
		/// <summary>
		/// Properties that provide additional specification of the Simulated entity in addition to its geographic information.
		/// </summary>
		private eu.driver.model.geojson.sim.SimulatedEntityProperties _properties;
		public virtual Schema Schema
		{
			get
			{
				return Feature._SCHEMA;
			}
		}
		public eu.driver.model.geojson.sim.FeatureType type
		{
			get
			{
				return this._type;
			}
			set
			{
				this._type = value;
			}
		}
		public IList<System.Double> bbox
		{
			get
			{
				return this._bbox;
			}
			set
			{
				this._bbox = value;
			}
		}
		public object geometry
		{
			get
			{
				return this._geometry;
			}
			set
			{
				this._geometry = value;
			}
		}
		/// <summary>
		/// Properties that provide additional specification of the Simulated entity in addition to its geographic information.
		/// </summary>
		public eu.driver.model.geojson.sim.SimulatedEntityProperties properties
		{
			get
			{
				return this._properties;
			}
			set
			{
				this._properties = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.type;
			case 1: return this.bbox;
			case 2: return this.geometry;
			case 3: return this.properties;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.type = (eu.driver.model.geojson.sim.FeatureType)fieldValue; break;
			case 1: this.bbox = (IList<System.Double>)fieldValue; break;
			case 2: this.geometry = (System.Object)fieldValue; break;
			case 3: this.properties = (eu.driver.model.geojson.sim.SimulatedEntityProperties)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}
