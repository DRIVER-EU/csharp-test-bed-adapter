// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by avrogen.exe, version 0.9.0.0
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace eu.driver.model.sim.entity
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using Avro;
	using Avro.Specific;
	
	public partial class Station : ISpecificRecord
	{
		public static Schema _SCHEMA = Avro.Schema.Parse("{\"type\":\"record\",\"name\":\"Station\",\"namespace\":\"eu.driver.model.sim.entity\",\"field" +
				"s\":[{\"name\":\"guid\",\"doc\":\"Globally unique identifier for this entity\",\"type\":\"st" +
				"ring\"},{\"name\":\"name\",\"doc\":\"Name of this entity\",\"type\":\"string\"},{\"name\":\"owne" +
				"r\",\"doc\":\"Identifier of the simulator currently responsible for this entity\",\"ty" +
				"pe\":\"string\"},{\"name\":\"location\",\"doc\":\"Geo-referenced location of this station\"" +
				",\"type\":{\"type\":\"record\",\"name\":\"Location\",\"namespace\":\"eu.driver.model.sim.geo\"" +
				",\"fields\":[{\"name\":\"latitude\",\"doc\":\"Latitude in degrees (-90, 90] - 0 is equato" +
				"r\",\"type\":\"double\"},{\"name\":\"longitude\",\"doc\":\"Longitude in degrees (-180, 180] " +
				"- 0 is line [geographic north - Greenwich - geographic south]\",\"type\":\"double\"}," +
				"{\"name\":\"altitude\",\"doc\":\"Altitude in meters - 0 is surface of WGS84-based ellip" +
				"soid\",\"type\":\"double\"}]}},{\"name\":\"address\",\"doc\":\"Address information regarding" +
				" the station\",\"type\":{\"type\":\"record\",\"name\":\"Address\",\"namespace\":\"eu.driver.mo" +
				"del.sim.geo\",\"fields\":[{\"name\":\"street\",\"doc\":\"The street name\",\"type\":\"string\"}" +
				",{\"name\":\"postalCode\",\"doc\":\"The postal code\",\"type\":\"string\"},{\"name\":\"city\",\"d" +
				"oc\":\"The city name\",\"type\":\"string\"},{\"name\":\"state\",\"doc\":\"The state or provinc" +
				"e name\",\"type\":\"string\"},{\"name\":\"country\",\"doc\":\"The country name\",\"type\":\"stri" +
				"ng\"}]}},{\"name\":\"visibleForParticipant\",\"doc\":\"Indication whether or not this st" +
				"ation is visible for all participants\",\"type\":\"boolean\"},{\"name\":\"movable\",\"doc\"" +
				":\"Indication whether or not this station is movable in the simulation world\",\"ty" +
				"pe\":\"boolean\"},{\"name\":\"scenarioLabel\",\"doc\":\"Scenario category of this station\"" +
				",\"default\":null,\"type\":[\"null\",{\"type\":\"enum\",\"name\":\"ScenarioLabel\",\"namespace\"" +
				":\"eu.driver.model.sim.entity.station\",\"symbols\":[\"GENERIC\",\"POLICE\",\"AMBULANCE\"," +
				"\"FIRE\",\"HOSPITAL\",\"MILITARY\",\"INCIDENT\",\"INCIDENT_MANAGEMENT\"]}]},{\"name\":\"userT" +
				"ags\",\"doc\":\"List of all tags the user provided associated with this station\",\"de" +
				"fault\":null,\"type\":[\"null\",{\"type\":\"array\",\"items\":\"string\"}]},{\"name\":\"items\",\"" +
				"doc\":\"List of physical item references (represented by their GUIDs) that are cur" +
				"rently at the station\",\"default\":null,\"type\":[\"null\",{\"type\":\"array\",\"items\":\"st" +
				"ring\"}]}]}");
		/// <summary>
		/// Globally unique identifier for this entity
		/// </summary>
		private string _guid;
		/// <summary>
		/// Name of this entity
		/// </summary>
		private string _name;
		/// <summary>
		/// Identifier of the simulator currently responsible for this entity
		/// </summary>
		private string _owner;
		/// <summary>
		/// Geo-referenced location of this station
		/// </summary>
		private eu.driver.model.sim.geo.Location _location;
		/// <summary>
		/// Address information regarding the station
		/// </summary>
		private eu.driver.model.sim.geo.Address _address;
		/// <summary>
		/// Indication whether or not this station is visible for all participants
		/// </summary>
		private bool _visibleForParticipant;
		/// <summary>
		/// Indication whether or not this station is movable in the simulation world
		/// </summary>
		private bool _movable;
		/// <summary>
		/// Scenario category of this station
		/// </summary>
		private System.Nullable<eu.driver.model.sim.entity.station.ScenarioLabel> _scenarioLabel;
		/// <summary>
		/// List of all tags the user provided associated with this station
		/// </summary>
		private IList<System.String> _userTags;
		/// <summary>
		/// List of physical item references (represented by their GUIDs) that are currently at the station
		/// </summary>
		private IList<System.String> _items;
		public virtual Schema Schema
		{
			get
			{
				return Station._SCHEMA;
			}
		}
		/// <summary>
		/// Globally unique identifier for this entity
		/// </summary>
		public string guid
		{
			get
			{
				return this._guid;
			}
			set
			{
				this._guid = value;
			}
		}
		/// <summary>
		/// Name of this entity
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
		/// Identifier of the simulator currently responsible for this entity
		/// </summary>
		public string owner
		{
			get
			{
				return this._owner;
			}
			set
			{
				this._owner = value;
			}
		}
		/// <summary>
		/// Geo-referenced location of this station
		/// </summary>
		public eu.driver.model.sim.geo.Location location
		{
			get
			{
				return this._location;
			}
			set
			{
				this._location = value;
			}
		}
		/// <summary>
		/// Address information regarding the station
		/// </summary>
		public eu.driver.model.sim.geo.Address address
		{
			get
			{
				return this._address;
			}
			set
			{
				this._address = value;
			}
		}
		/// <summary>
		/// Indication whether or not this station is visible for all participants
		/// </summary>
		public bool visibleForParticipant
		{
			get
			{
				return this._visibleForParticipant;
			}
			set
			{
				this._visibleForParticipant = value;
			}
		}
		/// <summary>
		/// Indication whether or not this station is movable in the simulation world
		/// </summary>
		public bool movable
		{
			get
			{
				return this._movable;
			}
			set
			{
				this._movable = value;
			}
		}
		/// <summary>
		/// Scenario category of this station
		/// </summary>
		public System.Nullable<eu.driver.model.sim.entity.station.ScenarioLabel> scenarioLabel
		{
			get
			{
				return this._scenarioLabel;
			}
			set
			{
				this._scenarioLabel = value;
			}
		}
		/// <summary>
		/// List of all tags the user provided associated with this station
		/// </summary>
		public IList<System.String> userTags
		{
			get
			{
				return this._userTags;
			}
			set
			{
				this._userTags = value;
			}
		}
		/// <summary>
		/// List of physical item references (represented by their GUIDs) that are currently at the station
		/// </summary>
		public IList<System.String> items
		{
			get
			{
				return this._items;
			}
			set
			{
				this._items = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.guid;
			case 1: return this.name;
			case 2: return this.owner;
			case 3: return this.location;
			case 4: return this.address;
			case 5: return this.visibleForParticipant;
			case 6: return this.movable;
			case 7: return this.scenarioLabel;
			case 8: return this.userTags;
			case 9: return this.items;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.guid = (System.String)fieldValue; break;
			case 1: this.name = (System.String)fieldValue; break;
			case 2: this.owner = (System.String)fieldValue; break;
			case 3: this.location = (eu.driver.model.sim.geo.Location)fieldValue; break;
			case 4: this.address = (eu.driver.model.sim.geo.Address)fieldValue; break;
			case 5: this.visibleForParticipant = (System.Boolean)fieldValue; break;
			case 6: this.movable = (System.Boolean)fieldValue; break;
			case 7: this.scenarioLabel = fieldValue == null ? (System.Nullable<eu.driver.model.sim.entity.station.ScenarioLabel>)null : (eu.driver.model.sim.entity.station.ScenarioLabel)fieldValue; break;
			case 8: this.userTags = (IList<System.String>)fieldValue; break;
			case 9: this.items = (IList<System.String>)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}
