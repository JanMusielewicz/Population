using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Population.Models
{
    public interface IPopulation
    {
        IQueryable<PopulationDto> GetSearchData(string start, string length, string sortColumn, string sortColumnDirection, string searchValue);
        PopulationDto GetPopulationRecord(int id);
        void SavePopulationRecord(PopulationDto populationDto);
        bool DeletePopulationRecord(int id);
    }

    public class PopulationModel : IPopulation
    {
        public IQueryable<PopulationDto> GetSearchData(string start,string length,string sortColumn,string sortColumnDirection,string searchValue)
        {
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            // Getting all population data
            var populationData = GetPopulationTBs();

            //Sorting
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                populationData = populationData.OrderBy(sortColumn + " " + sortColumnDirection);
            }
            //Search
            if (!string.IsNullOrEmpty(searchValue))
            {
                populationData = populationData.Where(m => m.Country.Contains(searchValue) || m.TotalFemale.ToString().Contains(searchValue)
                || m.TotalMale.ToString().Contains(searchValue) || m.Year.ToString().Contains(searchValue));
            }

            return populationData;
        }

        public PopulationDto GetPopulationRecord(int id)
        {
            var populationData = GetPopulationTBs();
            return populationData.Where(a => a.PopulationID == id).FirstOrDefault();

        }
        
        public bool DeletePopulationRecord(int id)
        {
            try
            {
                var lstPopulationDto = GetPopulationTBs().ToList();
                var obj = lstPopulationDto.FirstOrDefault(a => a.PopulationID == id);
                bool flag = lstPopulationDto.Remove(obj);
                SaveToCsv(lstPopulationDto);
                return flag;
            }
            catch(Exception ex)
            {
                return false;
            }

        }
        public void SavePopulationRecord(PopulationDto populationDto)
        {
            var lstPopulationDto = GetPopulationTBs().ToList();
            var obj = lstPopulationDto.FirstOrDefault(a => a.PopulationID == populationDto.PopulationID);
            obj.Country = populationDto.Country;
            obj.TotalFemale = populationDto.TotalFemale;
            obj.TotalMale = populationDto.TotalMale;
            obj.Year = populationDto.Year;
            SaveToCsv(lstPopulationDto);
        }
        public static PopulationDto FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');
            PopulationDto PopulationDto = new PopulationDto();
            int.TryParse(values[0], out int PopulationID);
            PopulationDto.PopulationID = PopulationID;
            PopulationDto.Country = values[1];
            int.TryParse(values[2], out int Year);
            PopulationDto.Year = Year;
            decimal.TryParse(values[3], out decimal TotalMale);
            PopulationDto.TotalMale = TotalMale;
            decimal.TryParse(values[4], out decimal TotalFemale);
            PopulationDto.TotalFemale = TotalFemale;
            PopulationDto.Summary = (TotalMale + TotalFemale) / 2;
            return PopulationDto;
        }
        private IQueryable<PopulationDto> GetPopulationTBs()
        {

            List<PopulationDto> values = System.IO.File.ReadAllLines("../Population/data/PopulationSampleDataSet.csv")
                                           .Skip(1)
                                           .Select(v => PopulationModel.FromCsv(v))
                                           .ToList();
            return values?.AsQueryable<PopulationDto>();
        }
        private void SaveToCsv<T>(List<T> reportData)
        {
            var lines = new List<string>();
            IEnumerable<PropertyDescriptor> props = TypeDescriptor.GetProperties(typeof(T)).OfType<PropertyDescriptor>();
            var header = string.Join(",", props.ToList().Select(x => x.Name));
            lines.Add(header);
            var valueLines = reportData.Select(row => string.Join(",", header.Split(',').Select(a => row.GetType().GetProperty(a).GetValue(row, null))));
            lines.AddRange(valueLines);
            System.IO.File.WriteAllLines("../Population/data/PopulationSampleDataSet.csv", lines.ToArray());
        }

       
    }
}
