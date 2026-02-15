namespace ECommerce.Domain.Constants;

public static class ShippingAddressConstants
{
    public const int FullNameMaxLength = 200;
    public const int AddressLineMaxLength = 500;
    public const int CityMaxLength = 100;
    public const int StateMaxLength = 100;
    public const int PostalCodeMaxLength = 20;
    public const int CountryMaxLength = 100;
    public const int PhoneNumberMaxLength = 20;

    public const string FullNameRequired = "Full name is required";
    public const string FullNameLengthInvalid = "Full name must not exceed 200 characters";
    public const string AddressLine1Required = "Address line 1 is required";
    public const string AddressLineLengthInvalid = "Address line must not exceed 500 characters";
    public const string CityRequired = "City is required";
    public const string CityLengthInvalid = "City must not exceed 100 characters";
    public const string StateRequired = "State is required";
    public const string StateLengthInvalid = "State must not exceed 100 characters";
    public const string PostalCodeRequired = "Postal code is required";
    public const string PostalCodeLengthInvalid = "Postal code must not exceed 20 characters";
    public const string CountryRequired = "Country is required";
    public const string CountryLengthInvalid = "Country must not exceed 100 characters";
    public const string PhoneNumberLengthInvalid = "Phone number must not exceed 20 characters";
    public const string AddressNotFound = "Shipping address not found";
    public const string UnauthorizedAccess = "You are not authorized to access this address";
}
