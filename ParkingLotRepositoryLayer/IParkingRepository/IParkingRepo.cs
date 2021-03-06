﻿using ParkingLotModelLayer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ParkingLotRepositoryLayer.IParkingRepository
{
    public interface IParkingRepo
    {
        Task<int> Parking(ParkingDetails parkingDetails);
        object UnParking(int slotNumber);
        ParkingDetails SearchByVehicleNumber(string vehicleNumber);
        ParkingDetails SearchBySlotNumber(int slotNumber);
        Task<int> DeleteAllUnParkedVehicles();
        IEnumerable<ParkingDetails> GetAllParkingDetails();
    }
}
