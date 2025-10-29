using FluentAssertions;
using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Abstractions.Tests;

[TestClass]
public class ResponseTests
{
    [TestMethod]
    public void Constructor_ShouldInitializeWithDefaults()
    {
        // Act
        var response = new Response<string>();

        // Assert
        response.Data.Should().BeNull();
        response.IsSuccess.Should().BeFalse();
        response.ErrorMessage.Should().BeNull();
        response.StatusCode.Should().BeNull();
    }

    [TestMethod]
    public void Success_WithData_ShouldCreateSuccessfulResponse()
    {
        // Arrange
        string data = "test data";

        // Act
        var response = Response<string>.Success(data);

        // Assert
        response.Data.Should().Be(data);
        response.IsSuccess.Should().BeTrue();
        response.ErrorMessage.Should().BeNull();
        response.StatusCode.Should().BeNull();
    }

    [TestMethod]
    public void Success_WithDataAndStatusCode_ShouldCreateSuccessfulResponse()
    {
        // Arrange
        int data = 42;
        int statusCode = 200;

        // Act
        var response = Response<int>.Success(data, statusCode);

        // Assert
        response.Data.Should().Be(data);
        response.IsSuccess.Should().BeTrue();
        response.ErrorMessage.Should().BeNull();
        response.StatusCode.Should().Be(statusCode);
    }

    [TestMethod]
    public void Failure_WithErrorMessage_ShouldCreateFailedResponse()
    {
        // Arrange
        string errorMessage = "An error occurred";

        // Act
        var response = Response<string>.Failure(errorMessage);

        // Assert
        response.Data.Should().BeNull();
        response.IsSuccess.Should().BeFalse();
        response.ErrorMessage.Should().Be(errorMessage);
        response.StatusCode.Should().BeNull();
    }

    [TestMethod]
    public void Success_WithComplexObject_ShouldPreserveData()
    {
        // Arrange
        var complexData = new { Id = 1, Name = "Test", Items = new[] { 1, 2, 3 } };

        // Act
        var response = Response<object>.Success(complexData);

        // Assert
        response.Data.Should().BeSameAs(complexData);
        response.IsSuccess.Should().BeTrue();
    }

    [TestMethod]
    public void Success_WithNullData_ShouldCreateSuccessfulResponseWithNullData()
    {
        // Act
        var response = Response<string?>.Success(null!);

        // Assert
        response.Data.Should().BeNull();
        response.IsSuccess.Should().BeTrue();
    }

    [TestMethod]
    [DataRow(200, "OK")]
    [DataRow(201, "Created")]
    [DataRow(204, "No Content")]
    [DataRow(400, "Bad Request")]
    [DataRow(404, "Not Found")]
    [DataRow(500, "Internal Server Error")]
    public void Success_WithVariousStatusCodes_ShouldStoreStatusCode(int statusCode, string _)
    {
        // Act
        var response = Response<string>.Success("data", statusCode);

        // Assert
        response.StatusCode.Should().Be(statusCode);
        response.IsSuccess.Should().BeTrue();
    }

    [TestMethod]
    public void Failure_WithLongErrorMessage_ShouldPreserve()
    {
        // Arrange
        string longError = new string('E', 10000);

        // Act
        var response = Response<string>.Failure(longError);

        // Assert
        response.ErrorMessage.Should().HaveLength(10000);
        response.IsSuccess.Should().BeFalse();
    }

    [TestMethod]
    public void Failure_WithUnicodeErrorMessage_ShouldPreserve()
    {
        // Arrange
        string unicodeError = "エラーが発生しました: 操作に失敗しました";

        // Act
        var response = Response<string>.Failure(unicodeError);

        // Assert
        response.ErrorMessage.Should().Be(unicodeError);
        response.IsSuccess.Should().BeFalse();
    }

    [TestMethod]
    public void Success_WithValueType_ShouldStore()
    {
        // Act
        var response = Response<int>.Success(42);

        // Assert
        response.Data.Should().Be(42);
        response.IsSuccess.Should().BeTrue();
    }

    [TestMethod]
    public void Success_WithReferenceType_ShouldStore()
    {
        // Arrange
        var data = new List<string> { "item1", "item2" };

        // Act
        var response = Response<List<string>>.Success(data);

        // Assert
        response.Data.Should().BeSameAs(data);
        response.IsSuccess.Should().BeTrue();
    }

    [TestMethod]
    public void MultipleInstances_ShouldBeIndependent()
    {
        // Act
        var response1 = Response<string>.Success("data1", 200);
        var response2 = Response<int>.Failure("error2");

        // Assert
        response1.Data.Should().Be("data1");
        response1.IsSuccess.Should().BeTrue();
        response1.StatusCode.Should().Be(200);
        
        response2.Data.Should().Be(0);
        response2.IsSuccess.Should().BeFalse();
        response2.ErrorMessage.Should().Be("error2");
    }

    [TestMethod]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var response = new Response<string>();

        // Act
        response.Data = "test";
        response.IsSuccess = true;
        response.ErrorMessage = "error";
        response.StatusCode = 201;

        // Assert
        response.Data.Should().Be("test");
        response.IsSuccess.Should().BeTrue();
        response.ErrorMessage.Should().Be("error");
        response.StatusCode.Should().Be(201);
    }

    [TestMethod]
    public void Success_WithEmptyString_ShouldPreserve()
    {
        // Act
        var response = Response<string>.Success(string.Empty);

        // Assert
        response.Data.Should().BeEmpty();
        response.IsSuccess.Should().BeTrue();
    }

    [TestMethod]
    public void Failure_WithEmptyErrorMessage_ShouldPreserve()
    {
        // Act
        var response = Response<string>.Failure(string.Empty);

        // Assert
        response.ErrorMessage.Should().BeEmpty();
        response.IsSuccess.Should().BeFalse();
    }

    [TestMethod]
    public void Success_WithNegativeStatusCode_ShouldStore()
    {
        // Act
        var response = Response<string>.Success("data", -1);

        // Assert
        response.StatusCode.Should().Be(-1);
        response.IsSuccess.Should().BeTrue();
    }

    [TestMethod]
    public void Success_WithMaxStatusCode_ShouldStore()
    {
        // Act
        var response = Response<string>.Success("data", int.MaxValue);

        // Assert
        response.StatusCode.Should().Be(int.MaxValue);
        response.IsSuccess.Should().BeTrue();
    }

    [TestMethod]
    public void Failure_WithNullErrorMessage_ShouldStoreNull()
    {
        // Act
        var response = Response<string>.Failure(null!);

        // Assert
        response.ErrorMessage.Should().BeNull();
        response.IsSuccess.Should().BeFalse();
    }

    [TestMethod]
    public void Success_WithDifferentGenericTypes_ShouldWorkCorrectly()
    {
        // Act
        var stringResponse = Response<string>.Success("text");
        var intResponse = Response<int>.Success(123);
        var boolResponse = Response<bool>.Success(true);

        // Assert
        stringResponse.Data.Should().Be("text");
        intResponse.Data.Should().Be(123);
        boolResponse.Data.Should().BeTrue();
    }
}
