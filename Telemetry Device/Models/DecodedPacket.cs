namespace Telemetry_Device.Models
{
    public class DecodedPacket
    {
        public double Timestamp { get; set; }

        public float Volt1 { get; set; }
        public float Volt2 { get; set; }

        public float Amp1 { get; set; }
        public float Amp2 { get; set; }

        public float FQtyL { get; set; }
        public float FQtyR { get; set; }

        public float E1FFlow { get; set; }
        public float E1OilT { get; set; }
        public float E1OilP { get; set; }
        public float E1RPM { get; set; }

        public float E1CHT1 { get; set; }
        public float E1CHT2 { get; set; }
        public float E1CHT3 { get; set; }
        public float E1CHT4 { get; set; }

        public float E1EGT1 { get; set; }
        public float E1EGT2 { get; set; }
        public float E1EGT3 { get; set; }
        public float E1EGT4 { get; set; }

        public float OAT { get; set; }
        public float IAS { get; set; }
        public float VSpd { get; set; }
        public float NormAc { get; set; }
        public float AltMSL { get; set; }

        public int Timestep { get; set; }
        public int Cluster { get; set; }
        public int MasterIndex { get; set; }
        public int DateDiff { get; set; }
        public int FlightLength { get; set; }
        public int NumberFlightsBefore { get; set; }
    }
}
