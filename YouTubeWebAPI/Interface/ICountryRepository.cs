using YouTubeWebAPI.Models;

namespace YouTubeWebAPI.Interface
{
    public interface ICountryRepository
    {
        ICollection<Country> GetCountries();
        Country GetCountry(int id);
        Country GetCountry(string name);
        Country GetCountryByOwner(int  ownerId);
        ICollection<Owner> GetOwnersFromACountry(int countryId);
        bool CountryExsits(int id);
        bool CreateCountry(Country country);
        bool UpdateCountry(Country country);
        bool Save();
    }
}
