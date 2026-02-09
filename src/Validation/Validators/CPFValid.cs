namespace GerenciadorFuncionarios.Validation.Validators;

using System;
using System.Text.RegularExpressions;
using System.Linq;

public class CPFValid
{
    private CPFValid() { }

    public static bool IsValid(string value)
    {
        string cpf = Regex.Replace(value, @"\D", "");

        if (!Regex.IsMatch(cpf, @"^\d{11}$"))
            return false;

        if (cpf.Distinct().Count() == 1)
            return false;

        int reminderFirstDigit = CalculateDigit(cpf, 9);
        int reminderSecondDigit = CalculateDigit(cpf, 10);

        return reminderFirstDigit == (cpf[9] - '0')
            && reminderSecondDigit == (cpf[10] - '0');
    }

    public static int CalculateDigit(string value, int size)
    {
        int sum = 0;
        int multiplier = size + 1;

        for (int i = 0; i < size; i++)
        {
            sum += (value[i] - '0') * multiplier;
            multiplier--;
        }

        int remainder = sum * 10 % 11;

        return remainder == 10 ? 0 : remainder;
    }
}
