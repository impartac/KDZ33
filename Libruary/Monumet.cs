using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Library
{
    /// <summary>
    ///  Основной класс базы данных.
    /// </summary>
    public class Monument
    {
        [JsonRequired]
        [JsonPropertyName("ID")]
        public int? Id { get; set; }

        [JsonRequired]
        [JsonPropertyName("SculpName")]
        public string? SculpName { get; set; }

        [JsonRequired]
        [JsonPropertyName("Photo")]
        public string? Photo { get; set; }

        [JsonRequired]
        [JsonPropertyName("Author")]

        public string? Author { get; set; }

        [JsonRequired]
        [JsonPropertyName("ManufactYear")]

        public int? ManufactYear { get; set; }

        [JsonRequired]
        [JsonPropertyName("Material")]

        public string? Material { get; set; }

        [JsonRequired]
        [JsonPropertyName("Description")]

        public string? Description { get; set; }

        [JsonRequired]
        [JsonPropertyName("LocationPlace")]
        
        public string? LocationPlace { get; set; }

        [JsonRequired]
        [JsonPropertyName("Longitude_WGS84")]

        public double? LongitudeWGS84 { get; set; }

        [JsonRequired]
        [JsonPropertyName("Latitude_WGS84")]

        public double? LatitudeWGS84 { get; set; }

        [JsonRequired]
        [JsonPropertyName("global_id")]

        public long? GlobalId { get; set; }

        [JsonRequired]
        [JsonPropertyName("geodata_center")]

        public string? GeodataCenter { get; set; }

        [JsonRequired]
        [JsonPropertyName("geoarea")]

        public string? GeoArea { get; set; }

        public Monument(string[] fields) 
        {
            Utils utils = new Utils();
            for (int i = 0; i < fields.Length; i++) 
            {
                fields[i] = fields[i].Trim('"');
                fields[i] = fields[i].Replace('.', ',');
            }
            try
            {
                Id = utils.Convert(fields[0], "int");
                SculpName = utils.Convert(fields[1], "string");
                Photo = utils.Convert(fields[2], "string");
                Author = utils.Convert(fields[3], "string");
                ManufactYear = utils.Convert(fields[4], "int");
                Material = utils.Convert(fields[5], "string");
                Description = utils.Convert(fields[6], "string");
                LocationPlace = utils.Convert(fields[7], "string");
                LongitudeWGS84 = utils.Convert(fields[8], "double");
                LatitudeWGS84 = utils.Convert(fields[9], "double");
                GlobalId = utils.Convert(fields[10], "long");
                GeodataCenter = utils.Convert(fields[11], "string");
                GeoArea = utils.Convert(fields[12], "string");
            }
            catch (Exception ex)
            {
                throw new TypeAccessException();
            }
        }
        public Monument(int? id, 
                        string? sculpName, 
                        string? photo, 
                        string? author, 
                        int? manufactYear, 
                        string? material, 
                        string? description, 
                        string? locationPlace, 
                        double? longitude_WGS84, 
                        double? latitude_WGS84, 
                        int? globalId, 
                        string? geodata_center, 
                        string? geoarea)
        {
            Id = id;
            SculpName = sculpName;
            Photo = photo;
            Author = author;
            ManufactYear = manufactYear;
            Material = material;
            Description = description;
            LocationPlace = locationPlace;
            LongitudeWGS84 = longitude_WGS84;
            LatitudeWGS84 = latitude_WGS84;
            GlobalId = globalId;
            GeodataCenter = geodata_center;
            GeoArea = geoarea;
        }

        public Monument() { }

        /// <summary>
        /// Вывод экземпляра в формате csv.
        /// </summary>
        /// <returns></returns>
        public string ToCsv()
        {
            string temp = "";
            foreach (var v in GetType().GetFields(BindingFlags.NonPublic|BindingFlags.Instance)) 
            {
                if (v.GetValue(this) is null)
                {
                    temp += "\"\"" + ';';
                }
                else
                {
                    temp += '"' + v.GetValue(this).ToString() + '"' + ';';
                }
            }
            return temp;
        }
        public override string ToString()
        {
            string temp = "";
            foreach (var v in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (v.GetValue(this) is null)
                {
                    temp += v.Name+":"+ "null" +'\n';
                }
                else
                {
                    temp += v.Name +":"+ v.GetValue(this).ToString() + '\n';
                }
            }
            return temp;
        }
    }
}
/*
░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
░░░░ЗАПУСКАЕМ░ГУСЕЙ-РАЗВЕДЧИКОВ░░░░
░░░░░▄▀▀▀▄░░░▄▀▀▀▀▄░░░▄▀▀▀▄░░░░░
▄███▀░◐░░░▌░▐0░░░░0▌░▐░░░◐░▀███▄
░░░░▌░░░░░▐░▌░▐▀▀▌░▐░▌░░░░░▐░░░░
░░░░▐░░░░░▐░▌░▌▒▒▐░▐░▌░░░░░▌░░░░
*/