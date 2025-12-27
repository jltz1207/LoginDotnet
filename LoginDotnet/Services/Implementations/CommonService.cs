namespace LoginDotnet.Services.Implementations
{
    public static class CommonService
    {
        public static DateTime GenerateHKTime()
        {
            TimeZoneInfo hkTimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
            DateTime hkTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, hkTimeZone);
            return hkTime;
        }
    }
}
