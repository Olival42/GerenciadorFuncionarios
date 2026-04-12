using System.Text.RegularExpressions;
using GerenciadorFuncionarios.Web.Validation.Validators;

namespace GerenciadorFuncionarios.Domain.ValueObjects;

public sealed class Cpf
{
    public string Value { get; }

    public Cpf(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("CPF não pode ser vazio.");

        value = Normalize(value);

        if (!IsValid(value))
            throw new ArgumentException("CPF inválido.");

        Value = value;
    }

    public static implicit operator string(Cpf cpf) => cpf.Value;

    private static string Normalize(string input)
        => Regex.Replace(input, @"\D", "");

    private static bool IsValid(string cpf)
    {
        return CPFValid.IsValid(cpf);
    }
}