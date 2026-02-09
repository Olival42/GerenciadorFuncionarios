namespace GerenciadorFuncionarios.Validation.Attributes;

using System;
using System.ComponentModel.DataAnnotations;
using GerenciadorFuncionarios.Validation.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class CPFValidAttribute : ValidationAttribute
{

	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		if(value is string str)
		{
			if (!CPFValid.IsValid(str))
				return new ValidationResult("CPF é inválido.");

			return ValidationResult.Success;
        }
        return new ValidationResult("Tipo inválido para validação.");
    }
}
