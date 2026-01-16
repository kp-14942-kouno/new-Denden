using System.Text.RegularExpressions;

namespace DenDen.Common.Helpers;

/// <summary>
/// バリデーションヘルパー
/// 共通のバリデーションルールを提供
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// 必須入力チェック
    /// </summary>
    /// <param name="value">値</param>
    /// <returns>空でない場合true</returns>
    public static bool IsRequired(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// 最大文字数チェック
    /// </summary>
    /// <param name="value">値</param>
    /// <param name="maxLength">最大文字数</param>
    /// <returns>最大文字数以下の場合true</returns>
    public static bool MaxLength(string? value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return true;
        return value.Length <= maxLength;
    }

    /// <summary>
    /// 最小文字数チェック
    /// </summary>
    /// <param name="value">値</param>
    /// <param name="minLength">最小文字数</param>
    /// <returns>最小文字数以上の場合true</returns>
    public static bool MinLength(string? value, int minLength)
    {
        if (string.IsNullOrEmpty(value)) return false;
        return value.Length >= minLength;
    }

    /// <summary>
    /// 文字数範囲チェック
    /// </summary>
    /// <param name="value">値</param>
    /// <param name="minLength">最小文字数</param>
    /// <param name="maxLength">最大文字数</param>
    /// <returns>範囲内の場合true</returns>
    public static bool LengthRange(string? value, int minLength, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return minLength == 0;
        return value.Length >= minLength && value.Length <= maxLength;
    }

    /// <summary>
    /// 電話番号形式チェック（日本の電話番号）
    /// </summary>
    /// <param name="value">値</param>
    /// <returns>有効な形式の場合true</returns>
    public static bool IsPhoneNumber(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return true;

        // ハイフンあり・なし両対応
        var pattern = @"^(0\d{1,4}[-]?\d{1,4}[-]?\d{3,4})$";
        return Regex.IsMatch(value, pattern);
    }

    /// <summary>
    /// メールアドレス形式チェック
    /// </summary>
    /// <param name="value">値</param>
    /// <returns>有効な形式の場合true</returns>
    public static bool IsEmail(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return true;

        var pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(value, pattern);
    }

    /// <summary>
    /// 数値チェック（整数）
    /// </summary>
    /// <param name="value">値</param>
    /// <returns>整数の場合true</returns>
    public static bool IsInteger(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return true;
        return int.TryParse(value, out _);
    }

    /// <summary>
    /// 数値チェック（小数含む）
    /// </summary>
    /// <param name="value">値</param>
    /// <returns>数値の場合true</returns>
    public static bool IsNumeric(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return true;
        return decimal.TryParse(value, out _);
    }

    /// <summary>
    /// 日付形式チェック
    /// </summary>
    /// <param name="value">値</param>
    /// <returns>有効な日付の場合true</returns>
    public static bool IsDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return true;
        return DateTime.TryParse(value, out _);
    }

    /// <summary>
    /// 半角英数字チェック
    /// </summary>
    /// <param name="value">値</param>
    /// <returns>半角英数字のみの場合true</returns>
    public static bool IsAlphanumeric(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return true;
        return Regex.IsMatch(value, @"^[a-zA-Z0-9]+$");
    }

    /// <summary>
    /// 半角英数字と記号チェック（ログインID用）
    /// </summary>
    /// <param name="value">値</param>
    /// <returns>有効な形式の場合true</returns>
    public static bool IsValidLoginId(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        return Regex.IsMatch(value, @"^[a-zA-Z0-9_.-]+$");
    }

    /// <summary>
    /// パスワード強度チェック
    /// 8文字以上、英数字含む
    /// </summary>
    /// <param name="value">値</param>
    /// <returns>有効な強度の場合true</returns>
    public static bool IsStrongPassword(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        if (value.Length < 8) return false;

        var hasLetter = Regex.IsMatch(value, @"[a-zA-Z]");
        var hasDigit = Regex.IsMatch(value, @"\d");

        return hasLetter && hasDigit;
    }

    /// <summary>
    /// 範囲チェック（整数）
    /// </summary>
    /// <param name="value">値</param>
    /// <param name="min">最小値</param>
    /// <param name="max">最大値</param>
    /// <returns>範囲内の場合true</returns>
    public static bool IsInRange(int value, int min, int max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// 範囲チェック（日付）
    /// </summary>
    /// <param name="value">値</param>
    /// <param name="min">最小日付</param>
    /// <param name="max">最大日付</param>
    /// <returns>範囲内の場合true</returns>
    public static bool IsInRange(DateTime value, DateTime min, DateTime max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// バリデーション結果を保持するクラス
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// バリデーション成功
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// エラーメッセージ一覧
        /// </summary>
        public List<string> Errors { get; } = new();

        /// <summary>
        /// エラーを追加
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        public void AddError(string message)
        {
            Errors.Add(message);
            IsValid = false;
        }

        /// <summary>
        /// 全エラーメッセージを結合して取得
        /// </summary>
        /// <param name="separator">区切り文字</param>
        /// <returns>結合されたエラーメッセージ</returns>
        public string GetErrorMessage(string separator = "\n")
        {
            return string.Join(separator, Errors);
        }

        /// <summary>
        /// 成功結果を作成
        /// </summary>
        public static ValidationResult Success() => new() { IsValid = true };

        /// <summary>
        /// 失敗結果を作成
        /// </summary>
        /// <param name="errorMessage">エラーメッセージ</param>
        public static ValidationResult Fail(string errorMessage)
        {
            var result = new ValidationResult { IsValid = false };
            result.AddError(errorMessage);
            return result;
        }
    }
}
