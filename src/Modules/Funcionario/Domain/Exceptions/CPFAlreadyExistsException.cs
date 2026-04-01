namespace GerenciadorFuncionarios.Modules.Funcionario.Domain.Exceptions;

using System;

public class CPFAlreadyExistsException : Exception
{
	public CPFAlreadyExistsException(string message) 
		: base(message) { }
}
