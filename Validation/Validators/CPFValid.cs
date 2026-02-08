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

        var x = 10;
        var sumFirstDigit = 0;
        var sumSecondDigit = 0;

        for (var i = 0; i < 9; i++)
        {
            sumFirstDigit = sumFirstDigit + (cpf[i] - '0') * x;
            x--;
        }

        x = 11;
        for (var i = 0; i < 10; i++)
        {
            sumSecondDigit = sumSecondDigit + (cpf[i] - '0') * x;
            x--;
        }

        var reminderFirstDigit = sumFirstDigit * 10 % 11;
        if (reminderFirstDigit == 10)
            reminderFirstDigit = 0;

        var reminderSecondDigit = sumSecondDigit * 10 % 11;

        return reminderFirstDigit == (cpf[9] - '0')
            && reminderSecondDigit == (cpf[10] - '0');

    }
}
