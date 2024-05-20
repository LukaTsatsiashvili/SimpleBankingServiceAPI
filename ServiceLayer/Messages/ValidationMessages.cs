namespace ServiceLayer.Messages
{
	public static class ValidationMessages
	{
		public static string NullEmptyMessage(string propName)
		{
			return $"{propName} must have a value!";
		}

		public static string GreaterThanMessage(string propName, int restriction)
		{
			return $"{propName} must be greater than {restriction} characters!";
		}

		public static string LessThanMessage(string propName, int restriction)
		{
			return $"{propName} must be less than {restriction} characters!";
		}

		public static string EmailMessage(string propName)
		{
			return $"{propName} is in invalid format!";
		}

		public static string ComparePasswordMessage(string propName, string secondPropName)
		{
			return $"{propName} and {secondPropName} must be same!";
		}
	}
}
