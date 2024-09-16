using EntityLayer.DTOs.Account;
using EntityLayer.DTOs.Auth;
using EntityLayer.DTOs.Image;
using EntityLayer.DTOs.User;

namespace Test.Controller;

public class UserControllerTest
{
	private readonly IUserService _userService;
	private readonly IFileValidator _validator;
    private readonly UserController _controller;

    public UserControllerTest()
    {
        // Set up Dependencies
        _userService = A.Fake<IUserService>();
        _validator = A.Fake<IFileValidator>();

        // SUT -> System Under Test
        _controller = new UserController(_userService, _validator);
    }

    private static ImageUploadDTO CreateFakeImageUploadDTO()
    {
        // Mock a valid file stream
        var mockFile = A.Fake<IFormFile>();
        A.CallTo(() => mockFile.FileName).Returns("Image.jpg");
        A.CallTo(() => mockFile.Length).Returns(1024);
        A.CallTo(() => mockFile.OpenReadStream()).Returns(new MemoryStream(new byte[1024]));

        // Create a new ImageUploadDTO instance with the mocked FileName and File 
        var fakeImageUploadDto = new ImageUploadDTO
        {
            FileName = "Image.jpg",
            File = mockFile
        };

        return fakeImageUploadDto;
	} 
    private static ClaimsPrincipal CreateFakeUser(string userId)
    {
        // Create a list of claims with the user ID
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

        // Create a ClaimsIdentity with the claims and authentication type
        var identity = new ClaimsIdentity(claims, "Bearer");

        // Create a ClaimsPrincipal with the Identity
        return new ClaimsPrincipal(identity);
    }

    // Create 
    // This method returns Ok(success) | BadRequest(fails) action results
    [Fact]
    public async void UserController_Create_ReturnOk()
    {
        // Arrange
        var userId = Convert.ToString(Guid.NewGuid());
        var image = CreateFakeImageUploadDTO();
        var imagePath = "Fake/path/to/image.jpg";
        var mockResponse = new ProfilePictureUploadResponse(true, imagePath ,"Image uploaded successfully!");
        var mockValidationResponse = new GeneralResponse(true, null!);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateFakeUser(userId!)
            }
        };

        // Act
        A.CallTo(() => _validator.ValidateFile(image)).Returns(mockValidationResponse);
        A.CallTo(() => _userService.UploadProfilePictureAsync(userId!, image)).Returns(Task.FromResult(mockResponse));
        var result = (OkObjectResult)await _controller.UploadProfilePicture(image);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Should().NotBeNull();
    }

    // Delete
    // This method returns Ok(success) | BadRequest(fails) action results
    [Fact]
    public async void UserController_Delete_ReturnsOk()
    {
        // Arrange
        var userId = Convert.ToString(Guid.NewGuid());
        var mockResponse = new GeneralResponse(true, "Image removed successfully!");

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateFakeUser(userId!)
            }
        };

        // Act
        A.CallTo(() => _userService.RemoveProfilePictureAsync(userId!)).Returns(mockResponse);
        var result = (OkObjectResult)await _controller.RemoveProfilePicture();

        // Assert
        result.StatusCode.Should().Be(200);
        result.Should().NotBeNull();
    }

    // Update
    // This method returns Ok(success) | BadRequest(fails) action results
    [Fact]
    public async void UserController_Update_ReturnsOk()
    {
        // Arrange
        var userId = Convert.ToString(Guid.NewGuid());
        var mockResponse = new GeneralResponse(true, "Password changed successfully!");

		_controller.ControllerContext = new ControllerContext
		{
			HttpContext = new DefaultHttpContext
			{
				User = CreateFakeUser(userId!)
			}
		};

        var changePasswordDTO = new ChangePasswordDTO
        {
            CurrentPassword = "Fake123@",
            NewPassword = "2Fake123@"
        };

        // Act
        A.CallTo(() => _userService.ChangePasswordAsync(userId!, changePasswordDTO)).Returns(mockResponse);
        var result = (OkObjectResult)await _controller.ChangePassword(changePasswordDTO);

		// Assert
		result.StatusCode.Should().Be(200);
		result.Should().NotBeNull();
	}

    // Get
    // This method returns Ok(success) | BadRequest(fails) action results
    [Fact]
    public async void UserController_Get_ReturnsOk()
    {
        // Arrange
        var userId = Convert.ToString(Guid.NewGuid());
        var accountDTO = new List<AccountDTO>
        {
            new("1999999999", 450, null, null)
        }; 
        var userInformationDTO = new UserInformationDTO("Fake@gmail.com", "123456789", "Fake/image/path", accountDTO);
        var mockResponse = new GetUserInformationResponse(true, null!, userInformationDTO);

		_controller.ControllerContext = new ControllerContext
		{
			HttpContext = new DefaultHttpContext
			{
				User = CreateFakeUser(userId!)
			}
		};

        // Act
        A.CallTo(() => _userService.GetUserInformationAsync(userId!)).Returns(mockResponse);
        var result = (OkObjectResult)await _controller.GetUserInformation();

		// Assert
		result.StatusCode.Should().Be(200);
		result.Should().NotBeNull();
	}

    // Delete
    // This method returns Ok(success) | BadRequest(fails) action results
    [Fact]
    public async void UserController_DeleteAccount_ReturnsOk()
    {
        // Arrange
        var userId = Convert.ToString(Guid.NewGuid());
        var mockResponse = new GeneralResponse(true, "Account deleted successfully!");

		_controller.ControllerContext = new ControllerContext
		{
			HttpContext = new DefaultHttpContext
			{
				User = CreateFakeUser(userId!)
			}
		};

        // Act
        A.CallTo(() => _userService.DeleteUserAsync(userId!)).Returns(mockResponse);
        var result = (OkObjectResult)await _controller.DeleteAccount();

		// Assert
		result.StatusCode.Should().Be(200);
		result.Should().NotBeNull();
	}
}
