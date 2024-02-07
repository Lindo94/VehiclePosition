using System.Collections.Generic;
using VehiclePosition;



class Program
{
    static void Main()
    {
        List<Position> testPosition = new List<Position>()
        {
           new Position () { PositionId = 1, Latitude = 34.544909, Longitude = -102.100843 },
           new Position () { PositionId = 2, Latitude = 32.345544, Longitude = -99.123124 },
           new Position () { PositionId = 3, Latitude = 33.234235, Longitude = -100.214124 },
           new Position () { PositionId = 4, Latitude = 35.195739, Longitude = -95.348899 },
           new Position () { PositionId = 5, Latitude = 31.895839, Longitude = -97.789573 },
           new Position () { PositionId = 6, Latitude = 32.895839, Longitude = -101.789573 },
           new Position () { PositionId = 7, Latitude = 34.115839, Longitude = -100.225732 },
           new Position () { PositionId = 8, Latitude = 32.335839, Longitude = -99.992232 },
           new Position () { PositionId = 9, Latitude = 33.535339, Longitude = -94.792232 },
           new Position () { PositionId = 10, Latitude = 32.234235, Longitude = -100.222222 },

        };

        Console.WriteLine("Start Time : " + DateTime.UtcNow.ToString());
        List<Vehicle> vehicleDataList = ReadBinaryDataFile("VehiclePositions.dat");
        
        if(vehicleDataList.Count > 0)
        {
            for (int x =0; x < testPosition.Count; x++)
            {
                FindNearestCoordinate(testPosition[x], vehicleDataList);
            }
        }
        else
            Console.WriteLine("No Vehicle List to search from");


        Console.WriteLine("End Time : " + DateTime.UtcNow.ToString());
    }

    public static void FindNearestCoordinate(Position position, List<Vehicle> vehicleData)
    {
        var nearest = vehicleData.MinBy(v => CalculateDistance(position.Latitude, position.Longitude, v.Latitude, v.Longitude));

        Console.WriteLine("Nearest Vehicle for Position #" + position.PositionId + " : " + nearest.VehicleRegistration);
    }
    public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double radius = 6371; 
        double x = (lon2 - lon1) * Math.Cos((lat1 + lat2) / 2);
        double y = lat2 - lat1;
        return Math.Sqrt(x * x + y * y) * radius;
    }

    static List<Vehicle> ReadBinaryDataFile(string filePath)
    {
        List<Vehicle> vehicleData = new List<Vehicle>();
        try
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (BinaryReader binaryReader = new BinaryReader(fileStream))
            {
                while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                {
                    Vehicle vehicle = new Vehicle
                    {
                        VehicleId = binaryReader.ReadInt32(),
                        VehicleRegistration = ReadNullTerminatedString(binaryReader),
                        Latitude = binaryReader.ReadSingle(),
                        Longitude = binaryReader.ReadSingle(),
                        RecordedTimeUTC = binaryReader.ReadUInt64()
                    };
                    vehicleData.Add(vehicle);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred : " + ex.Message);
        }
        return vehicleData;
    }

    static string ReadNullTerminatedString(BinaryReader binaryReader)
    {
        List<byte> bytes = new List<byte>();

        byte currentByte;
        while ((currentByte = binaryReader.ReadByte()) != 0)
        {
            bytes.Add(currentByte);
        }

        return System.Text.Encoding.ASCII.GetString(bytes.ToArray());
    }
}