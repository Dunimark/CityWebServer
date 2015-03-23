﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using CityWebServer.Extensibility;
using CityWebServer.Helpers;
using ColossalFramework;
using ColossalFramework.Plugins;

namespace CityWebServer.RequestHandlers
{
    public class VehicleRequestHandler : IRequestHandler
    {
        public Guid HandlerID
        {
            get { return new Guid("2be6546a-d416-4939-8e08-1d0b739be835"); }
        }

        public int Priority
        {
            get { return 100; }
        }

        public string Name
        {
            get { return "Vehicle"; }
        }

        public string Author
        {
            get { return "Rychard"; }
        }

        public string MainPath
        {
            get { return "/Vehicle"; }
        }

        public bool ShouldHandle(HttpListenerRequest request)
        {
            return (request.Url.AbsolutePath.StartsWith("/Vehicle", StringComparison.OrdinalIgnoreCase));
        }

        public void Handle(HttpListenerRequest request, HttpListenerResponse response)
        {
            var vehicleManager = Singleton<VehicleManager>.instance;

            if (request.Url.AbsolutePath.StartsWith("/Vehicle/List"))
            {
                List<ushort> vehicleIds = new List<ushort>();
                
                var len = vehicleManager.m_vehicles.m_buffer.Length;
                for (ushort i = 0; i < len; i++)
                {
                    if (vehicleManager.m_vehicles.m_buffer[i].m_flags == Vehicle.Flags.None) { continue; }

                    vehicleIds.Add(i);
                }
                response.WriteJson(vehicleIds);
                return;
            }

            //BuildingManager instance = Singleton<BuildingManager>.instance;
            //foreach (Building building in instance.m_buildings.m_buffer)
            //{
            //    if (building.m_flags == Building.Flags.None) { continue; }
            //    var districtID = (int)districtManager.GetDistrict(building.m_position);
            //    if (districtBuildings.ContainsKey(districtID))
            //    {
            //        districtBuildings[districtID]++;
            //    }
            //    else
            //    {
            //        districtBuildings.Add(districtID, 1);
            //    }
            //}


            List<ushort> s = new List<ushort>();

            foreach (var vehicle in vehicleManager.m_vehicles.m_buffer)
            {
                if (vehicle.m_flags == Vehicle.Flags.None) { continue; }

                if((vehicle.m_flags & Vehicle.Flags.Spawned) == Vehicle.Flags.Spawned && (vehicle.m_flags & Vehicle.Flags.Created) == Vehicle.Flags.Created)
                {
                    var origin = (vehicle.m_sourceBuilding);
                    var target = (vehicle.m_targetBuilding);

                    //s.Add((Enum.GetName(typeof(VehicleInfo.VehicleType), vehicle.Info.m_vehicleType)));
                    if (origin > 0) { s.Add(origin); }
                    if (target > 0) { s.Add(target); }    
                }
            }

            var grouped = s.GroupBy(obj => obj).Select(group => new { BuildingID = group.Key, Count = group.Count() }).OrderByDescending(obj => obj.Count).Select(obj => new { Building = BuildingManager.instance.GetBuildingName(obj.BuildingID, new InstanceID()), obj.Count }).ToList();
            
            //s.Sort();
            
            //response.WriteJson(s);
            response.WriteJson(grouped);
        }
    }
}
