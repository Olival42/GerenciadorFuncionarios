namespace GerenciadorFuncionarios.Modules.Funcionario.Domain.Exceptions;

using System;

public class UserNameAlreadyExistsException : Exception
{
	public UserNameAlreadyExistsException(string message) 
		: base(message) { }
}
