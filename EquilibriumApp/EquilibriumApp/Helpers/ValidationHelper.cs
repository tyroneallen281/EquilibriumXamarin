using EquilibriumApp.Mobile.Helpers;
using EquilibriumApp.Mobile.Models.AppModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EquilibriumApp.Helpers
{
    public static class ValidationHelper
    {

        public static bool ValidatePassword(string password)
        {
            const int MIN_LENGTH = 6;

            if (password == null) throw new ArgumentNullException();

            bool meetsLengthRequirements = password.Length >= MIN_LENGTH;
            bool hasUpperCaseLetter = false;
            bool hasLowerCaseLetter = false;
            bool hasDecimalDigit = false;

            if (meetsLengthRequirements)
            {
                foreach (char c in password)
                {
                    if (char.IsUpper(c)) hasUpperCaseLetter = true;
                    else if (char.IsLower(c)) hasLowerCaseLetter = true;
                    else if (char.IsDigit(c)) hasDecimalDigit = true;
                }
            }

            bool isValid = meetsLengthRequirements
                        && hasUpperCaseLetter
                        && hasLowerCaseLetter
                        && hasDecimalDigit;
            return isValid;

        }

        public static bool ValidateEmail(string email)
        {
            return Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }

        public static bool ValidatePhoneNumber(string number)
        {
            if (number.Length == 9 && number[0] != '0')
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool ValidateLink(string link)
        {

            return Uri.IsWellFormedUriString(link, UriKind.Absolute);
        }
    }
}
