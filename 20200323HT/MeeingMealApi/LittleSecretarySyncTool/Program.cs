using MeetingMealApiClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleSecretarySyncTool
{
    public class Program
    {
        static void Main(string[] args)
        {
            var now = DateTime.Now;

            var channel = OpenApiChannelFactory.GetChannel();
            var connString = System.Configuration.ConfigurationManager.ConnectionStrings["db1"].ConnectionString;
            var citys = channel.syncCity();
            var hospitals = channel.syncHospital("0");

            var rel = citys.result;
            using (var connection = new SqlConnection(connString))
            {
                connection.Open();
                var tran = connection.BeginTransaction();

                string sqlDelProvince = "delete P_PROVINCE";
                SqlCommand comDelProvince = new SqlCommand(sqlDelProvince, connection);
                comDelProvince.Transaction = tran;
                comDelProvince.ExecuteNonQuery();
                string sqlDelCity = "delete P_CITY";
                SqlCommand comDelCity = new SqlCommand(sqlDelCity, connection);
                comDelCity.Transaction = tran;
                comDelCity.ExecuteNonQuery();
                string sqlDelHospital = "delete P_HOSPITAL";
                SqlCommand comDelHospital = new SqlCommand(sqlDelHospital, connection);
                comDelHospital.Transaction = tran;
                comDelHospital.ExecuteNonQuery();

                foreach (var province in rel)
                {
                    string sqlProvince = "INSERT INTO P_PROVINCE VALUES(@ID,@Name,@PinYin,@CreateDate)";
                    SqlCommand commandProvince = new SqlCommand(sqlProvince, connection);
                    commandProvince.Transaction = tran;
                    commandProvince.Parameters.AddRange(new SqlParameter[]
                        {
                            new SqlParameter("@ID", province.provinceId),
                            new SqlParameter("@Name", province.provinceName),
                            new SqlParameter("@PinYin", province.pinYin),
                            new SqlParameter("@CreateDate", now)
                        });
                    commandProvince.ExecuteNonQuery();

                    foreach (var city in province.citys)
                    {
                        string sqlCity = "INSERT INTO P_CITY VALUES(@ID,@ProvinceId,@Name,@PinYin,@CreateDate)";
                        SqlCommand commandCity = new SqlCommand(sqlCity, connection);
                        commandCity.Transaction = tran;
                        commandCity.Parameters.AddRange(new SqlParameter[]
                        {
                            new SqlParameter("@ID", city.cityId),
                            new SqlParameter("@ProvinceId", province.provinceId),
                            new SqlParameter("@Name", city.cityName),
                            new SqlParameter("@PinYin", city.pinYin),
                            new SqlParameter("@CreateDate", now)
                        });
                        commandCity.ExecuteNonQuery();
                    }
                    
                }


                foreach (var hospital in hospitals.result)
                {
                    string sqlHospital = "INSERT INTO P_HOSPITAL(ID,CityId,GskHospital,Name,FirstLetters,Address,Latitude,Longitude,Type,[External],CreateDate) VALUES(@ID,@CityId,@GskHospital,@Name,@FirstLetters,@Address,@Latitude,@Longitude,@Type,@External,@CreateDate)";
                    SqlCommand commandHospital = new SqlCommand(sqlHospital, connection);
                    commandHospital.Transaction = tran;
                    var gskHospital = string.IsNullOrEmpty(hospital.gskHospital) ? "" : hospital.gskHospital;
                    var type = string.IsNullOrEmpty(hospital.type) ? "" : hospital.type;
                    commandHospital.Parameters.AddRange(new SqlParameter[]
                       {
                                   new SqlParameter("@ID", hospital.hospitalId),
                                   new SqlParameter("@CityId", hospital.cityId),
                                   new SqlParameter("@GskHospital", gskHospital),
                                   new SqlParameter("@Name", hospital.hospitalName),
                                   new SqlParameter("@FirstLetters", hospital.firstLetters),
                                   new SqlParameter("@Address", hospital.address),
                                   new SqlParameter("@Latitude", hospital.latitude),
                                   new SqlParameter("@Longitude", hospital.longitude),
                                   new SqlParameter("@Type", type),
                                   new SqlParameter("@External", hospital.External),
                                   new SqlParameter("@CreateDate", now)
                       });
                    commandHospital.ExecuteNonQuery();
                }

                tran.Commit();
            }
        }
    }
}
