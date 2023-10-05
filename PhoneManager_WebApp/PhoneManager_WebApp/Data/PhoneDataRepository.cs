using Newtonsoft.Json;
using PhoneManager_WebApp.Models;
using System.Collections;
using System.Text;
using TelephoneManager.Models;

namespace PhoneManager_WebApp.Data
{
    public class PhoneDataRepository : IPhoneDataRepository
    {
        //Separeted collections for each data
        private readonly List<Country> countryList = new List<Country>();
        private readonly List<Area> areaList = new List<Area>();
        private readonly List<InputData> inputList = new List<InputData>();
        private readonly List<OutputData> outputList = new List<OutputData>();
        private readonly PhoneDataViewBag pdvb = new PhoneDataViewBag();

        public PhoneDataRepository()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);//Nuget package to handle ANSI

            //Processing txt data to objects
            TransformFromTxtToObjectList<Country>(countryList, ReadFromFile("country.txt"));
            TransformFromTxtToObjectList<Area>(areaList, ReadFromFile("area.txt"));
            TransformFromTxtToObjectList<InputData>(inputList, ReadFromFile("input.txt"));
            TransformFromTxtToObjectList<OutputData>(outputList, ReadFromFile("output.txt"));

            //Processing the callings by checking destination phonenumber
            for (int i = 0; i < inputList.Count(); i++)
            {
                Area area = AreaFromDestPhoneNumber(inputList[i].DestinationPhoneNumber);

                if (area != null)
                {
                    //We check if given output data already exists using country and area code. If they are, we just increase the calling sec value.
                    if (outputList.Any(o => o.CountryCallingCode == area.CountryCallingCode && o.AreaCallingCode == area.AreaCallingCode))
                    {
                        var existingOutputData = outputList.Where(o => o.CountryCallingCode == area.CountryCallingCode && o.AreaCallingCode == area.AreaCallingCode).Single();
                        existingOutputData.SumCallingTime += inputList[i].CallingTime;
                    }
                    else
                    {
                        //If output data does not exists, we create one.
                        outputList.Add(new OutputData()
                        {
                            CountryName = countryList.Where(c => c.CountryCallingCode == area.CountryCallingCode).Select(c => c.Name).SingleOrDefault(),
                            AreaName = area.AreaName,
                            CountryCallingCode = area.CountryCallingCode,
                            AreaCallingCode = area.AreaCallingCode,
                            SumCallingTime = inputList[i].CallingTime
                        });
                    }
                }
            }

            pdvb.InputDataList = inputList;
            pdvb.OutputDataList = outputList;
            pdvb.AreaList = areaList;
            pdvb.CountryList = countryList;
            pdvb.OutPutDataJSON = JsonConvert.SerializeObject(outputList.Select(prop => new
            {
                label = prop.AreaName,
                legendText = prop.CountryName + "-" + prop.AreaName + "-(" + prop.CountryCallingCode + "-" + prop.AreaCallingCode+")-"+prop.SumCallingTime+" sec",
                y = prop.SumCallingTime
            }).ToList());


        }

        public PhoneDataViewBag ReadAllPhoneData()
        {
            return pdvb;
        }

        private Area AreaFromDestPhoneNumber(string pn)
        {
            string nc = "";
            for (int i = 1; i <= 2; i++)//First, we check if country code exists. 
            {
                nc += pn[i].ToString();//There are diferent lenght country codes. So we get the first character(number), then the first two, if not found with one.
                                       //+countrycode areacode
                var areasByCountryCallingCode = areaList.Where(a => a.CountryCallingCode == int.Parse(nc)).ToList();//Checking if there are areas with given country code

                if (areasByCountryCallingCode.Count() > 1)//If there are more than two areas by country code, use area code to find that particular area.
                {
                    //There are different lenght country codes, and area codes. We use "i" to know where the area code starts, and the possible lenght of area codes to compare data to each other.
                    return areasByCountryCallingCode.Where(a => a.AreaCallingCode == int.Parse(pn.Substring(i + 1, a.AreaCallingCode.ToString().Length))).SingleOrDefault();
                }
                else if (areasByCountryCallingCode.Count() == 1)//If we found one area, return with the Area object for further reference options
                {
                    return areasByCountryCallingCode[0];

                }
            }
            return null;
        }
        private List<string> ReadFromFile(string filename)
        {
            return File.ReadAllLines(Path.Combine("DataSource", filename), Encoding.GetEncoding(1250)).ToList();
        }

        private void TransformFromTxtToObjectList<T>(List<T> glist, List<string> list)
        {
            Type type = typeof(T);
            foreach (var row in list)
            {
                T instance = (T)Activator.CreateInstance(type);
                for (int i = 0; i < type.GetProperties().Length; i++)
                {
                    Type pt = type.GetProperties()[i].PropertyType;

                    if (pt == typeof(int))
                    {
                        type.GetProperties()[i].SetValue(instance, int.Parse(row.Split('\t')[i]));
                    }

                    if (pt == typeof(string))
                    {
                        type.GetProperties()[i].SetValue(instance, row.Split('\t')[i]);
                    }

                    if (pt == typeof(DateOnly))
                    {
                        DateOnly dato = new DateOnly(int.Parse(row.Split('\t')[i].Split('-')[0]), int.Parse(row.Split('\t')[i].Split('-')[1]), int.Parse(row.Split('\t')[i].Split('-')[2]));
                        type.GetProperties()[i].SetValue(instance, dato);
                    }

                    if (pt == typeof(TimeOnly))
                    {
                        TimeOnly tonly = new TimeOnly(int.Parse(row.Split('\t')[i].Split(':')[0]), int.Parse(row.Split('\t')[i].Split(':')[1]));
                        type.GetProperties()[i].SetValue(instance, tonly);
                    }
                }
                glist.Add(instance);
            }
        }

    }

}
