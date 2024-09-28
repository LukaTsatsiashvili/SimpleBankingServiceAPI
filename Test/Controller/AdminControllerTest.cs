using EntityLayer.DTOs;
using EntityLayer.DTOs.Account;
using EntityLayer.DTOs.Transaction;
using EntityLayer.DTOs.User;

namespace Test.Controller;

public class AdminControllerTest : BaseControllerTest<AdminController>
{
	private readonly IAdminService _service;
	private readonly IFileValidator _validator;

	private readonly byte[] fakeFile = [0x01, 0x02, 0x03, 0x04, 0x05];
	private readonly string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

	public AdminControllerTest()
	{
		// Set up dependencies
		_service = A.Fake<IAdminService>();
		_validator = A.Fake<IFileValidator>();

		// SUT -> System Under Test
		InitializeController(CreateController());
		SetUpControllerContext();
	}

	protected override AdminController CreateController()
	{
		return new AdminController(_service, _validator);
	}

	// Get 
	// This method returns Ok(success) | BadRequest(fails) action results
	[Fact]
	public async void AdminController_GetAll_ReturnsOk()
	{
		// Arrange
		var accountDTO = new List<AccountDTO>
		{
			new("1999999999", 450, null, null)
		};
		List<UserInformationDTO> userDTO =
			[
				new("Fake@gmail.com", "123456789", "Fake/image/path", accountDTO)
			];
		var mockResponse = new GetAllUsersResponse(true, null!, userDTO);

		// Act
		A.CallTo(() => _service.GetAllUsersAsync()).Returns(mockResponse);
		var result = (OkObjectResult)await _controller.LoadAllUsers();

		// Assert
		result.StatusCode.Should().Be(200);
		result.Should().NotBeNull();
		result.Value.Should().BeEquivalentTo(mockResponse);
	}

	// Get
	// This method returns Ok(success) | BadRequest(fails) action results
	[Fact]
	public async void AdminController_GetSingle_ReturnsOk()
	{
		// Arrange
		Guid id = Guid.NewGuid();
		List<AccountDTO> accountDTO =
		[
			new("1999999999", 450, null, null)
		];
		UserInformationDTO userDTO = new("Fake@gmail.com", "123456789", "Fake/image/path", accountDTO);
		GetSingleUserResponse mockResponse = new(true, null!, userDTO);

		// Act
		A.CallTo(() => _service.GetSingleUserAsync(id)).Returns(mockResponse);
		var result = (OkObjectResult)await _controller.LoadSingleUser(id);

		// Assert
		result.StatusCode.Should().Be(200);
		result.Should().NotBeNull();
		result.Value.Should().BeEquivalentTo(mockResponse);
	}

	// Get
	// This method returns Ok(success) | BadRequest(fails) action results
	[Fact]
	public async void AdminController_GetTransaction_ReturnsOk()
	{
		// Arrange
		Guid id = Guid.NewGuid();
		List<TransactionDTO> sentTransactions =
			[
				new(250, "10/11/21", "1234567890", "1123456789")
			];
		List<TransactionDTO> recivedTransactions =
			[
				new(250, "10/11/21", "1234567890", "1123456789")
			];
		GetTransactionHistoryResponse mockResponse = new(true, null!, sentTransactions, recivedTransactions);

		// Act
		A.CallTo(() => _service.GetUserTransactionsAsync(id)).Returns(mockResponse);
		var result = (OkObjectResult)await _controller.GetUserTransaction(id);

		// Assert
		result.StatusCode.Should().Be(200);
		result.Should().NotBeNull();
		result.Value.Should().BeEquivalentTo(mockResponse);
	}

	// Put
	// This method returns Ok(success) | BadRequest(fails) action results
	[Fact]
	public async void AdminController_Put_ReturnsOk()
	{
		// Arrange 
		Guid id = Guid.NewGuid();
		UpdateUsersInformationDTO userDTO = new("fake@gmail.com","1234567890","Test_Name");
		GeneralResponse mockResponse = new(true, null!);

		// Act
		A.CallTo(() => _service.UpdateUserInformationAsync(id, userDTO)).Returns(mockResponse);
		var result = (OkObjectResult)await _controller.UpdateUserInformation(id, userDTO);

		// Assert
		result.StatusCode.Should().Be(200);
		result.Should().NotBeNull();
		result.Value.Should().BeEquivalentTo(mockResponse);
	}

	// Post
	// This method returns Ok(success) | BadRequest(fails) action results
	[Fact]
	public async void AdminController_Post_ReturnsOk()
	{
		// Arrange
		CreateUserDTO userDTO = new("fake@gmail.com", "Password1212@", "1234567890", "FakeRole");
		GeneralResponse mockResponse = new(true, null!);

		// Act
		A.CallTo(() => _service.CreateUserAsync(userDTO)).Returns(mockResponse);
		var result = (OkObjectResult)await _controller.CreateUser(userDTO);

		// Assert
		result.StatusCode.Should().Be(200);
		result.Should().NotBeNull();
		result.Value.Should().BeEquivalentTo(mockResponse.Message);
	}

	// Get
	// This method returns Ok(success) | BadRequest(fails) action results
	[Fact]
	public async void AdminController_GetUserExcelFile_ReturnsOk()
	{
		// Arrange
		Guid id = Guid.NewGuid();
		GenerateExcelFileResponse mockResponse = new(
			true,
			null!,
			fakeFile,
			"Test_Name",
			contentType);

		// Act
		A.CallTo(() => _service.GenerateUserExcelFileAsync(id)).Returns(mockResponse);
		var result = (FileContentResult)await _controller.GenerateUserExcelFile(id);

		// Assert
		result.Should().NotBeNull();
		result!.FileContents.Should().BeEquivalentTo(fakeFile);
		result.ContentType.Should().Be(contentType);
		result.FileDownloadName.Should().Be("Test_Name");
	}

	// Get
	// This method returns Ok(success) | BadRequest(fails) action results
	[Fact]
	public async void AdminController_GetAuditsExcelFile_ReturnsOk()
	{
		// Arrange
		string fakeEmail = "Fake@gmail.com";
		GenerateExcelFileResponse mockResponse = new(true, null!, fakeFile, "Test_Name", contentType);

		// Act
		A.CallTo(() => _service.GenerateAuditLogsExcelFileAsync(fakeEmail)).Returns(mockResponse);
		var result = (FileContentResult)await _controller.GenerateAuditLogExcelFile(fakeEmail);

		// Assert
		result.Should().NotBeNull();
		result.FileContents.Should().BeEquivalentTo(fakeFile);
		result.ContentType.Should().Be(contentType);
		result.FileDownloadName.Should().Be("Test_Name");
	}

	// Post
	// This method returns Ok(success) | BadRequest(fails) action results
	[Fact]
	public async void AdminController_PostMonthlyReport_ReturnsOk()
	{
		// Arrange
		MonthlyReportDTO reportDTO = new("Test", CreateFakeFile());
		GeneralResponse mockResponse = new(true, null!);

		// Act
		A.CallTo(() => _validator.ValidateExcelFile(reportDTO)).Returns(mockResponse);
		A.CallTo(() => _service.ImportMonthlyReportFileToDBAsync(reportDTO)).Returns(mockResponse);
		var result = (OkObjectResult)await _controller.UploadMonthlyReportFile(reportDTO);

		// Assert
		result.Should().NotBeNull();
		result.StatusCode.Should().Be(200);
		result.Value.Should().BeEquivalentTo(mockResponse.Message);
	}




	private IFormFile CreateFakeFile()
	{
		var stream = new MemoryStream(fakeFile);

		IFormFile file = new FormFile(stream, 0, stream.Length, "file", "fakefile.xlsx")
		{
			Headers = new HeaderDictionary(),
			ContentType = contentType
		};

		return file;
	}
}
