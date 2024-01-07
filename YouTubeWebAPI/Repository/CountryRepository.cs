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
    }
}
