using EntityLayer.DTOs.Account;

namespace Test.Controller;

public class AccountControllerTest : BaseControllerTest<AccountController>
{
	private readonly IAccountService _service;

    public AccountControllerTest()
    {
        // Set up dependencies
        _service = A.Fake<IAccountService>();

        // SUT -> System Under Test
        InitializeController(CreateController());
        SetUpControllerContext();
    }

	protected override AccountController CreateController()
	{
        return new AccountController(_service);
	}

    // Create
    // This method returns Ok(success) | BadRequest(fails) action results
    [Fact]
    public async void AccountController_Create_ReturnsOk()
    {
        // Arrange
        var mockResponse = new GeneralResponse(true, null!);

        // Act
        A.CallTo(() => _service.CreateAccountAsync(userId)).Returns(mockResponse);
        var result = (OkObjectResult)await _controller.CreateAccount();

		// Assert
		result.StatusCode.Should().Be(200);
		result.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(mockResponse.Message);
	}

    // Get
    // This method returns Ok(success) | BadRequest(fails) action results
    [Fact]
    public async void AccountController_Get_ReturnsOk()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var accountDTO = new AccountDTO("1234567890", 450.5m, null, null);
        var mockResponse = new AccountResponse(true, null!, accountDTO);

        // Act
        A.CallTo(() => _service.GetAccountByIdAsync(accountId)).Returns(mockResponse);
        var result = (OkObjectResult)await _controller.GetAccount(accountId);

		// Assert
		result.StatusCode.Should().Be(200);
		result.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(mockResponse.Data);
	}

    // Delete
    // This method returns Ok(success) | BadRequest(fails) action results
    [Fact]
    public async void AccountController_Delete_ReturnsOk()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var mockResponse = new GeneralResponse(true, null!);

        // Act
        A.CallTo(() => _service.RemoveAccountAsync(userId, accountId)).Returns(mockResponse);
        var result = (OkObjectResult)await _controller.DeleteAccount(accountId);

		// Assert
		result.StatusCode.Should().Be(200);
		result.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(mockResponse.Message);
	}
}
