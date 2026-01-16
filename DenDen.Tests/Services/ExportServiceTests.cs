using System.IO;
using System.Text;
using DenDen.Models.Entities;
using DenDen.Services;
using FluentAssertions;
using Xunit;

namespace DenDen.Tests.Services;

/// <summary>
/// ExportServiceのテスト
/// </summary>
public class ExportServiceTests : IDisposable
{
    private readonly ExportService _sut;
    private readonly string _tempDirectory;

    public ExportServiceTests()
    {
        _sut = new ExportService();
        _tempDirectory = Path.Combine(Path.GetTempPath(), "DenDenTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDirectory);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, true);
        }
    }

    [Fact]
    public async Task ExportInquiryHistoriesToCsvAsync_EmptyList_ShouldCreateFileWithHeaderOnly()
    {
        // Arrange
        var filePath = Path.Combine(_tempDirectory, "test_empty.csv");
        var emptyList = new List<InquiryHistory>();

        // Act
        await _sut.ExportInquiryHistoriesToCsvAsync(emptyList, filePath);

        // Assert
        File.Exists(filePath).Should().BeTrue();
        var content = await File.ReadAllTextAsync(filePath, new UTF8Encoding(true));
        content.Should().Contain("問合せID");
        content.Should().Contain("案件ID");
        content.Should().Contain("顧客Key");
    }

    [Fact]
    public async Task ExportInquiryHistoriesToCsvAsync_WithData_ShouldCreateCorrectCsv()
    {
        // Arrange
        var filePath = Path.Combine(_tempDirectory, "test_data.csv");
        var inquiries = new List<InquiryHistory>
        {
            new InquiryHistory
            {
                InquiryID = 1,
                ProjectID = 1,
                CustomerKey = "CUST001",
                OperatorID = 1,
                CategoryID = 1,
                StatusID = 1,
                InquiryContent = "テスト問合せ",
                ResponseContent = "テスト対応",
                FirstReceivedDateTime = new DateTime(2025, 1, 15, 10, 30, 0),
                CreatedAt = new DateTime(2025, 1, 15, 10, 30, 0)
            }
        };

        // Act
        await _sut.ExportInquiryHistoriesToCsvAsync(inquiries, filePath);

        // Assert
        var content = await File.ReadAllTextAsync(filePath, new UTF8Encoding(true));
        content.Should().Contain("1"); // InquiryID
        content.Should().Contain("CUST001"); // CustomerKey
        content.Should().Contain("テスト問合せ");
        content.Should().Contain("テスト対応");
        content.Should().Contain("2025/01/15 10:30:00");
    }

    [Fact]
    public async Task ExportInquiryHistoriesToCsvAsync_WithCommaInField_ShouldEscapeCorrectly()
    {
        // Arrange
        var filePath = Path.Combine(_tempDirectory, "test_comma.csv");
        var inquiries = new List<InquiryHistory>
        {
            new InquiryHistory
            {
                InquiryID = 1,
                ProjectID = 1,
                OperatorID = 1,
                InquiryContent = "問合せ,カンマあり",
                FirstReceivedDateTime = DateTime.Now,
                CreatedAt = DateTime.Now
            }
        };

        // Act
        await _sut.ExportInquiryHistoriesToCsvAsync(inquiries, filePath);

        // Assert
        var content = await File.ReadAllTextAsync(filePath, new UTF8Encoding(true));
        content.Should().Contain("\"問合せ,カンマあり\"");
    }

    [Fact]
    public async Task ExportInquiryHistoriesToCsvAsync_WithQuoteInField_ShouldEscapeCorrectly()
    {
        // Arrange
        var filePath = Path.Combine(_tempDirectory, "test_quote.csv");
        var inquiries = new List<InquiryHistory>
        {
            new InquiryHistory
            {
                InquiryID = 1,
                ProjectID = 1,
                OperatorID = 1,
                InquiryContent = "問合せ\"ダブルクォートあり",
                FirstReceivedDateTime = DateTime.Now,
                CreatedAt = DateTime.Now
            }
        };

        // Act
        await _sut.ExportInquiryHistoriesToCsvAsync(inquiries, filePath);

        // Assert
        var content = await File.ReadAllTextAsync(filePath, new UTF8Encoding(true));
        content.Should().Contain("\"問合せ\"\"ダブルクォートあり\"");
    }

    [Fact]
    public async Task ExportInquiryHistoriesToCsvAsync_WithNewlineInField_ShouldEscapeCorrectly()
    {
        // Arrange
        var filePath = Path.Combine(_tempDirectory, "test_newline.csv");
        var inquiries = new List<InquiryHistory>
        {
            new InquiryHistory
            {
                InquiryID = 1,
                ProjectID = 1,
                OperatorID = 1,
                InquiryContent = "問合せ\n改行あり",
                FirstReceivedDateTime = DateTime.Now,
                CreatedAt = DateTime.Now
            }
        };

        // Act
        await _sut.ExportInquiryHistoriesToCsvAsync(inquiries, filePath);

        // Assert
        var content = await File.ReadAllTextAsync(filePath, new UTF8Encoding(true));
        content.Should().Contain("\"問合せ\n改行あり\"");
    }

    [Fact]
    public async Task ExportInquiryHistoriesToCsvAsync_ShouldWriteUtf8WithBom()
    {
        // Arrange
        var filePath = Path.Combine(_tempDirectory, "test_bom.csv");
        var inquiries = new List<InquiryHistory>
        {
            new InquiryHistory
            {
                InquiryID = 1,
                ProjectID = 1,
                OperatorID = 1,
                InquiryContent = "日本語テスト",
                FirstReceivedDateTime = DateTime.Now,
                CreatedAt = DateTime.Now
            }
        };

        // Act
        await _sut.ExportInquiryHistoriesToCsvAsync(inquiries, filePath);

        // Assert
        var bytes = await File.ReadAllBytesAsync(filePath);
        // UTF-8 BOM: EF BB BF
        bytes[0].Should().Be(0xEF);
        bytes[1].Should().Be(0xBB);
        bytes[2].Should().Be(0xBF);
    }

    [Fact]
    public async Task ExportInquiryHistoriesToCsvAsync_NullFields_ShouldHandleGracefully()
    {
        // Arrange
        var filePath = Path.Combine(_tempDirectory, "test_null.csv");
        var inquiries = new List<InquiryHistory>
        {
            new InquiryHistory
            {
                InquiryID = 1,
                ProjectID = 1,
                CustomerKey = null,
                OperatorID = 1,
                CategoryID = null,
                StatusID = null,
                InquiryContent = "問合せ",
                ResponseContent = null,
                FirstReceivedDateTime = DateTime.Now,
                UpdatedDateTime = null,
                CreatedAt = DateTime.Now,
                CustomCol01 = null,
                CustomCol02 = null
            }
        };

        // Act
        await _sut.ExportInquiryHistoriesToCsvAsync(inquiries, filePath);

        // Assert
        File.Exists(filePath).Should().BeTrue();
        var content = await File.ReadAllTextAsync(filePath, new UTF8Encoding(true));
        content.Should().NotContain("null");
    }

    [Fact]
    public async Task ExportInquiryHistoriesToCsvAsync_MultipleRecords_ShouldExportAll()
    {
        // Arrange
        var filePath = Path.Combine(_tempDirectory, "test_multiple.csv");
        var inquiries = new List<InquiryHistory>
        {
            new InquiryHistory
            {
                InquiryID = 1,
                ProjectID = 1,
                OperatorID = 1,
                InquiryContent = "問合せ1",
                FirstReceivedDateTime = DateTime.Now,
                CreatedAt = DateTime.Now
            },
            new InquiryHistory
            {
                InquiryID = 2,
                ProjectID = 1,
                OperatorID = 1,
                InquiryContent = "問合せ2",
                FirstReceivedDateTime = DateTime.Now,
                CreatedAt = DateTime.Now
            },
            new InquiryHistory
            {
                InquiryID = 3,
                ProjectID = 1,
                OperatorID = 1,
                InquiryContent = "問合せ3",
                FirstReceivedDateTime = DateTime.Now,
                CreatedAt = DateTime.Now
            }
        };

        // Act
        await _sut.ExportInquiryHistoriesToCsvAsync(inquiries, filePath);

        // Assert
        var lines = await File.ReadAllLinesAsync(filePath, new UTF8Encoding(true));
        lines.Length.Should().Be(4); // 1 header + 3 data rows
    }

    [Fact]
    public async Task ExportInquiryHistoriesToCsvAsync_CustomColumns_ShouldExport()
    {
        // Arrange
        var filePath = Path.Combine(_tempDirectory, "test_custom.csv");
        var inquiries = new List<InquiryHistory>
        {
            new InquiryHistory
            {
                InquiryID = 1,
                ProjectID = 1,
                OperatorID = 1,
                InquiryContent = "問合せ",
                FirstReceivedDateTime = DateTime.Now,
                CreatedAt = DateTime.Now,
                CustomCol01 = "カスタム1",
                CustomCol02 = "カスタム2",
                CustomCol03 = "カスタム3"
            }
        };

        // Act
        await _sut.ExportInquiryHistoriesToCsvAsync(inquiries, filePath);

        // Assert
        var content = await File.ReadAllTextAsync(filePath, new UTF8Encoding(true));
        content.Should().Contain("カスタム1");
        content.Should().Contain("カスタム2");
        content.Should().Contain("カスタム3");
    }
}
