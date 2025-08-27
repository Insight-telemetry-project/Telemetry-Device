using System;
using System.Collections.Generic;
using Telemetry_Device.Models;

namespace Telemetry_Device.Services
{
    public class FieldMapping
    {
        public (Action<DecodedPacket, float> setter, int offset)[] FloatFields { get; } =
        {
            ((p, v) => p.Volt1 = v, 0),
            ((p, v) => p.Volt2 = v, 4),
            ((p, v) => p.Amp1 = v, 8),
            ((p, v) => p.Amp2 = v, 12),
            ((p, v) => p.FQtyL = v, 16),
            ((p, v) => p.FQtyR = v, 20),
            ((p, v) => p.E1FFlow = v, 24),
            ((p, v) => p.E1OilT = v, 28),
            ((p, v) => p.E1OilP = v, 32),
            ((p, v) => p.E1RPM = v, 36),
            ((p, v) => p.E1CHT1 = v, 40),
            ((p, v) => p.E1CHT2 = v, 44),
            ((p, v) => p.E1CHT3 = v, 48),
            ((p, v) => p.E1CHT4 = v, 52),
            ((p, v) => p.E1EGT1 = v, 56),
            ((p, v) => p.E1EGT2 = v, 60),
            ((p, v) => p.E1EGT3 = v, 64),
            ((p, v) => p.E1EGT4 = v, 68),
            ((p, v) => p.OAT = v, 72),
            ((p, v) => p.IAS = v, 76),
            ((p, v) => p.VSpd = v, 80),
            ((p, v) => p.NormAc = v, 84),
            ((p, v) => p.AltMSL = v, 88),
        };

        public (Action<DecodedPacket, int> setter, int offset)[] IntFields { get; } =
        {
            ((p, v) => p.Timestep = v, 92),
            ((p, v) => p.Cluster = v, 96),
            ((p, v) => p.MasterIndex = v, 100),
            ((p, v) => p.DateDiff = v, 104),
            ((p, v) => p.FlightLength = v, 108),
            ((p, v) => p.NumberFlightsBefore = v, 112),
        };
    }
}
