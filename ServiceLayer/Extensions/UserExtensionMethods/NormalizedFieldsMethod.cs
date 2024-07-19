using EntityLayer.Entities.Auth;

namespace ServiceLayer.Extensions.UserExtensionMethods
{
	public static class NormalizedFieldsMethod
	{
		public static void UpdateNormalizedFields(this AppUser user)
		{
			user.NormalizedEmail = user.Email!.ToUpper();
			user.NormalizedUserName = user.UserName!.ToUpper();
		}
	}
}
