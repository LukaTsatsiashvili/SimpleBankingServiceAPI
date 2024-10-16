using EntityLayer.DTOs.Transaction;

namespace Test.Controller;

public class TransactionControllerTest : BaseControllerTest<TransactionController>
{
	private readonly ITransactionService _service;

    public TransactionControllerTest()
    {
        // Set up dependencies
        _service = A.Fake<ITransactionService>();

        // SUT -> System Under Test
        InitializeController(CreateController());
        SetUpControllerContext();
    }

    protected override TransactionController CreateController()
	{
		return new TransactionController(_service);
	}


    // Create
    // This method returns Ok(success) | BadRequest(fails) action results
    [Fact]
    public async void TransactionController_Create_ReturnsOk()
    {
        // Arrange
        TransactionDTO modelDTO = new(150, "11/20/2021", "1234567890", "1345678920");
        TransactionCreateDTO transactionDTO = new(150, "1345678920");
        TransactionResponse mockResponse = new (true, null!, "1234567890", modelDTO);

        // Act
        A.CallTo(() => _service.CreateTransactionAsync(userId, transactionDTO)).Returns(mockResponse);
        var result = (OkObjectResult)await _controller.CreateTransaction(transactionDTO);

		// Assert
		result.StatusCode.Should().Be(200);
		result.Should().NotBeNull();
		result.Value.Should().BeEquivalentTo(mockResponse);
	}
}
