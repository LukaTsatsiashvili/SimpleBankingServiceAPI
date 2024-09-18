namespace Test.Controller;

public abstract class BaseControllerTest<TController> where TController : ControllerBase
{
    protected TController _controller;
    protected string userId = Guid.NewGuid().ToString();

 
    protected abstract TController CreateController();

    protected void InitializeController(TController controller)
    {
        _controller = controller;
    }

    protected void SetUpControllerContext()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateFakeUser(userId!)
            }
        };
    }

    private static ClaimsPrincipal CreateFakeUser(string userId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

        var identity = new ClaimsIdentity(claims, "Bearer");

        return new ClaimsPrincipal(identity);
    }
}
