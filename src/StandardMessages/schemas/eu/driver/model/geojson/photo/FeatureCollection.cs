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
	
	public partial class FeatureCollection : ISpecificRecord
	{
		public static Schema _SCHEMA = Avro.Schema.Parse("{\"type\":\"record\",\"name\":\"FeatureCollection\",\"namespace\":\"eu.driver.model.geojson." +
				"photo\",\"fields\":[{\"name\":\"type\",\"default\":\"FeatureCollection\",\"type\":{\"type\":\"en" +
				"um\",\"name\":\"FeatureCollectionType\",\"namespace\":\"eu.driver.model.geojson.photo\",\"" +
				"symbols\":[\"FeatureCollection\"]}},{\"name\":\"bbox\",\"default\":null,\"type\":[\"null\",{\"" +
				"type\":\"array\",\"items\":\"double\"}]},{\"name\":\"features\",\"type\":[\"null\",{\"type\":\"arr" +
				"ay\",\"items\":{\"type\":\"record\",\"name\":\"Feature\",\"namespace\":\"eu.driver.model.geojs" +
				"on.photo\",\"fields\":[{\"name\":\"type\",\"default\":\"Feature\",\"type\":{\"type\":\"enum\",\"na" +
				"me\":\"FeatureType\",\"namespace\":\"eu.driver.model.geojson.photo\",\"symbols\":[\"Featur" +
				"e\"]}},{\"name\":\"bbox\",\"default\":null,\"type\":[\"null\",{\"type\":\"array\",\"items\":\"doub" +
				"le\"}]},{\"name\":\"geometry\",\"type\":[{\"type\":\"record\",\"name\":\"Point\",\"namespace\":\"e" +
				"u.driver.model.geojson.photo\",\"fields\":[{\"name\":\"type\",\"default\":\"Point\",\"type\":" +
				"{\"type\":\"enum\",\"name\":\"PointType\",\"namespace\":\"eu.driver.model.geojson.photo\",\"s" +
				"ymbols\":[\"Point\"]}},{\"name\":\"coordinates\",\"type\":{\"type\":\"array\",\"items\":\"double" +
				"\"}}]},{\"type\":\"record\",\"name\":\"LineString\",\"namespace\":\"eu.driver.model.geojson." +
				"photo\",\"fields\":[{\"name\":\"type\",\"default\":\"LineString\",\"type\":{\"type\":\"enum\",\"na" +
				"me\":\"LineStringType\",\"namespace\":\"eu.driver.model.geojson.photo\",\"symbols\":[\"Lin" +
				"eString\"]}},{\"name\":\"coordinates\",\"type\":{\"type\":\"array\",\"items\":{\"type\":\"array\"" +
				",\"items\":\"double\"}}}]},{\"type\":\"record\",\"name\":\"MultiLineString\",\"namespace\":\"eu" +
				".driver.model.geojson.photo\",\"fields\":[{\"name\":\"type\",\"default\":\"MultiLineString" +
				"\",\"type\":{\"type\":\"enum\",\"name\":\"MultiLineStringType\",\"namespace\":\"eu.driver.mode" +
				"l.geojson.photo\",\"symbols\":[\"MultiLineString\"]}},{\"name\":\"coordinates\",\"type\":{\"" +
				"type\":\"array\",\"items\":{\"type\":\"array\",\"items\":{\"type\":\"array\",\"items\":\"double\"}}" +
				"}}]},{\"type\":\"record\",\"name\":\"Polygon\",\"namespace\":\"eu.driver.model.geojson.phot" +
				"o\",\"fields\":[{\"name\":\"type\",\"default\":\"Polygon\",\"type\":{\"type\":\"enum\",\"name\":\"Po" +
				"lygonType\",\"namespace\":\"eu.driver.model.geojson.photo\",\"symbols\":[\"Polygon\"]}},{" +
				"\"name\":\"coordinates\",\"type\":{\"type\":\"array\",\"items\":{\"type\":\"array\",\"items\":{\"ty" +
				"pe\":\"array\",\"items\":\"double\"}}}}]},{\"type\":\"record\",\"name\":\"MultiPolygon\",\"names" +
				"pace\":\"eu.driver.model.geojson.photo\",\"fields\":[{\"name\":\"type\",\"default\":\"MultiP" +
				"olygon\",\"type\":{\"type\":\"enum\",\"name\":\"MultiPolygonType\",\"namespace\":\"eu.driver.m" +
				"odel.geojson.photo\",\"symbols\":[\"MultiPolygon\"]}},{\"name\":\"coordinates\",\"type\":{\"" +
				"type\":\"array\",\"items\":{\"type\":\"array\",\"items\":{\"type\":\"array\",\"items\":{\"type\":\"a" +
				"rray\",\"items\":\"double\"}}}}}]}]},{\"name\":\"properties\",\"type\":{\"type\":\"record\",\"na" +
				"me\":\"properties\",\"namespace\":\"eu.driver.model.geojson.photo\",\"fields\":[{\"name\":\"" +
				"id\",\"type\":\"long\"},{\"name\":\"priority\",\"type\":\"long\"},{\"name\":\"viewed\",\"type\":\"bo" +
				"olean\"},{\"name\":\"signature\",\"type\":\"string\"},{\"name\":\"instant\",\"type\":\"string\"}," +
				"{\"name\":\"created\",\"type\":[\"null\",\"string\"]},{\"name\":\"updated\",\"type\":\"string\"},{" +
				"\"name\":\"caption\",\"type\":[\"null\",\"string\"]},{\"name\":\"interpretation\",\"type\":[\"str" +
				"ing\",\"null\"]},{\"name\":\"meta\",\"type\":{\"type\":\"record\",\"name\":\"meta\",\"namespace\":\"" +
				"eu.driver.model.geojson.photo\",\"fields\":[{\"name\":\"Source\",\"type\":{\"type\":\"record" +
				"\",\"name\":\"Source\",\"namespace\":\"eu.driver.model.geojson.photo\",\"fields\":[{\"name\":" +
				"\"filename\",\"type\":\"string\"},{\"name\":\"camera_make\",\"type\":[\"null\",\"string\"]},{\"na" +
				"me\":\"width\",\"type\":\"long\"},{\"name\":\"size\",\"type\":\"long\"},{\"name\":\"height\",\"type\"" +
				":\"long\"},{\"name\":\"direction\",\"type\":[\"null\",\"double\"]},{\"name\":\"camera_model\",\"t" +
				"ype\":[\"null\",\"string\"]},{\"name\":\"field_of_view\",\"type\":[\"null\",\"double\"]}]}}]}}," +
				"{\"name\":\"version\",\"type\":[\"string\",\"null\"]},{\"name\":\"application_id\",\"type\":[\"st" +
				"ring\",\"null\"]},{\"name\":\"seen\",\"type\":[\"string\",\"null\"]},{\"name\":\"started\",\"type\"" +
				":[\"string\",\"null\"]},{\"name\":\"stopped\",\"type\":[\"string\",\"null\"]},{\"name\":\"locatio" +
				"n_latitude\",\"type\":\"double\"},{\"name\":\"location_longitude\",\"type\":\"double\"},{\"nam" +
				"e\":\"location_time\",\"type\":\"string\"},{\"name\":\"location_accuracy\",\"type\":[\"double\"" +
				",\"null\"]},{\"name\":\"location_altitude\",\"type\":[\"double\",\"null\"]},{\"name\":\"locatio" +
				"n_provider\",\"type\":[\"string\",\"null\"]},{\"name\":\"location_speed\",\"type\":[\"double\"," +
				"\"null\"]},{\"name\":\"location_meta\",\"type\":[\"string\",\"null\"]},{\"name\":\"mission_id\"," +
				"\"type\":[\"long\",\"null\"]},{\"name\":\"mission_name\",\"type\":[\"string\",\"null\"]},{\"name\"" +
				":\"thumbnail_hash\",\"type\":\"string\"},{\"name\":\"preview_hash\",\"type\":\"string\"},{\"nam" +
				"e\":\"category_id\",\"type\":[\"long\",\"null\"]},{\"name\":\"category_name\",\"type\":[\"string" +
				"\",\"null\"]},{\"name\":\"application_device_type\",\"type\":[\"int\",\"null\"]},{\"name\":\"app" +
				"lication_last_login\",\"type\":[\"string\",\"null\"]},{\"name\":\"application_phone\",\"type" +
				"\":[\"string\",\"null\"]},{\"name\":\"application_last_rate\",\"type\":[\"long\",\"null\"]},{\"n" +
				"ame\":\"application_updated\",\"type\":[\"string\",\"null\"]},{\"name\":\"application_create" +
				"d\",\"type\":[\"string\",\"null\"]},{\"name\":\"application_application_type\",\"type\":[\"int" +
				"\",\"null\"]},{\"name\":\"application_connection_type\",\"type\":[\"int\",\"null\"]},{\"name\":" +
				"\"application_connection_state\",\"type\":[\"int\",\"null\"]},{\"name\":\"user_name\",\"type\"" +
				":\"string\"},{\"name\":\"user_id\",\"type\":\"long\"},{\"name\":\"user_username\",\"type\":\"stri" +
				"ng\"},{\"name\":\"user_color\",\"type\":[\"string\",\"null\"]},{\"name\":\"user_connection_typ" +
				"e\",\"type\":[\"int\",\"null\"]},{\"name\":\"user_last_login\",\"type\":[\"string\",\"null\"]},{\"" +
				"name\":\"user_last_rate\",\"type\":[\"long\",\"null\"]},{\"name\":\"observation_url\",\"type\":" +
				"\"string\"},{\"name\":\"observation_type\",\"type\":\"string\"},{\"name\":\"preview_url\",\"typ" +
				"e\":\"string\"},{\"name\":\"preview_with_overlay_url\",\"type\":\"string\"},{\"name\":\"thumbn" +
				"ail_url\",\"type\":\"string\"},{\"name\":\"files\",\"type\":{\"type\":\"array\",\"items\":{\"type\"" +
				":\"record\",\"name\":\"files\",\"namespace\":\"eu.driver.model.geojson.photo.files\",\"fiel" +
				"ds\":[{\"name\":\"id\",\"type\":\"long\"},{\"name\":\"file_type\",\"type\":\"int\"},{\"name\":\"size" +
				"\",\"type\":[\"long\",\"null\"]},{\"name\":\"state\",\"type\":\"int\"},{\"name\":\"created\",\"type\"" +
				":\"string\"},{\"name\":\"request_until\",\"default\":null,\"type\":[\"null\",\"string\"]},{\"na" +
				"me\":\"meta\",\"type\":[\"null\",\"string\",{\"type\":\"record\",\"name\":\"meta\",\"namespace\":\"e" +
				"u.driver.model.geojson.photo.files\",\"fields\":[{\"name\":\"Region\",\"type\":{\"type\":\"r" +
				"ecord\",\"name\":\"Region\",\"namespace\":\"eu.driver.model.geojson.photo.files\",\"fields" +
				"\":[{\"name\":\"x\",\"type\":\"long\"},{\"name\":\"y\",\"type\":\"long\"},{\"name\":\"width\",\"type\":" +
				"\"long\"},{\"name\":\"height\",\"type\":\"long\"},{\"name\":\"scale\",\"type\":\"double\"},{\"name\"" +
				":\"quality\",\"type\":\"int\"}]}}]}]},{\"name\":\"hash\",\"type\":[\"null\",\"string\"]},{\"name\"" +
				":\"url\",\"type\":[\"null\",\"string\"]}]}}}]}}]}}]}]}");
		private eu.driver.model.geojson.photo.FeatureCollectionType _type;
		private IList<System.Double> _bbox;
		private IList<eu.driver.model.geojson.photo.Feature> _features;
		public virtual Schema Schema
		{
			get
			{
				return FeatureCollection._SCHEMA;
			}
		}
		public eu.driver.model.geojson.photo.FeatureCollectionType type
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
		public IList<eu.driver.model.geojson.photo.Feature> features
		{
			get
			{
				return this._features;
			}
			set
			{
				this._features = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.type;
			case 1: return this.bbox;
			case 2: return this.features;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.type = (eu.driver.model.geojson.photo.FeatureCollectionType)fieldValue; break;
			case 1: this.bbox = (IList<System.Double>)fieldValue; break;
			case 2: this.features = (IList<eu.driver.model.geojson.photo.Feature>)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}