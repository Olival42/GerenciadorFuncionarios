namespace GerenciadorFuncionarios.Modules.Funcionario.Domain.Exceptions;

using System;

public class EmailAlreadyExistsException : Exception
{
	public EmailAlreadyExistsException(string message) 
		: base(message) { }
}
