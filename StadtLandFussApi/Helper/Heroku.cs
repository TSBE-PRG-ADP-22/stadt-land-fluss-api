namespace StadtLandFussApi.Helper
{
    public static class Heroku
    {
        public static string GetConnectionString()
        {
            var connectionUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            if (connectionUrl == null)
            {
                throw new ArgumentNullException("DATABASE_URL");
            }

            var databaseUri = new Uri(connectionUrl);
            var db = databaseUri.LocalPath.TrimStart('/');
            var userInfo = databaseUri.UserInfo.Split(':', StringSplitOptions.RemoveEmptyEntries);

            return $"User ID={userInfo[0]};Password={userInfo[1]};Host={databaseUri.Host};Port={databaseUri.Port};Database={db};Pooling=true;SSL Mode=Require;Trust Server Certificate=True;";
        }
    }
}
