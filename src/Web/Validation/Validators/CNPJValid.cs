namespace GerenciadorFuncionarios.Web.Validation.Validators;

using System;
using System.Text.RegularExpressions;
using System.Linq;

public class CNPJCalid
{

    private CNPJCalid() { }

    private readonly static int[] FIRST_WEIGHTS = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

    private readonly static int[] SECOND_WEIGHTS = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };


    public static bool IsValid(String value)
    {

        string digits = Regex.Replace(value, @"\D", "");

        if (!Regex.IsMatch(digits, @"^\d{14}$"))
            return false;

        if (digits.Distinct().Count() == 1)
            return false;

        int firstDigit = CalculateDigit(digits, FIRST_WEIGHTS);
        int secondDigit = CalculateDigit(digits, SECOND_WEIGHTS);

        return firstDigit == (digits[12] - '0')
                && secondDigit == (digits[13] - '0');
    }

    private static int CalculateDigit(string digits, int[] weights)
    {
        int sum = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            sum += (digits[i] - '0') * weights[i];
        }

        int remainder = sum % 11;

        return remainder < 2 ? 0 : 11 - remainder;
    }

}