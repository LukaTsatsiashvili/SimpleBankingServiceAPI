using EntityLayer.DTOs.Auth;
using ServiceLayer.Services.Auth.Abstract;

namespace Test.Controller;

public class AuthControllerTest : BaseControllerTest<AuthController>
{
	private readonly IAuthService _service;

    public AuthControllerTest()
    {
        // Set up dependencies
        _service = A.Fake<IAuthService>();

        // SUT -> System Under Test
        InitializeController(CreateController());
        SetUpControllerContext();
    }

    protected override AuthController CreateController()
	{
        return new AuthController(_service);
	}


    // Create
    // This method returns Ok(success) | BadRequset(fails) action results
    [Fact]
    public async void AuthController_Register_ReturnsOk()
    {
        // Arrange
        UserDTO modelDTO = new()
        {
            Email = "Test@email.com",
            Password = "TestPassword123@",
            ConfirmPassword = "TestPassword123@"
		};
        GeneralResponse mockResponse = new(true, null!);

        // Act
        A.CallTo(() => _service.RegisterUserAsync(modelDTO)).Returns(mockResponse);
        var result = (OkObjectResult) await _controller.Register(modelDTO);

		// Assert
		result.StatusCode.Should().Be(200);
		result.Should().NotBeNull();
		result.Value.Should().BeEquivalentTo(mockResponse.Message);
	}

    // Create 
    // This method returns Ok(success) | BadRequest(fails) action results
    [Fact]
    public async void AuthController_Login_ReturnsOk()
    {
        // Arrange
        LoginDTO modelDTO = new()
        {
            Email = "Test@email.com",
            Password = "TestPassword123@"
        };
        LoginResponse mockResponse = new(true, "Test_JWTBearer_Token", null!);

        // Act
        A.CallTo(() => _service.LoginAsync(modelDTO)).Returns(mockResponse);
        var result = (OkObjectResult) await (_controller.Login(modelDTO));

		// Assert
		result.StatusCode.Should().Be(200);
		result.Should().NotBeNull();
		result.Value.Should().BeEquivalentTo(mockResponse);
	}

    // Create 
    // This method returns Ok(success) | BadRequest(fails) action results
    [Fact]
    public async void AuthController_ForgotPassword_ReturnsOk()
    {
        // Arrange
        ForgotPasswordDTO modelDTO = new()
        {
            Email = "Test@email.com"
        };
        ForgotPasswordResponse mockResponse = new(true, null!);

        // Act
        A.CallTo(() => _service.ForgotPasswordAsync(modelDTO)).Returns(mockResponse);
        var result = (OkObjectResult)await _controller.ForgotPassword(modelDTO);

		// Assert
		result.StatusCode.Should().Be(200);
		result.Should().NotBeNull();
		result.Value.Should().BeEquivalentTo(mockResponse.Message);
	}

    // Create 
    // This method returns Ok(success) | BadRequest(fails) action results
    [Fact]
    public async void AuthController_ResetPassword_ReturnsOk()
    {
        // Arrange
        ResetPasswordDTO modelDTO = new()
        {
            UserId = userId,
            NewPassword = "NewTestPassword123@",
            Token = "Test_Token"
        };
        ResetPasswordResponse mockResponse = new(true, null!);

        // Act
        A.CallTo(() => _service.ResetPasswordAsync(modelDTO)).Returns(mockResponse);
        var result = (OkObjectResult)await _controller.ResetPassword(modelDTO);

		// Assert
		result.StatusCode.Should().Be(200);
		result.Should().NotBeNull();
		result.Value.Should().BeEquivalentTo(mockResponse.Message);
	}
}
