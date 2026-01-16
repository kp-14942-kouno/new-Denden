using DenDen.Data.Repositories;
using DenDen.Models.DTOs;

namespace DenDen.Services;

/// <summary>
/// 認証サービスの実装
/// BCryptを使用したパスワードハッシュ化と認証処理を提供
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IOperatorRepository _operatorRepository;
    private readonly IProjectRepository _projectRepository;

    public AuthenticationService(
        IOperatorRepository operatorRepository,
        IProjectRepository projectRepository)
    {
        _operatorRepository = operatorRepository;
        _projectRepository = projectRepository;
    }

    /// <inheritdoc/>
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var response = new LoginResponse();

        // 入力値検証
        if (request.ProjectId <= 0)
        {
            response.IsSuccess = false;
            response.ErrorMessage = "案件を選択してください。";
            return response;
        }

        if (string.IsNullOrWhiteSpace(request.LoginId))
        {
            response.IsSuccess = false;
            response.ErrorMessage = "ログインIDを入力してください。";
            return response;
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            response.IsSuccess = false;
            response.ErrorMessage = "パスワードを入力してください。";
            return response;
        }

        // 案件の取得と検証
        var project = await _projectRepository.GetByIdAsync(request.ProjectId);
        if (project == null)
        {
            response.IsSuccess = false;
            response.ErrorMessage = "選択された案件が見つかりません。";
            return response;
        }

        if (!project.IsActive)
        {
            response.IsSuccess = false;
            response.ErrorMessage = "選択された案件は現在無効です。";
            return response;
        }

        // オペレーターの取得と検証
        var operatorMaster = await _operatorRepository.GetByLoginIdAsync(request.ProjectId, request.LoginId);
        if (operatorMaster == null)
        {
            response.IsSuccess = false;
            response.ErrorMessage = "ログインIDまたはパスワードが正しくありません。";
            return response;
        }

        if (!operatorMaster.IsActive)
        {
            response.IsSuccess = false;
            response.ErrorMessage = "このアカウントは無効化されています。";
            return response;
        }

        // パスワード検証
        if (!VerifyPassword(request.Password, operatorMaster.PasswordHash))
        {
            response.IsSuccess = false;
            response.ErrorMessage = "ログインIDまたはパスワードが正しくありません。";
            return response;
        }

        // 認証成功
        response.IsSuccess = true;
        response.Operator = operatorMaster;
        response.Project = project;

        return response;
    }

    /// <inheritdoc/>
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    /// <inheritdoc/>
    public bool VerifyPassword(string password, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            return false;
        }
    }
}
