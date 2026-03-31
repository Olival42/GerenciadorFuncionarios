namespace GerenciadorFuncionarios.Services;

using System;
using System.Threading.Tasks;
using GerenciadorFuncionarios.DTOs.Funcionario.Requests;
using GerenciadorFuncionarios.DTOs.Funcionario.Responses;
using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.Shared.Responses;
using Mapster;
using GerenciadorFuncionarios.Exceptions;
using BCrypt.Net;
using GerenciadorFuncionarios.Adapters;

public class FuncionarioService : IFuncionarioService
{
    private readonly IFuncionarioRepository _funcRepository;

    private readonly IDepartamentoRepository _deparRepository;

    private readonly ILogger<FuncionarioService> _logger;

    public FuncionarioService(IFuncionarioRepository funcRepository, IDepartamentoRepository deparRepository, ILogger<FuncionarioService> logger)
    {
        _funcRepository = funcRepository;
        _deparRepository = deparRepository;
        _logger = logger;
    }

    public async Task<ApiResponse<ResponseFuncionarioDTO>> RegistrarFuncionarioAsync(RegisterFuncionarioDTO data)
    {
        _logger.LogInformation(
            "Tentativa de cadastro de funcionário. Email: {Email}",
            data.Email);

        if (await _funcRepository.AnyByCPFAsync(data.CPF))
        {
            _logger.LogWarning(
                "CPF já cadastrado. CPF: {CPF}",
                data.CPF);

            throw new CPFAlreadyExistsException("CPF já cadastrado para outro funcionário.");
        }

        if (await _funcRepository.AnyByEmailAsync(data.Email))
        {
            _logger.LogWarning(
                "Email já cadastrado. Email: {Email}",
                data.Email);

            throw new EmailAlreadyExistsException("Email já cadastrado para outro funcionário.");
        }

        var departamento = await _deparRepository.GetById(data.DepartamentoId);

        if (departamento == null)
        {
            _logger.LogWarning(
                "Departamento não encontrado. Id: {Id}",
                data.DepartamentoId);

            throw new EntityNotFoundException("Departamento não encontrado.");
        }

        var func = data.Adapt<Funcionario>();

        func.PasswordHash = BCrypt.HashPassword(data.Password);
        func.DepartamentoId = departamento.Id;

        await _funcRepository.Add(func);

        var dto = func.Adapt<ResponseFuncionarioDTO>();

        _logger.LogInformation(
            "Funcionário cadastrado. Id: {Id} Email: {Email}",
            func.Id,
            func.Email);

        return ApiResponse<ResponseFuncionarioDTO>.Ok(dto);
    }

    public async Task<ApiResponse<ResponseFuncionarioDTO>> ObterFuncionarioPorId(Guid id)
    {
        var func = await _funcRepository.GetByIdAsync(id);

        if (func == null)
        {
            _logger.LogWarning(
                "Funcionário não encontrado. Id: {Id}",
                id);

            throw new EntityNotFoundException("Funcionário não encontrado.");
        }

        return ApiResponse<ResponseFuncionarioDTO>.Ok(func.Adapt<ResponseFuncionarioDTO>());
    }

    public async Task InativarPorId(Guid id)
    {
        _logger.LogInformation(
            "Tentativa de inativação de funcionário. Id: {Id}",
            id);

        var func = await _funcRepository.GetByIdAsync(id);

        if (func == null)
        {
            _logger.LogWarning(
                "Funcionário não encontrado. Id: {Id}",
                id);

            throw new EntityNotFoundException("Funcionário não encontrado.");
        }

        func.IsActive = false;

        await _funcRepository.SaveChangesAsync();

        _logger.LogInformation(
            "Funcionário inativado. Id: {Id} Email: {Email}",
            func.Id,
            func.Email);
    }

    public async Task<ApiResponse<ResponseFuncionarioDTO>> Atualizar(Guid id, UpdateFuncionarioDTO data)
    {
        _logger.LogInformation(
            "Tentativa de atualização de funcionário. Id: {Id}",
            id);

        var func = await _funcRepository.GetByIdAsync(id);

        if (func == null)
        {
            _logger.LogWarning(
                "Funcionário não encontrado. Id: {Id}",
                id);

            throw new EntityNotFoundException("Funcionário não encontrado.");
        }

        if (data.Name is not null)
            func.Name = data.Name;

        if (data.Email is not null)
            func.Email = data.Email;

        if (data.Password is not null)
            func.PasswordHash = BCrypt.HashPassword(data.Password);

        if (data.Role is not null)
            func.Role = data.Role.Value;

        if (data.CPF is not null)
            func.CPF = data.CPF;

        if (data.Phone is not null)
            func.Phone = data.Phone;

        await _funcRepository.SaveChangesAsync();

        var dto = func.Adapt<ResponseFuncionarioDTO>();

        _logger.LogInformation(
            "Funcionário atualizado. Id: {Id} Email: {Email}",
            func.Id,
            func.Email);

        return ApiResponse<ResponseFuncionarioDTO>.Ok(dto);
    }

    public async Task<ApiResponse<ResponseFuncionarioDTO>> AtualizarDepartamento(Guid id, UpdateDepartamentoFuncionario data)
    {
        _logger.LogInformation(
            "Tentativa de atualização de departamento do funcionário. Id: {Id}",
            id);

        var func = await _funcRepository.GetByIdAsync(id);

        if (func == null)
        {
            _logger.LogWarning(
                "Funcionário não encontrado. Id: {Id}",
                id);

            throw new EntityNotFoundException("Funcionário não encontrado.");
        }

        var departamento = await _deparRepository.GetById(data.DepartamentoId);

        if (departamento == null)
        {
            _logger.LogWarning(
                "Departamento não encontrado. Id: {Id}",
                data.DepartamentoId);

            throw new EntityNotFoundException("Departamento não encontrado.");
        }

        func.DepartamentoId = departamento.Id;

        await _funcRepository.SaveChangesAsync();

        var dto = func.Adapt<ResponseFuncionarioDTO>();

        _logger.LogInformation(
            "Departamento do funcionário atualizado. Id: {Id} Email: {Email}",
            func.Id,
            func.Email);

        return ApiResponse<ResponseFuncionarioDTO>.Ok(dto);
    }

    public async Task<ApiResponse<PaginationResponse<ResponseFuncionarioDTO>>> ObterTodosFuncionarios(int page, int pageSize, Guid? departamentoId)
    {
        var paginated = await _funcRepository.GetAllAsync(page, pageSize, departamentoId);

        return ApiResponse<PaginationResponse<ResponseFuncionarioDTO>>.Ok(paginated);
    }
}
