﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ParkingLotApplication.MSMQ;
using ParkingLotManagerLayer.IParkingLotManager;
using ParkingLotModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingLotApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Security,Policemen")]
    public class SecurityController : ControllerBase
    {
        private readonly IParkingManager parkingManager;
        private readonly MSMQService mSMQService = new MSMQService();
        public SecurityController(IParkingManager parkingManager)
        {
            this.parkingManager = parkingManager;
        }
        [HttpPost]
        [Route("Parking")]
        public async Task<ActionResult> VehicleParking(ParkingDetails parkingDetails)
        {
            try
            {
                var result = await parkingManager.Parking(parkingDetails);
                if (result == 1 && parkingDetails.DriverType == 4)
                {
                    this.mSMQService.AddToQueue("Vehicle Parked Sucssesfully...Which vehicle number is " + parkingDetails.VehicleNumber + " in Parking Slot " + parkingDetails.ParkingSlotNumber);
                    return this.Ok(new { Status = true, Message = "Vehicle Parked Sucssesfully", Data = parkingDetails });
                }
                return this.BadRequest(new { Status = true, Message = "Vehicle Parking Un-Sucssesfull!!" });
            }
            catch (Exception e)
            {
                return this.NotFound(new { Status = "False", Message = e.Message });
            }
        }
        [HttpPut]
        [Route("UnParking")]
        public ActionResult VehicleUnParking(int slotNumber)
        {
            try
            {
                var result = parkingManager.UnParking(slotNumber);
                if (result != null)
                {
                    this.mSMQService.AddToQueue("Vehicle Un-Parked Sucssesfully  from slot Number " + slotNumber);
                    return this.Ok(new { Status = true, Message = "Vehicle Un-Parked Sucssesfully", Data = result });
                }
                return this.BadRequest(new { Status = true, Message = "Vehicle Un-Parking was Un-Sucssesfull!!" });
            }
            catch (Exception e)
            {
                return this.NotFound(new { Status = false, Message = e.Message });
            }
        }
        [HttpGet]
        [Route("SearchByVehicleNumber")]
        public ActionResult SearchByVehicleNumber(string vehicleNumber)
        {
            try
            {
                var result = parkingManager.SearchByVehicleNumber(vehicleNumber);
                if (result != null)
                {
                    return this.Ok(new { Status = true, Message = "Vehicle Detail Found", Data = result });
                }
                return this.BadRequest(new { Status = true, Message = "Searched Vehicle Data Not Found" });
            }
            catch (Exception e)
            {
                return this.NotFound(new { Status = false, Message = e.Message });
            }
        }
        [HttpDelete]
        [Route("DeleteAllUnParkedVehicles")]
        public async Task<ActionResult> DeleteAllUnParkedVehicles()
        {
            try
            {
                var result = await parkingManager.DeleteAllUnParkedVehicles();
                if (result >= 1)
                {
                    return this.Ok(new { Status = true, Message = "Vehicle's Parking Details Deleted Sucssesfully" });
                }
                return this.BadRequest(new { Status = false, Message = "There is nothing to Delete" });
            }
            catch (Exception e)
            {
                return NotFound(new { Status = false, Message = e.Message });
            }
        }
    }
}
