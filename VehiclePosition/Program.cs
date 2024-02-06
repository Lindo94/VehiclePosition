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

        foreach (var position in testPosition)
        {
            FindNearestCoordinate(position, vehicleDataList);
        }

        Console.WriteLine("End Time : " + DateTime.UtcNow.ToString());
    }
    public static void FindNearestCoordinate(Position position, List<Vehicle> vehicleData)
    {
        Vehicle nearest = vehicleData[0];
        double minDistance = CalculateDistance(position.Latitude, position.Longitude, nearest.Latitude,nearest.Longitude);

        foreach (var coordinate in vehicleData)
        {
            double distance = CalculateDistance(position.Latitude, position.Longitude, coordinate.Latitude, coordinate.Longitude);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = coordinate;
            }
        }

        Console.WriteLine("Nearest Vehicle for Position #"+ position.PositionId + " : "  + nearest.VehicleRegistration);
    }
    public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        // Convert latitude and longitude from degrees to radians
        double lat1Rad = Math.PI * lat1 / 180;
        double lon1Rad = Math.PI * lon1 / 180;
        double lat2Rad = Math.PI * lat2 / 180;
        double lon2Rad = Math.PI * lon2 / 180;

        // Calculate differences
        double deltaLat = lat2Rad - lat1Rad;
        double deltaLon = lon2Rad - lon1Rad;

        // Haversine formula
        double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // Calculate distance using Radius of the Earth in kilometers(6371)
        double distance = 6371 * c;
        return distance;
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