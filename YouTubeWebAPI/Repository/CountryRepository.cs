using YouTubeWebAPI.Data;
using YouTubeWebAPI.Interface;
using YouTubeWebAPI.Models;

namespace YouTubeWebAPI.Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DataContext _contex;
        public CountryRepository(DataContext context)
        {
            _contex = context;
        }
        public bool CountryExsits(int id)
        {
            return _contex.Countries.Any(c => c.Id == id);
        }

        public bool CreateCountry(Country country)
        {
            _contex.Countries.Add(country);
            return Save();
        }

        public ICollection<Country> GetCountries()
        {
            var countryList = _contex.Countries.ToList();
            return countryList;
        }

        public Country GetCountry(int id)
        {
            return _contex.Countries.Where(c => c.Id == id).FirstOrDefault();
        }

        public Country GetCountry(string name)
        {
            return _contex.Countries.Where(c => c.Name == name).FirstOrDefault();
        }

        public Country GetCountryByOwner(int ownerId)
        {
            var countries = _contex.Owners.Where(o => o.Id == ownerId).Select(c => c.Country).FirstOrDefault();
            return countries;
        }

        public ICollection<Owner> GetOwnersFromACountry(int countryId)
        {
            return _contex.Owners.Where(o => o.Country.Id == countryId).ToList();
        }

        public bool Save()
        {
            try
            {
                var save = _contex.SaveChanges();
                return save > 0 ? true : false;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        public bool UpdateCountry(Country country)
        {
            _contex.Update(country);
            return Save();
        }
    }
}
