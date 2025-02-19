namespace KeyInject.DependencyInjectionExample;

public class ExampleService
{
	private readonly IConfiguration _configuration;

	public ExampleService(IConfiguration configuration)
	{
		_configuration = configuration;
	}
	public void Display() => Console.WriteLine(_configuration["key-1"]);
}